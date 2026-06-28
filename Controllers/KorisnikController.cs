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
        private readonly IWebHostEnvironment _env;

        private static readonly string[] DopusteneEkstenzijeSlike = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxVelicinaSlike = 5 * 1024 * 1024; // 5 MB

        public KorisnikController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db, IWebHostEnvironment env, ILogger<KorisnikController> logger)
            : base(userMgr, db)
        {
            _env = env;
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

            var jeVlasnikIliAdmin = IsAdmin || await IsOwnerAsync(id);
            if (!jeVlasnikIliAdmin)
                return Forbid();

            string? oib = null, jmbg = null;
            var uloga = "Korisnik";
            if (!string.IsNullOrEmpty(korisnik.AppUserId))
            {
                var appUser = await UserMgr.FindByIdAsync(korisnik.AppUserId);
                oib = appUser?.OIB;
                jmbg = appUser?.JMBG;
                if (appUser != null)
                {
                    var korisnikoveUloge = await UserMgr.GetRolesAsync(appUser);
                    if (korisnikoveUloge.Contains("Admin")) uloga = "Admin";
                    else if (korisnikoveUloge.Contains("Planinar")) uloga = "Planinar";
                }
            }

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
                OIB = oib,
                JMBG = jmbg,
                ProfilnaSlika = FormatProfileSlika(korisnik.ProfilnaSlika),
                StatusAktivan = korisnik.StatusAktivan,
                MozeUredivati = jeVlasnikIliAdmin,
                Uloga = uloga,
                MozePromijeniUlogu = IsAdmin,
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> UrediOsobnePodatke(int id, string? brojMobitela, DateTime? datumRodenja, string? oib, string? jmbg)
        {
            var korisnik = await Db.Korisnici.FirstOrDefaultAsync(k => k.IdKorisnik == id && k.StatusAktivan);
            if (korisnik == null) return NotFound();

            if (!IsAdmin && !await IsOwnerAsync(id))
                return Forbid();

            if (!string.IsNullOrWhiteSpace(oib) && (oib.Length != 11 || !oib.All(char.IsDigit)))
            {
                TempData["Error"] = "OIB mora imati točno 11 znamenki.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (!string.IsNullOrWhiteSpace(jmbg) && (jmbg.Length != 13 || !jmbg.All(char.IsDigit)))
            {
                TempData["Error"] = "JMBG mora imati točno 13 znamenki.";
                return RedirectToAction(nameof(Details), new { id });
            }

            korisnik.BrojMobitela = string.IsNullOrWhiteSpace(brojMobitela) ? null : brojMobitela.Trim();
            korisnik.DatumRodenja = datumRodenja;
            await Db.SaveChangesAsync();

            if (!string.IsNullOrEmpty(korisnik.AppUserId))
            {
                var appUser = await UserMgr.FindByIdAsync(korisnik.AppUserId);
                if (appUser != null)
                {
                    appUser.OIB = oib?.Trim() ?? string.Empty;
                    appUser.JMBG = jmbg?.Trim() ?? string.Empty;
                    await UserMgr.UpdateAsync(appUser);
                }
            }

            _logger.LogInformation("Korisnik {IdKorisnik} ažurirao osobne podatke.", id);
            TempData["Success"] = "Osobni podaci su uspjesno azurirani.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // Samo Admin smije mijenjati ulogu drugom korisniku (Korisnik -> Planinar -> Admin).
        private static readonly string[] DopusteneUloge = { "Korisnik", "Planinar", "Admin" };

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PromijeniUlogu(int id, string uloga)
        {
            if (!DopusteneUloge.Contains(uloga))
            {
                TempData["Error"] = "Nepoznata uloga.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var korisnik = await Db.Korisnici.FirstOrDefaultAsync(k => k.IdKorisnik == id && k.StatusAktivan);
            if (korisnik == null || string.IsNullOrEmpty(korisnik.AppUserId))
            {
                return NotFound();
            }

            var appUser = await UserMgr.FindByIdAsync(korisnik.AppUserId);
            if (appUser == null)
            {
                return NotFound();
            }

            var postojeceUloge = await UserMgr.GetRolesAsync(appUser);
            await UserMgr.RemoveFromRolesAsync(appUser, postojeceUloge);
            await UserMgr.AddToRoleAsync(appUser, uloga);

            _logger.LogInformation("Admin {AppUserId} je promijenio ulogu korisnika {IdKorisnik} u {Uloga}.", AppUserId, id, uloga);
            TempData["Success"] = "Uloga korisnika je uspjesno promijenjena.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> UploadSlika(int id, IFormFile? file)
        {
            var korisnik = await Db.Korisnici.FirstOrDefaultAsync(k => k.IdKorisnik == id && k.StatusAktivan);
            if (korisnik == null) return NotFound();

            if (!IsAdmin && !await IsOwnerAsync(id))
                return Forbid();

            if (file == null || file.Length == 0)
            {
                return BadRequest(new { success = false, error = "Datoteka nije priložena." });
            }

            if (file.Length > MaxVelicinaSlike)
            {
                return BadRequest(new { success = false, error = "Datoteka je prevelika (maksimalno 5 MB)." });
            }

            var ekstenzija = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!DopusteneEkstenzijeSlike.Contains(ekstenzija))
            {
                return BadRequest(new { success = false, error = "Dopušteni su samo formati: JPG, PNG, WEBP." });
            }

            if (string.IsNullOrEmpty(file.ContentType) ||
                !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { success = false, error = "Datoteka mora biti slika." });
            }

            var webRoot = !string.IsNullOrEmpty(_env.WebRootPath) ? _env.WebRootPath : Path.Combine(_env.ContentRootPath, "wwwroot");
            var apsolutniFolder = Path.Combine(webRoot, "uploads", "korisnici", id.ToString());
            Directory.CreateDirectory(apsolutniFolder);

            var spremljenoIme = $"{Guid.NewGuid()}{ekstenzija}";
            var apsolutnaPutanja = Path.Combine(apsolutniFolder, spremljenoIme);

            using (var stream = new FileStream(apsolutnaPutanja, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            korisnik.ProfilnaSlika = $"/uploads/korisnici/{id}/{spremljenoIme}";
            await Db.SaveChangesAsync();

            _logger.LogInformation("Korisnik {IdKorisnik} ažurirao profilnu sliku.", id);
            return Json(new { success = true, url = korisnik.ProfilnaSlika });
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