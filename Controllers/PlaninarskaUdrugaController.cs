using Microsoft.AspNetCore.Mvc;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Repositories;
using System.Linq;

namespace planinarenje.Controllers;

public class PlaninarskaUdrugaController : Controller
{
    private readonly IPlaninarskaUdrugaMockRepository _udrugaRepository;
    private readonly IPlaninarskiObjektMockRepository _objektRepository;

    public PlaninarskaUdrugaController(IPlaninarskaUdrugaMockRepository udrugaRepository, IPlaninarskiObjektMockRepository objektRepository)
    {
        _udrugaRepository = udrugaRepository;
        _objektRepository = objektRepository;
    }

    private string FormatirajTipObjekta(TipObjekta tip)
    {
        return tip switch
        {
            TipObjekta.Dom => "Planinarski dom",
            TipObjekta.Kuca => "Planinarska kuća",
            TipObjekta.Skloniste => "Planinarsko sklonište",
            _ => "Nepoznato"
        };
    }

    public IActionResult Index()
    {
        var model = _udrugaRepository.GetAll()
            .OrderBy(u => u.Naziv)
            .Select(u => new PlaninarskaUdrugaIndexViewModel
            {
                IdPlaninarskaUdruga = u.IdPlaninarskaUdruga,
                Naziv = u.Naziv,
                OIB = u.OIB,
                Grad = u.Grad,
                Zupanija = u.Zupanija,
                BrojClanova = u.BrojClanova
            }).ToList();

        return View(model);
    }

    public IActionResult Details(int id)
    {
        var u = _udrugaRepository.GetById(id);

        if (u == null) return NotFound();

        var objekti = _objektRepository.GetAll()
            .Where(o => o.IdPlaninarskaUdruga == id)
            .Select(o => new ObjektUdrugeViewModel
            {
                IdPlaninarskiObjekt = o.IdPlaninarskiObjekt,
                Naziv = o.Naziv,
                TipObjekta = FormatirajTipObjekta(o.TipObjekta),
                NadmorskaVisina = o.NadmorskaVisina,
                ImaNocenje = o.ImaNocenje
            }).ToList();

        var model = new PlaninarskaUdrugaDetailsViewModel
        {
            IdPlaninarskaUdruga = u.IdPlaninarskaUdruga,
            Naziv = u.Naziv,
            OIB = u.OIB,
            Email = u.Email,
            BrojTelefona = u.BrojTelefona,
            Adresa = u.Adresa,
            PostanskiBroj = u.PostanskiBroj,
            Grad = u.Grad,
            Zupanija = u.Zupanija,
            BrojClanova = u.BrojClanova,
            PlaninarskiObjekti = objekti
        };

        return View(model);
    }
}