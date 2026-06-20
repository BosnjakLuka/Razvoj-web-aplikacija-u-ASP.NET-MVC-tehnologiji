using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Models.ViewModels;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace planinarenje.Controllers
{
    public class KorisnikController : BaseController
    {
        private readonly ILogger<KorisnikController> _logger;

        public KorisnikController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db, ILogger<KorisnikController> logger)
            : base(userMgr, db)
        {
            _logger = logger;
        }

        // Helpar za dohvacanje puta slike ako je string pun
        private string? FormatProfileSlika(string? absolutePath)
        {
            if (string.IsNullOrWhiteSpace(absolutePath)) return null;

            if (absolutePath.StartsWith("/Slike/Profil/", StringComparison.OrdinalIgnoreCase))
            {
                return absolutePath;
            }

            if (absolutePath.Contains("\\Slike\\Profil\\", StringComparison.OrdinalIgnoreCase) ||
                absolutePath.Contains("/Slike/Profil/", StringComparison.OrdinalIgnoreCase))
            {
                return "/Slike/Profil/" + Path.GetFileName(absolutePath);
            }

            return absolutePath;
        }

        private bool DodajGreskeZaDuplikatKorisnika(string email, string korisnickoIme, int? excludeId = null)
        {
            var postojiEmail = Db.Korisnici.Any(k =>
                k.Email == email && (!excludeId.HasValue || k.IdKorisnik != excludeId.Value));

            var postojiKorisnickoIme = Db.Korisnici.Any(k =>
                k.KorisnickoIme == korisnickoIme && (!excludeId.HasValue || k.IdKorisnik != excludeId.Value));

            if (postojiEmail)
            {
                ModelState.AddModelError(nameof(KorisnikCreateModel.Email), "Korisnik s ovim emailom već postoji.");
            }

            if (postojiKorisnickoIme)
            {
                ModelState.AddModelError(nameof(KorisnikCreateModel.KorisnickoIme), "Korisnik s ovim korisničkim imenom već postoji.");
            }

            return postojiEmail || postojiKorisnickoIme;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var model = BuildIndexModel(null);
            ViewData["Title"] = "Korisnici";
            return View(model);
        }

        [HttpGet]
        public IActionResult Search(string? searchTerm)
        {
            var model = BuildIndexModel(searchTerm);
            return PartialView("_KorisnikListPartial", model);
        }

        [HttpGet]
        public IActionResult AutocompleteSearch(string term)
        {
            var results = Db.Korisnici
                .Where(k => k.StatusAktivan &&
                            (k.Ime.Contains(term) || k.Prezime.Contains(term) || k.KorisnickoIme.Contains(term)))
                .OrderBy(k => k.Prezime)
                .ThenBy(k => k.Ime)
                .Take(15)
                .Select(k => new
                {
                    value = k.IdKorisnik,
                    label = k.Ime + " " + k.Prezime + " (@" + k.KorisnickoIme + ")"
                })
                .ToList();

            return Json(results);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["Title"] = "Novi korisnik";
            return View(new KorisnikCreateModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(KorisnikCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Novi korisnik";
                return View(model);
            }

            if (DodajGreskeZaDuplikatKorisnika(model.Email, model.KorisnickoIme))
            {
                ViewData["PopupWarning"] = "Korisnik s ovim podacima već postoji. Promijeni email adresu i korisničko ime.";
                ViewData["Title"] = "Novi korisnik";
                return View(model);
            }

            var korisnik = new Korisnik
            {
                Ime = model.Ime,
                Prezime = model.Prezime,
                Email = model.Email,
                KorisnickoIme = model.KorisnickoIme,
                BrojMobitela = model.BrojMobitela,
                DatumRodenja = model.DatumRodenja,
                DatumRegistracije = DateTime.UtcNow,
                StatusAktivan = true
            };

            try
            {
                Db.Korisnici.Add(korisnik);
                await Db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!DodajGreskeZaDuplikatKorisnika(model.Email, model.KorisnickoIme))
                {
                    ViewData["PopupWarning"] = "Došlo je do pogreške pri spremanju korisnika.";
                    ModelState.AddModelError(string.Empty, "Došlo je do pogreške pri spremanju korisnika.");
                }

                ViewData["Title"] = "Novi korisnik";
                return View(model);
            }

            _logger.LogInformation("Korisnik {IdKorisnik} kreiran ({KorisnickoIme}).", korisnik.IdKorisnik, korisnik.KorisnickoIme);
            TempData["NewId"] = korisnik.IdKorisnik;
            TempData["Success"] = "Korisnik je uspjesno dodan.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [ActionName("Edit")]
        public async Task<IActionResult> EditGet(int id)
        {
            var korisnik = await Db.Korisnici
                .FirstOrDefaultAsync(k => k.IdKorisnik == id && k.StatusAktivan);
            if (korisnik == null)
            {
                return NotFound();
            }

            if (!IsAdmin && !await IsOwnerAsync(id))
                return Forbid();

            var model = new KorisnikEditModel
            {
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                Email = korisnik.Email,
                KorisnickoIme = korisnik.KorisnickoIme,
                BrojMobitela = korisnik.BrojMobitela,
                DatumRodenja = korisnik.DatumRodenja
            };

            ViewData["Title"] = "Uredi korisnika";
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditPost(int id, KorisnikEditModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Uredi korisnika";
                return View(model);
            }

            var korisnik = await Db.Korisnici
                .FirstOrDefaultAsync(k => k.IdKorisnik == id && k.StatusAktivan);
            if (korisnik == null)
            {
                return NotFound();
            }

            if (DodajGreskeZaDuplikatKorisnika(model.Email, model.KorisnickoIme, id))
            {
                ViewData["PopupWarning"] = "Korisnik s ovim podacima već postoji. Promijeni email adresu i korisničko ime.";
                ViewData["Title"] = "Uredi korisnika";
                return View(model);
            }

            // ownership check
            if (!IsAdmin && !await IsOwnerAsync(id))
            {
                _logger.LogWarning("Korisnik {AppUserId} bez prava pristupa pokušao je urediti korisnika {IdKorisnik}.", AppUserId, id);
                return Forbid();
            }

            korisnik.Ime = model.Ime;
            korisnik.Prezime = model.Prezime;
            korisnik.Email = model.Email;
            korisnik.KorisnickoIme = model.KorisnickoIme;
            korisnik.BrojMobitela = model.BrojMobitela;
            korisnik.DatumRodenja = model.DatumRodenja;

            try
            {
                await Db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!DodajGreskeZaDuplikatKorisnika(model.Email, model.KorisnickoIme, id))
                {
                    ViewData["PopupWarning"] = "Došlo je do pogreške pri spremanju korisnika.";
                    ModelState.AddModelError(string.Empty, "Došlo je do pogreške pri spremanju korisnika.");
                }

                ViewData["Title"] = "Uredi korisnika";
                return View(model);
            }

            _logger.LogInformation("Korisnik {IdKorisnik} ažuriran.", id);
            TempData["Success"] = "Korisnik je uspjesno azuriran.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var korisnik = await Db.Korisnici
                .FirstOrDefaultAsync(k => k.IdKorisnik == id && k.StatusAktivan);
            if (korisnik == null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Obrisi korisnika";
            return View(korisnik);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var korisnik = await Db.Korisnici
                .FirstOrDefaultAsync(k => k.IdKorisnik == id && k.StatusAktivan);
            if (korisnik == null)
            {
                return NotFound();
            }

            korisnik.StatusAktivan = false;
            await Db.SaveChangesAsync();
            _logger.LogInformation("Korisnik {IdKorisnik} obrisan (soft delete).", id);
            TempData["Success"] = "Korisnik je uspjesno obrisan.";
            return RedirectToAction(nameof(Index));
        }

        [Route("planinar/{id:int}")]
        [Route("[controller]/[action]/{id:int}")]
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var korisnik = await Db.Korisnici
                .Include(k => k.Knjizica)
                .FirstOrDefaultAsync(k => k.IdKorisnik == id && k.StatusAktivan);

            if (korisnik == null)
            {
                return NotFound();
            }

            if (!IsAdmin && !await IsOwnerAsync(id))
                return Forbid();

            var model = new KorisnikDetailsViewModel
            {
                IdKorisnik = korisnik.IdKorisnik,
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                Email = korisnik.Email,
                KorisnickoIme = korisnik.KorisnickoIme,
                DatumRodenja = korisnik.DatumRodenja,
                DatumRegistracije = korisnik.DatumRegistracije,
                BrojMobitela = korisnik.BrojMobitela,
                ProfilnaSlika = FormatProfileSlika(korisnik.ProfilnaSlika),
                StatusAktivan = korisnik.StatusAktivan,
                Knjizica = korisnik.Knjizica != null ? new KnjizicaViewModel
                {
                    IdKnjizica = korisnik.Knjizica.IdKnjizica,
                    NazivAplikacije = "Digitalna planinarska knjižica",
                    DatumIzdavanja = korisnik.Knjizica.DatumKreiranja
                } : null,
                Posjeti = new List<PosjetViewModel>()
            };
            
            model.Posjeti = Db.Posjeti
                .Where(p => p.IdKorisnik == id && p.DeletedAt == null)
                .Include(p => p.KontrolnaTocka)
                .Include(p => p.Ruta)
                .Include(p => p.Fotografije)
                .OrderByDescending(p => p.DatumVrijemePosjeta)
                .AsEnumerable()
                .Select(p => new PosjetViewModel
                {
                    IdPosjet = p.IdPosjet,
                    DatumPosjeta = p.DatumVrijemePosjeta,
                    NazivKontrolneTocke = p.KontrolnaTocka != null ? p.KontrolnaTocka.Naziv : "Nepoznato",
                    SlikaUrl = p.Fotografije.Where(f => f.DeletedAt == null).Select(f => f.PutanjaDatoteke).FirstOrDefault() ?? "/Slike/Dizajn/AppThemeBase.jpg",
                    NazivRute = p.Ruta != null ? p.Ruta.Naziv : "Samostalni posjet"
                })
                .ToList();

            model.Medalje = Db.KorisnikMedalje
                .Where(km => km.IdKorisnik == id && km.DeletedAt == null)
                .Include(km => km.Medalja)
                .OrderByDescending(km => km.DatumDodjele)
                .AsEnumerable()
                .Select(km => new KorisnikMedaljaViewModel
                {
                    NazivMedalje = km.Medalja != null ? km.Medalja.Naziv : "Nepoznato",
                    DatumOsvajanja = km.DatumDodjele,
                    Opis = km.Medalja?.Opis ?? "N/A"
                })
                .ToList();

            return View(model);
        }

        private List<KorisnikIndexCardViewModel> BuildIndexModel(string? searchTerm)
        {
            var query = Db.Korisnici
                .Where(k => k.StatusAktivan);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(k =>
                    k.Ime.Contains(term) ||
                    k.Prezime.Contains(term) ||
                    k.Email.Contains(term) ||
                    k.KorisnickoIme.Contains(term));
            }

            return query
                .OrderBy(k => k.Prezime)
                .ThenBy(k => k.Ime)
                .Select(k => new KorisnikIndexCardViewModel
                {
                    IdKorisnik = k.IdKorisnik,
                    Ime = k.Ime,
                    Prezime = k.Prezime,
                    Email = k.Email,
                    KorisnickoIme = k.KorisnickoIme,
                    DatumRegistracije = k.DatumRegistracije,
                    StatusAktivan = k.StatusAktivan
                })
                .ToList();
        }
    }
}