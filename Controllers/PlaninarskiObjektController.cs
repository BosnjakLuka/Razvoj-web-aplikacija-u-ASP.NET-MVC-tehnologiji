using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;

namespace planinarenje.Controllers;

public class PlaninarskiObjektController : Controller
{
    private readonly PlaninarstvoDbContext _dbContext;

    public PlaninarskiObjektController(PlaninarstvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IActionResult Index()
    {
        var model = _dbContext.PlaninarskiObjekti
            .Include(po => po.Podrucje)
            .Include(po => po.PlaninarskaUdruga)
            .OrderBy(po => po.Naziv)
            .AsEnumerable()
            .Select(po => new PlaninarskiObjektIndexCardViewModel
            {
                IdPlaninarskiObjekt = po.IdPlaninarskiObjekt,
                Naziv = po.Naziv,
                TipObjektaNaziv = DajNazivTipa(po.TipObjekta).ToUpper(), // uppercase per design
                PodrucjeNaziv = po.Podrucje?.Naziv ?? "NOVO PODRUCJE",
                UdrugaNaziv = po.PlaninarskaUdruga?.Naziv,
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
        var objekt = _dbContext.PlaninarskiObjekti
            .Include(po => po.Podrucje)
            .Include(po => po.PlaninarskaUdruga)
            .FirstOrDefault(po => po.IdPlaninarskiObjekt == id);

        if (objekt == null)
            return NotFound();

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