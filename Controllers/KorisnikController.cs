using Microsoft.AspNetCore.Mvc;
using planinarenje.Entiteti;
using planinarenje.Models;
using System.Linq;

namespace planinarenje.Controllers
{
    public class KorisnikController : Controller
    {
        // Helpar za dohvacanje puta slike ako je string pun
        private string? FormatProfileSlika(string? absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath)) return null;
            var idx = absolutePath.IndexOf("Slike", StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                return "/" + absolutePath.Substring(idx).Replace("\\", "/");
            }
            return absolutePath;
        }

        public IActionResult Index()
        {
            var podaci = Lab1PodaciFactory.Kreiraj();

            var model = podaci.Korisnici
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
            var podaci = Lab1PodaciFactory.Kreiraj();
            var korisnik = podaci.Korisnici.FirstOrDefault(k => k.IdKorisnik == id);

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
            
            // To properly mock relations:
            model.Posjeti = podaci.Posjeti
                .Where(p => p.IdKorisnik == id)
                .OrderByDescending(p => p.DatumVrijemePosjeta)
                .Select(p => {
                    var kt = podaci.KontrolneTocke.FirstOrDefault(kt => kt.IdKontrolnaTocka == p.IdKontrolnaTocka);
                    var fotografija = podaci.Fotografije.FirstOrDefault(f => f.IdPosjet == p.IdPosjet);
                    return new PosjetViewModel
                    {
                        IdPosjet = p.IdPosjet,
                        DatumPosjeta = p.DatumVrijemePosjeta,
                        NazivKontrolneTocke = kt?.Naziv ?? "Nepoznato",
                        SlikaUrl = fotografija?.PutanjaDatoteke ?? "/Slike/Dizajn/AppThemeBase.jpg",
                        NazivRute = podaci.Rute.FirstOrDefault(r => r.IdRuta == p.IdRuta)?.Naziv ?? "Samostalni posjet"
                    };
                }).ToList();

            model.Medalje = podaci.KorisnikMedalje
                .Where(km => km.IdKorisnik == id)
                .OrderByDescending(km => km.DatumDodjele)
                .Select(km => {
                    var medalja = podaci.Medalje.FirstOrDefault(m => m.IdMedalja == km.IdMedalja);
                    return new KorisnikMedaljaViewModel
                    {
                        NazivMedalje = medalja?.Naziv ?? "Nepoznato",
                        DatumOsvajanja = km.DatumDodjele,
                        Opis = medalja?.Opis ?? "N/A"
                    };
                }).ToList();

            return View(model);
        }
    }
}