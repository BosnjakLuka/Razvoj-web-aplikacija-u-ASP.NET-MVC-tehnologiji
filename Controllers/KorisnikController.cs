using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
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
            var model = _dbContext.Korisnici
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
                }).ToList();

            return View(model);
        }

        public IActionResult Details(int id)
        {
            var korisnik = _dbContext.Korisnici
                .Include(k => k.Knjizica)
                .FirstOrDefault(k => k.IdKorisnik == id);

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
                .Where(p => p.IdKorisnik == id)
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
                    SlikaUrl = p.Fotografije.Select(f => f.PutanjaDatoteke).FirstOrDefault() ?? "/Slike/Dizajn/AppThemeBase.jpg",
                    NazivRute = p.Ruta != null ? p.Ruta.Naziv : "Samostalni posjet"
                })
                .ToList();

            model.Medalje = _dbContext.KorisnikMedalje
                .Where(km => km.IdKorisnik == id)
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
    }
}