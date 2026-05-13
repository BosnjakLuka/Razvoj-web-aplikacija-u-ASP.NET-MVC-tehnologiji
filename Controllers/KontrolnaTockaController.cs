using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Models.ViewModels;

namespace planinarenje.Controllers
{
    public class KontrolnaTockaController : Controller
    {
        private readonly PlaninarstvoDbContext _dbContext;

        public KontrolnaTockaController(PlaninarstvoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var model = BuildIndexModel(null);
            ViewData["Title"] = "Kontrolne tocke";
            return View(model);
        }

        [HttpGet]
        public IActionResult Search(string? searchTerm)
        {
            var model = BuildIndexModel(searchTerm);
            return PartialView("_KontrolnaTockaListPartial", model);
        }

        public IActionResult Create()
        {
            PopulatePodrucjaSelectList();
            ViewData["Title"] = "Nova kontrolna tocka";
            return View(new KontrolnaTockaCreateModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(KontrolnaTockaCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulatePodrucjaSelectList(model.IdPodrucje);
                ViewData["Title"] = "Nova kontrolna tocka";
                return View(model);
            }

            var entity = new KontrolnaTocka
            {
                Naziv = model.Naziv,
                GUIDOznaka = model.GUIDOznaka,
                IdPodrucje = model.IdPodrucje,
                TipKontrolneTocke = model.TipKontrolneTocke,
                NadmorskaVisina = model.NadmorskaVisina,
                Opis = model.Opis,
                Koordinate = model.Koordinate,
                OpisZiga = model.OpisZiga
            };

            _dbContext.KontrolneTocke.Add(entity);
            _dbContext.SaveChanges();
            TempData["Success"] = "Kontrolna tocka je uspjesno dodana.";
            return RedirectToAction(nameof(Index));
        }

        [ActionName("Edit")]
        public IActionResult EditGet(int id)
        {
            var entity = _dbContext.KontrolneTocke
                .FirstOrDefault(k => k.IdKontrolnaTocka == id && k.DeletedAt == null);
            if (entity is null)
            {
                return NotFound();
            }

            var model = new KontrolnaTockaEditModel
            {
                Naziv = entity.Naziv,
                GUIDOznaka = entity.GUIDOznaka,
                IdPodrucje = entity.IdPodrucje,
                TipKontrolneTocke = entity.TipKontrolneTocke,
                NadmorskaVisina = entity.NadmorskaVisina,
                Opis = entity.Opis,
                Koordinate = entity.Koordinate,
                OpisZiga = entity.OpisZiga
            };

            PopulatePodrucjaSelectList(model.IdPodrucje);
            ViewData["Title"] = "Uredi kontrolnu tocku";
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult EditPost(int id, KontrolnaTockaEditModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulatePodrucjaSelectList(model.IdPodrucje);
                ViewData["Title"] = "Uredi kontrolnu tocku";
                return View(model);
            }

            var entity = _dbContext.KontrolneTocke
                .FirstOrDefault(k => k.IdKontrolnaTocka == id && k.DeletedAt == null);
            if (entity is null)
            {
                return NotFound();
            }

            entity.Naziv = model.Naziv;
            entity.GUIDOznaka = model.GUIDOznaka;
            entity.IdPodrucje = model.IdPodrucje;
            entity.TipKontrolneTocke = model.TipKontrolneTocke;
            entity.NadmorskaVisina = model.NadmorskaVisina;
            entity.Opis = model.Opis;
            entity.Koordinate = model.Koordinate;
            entity.OpisZiga = model.OpisZiga;

            _dbContext.SaveChanges();
            TempData["Success"] = "Kontrolna tocka je uspjesno azurirana.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var entity = _dbContext.KontrolneTocke
                .Include(k => k.Podrucje)
                .FirstOrDefault(k => k.IdKontrolnaTocka == id && k.DeletedAt == null);
            if (entity is null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Obrisi kontrolnu tocku";
            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var entity = _dbContext.KontrolneTocke
                .FirstOrDefault(k => k.IdKontrolnaTocka == id && k.DeletedAt == null);
            if (entity is null)
            {
                return NotFound();
            }

            entity.DeletedAt = DateTime.UtcNow;
            _dbContext.SaveChanges();
            TempData["Success"] = "Kontrolna tocka je uspjesno obrisana.";
            return RedirectToAction(nameof(Index));
        }

        [Route("vrh/{id:int}")]
        [Route("[controller]/[action]/{id:int}")]
        public IActionResult Details(int id)
        {
            var kontrolnaTocka = _dbContext.KontrolneTocke
                .Include(k => k.Podrucje)
                .FirstOrDefault(k => k.IdKontrolnaTocka == id && k.DeletedAt == null);
            if (kontrolnaTocka is null)
            {
                return NotFound();
            }
            ViewData["Title"] = kontrolnaTocka.Naziv;
            return View(kontrolnaTocka);
        }

        private static string MapTip(TipKontrolneTocke tip)
        {
            return tip switch
            {
                TipKontrolneTocke.Vrh => "Vrh",
                TipKontrolneTocke.Vidikovac => "Vidikovac",
                TipKontrolneTocke.KontrolnaTocka => "Kontrolna tocka",
                _ => "Tip nije definiran"
            };
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

        private List<KontrolnaTockaIndexCardViewModel> BuildIndexModel(string? searchTerm)
        {
            var query = _dbContext.KontrolneTocke
                .Include(k => k.Podrucje)
                .Where(k => k.DeletedAt == null && (k.Podrucje == null || k.Podrucje.DeletedAt == null));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(k =>
                    k.Naziv.Contains(term) ||
                    k.GUIDOznaka.Contains(term) ||
                    (k.Podrucje != null && k.Podrucje.Naziv.Contains(term)));
            }

            return query
                .OrderBy(k => k.Naziv)
                .Select(k => new KontrolnaTockaIndexCardViewModel
                {
                    IdKontrolnaTocka = k.IdKontrolnaTocka,
                    Naziv = k.Naziv,
                    TipKontrolneTockeNaziv = MapTip(k.TipKontrolneTocke),
                    NadmorskaVisina = k.NadmorskaVisina,
                    PodrucjeNaziv = k.Podrucje != null ? k.Podrucje.Naziv : "Nepoznato podrucje",
                    OpisPreview = TrimOpis(k.Opis, 140),
                    GUIDOznaka = k.GUIDOznaka
                })
                .ToList();
        }

        private void PopulatePodrucjaSelectList(int? selectedId = null)
        {
            var podrucja = _dbContext.Podrucja
                .Where(p => p.DeletedAt == null)
                .OrderBy(p => p.Naziv)
                .ToList();

            ViewBag.Podrucja = new SelectList(podrucja, "IdPodrucje", "Naziv", selectedId);
        }
    }
}
