using Microsoft.AspNetCore.Mvc;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Repositories;
using System.Linq;

namespace planinarenje.Controllers
{
    public class KorisnikController : Controller
    {
        private readonly IKorisnikMockRepository _korisnikRepository;
        private readonly IPosjetMockRepository _posjetRepository;
        private readonly IKontrolnaTockaMockRepository _kontrolnaTockaRepository;
        private readonly IFotografijaMockRepository _fotografijaRepository;
        private readonly IRutaMockRepository _rutaRepository;
        private readonly IKorisnikMedaljaMockRepository _korisnikMedaljaRepository;
        private readonly IMedaljaMockRepository _medaljaRepository;

        public KorisnikController(
            IKorisnikMockRepository korisnikRepository,
            IPosjetMockRepository posjetRepository,
            IKontrolnaTockaMockRepository kontrolnaTockaRepository,
            IFotografijaMockRepository fotografijaRepository,
            IRutaMockRepository rutaRepository,
            IKorisnikMedaljaMockRepository korisnikMedaljaRepository,
            IMedaljaMockRepository medaljaRepository)
        {
            _korisnikRepository = korisnikRepository;
            _posjetRepository = posjetRepository;
            _kontrolnaTockaRepository = kontrolnaTockaRepository;
            _fotografijaRepository = fotografijaRepository;
            _rutaRepository = rutaRepository;
            _korisnikMedaljaRepository = korisnikMedaljaRepository;
            _medaljaRepository = medaljaRepository;
        }

        // Helpar za dohvacanje puta slike ako je string pun
        private string? FormatProfileSlika(string? absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath)) return null;
            var idx = absolutePath.IndexOf("Slike", StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                var relPath = "/" + absolutePath.Substring(idx).Replace("\\", "/");
                if (!System.IO.File.Exists(absolutePath))
                {
                    if (absolutePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                    {
                        var altPath = absolutePath.Substring(0, absolutePath.Length - 4) + ".jpeg";
                        if (System.IO.File.Exists(altPath))
                        {
                            return relPath.Substring(0, relPath.Length - 4) + ".jpeg";
                        }
                    }
                    return null; 
                }
                return relPath;
            }
            return absolutePath;
        }

        public IActionResult Index()
        {
            var model = _korisnikRepository.GetAll()
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
            var korisnik = _korisnikRepository.GetById(id);

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
            model.Posjeti = _posjetRepository.GetAll()
                .Where(p => p.IdKorisnik == id)
                .OrderByDescending(p => p.DatumVrijemePosjeta)
                .Select(p => {
                    var kt = _kontrolnaTockaRepository.GetById(p.IdKontrolnaTocka);
                    var fotografija = _fotografijaRepository.GetAll().FirstOrDefault(f => f.IdPosjet == p.IdPosjet);
                    return new PosjetViewModel
                    {
                        IdPosjet = p.IdPosjet,
                        DatumPosjeta = p.DatumVrijemePosjeta,
                        NazivKontrolneTocke = kt?.Naziv ?? "Nepoznato",
                        SlikaUrl = fotografija?.PutanjaDatoteke ?? "/Slike/Dizajn/AppThemeBase.jpg",
                        NazivRute = _rutaRepository.GetById(p.IdRuta)?.Naziv ?? "Samostalni posjet"
                    };
                }).ToList();

            model.Medalje = _korisnikMedaljaRepository.GetAll()
                .Where(km => km.IdKorisnik == id)
                .OrderByDescending(km => km.DatumDodjele)
                .Select(km => {
                    var medalja = _medaljaRepository.GetById(km.IdMedalja);
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