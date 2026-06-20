using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace planinarenje.Controllers
{
    public class PodrucjeController : BaseController
    {
        private readonly ILogger<PodrucjeController> _logger;

        public PodrucjeController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db, ILogger<PodrucjeController> logger)
            : base(userMgr, db)
        {
            _logger = logger;
        }

        [AllowAnonymous]
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
            var results = Db.Podrucja
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

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["Title"] = "Novo podrucje";
            return View(new PodrucjeCreateModel());
        }

        private int DohvatiUkupnoKontrolnihTocaka(int idPodrucje)
        {
            return Db.KontrolneTocke.Count(kt => kt.IdPodrucje == idPodrucje && kt.DeletedAt == null);
        }

        private bool DodajGreskeZaDuplikatPodrucja(string naziv, string? regija, int minimalanBrojKt, int ukupnoKt, int? excludeId = null)
        {
            var postoji = Db.Podrucja
                .Where(p => p.DeletedAt == null)
                .Select(p => new
                {
                    p.IdPodrucje,
                    p.Naziv,
                    p.Regija,
                    p.MinimalanBrojKTZaObilazak,
                    UkupnoKT = Db.KontrolneTocke.Count(kt => kt.IdPodrucje == p.IdPodrucje && kt.DeletedAt == null)
                })
                .AsEnumerable()
                .Any(p => p.Naziv == naziv &&
                          string.Equals((p.Regija ?? string.Empty).Trim(), (regija ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase) &&
                          p.MinimalanBrojKTZaObilazak == minimalanBrojKt &&
                          p.UkupnoKT == ukupnoKt &&
                          (!excludeId.HasValue || p.IdPodrucje != excludeId.Value));

            if (!postoji)
            {
                return false;
            }

            var poruka = "Područje s istim nazivom, minimalnim brojem KT i ukupnim brojem KT već postoji.";
            ModelState.AddModelError(string.Empty, poruka);
            ViewData["PopupWarning"] = poruka;
            return true;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(PodrucjeCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Novo podrucje";
                return View(model);
            }

            if (DodajGreskeZaDuplikatPodrucja(model.Naziv, model.Regija, model.MinimalanBrojKTZaObilazak, model.UkupanBrojKT ?? 0))
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

            try
            {
                Db.Podrucja.Add(podrucje);
                await Db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ViewData["PopupWarning"] = "Područje s istim nazivom, minimalnim brojem KT i ukupnim brojem KT već postoji.";
                ViewData["Title"] = "Novo podrucje";
                return View(model);
            }

            _logger.LogInformation("Područje {IdPodrucje} kreirano ({Naziv}).", podrucje.IdPodrucje, podrucje.Naziv);
            TempData["NewId"] = podrucje.IdPodrucje;
            TempData["Success"] = "Podrucje je uspjesno dodano.";
            return RedirectToAction(nameof(Index));
        }

        [ActionName("Edit")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditGet(int id)
        {
            var podrucje = await Db.Podrucja.FirstOrDefaultAsync(p => p.IdPodrucje == id && p.DeletedAt == null);
            if (podrucje is null)
            {
                return NotFound();
            }

            var model = new PodrucjeEditModel
            {
                Naziv = podrucje.Naziv,
                Opis = podrucje.Opis,
                Regija = podrucje.Regija,
                MinimalanBrojKTZaObilazak = podrucje.MinimalanBrojKTZaObilazak,
                UkupanBrojKT = DohvatiUkupnoKontrolnihTocaka(id)
            };

            ViewData["Title"] = "Uredi podrucje";
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditPost(int id, PodrucjeEditModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Uredi podrucje";
                return View(model);
            }

            var podrucje = await Db.Podrucja.FirstOrDefaultAsync(p => p.IdPodrucje == id && p.DeletedAt == null);
            if (podrucje is null)
            {
                return NotFound();
            }

            var ukupnoKt = DohvatiUkupnoKontrolnihTocaka(id);
            if (DodajGreskeZaDuplikatPodrucja(model.Naziv, model.Regija, model.MinimalanBrojKTZaObilazak, model.UkupanBrojKT ?? ukupnoKt, id))
            {
                ViewData["Title"] = "Uredi podrucje";
                return View(model);
            }

            podrucje.Naziv = model.Naziv;
            podrucje.Opis = model.Opis;
            podrucje.Regija = model.Regija;
            podrucje.MinimalanBrojKTZaObilazak = model.MinimalanBrojKTZaObilazak;

            try
            {
                await Db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ViewData["PopupWarning"] = "Područje s istim nazivom, minimalnim brojem KT i ukupnim brojem KT već postoji.";
                ViewData["Title"] = "Uredi podrucje";
                return View(model);
            }

            _logger.LogInformation("Područje {IdPodrucje} ažurirano.", id);
            TempData["Success"] = "Podrucje je uspjesno azurirano.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var podrucje = await Db.Podrucja.FirstOrDefaultAsync(p => p.IdPodrucje == id && p.DeletedAt == null);
            if (podrucje is null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Obrisi podrucje";
            return View(podrucje);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var podrucje = await Db.Podrucja.FirstOrDefaultAsync(p => p.IdPodrucje == id && p.DeletedAt == null);
            if (podrucje is null)
            {
                return NotFound();
            }

            podrucje.DeletedAt = DateTime.UtcNow;
            await Db.SaveChangesAsync();
            _logger.LogInformation("Područje {IdPodrucje} obrisano (soft delete).", id);
            TempData["Success"] = "Podrucje je uspjesno obrisano.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            var podrucje = Db.Podrucja
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
            var podrucje = Db.Podrucja.FirstOrDefault(p => p.IdPodrucje == id && p.DeletedAt == null);
            if (podrucje == null) return NotFound();

            var tocke = Db.KontrolneTocke
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
            var filteredPodrucja = Db.Podrucja
                .Where(p => p.DeletedAt == null);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                filteredPodrucja = filteredPodrucja.Where(p =>
                    p.Naziv.Contains(term) ||
                    (p.Regija != null && p.Regija.Contains(term)) ||
                    (p.Opis != null && p.Opis.Contains(term)));
            }

            var ktCountByPodrucje = Db.KontrolneTocke
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
