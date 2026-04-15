using Microsoft.AspNetCore.Mvc;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Repositories;

namespace planinarenje.Controllers
{
    public class PodrucjeController : Controller
    {
        private readonly IPodrucjeMockRepository _podrucjeRepository;
        private readonly IKontrolnaTockaMockRepository _kontrolnaTockaRepository;

        public PodrucjeController(IPodrucjeMockRepository podrucjeRepository, IKontrolnaTockaMockRepository kontrolnaTockaRepository)
        {
            _podrucjeRepository = podrucjeRepository;
            _kontrolnaTockaRepository = kontrolnaTockaRepository;
        }

        public IActionResult Index()
        {
            var model = _podrucjeRepository.GetAll()
                .OrderBy(p => p.IdPodrucje)
                .Select(p => new PodrucjeIndexCardViewModel
                {
                    IdPodrucje = p.IdPodrucje,
                    Naziv = p.Naziv,
                    Regija = string.IsNullOrWhiteSpace(p.Regija) ? "Regija nije navedena" : p.Regija,
                    OpisPreview = TrimOpis(p.Opis, 170),
                    MinimalanBrojKTZaObilazak = p.MinimalanBrojKTZaObilazak,
                    UkupanBrojKT = p.UkupanBrojKT
                })
                .ToList();

            ViewData["Title"] = "Podrucja";
            return View(model);
        }

        public IActionResult Details(int id)
        {
            var podrucje = _podrucjeRepository.GetById(id);
            if (podrucje is null)
            {
                return NotFound();
            }

            podrucje.KontrolneTocke = _kontrolnaTockaRepository.GetAll()
                .Where(kt => kt.IdPodrucje == podrucje.IdPodrucje)
                .OrderBy(kt => kt.Naziv)
                .ToList();

            ViewData["Title"] = podrucje.Naziv;
            return View(podrucje);
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
