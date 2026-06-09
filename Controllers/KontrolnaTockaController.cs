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
    public class KontrolnaTockaController : BaseController
    {
        public KontrolnaTockaController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db)
            : base(userMgr, db)
        {
        }

        [AllowAnonymous]
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

        [HttpGet]
        public IActionResult AutocompleteSearch(string term)
        {
            var results = Db.KontrolneTocke
                .Where(k => k.DeletedAt == null && k.Naziv.Contains(term))
                .OrderBy(k => k.Naziv)
                .Take(15)
                .Select(k => new
                {
                    value = k.IdKontrolnaTocka,
                    label = k.Naziv,
                    guid = k.GUIDOznaka
                })
                .ToList();

            return Json(results);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["Title"] = "Nova kontrolna tocka";
            return View(new KontrolnaTockaCreateModel());
        }

        private bool ValidirajPodrucje(int idPodrucje)
        {
            var postoji = Db.Podrucja.Any(p => p.IdPodrucje == idPodrucje && p.DeletedAt == null);
            if (postoji)
            {
                return true;
            }

            ModelState.AddModelError(nameof(KontrolnaTockaCreateModel.IdPodrucje), "Odabrano područje više ne postoji ili je obrisano.");
            ViewData["PopupWarning"] = "Odabrano područje više ne postoji ili je obrisano.";
            return false;
        }

        private bool DodajGreskeZaDuplikatKontrolneTocke(string naziv, string guidOznaka, int idPodrucje, int? excludeId = null)
        {
            var postoji = Db.KontrolneTocke.Any(k =>
                k.Naziv == naziv &&
                k.GUIDOznaka == guidOznaka &&
                k.IdPodrucje == idPodrucje &&
                (!excludeId.HasValue || k.IdKontrolnaTocka != excludeId.Value) &&
                k.DeletedAt == null);

            if (!postoji)
            {
                return false;
            }

            var poruka = "Kontrolna točka s istim nazivom, GUID oznakom i područjem već postoji.";
            ModelState.AddModelError(string.Empty, poruka);
            ViewData["PopupWarning"] = poruka;
            return true;
        }

        private bool DodajGreskeZaZauzetGUID(string guidOznaka, int? excludeId = null)
        {
            var postoji = Db.KontrolneTocke.Any(k =>
                k.GUIDOznaka == guidOznaka && (!excludeId.HasValue || k.IdKontrolnaTocka != excludeId.Value) && k.DeletedAt == null);

            if (!postoji)
            {
                return false;
            }

            var poruka = "Kontrolna točka s ovom GUID oznakom već postoji. Promijeni oznaku ili uređuj postojeću točku.";
            ModelState.AddModelError(nameof(KontrolnaTockaCreateModel.GUIDOznaka), poruka);
            ViewData["PopupWarning"] = poruka;
            return true;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(KontrolnaTockaCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["PodrucjeText"] = GetPodrucjeNaziv(model.IdPodrucje);
                ViewData["Title"] = "Nova kontrolna tocka";
                return View(model);
            }

            if (!ValidirajPodrucje(model.IdPodrucje))
            {
                ViewData["PodrucjeText"] = GetPodrucjeNaziv(model.IdPodrucje);
                ViewData["Title"] = "Nova kontrolna tocka";
                return View(model);
            }

            if (DodajGreskeZaDuplikatKontrolneTocke(model.Naziv, model.GUIDOznaka, model.IdPodrucje))
            {
                ViewData["PodrucjeText"] = GetPodrucjeNaziv(model.IdPodrucje);
                ViewData["Title"] = "Nova kontrolna tocka";
                return View(model);
            }

            if (DodajGreskeZaZauzetGUID(model.GUIDOznaka))
            {
                ViewData["PodrucjeText"] = GetPodrucjeNaziv(model.IdPodrucje);
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

            try
            {
                Db.KontrolneTocke.Add(entity);
                await Db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ViewData["PopupWarning"] = "Kontrolna točka s ovim nazivom, GUID oznakom i područjem već postoji ili je odabrano područje neispravno.";
                ViewData["PodrucjeText"] = GetPodrucjeNaziv(model.IdPodrucje);
                ViewData["Title"] = "Nova kontrolna tocka";
                return View(model);
            }

            TempData["NewId"] = entity.IdKontrolnaTocka;
            TempData["Success"] = "Kontrolna tocka je uspjesno dodana.";
            return RedirectToAction(nameof(Index));
        }

        [ActionName("Edit")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditGet(int id)
        {
            var entity = await Db.KontrolneTocke
                .Include(k => k.Podrucje)
                .FirstOrDefaultAsync(k => k.IdKontrolnaTocka == id && k.DeletedAt == null);
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

            ViewData["PodrucjeText"] = entity.Podrucje?.Naziv ?? GetPodrucjeNaziv(model.IdPodrucje);
            ViewData["Title"] = "Uredi kontrolnu tocku";
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditPost(int id, KontrolnaTockaEditModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["PodrucjeText"] = GetPodrucjeNaziv(model.IdPodrucje);
                ViewData["Title"] = "Uredi kontrolnu tocku";
                return View(model);
            }

            var entity = await Db.KontrolneTocke
                .FirstOrDefaultAsync(k => k.IdKontrolnaTocka == id && k.DeletedAt == null);
            if (entity is null)
            {
                return NotFound();
            }

            if (!ValidirajPodrucje(model.IdPodrucje))
            {
                ViewData["PodrucjeText"] = GetPodrucjeNaziv(model.IdPodrucje);
                ViewData["Title"] = "Uredi kontrolnu tocku";
                return View(model);
            }

            if (DodajGreskeZaDuplikatKontrolneTocke(model.Naziv, model.GUIDOznaka, model.IdPodrucje, id))
            {
                ViewData["PodrucjeText"] = GetPodrucjeNaziv(model.IdPodrucje);
                ViewData["Title"] = "Uredi kontrolnu tocku";
                return View(model);
            }

            if (DodajGreskeZaZauzetGUID(model.GUIDOznaka, id))
            {
                ViewData["PodrucjeText"] = GetPodrucjeNaziv(model.IdPodrucje);
                ViewData["Title"] = "Uredi kontrolnu tocku";
                return View(model);
            }

            entity.Naziv = model.Naziv;
            entity.GUIDOznaka = model.GUIDOznaka;
            entity.IdPodrucje = model.IdPodrucje;
            entity.TipKontrolneTocke = model.TipKontrolneTocke;
            entity.NadmorskaVisina = model.NadmorskaVisina;
            entity.Opis = model.Opis;
            entity.Koordinate = model.Koordinate;
            entity.OpisZiga = model.OpisZiga;

            try
            {
                await Db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ViewData["PopupWarning"] = "Kontrolna točka s ovim nazivom, GUID oznakom i područjem već postoji ili je odabrano područje neispravno.";
                ViewData["PodrucjeText"] = GetPodrucjeNaziv(model.IdPodrucje);
                ViewData["Title"] = "Uredi kontrolnu tocku";
                return View(model);
            }

            TempData["Success"] = "Kontrolna tocka je uspjesno azurirana.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await Db.KontrolneTocke
                .Include(k => k.Podrucje)
                .FirstOrDefaultAsync(k => k.IdKontrolnaTocka == id && k.DeletedAt == null);
            if (entity is null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Obrisi kontrolnu tocku";
            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await Db.KontrolneTocke
                .FirstOrDefaultAsync(k => k.IdKontrolnaTocka == id && k.DeletedAt == null);
            if (entity is null)
            {
                return NotFound();
            }

            entity.DeletedAt = DateTime.UtcNow;
            await Db.SaveChangesAsync();
            TempData["Success"] = "Kontrolna tocka je uspjesno obrisana.";
            return RedirectToAction(nameof(Index));
        }

        [Route("vrh/{id:int}")]
        [Route("[controller]/[action]/{id:int}")]
        public IActionResult Details(int id)
        {
            var kontrolnaTocka = Db.KontrolneTocke
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
            var query = Db.KontrolneTocke
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

        private string? GetPodrucjeNaziv(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            return Db.Podrucja
                .Where(p => p.DeletedAt == null && p.IdPodrucje == id.Value)
                .Select(p => p.Naziv)
                .FirstOrDefault();
        }
    }
}
