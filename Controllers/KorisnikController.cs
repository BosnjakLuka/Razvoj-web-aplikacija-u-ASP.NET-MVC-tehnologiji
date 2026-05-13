using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Models.ViewModels;
using System.IO;
using System.Linq;

namespace planinarenje.Controllers
{
    public class KorisnikController : Controller
    {
        private readonly PlaninarstvoDbContext _dbContext;

        public KorisnikController(PlaninarstvoDbContext dbContext)
        {
            _dbContext = dbContext;
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

        public IActionResult Create()
        {
            ViewData["Title"] = "Novi korisnik";
            return View(new KorisnikCreateModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(KorisnikCreateModel model)
        {
            if (!ModelState.IsValid)
            {
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
                PasswordHash = "ChangeMe123!",
                StatusAktivan = true
            };

            _dbContext.Korisnici.Add(korisnik);
            _dbContext.SaveChanges();
            TempData["Success"] = "Korisnik je uspjesno dodan.";
            return RedirectToAction(nameof(Index));
        }

        [ActionName("Edit")]
        public IActionResult EditGet(int id)
        {
            var korisnik = _dbContext.Korisnici
                .FirstOrDefault(k => k.IdKorisnik == id && k.StatusAktivan);
            if (korisnik == null)
            {
                return NotFound();
            }

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
        public IActionResult EditPost(int id, KorisnikEditModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Uredi korisnika";
                return View(model);
            }

            var korisnik = _dbContext.Korisnici
                .FirstOrDefault(k => k.IdKorisnik == id && k.StatusAktivan);
            if (korisnik == null)
            {
                return NotFound();
            }

            korisnik.Ime = model.Ime;
            korisnik.Prezime = model.Prezime;
            korisnik.Email = model.Email;
            korisnik.KorisnickoIme = model.KorisnickoIme;
            korisnik.BrojMobitela = model.BrojMobitela;
            korisnik.DatumRodenja = model.DatumRodenja;

            _dbContext.SaveChanges();
            TempData["Success"] = "Korisnik je uspjesno azuriran.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var korisnik = _dbContext.Korisnici
                .FirstOrDefault(k => k.IdKorisnik == id && k.StatusAktivan);
            if (korisnik == null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Obrisi korisnika";
            return View(korisnik);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var korisnik = _dbContext.Korisnici
                .FirstOrDefault(k => k.IdKorisnik == id && k.StatusAktivan);
            if (korisnik == null)
            {
                return NotFound();
            }

            korisnik.StatusAktivan = false;
            _dbContext.SaveChanges();
            TempData["Success"] = "Korisnik je uspjesno obrisan.";
            return RedirectToAction(nameof(Index));
        }

        [Route("planinar/{id:int}")]
        [Route("[controller]/[action]/{id:int}")]
        public IActionResult Details(int id)
        {
            var korisnik = _dbContext.Korisnici
                .Include(k => k.Knjizica)
                .FirstOrDefault(k => k.IdKorisnik == id && k.StatusAktivan);

            if (korisnik == null)
            {
                return NotFound();
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
            
            model.Posjeti = _dbContext.Posjeti
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

            model.Medalje = _dbContext.KorisnikMedalje
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
            var query = _dbContext.Korisnici
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