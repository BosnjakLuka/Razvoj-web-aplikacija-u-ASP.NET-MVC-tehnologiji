using Microsoft.AspNetCore.Mvc;
using planinarenje.Entiteti;
using planinarenje.Models;

namespace planinarenje.Controllers
{
    public class KontrolnaTockaController : Controller
    {
        public IActionResult Index()
        {
            var podaci = Lab1PodaciFactory.Kreiraj();
            var podrucjaById = podaci.Podrucja.ToDictionary(p => p.IdPodrucje, p => p.Naziv);

            var model = podaci.KontrolneTocke
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
            var podaci = Lab1PodaciFactory.Kreiraj();
            var kontrolnaTocka = podaci.KontrolneTocke.SingleOrDefault(k => k.IdKontrolnaTocka == id);
            if (kontrolnaTocka is null)
            {
                return NotFound();
            }

            kontrolnaTocka.Podrucje = podaci.Podrucja.SingleOrDefault(p => p.IdPodrucje == kontrolnaTocka.IdPodrucje);
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
