using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Models.ViewModels;

namespace planinarenje.Controllers
{
    public class PodrucjeController : Controller
    {
        private readonly PlaninarstvoDbContext _dbContext;

        public PodrucjeController(PlaninarstvoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var model = BuildIndexModel(null);
            ViewData["Title"] = "Podrucja";
            return View(model);
        }

        [HttpGet]
        public IActionResult Search(string? searchTerm)
        {
            var model = BuildIndexModel(searchTerm);
            return PartialView("_PodrucjeListPartial", model);
        }

        [HttpGet]
        public IActionResult AutocompleteSearch(string term)
        {
            var results = _dbContext.Podrucja
                .Where(p => p.DeletedAt == null && p.Naziv.Contains(term))
                .OrderBy(p => p.Naziv)
                .Take(15)
                .Select(p => new
                {
                    value = p.IdPodrucje,
                    label = p.Naziv
                })
                .ToList();

            return Json(results);
        }

        public IActionResult Create()
        {
            ViewData["Title"] = "Novo podrucje";
            return View(new PodrucjeCreateModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PodrucjeCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Novo podrucje";
                return View(model);
            }

            var podrucje = new Podrucje
            {
                Naziv = model.Naziv,
                Opis = model.Opis,
                Regija = model.Regija,
                MinimalanBrojKTZaObilazak = model.MinimalanBrojKTZaObilazak
            };

            _dbContext.Podrucja.Add(podrucje);
            _dbContext.SaveChanges();
            TempData["Success"] = "Podrucje je uspjesno dodano.";
            return RedirectToAction(nameof(Index));
        }

        [ActionName("Edit")]
        public IActionResult EditGet(int id)
        {
            var podrucje = _dbContext.Podrucja.FirstOrDefault(p => p.IdPodrucje == id && p.DeletedAt == null);
            if (podrucje is null)
            {
                return NotFound();
            }

            var model = new PodrucjeEditModel
            {
                Naziv = podrucje.Naziv,
                Opis = podrucje.Opis,
                Regija = podrucje.Regija,
                MinimalanBrojKTZaObilazak = podrucje.MinimalanBrojKTZaObilazak
            };

            ViewData["Title"] = "Uredi podrucje";
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult EditPost(int id, PodrucjeEditModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Uredi podrucje";
                return View(model);
            }

            var podrucje = _dbContext.Podrucja.FirstOrDefault(p => p.IdPodrucje == id && p.DeletedAt == null);
            if (podrucje is null)
            {
                return NotFound();
            }

            podrucje.Naziv = model.Naziv;
            podrucje.Opis = model.Opis;
            podrucje.Regija = model.Regija;
            podrucje.MinimalanBrojKTZaObilazak = model.MinimalanBrojKTZaObilazak;

            _dbContext.SaveChanges();
            TempData["Success"] = "Podrucje je uspjesno azurirano.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var podrucje = _dbContext.Podrucja.FirstOrDefault(p => p.IdPodrucje == id && p.DeletedAt == null);
            if (podrucje is null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Obrisi podrucje";
            return View(podrucje);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var podrucje = _dbContext.Podrucja.FirstOrDefault(p => p.IdPodrucje == id && p.DeletedAt == null);
            if (podrucje is null)
            {
                return NotFound();
            }

            podrucje.DeletedAt = DateTime.UtcNow;
            _dbContext.SaveChanges();
            TempData["Success"] = "Podrucje je uspjesno obrisano.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            var podrucje = _dbContext.Podrucja
                .Include(p => p.KontrolneTocke)
                .Include(p => p.PlaninarskiObjekti)
                .FirstOrDefault(p => p.IdPodrucje == id && p.DeletedAt == null);
            if (podrucje is null)
            {
                return NotFound();
            }

            if (podrucje.KontrolneTocke != null)
            {
                podrucje.KontrolneTocke = podrucje.KontrolneTocke
                    .Where(kt => kt.DeletedAt == null)
                    .OrderBy(kt => kt.Naziv)
                    .ToList();
            }

            ViewData["Title"] = podrucje.Naziv;
            return View(podrucje);
        }

        [Route("podrucje/{id:int}/tocke")]
        public IActionResult KontrolneTockePodrucja(int id)
        {
            var podrucje = _dbContext.Podrucja.FirstOrDefault(p => p.IdPodrucje == id && p.DeletedAt == null);
            if (podrucje == null) return NotFound();

            var tocke = _dbContext.KontrolneTocke
                .Include(kt => kt.Podrucje)
                .Where(kt => kt.IdPodrucje == id && kt.DeletedAt == null)
                .OrderBy(kt => kt.Naziv)
                .ToList();

            ViewBag.Podrucje = podrucje;
            return View(tocke);
        }

        private static string? TrimOpis(string? opis, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(opis))
            {
                return null;
            }

            var normalized = opis.Trim();
            if (normalized.Length <= maxLength)
            {
                return normalized;
            }

            return normalized[..maxLength].TrimEnd() + "...";
        }

        private List<PodrucjeIndexCardViewModel> BuildIndexModel(string? searchTerm)
        {
            var filteredPodrucja = _dbContext.Podrucja
                .Where(p => p.DeletedAt == null);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                filteredPodrucja = filteredPodrucja.Where(p =>
                    p.Naziv.Contains(term) ||
                    (p.Regija != null && p.Regija.Contains(term)) ||
                    (p.Opis != null && p.Opis.Contains(term)));
            }

            var ktCountByPodrucje = _dbContext.KontrolneTocke
                .Where(kt => kt.DeletedAt == null)
                .GroupBy(kt => kt.IdPodrucje)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionary(x => x.Key, x => x.Count);

            return filteredPodrucja
                .OrderBy(p => p.IdPodrucje)
                .AsEnumerable()
                .Select(p => new PodrucjeIndexCardViewModel
                {
                    IdPodrucje = p.IdPodrucje,
                    Naziv = p.Naziv,
                    Regija = string.IsNullOrWhiteSpace(p.Regija) ? "Regija nije navedena" : p.Regija,
                    OpisPreview = TrimOpis(p.Opis, 170),
                    MinimalanBrojKTZaObilazak = p.MinimalanBrojKTZaObilazak,
                    UkupanBrojKT = ktCountByPodrucje.TryGetValue(p.IdPodrucje, out var count) ? count : 0
                })
                .ToList();
        }
    }
}
