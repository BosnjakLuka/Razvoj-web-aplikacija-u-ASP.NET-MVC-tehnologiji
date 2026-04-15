using Microsoft.AspNetCore.Mvc;
using planinarenje.Entiteti;
using planinarenje.Models;

namespace planinarenje.Controllers;

public class MedaljaController : Controller
{
    public IActionResult Index()
    {
        var podaci = Lab1PodaciFactory.Kreiraj();

        var model = podaci.Medalje
            .OrderBy(m => m.MinimalanBrojKontrolnihTocaka) // Order by difficulty
            .Select(m => new MedaljaIndexCardViewModel
            {
                IdMedalja = m.IdMedalja,
                Naziv = m.Naziv,
                OpisPreview = TrimOpis(m.Opis, 150),
                MinimalanBrojKontrolnihTocaka = m.MinimalanBrojKontrolnihTocaka,
                MinimalanBrojPodrucja = m.MinimalanBrojPodrucja,
                IkonaKlasa = OdrediIkonu(m.Naziv),
                BojaKlasa = OdrediBoju(m.Naziv)
            })
            .ToList();

        ViewData["Title"] = "Priznanja i Medalje";
        return View(model);
    }

    public IActionResult Details(int id)
    {
        var podaci = Lab1PodaciFactory.Kreiraj();
        var medalja = podaci.Medalje.SingleOrDefault(m => m.IdMedalja == id);

        if (medalja == null)
            return NotFound();

        ViewData["Title"] = medalja.Naziv;
        return View(medalja);
    }

    private static string? TrimOpis(string? opis, int max)
    {
        if (string.IsNullOrWhiteSpace(opis)) return null;
        var o = opis.Trim();
        if (o.Length <= max) return o;
        return o[..max].TrimEnd() + "...";
    }

    private static string OdrediIkonu(string naziv)
    {
        var n = naziv.ToLower();
        if (n.Contains("zlatn") || n.Contains("najvis")) return "bi-trophy-fill";
        if (n.Contains("visok")) return "bi-award-fill";
        return "bi-award";
    }

    private static string OdrediBoju(string naziv)
    {
        var n = naziv.ToLower();
        if (n.Contains("zlatn")) return "text-warning"; // Zlatna / žuta
        if (n.Contains("srebrn")) return "text-secondary"; // Srebrna / siva
        if (n.Contains("broncan")) return "text-danger"; // Brončana (najbliža out-of-box klasa ili ćemo koristiti inline kasnije, no idemo na danger zbog smedje-crvene)
        return "text-primary";
    }
}