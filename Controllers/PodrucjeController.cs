using Microsoft.AspNetCore.Mvc;
using planinarenje.Entiteti;
using planinarenje.Models;

namespace planinarenje.Controllers
{
    public class PodrucjeController : Controller
    {
        public IActionResult Index()
        {
            var podaci = Lab1PodaciFactory.Kreiraj();

            var model = podaci.Podrucja
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
            var podaci = Lab1PodaciFactory.Kreiraj();
            var podrucje = podaci.Podrucja.SingleOrDefault(p => p.IdPodrucje == id);
            if (podrucje is null)
            {
                return NotFound();
            }

            podrucje.KontrolneTocke = podaci.KontrolneTocke
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
