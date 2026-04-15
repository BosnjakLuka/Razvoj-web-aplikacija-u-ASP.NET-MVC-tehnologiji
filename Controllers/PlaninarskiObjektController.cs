using Microsoft.AspNetCore.Mvc;
using planinarenje.Entiteti;
using planinarenje.Models;

namespace planinarenje.Controllers;

public class PlaninarskiObjektController : Controller
{
    public IActionResult Index()
    {
        var podaci = Lab1PodaciFactory.Kreiraj();

        var model = podaci.PlaninarskiObjekti
            .OrderBy(po => po.Naziv)
            .Select(po => new PlaninarskiObjektIndexCardViewModel
            {
                IdPlaninarskiObjekt = po.IdPlaninarskiObjekt,
                Naziv = po.Naziv,
                TipObjektaNaziv = DajNazivTipa(po.TipObjekta).ToUpper(), // uppercase per design
                PodrucjeNaziv = podaci.Podrucja.FirstOrDefault(p => p.IdPodrucje == po.IdPodrucje)?.Naziv ?? "NOVO PODRUČJE",
                UdrugaNaziv = podaci.PlaninarskeUdruge.FirstOrDefault(u => u.IdPlaninarskaUdruga == po.IdPlaninarskaUdruga)?.Naziv,
                NadmorskaVisinaTekst = po.NadmorskaVisina.HasValue ? $"{po.NadmorskaVisina.Value} M N/V" : "NEMA VISINE",
                KapacitetTekst = po.Kapacitet.HasValue ? $"{po.Kapacitet.Value} mjesta" : "Kapacitet nepoznat",
                OdgovornaOsoba = po.ImeOdgovorneOsobe,
                ImaNocenje = po.ImaNocenje,
                ImaHranu = po.ImaHranu,
                OpisPreview = TrimOpis(po.Opis, 140)
            })
            .ToList();

        ViewData["Title"] = "Objekti";
        return View(model);
    }

    public IActionResult Details(int id)
    {
        var podaci = Lab1PodaciFactory.Kreiraj();
        var objekt = podaci.PlaninarskiObjekti.SingleOrDefault(po => po.IdPlaninarskiObjekt == id);

        if (objekt == null)
            return NotFound();

        // Bind related entities for details view
        objekt.Podrucje = podaci.Podrucja.SingleOrDefault(p => p.IdPodrucje == objekt.IdPodrucje);
        objekt.PlaninarskaUdruga = podaci.PlaninarskeUdruge.SingleOrDefault(u => u.IdPlaninarskaUdruga == objekt.IdPlaninarskaUdruga);

        ViewData["Title"] = objekt.Naziv;
        return View(objekt);
    }

    private static string DajNazivTipa(TipObjekta tip)
    {
        return tip switch
        {
            TipObjekta.Dom => "Planinarski dom",
            TipObjekta.Kuca => "Planinarska kuća",
            TipObjekta.Skloniste => "Planinarsko sklonište",
            _ => "Objekt"
        };
    }

    private static string? TrimOpis(string? opis, int max)
    {
        if (string.IsNullOrWhiteSpace(opis)) return null;
        var o = opis.Trim();
        if (o.Length <= max) return o;
        return o[..max].TrimEnd() + "...";
    }
}