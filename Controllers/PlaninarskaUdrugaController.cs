using Microsoft.AspNetCore.Mvc;
using planinarenje.Entiteti;
using planinarenje.Models;
using System.Linq;

namespace planinarenje.Controllers;

public class PlaninarskaUdrugaController : Controller
{
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
        var podaci = Lab1PodaciFactory.Kreiraj();

        var model = podaci.PlaninarskeUdruge
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
        var podaci = Lab1PodaciFactory.Kreiraj();
        var u = podaci.PlaninarskeUdruge.FirstOrDefault(x => x.IdPlaninarskaUdruga == id);

        if (u == null) return NotFound();

        var objekti = podaci.PlaninarskiObjekti
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