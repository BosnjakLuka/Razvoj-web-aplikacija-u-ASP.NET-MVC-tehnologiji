using Microsoft.AspNetCore.Mvc;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Repositories;

namespace planinarenje.Controllers;

public class PlaninarskiObjektController : Controller
{
    private readonly IPlaninarskiObjektMockRepository _planinarskiObjektRepository;
    private readonly IPodrucjeMockRepository _podrucjeRepository;
    private readonly IPlaninarskaUdrugaMockRepository _udrugaRepository;

    public PlaninarskiObjektController(
        IPlaninarskiObjektMockRepository planinarskiObjektRepository,
        IPodrucjeMockRepository podrucjeRepository,
        IPlaninarskaUdrugaMockRepository udrugaRepository)
    {
        _planinarskiObjektRepository = planinarskiObjektRepository;
        _podrucjeRepository = podrucjeRepository;
        _udrugaRepository = udrugaRepository;
    }

    public IActionResult Index()
    {
        var podrucja = _podrucjeRepository.GetAll();
        var udruge = _udrugaRepository.GetAll();

        var model = _planinarskiObjektRepository.GetAll()
            .OrderBy(po => po.Naziv)
            .Select(po => new PlaninarskiObjektIndexCardViewModel
            {
                IdPlaninarskiObjekt = po.IdPlaninarskiObjekt,
                Naziv = po.Naziv,
                TipObjektaNaziv = DajNazivTipa(po.TipObjekta).ToUpper(), // uppercase per design
                PodrucjeNaziv = podrucja.FirstOrDefault(p => p.IdPodrucje == po.IdPodrucje)?.Naziv ?? "NOVO PODRUCJE",
                UdrugaNaziv = udruge.FirstOrDefault(u => u.IdPlaninarskaUdruga == po.IdPlaninarskaUdruga)?.Naziv,
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
        var objekt = _planinarskiObjektRepository.GetById(id);

        if (objekt == null)
            return NotFound();

        // Bind related entities for details view
        objekt.Podrucje = _podrucjeRepository.GetById(objekt.IdPodrucje);
        objekt.PlaninarskaUdruga = _udrugaRepository.GetById(objekt.IdPlaninarskaUdruga);

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