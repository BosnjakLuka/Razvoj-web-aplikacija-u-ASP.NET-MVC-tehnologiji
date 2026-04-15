using Microsoft.AspNetCore.Mvc;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Repositories;

namespace planinarenje.Controllers
{
    public class KontrolnaTockaController : Controller
    {
        private readonly IKontrolnaTockaMockRepository _kontrolnaTockaRepository;
        private readonly IPodrucjeMockRepository _podrucjeRepository;

        public KontrolnaTockaController(
            IKontrolnaTockaMockRepository kontrolnaTockaRepository,
            IPodrucjeMockRepository podrucjeRepository)
        {
            _kontrolnaTockaRepository = kontrolnaTockaRepository;
            _podrucjeRepository = podrucjeRepository;
        }

        public IActionResult Index()
        {
            var podrucjaById = _podrucjeRepository.GetAll().ToDictionary(p => p.IdPodrucje, p => p.Naziv);

            var model = _kontrolnaTockaRepository.GetAll()
                .OrderBy(k => k.Naziv)
                .Select(k => new KontrolnaTockaIndexCardViewModel
                {
                    IdKontrolnaTocka = k.IdKontrolnaTocka,
                    Naziv = k.Naziv,
                    TipKontrolneTockeNaziv = MapTip(k.TipKontrolneTocke),
                    NadmorskaVisina = k.NadmorskaVisina,
                    PodrucjeNaziv = podrucjaById.TryGetValue(k.IdPodrucje, out var nazivPodrucja) ? nazivPodrucja : "Nepoznato podrucje",
                    OpisPreview = TrimOpis(k.Opis, 140),
                    GUIDOznaka = k.GUIDOznaka
                })
                .ToList();

            ViewData["Title"] = "Kontrolne tocke";
            return View(model);
        }

        public IActionResult Details(int id)
        {
            var kontrolnaTocka = _kontrolnaTockaRepository.GetById(id);
            if (kontrolnaTocka is null)
            {
                return NotFound();
            }

            kontrolnaTocka.Podrucje = _podrucjeRepository.GetById(kontrolnaTocka.IdPodrucje);
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
    }
}
