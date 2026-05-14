using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Models.ViewModels;
using System.Linq;

namespace planinarenje.Controllers;

public class PlaninarskaUdrugaController : Controller
{
    private readonly PlaninarstvoDbContext _dbContext;

    public PlaninarskaUdrugaController(PlaninarstvoDbContext dbContext)
    {
        _dbContext = dbContext;
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
        var model = BuildIndexModel(null);
        ViewData["Title"] = "Udruge";
        return View(model);
    }

    [HttpGet]
    public IActionResult Search(string? searchTerm)
    {
        var model = BuildIndexModel(searchTerm);
        return PartialView("_PlaninarskaUdrugaListPartial", model);
    }

    [HttpGet]
    public IActionResult AutocompleteSearch(string term)
    {
        var results = _dbContext.PlaninarskeUdruge
            .Where(u => u.DeletedAt == null && u.Naziv.Contains(term))
            .OrderBy(u => u.Naziv)
            .Take(15)
            .Select(u => new
            {
                value = u.IdPlaninarskaUdruga,
                label = u.Naziv
            })
            .ToList();

        return Json(results);
    }

    public IActionResult Create()
    {
        ViewData["Title"] = "Nova udruga";
        return View(new PlaninarskaUdrugaCreateModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(PlaninarskaUdrugaCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Nova udruga";
            return View(model);
        }

        var entity = new PlaninarskaUdruga
        {
            OIB = model.OIB,
            Naziv = model.Naziv,
            Email = model.Email,
            BrojTelefona = model.BrojTelefona,
            Adresa = model.Adresa,
            PostanskiBroj = model.PostanskiBroj,
            Grad = model.Grad,
            Zupanija = model.Zupanija,
            BrojClanova = model.BrojClanova
        };

        _dbContext.PlaninarskeUdruge.Add(entity);
        _dbContext.SaveChanges();
        TempData["Success"] = "Udruga je uspjesno dodana.";
        return RedirectToAction(nameof(Index));
    }

    [ActionName("Edit")]
    public IActionResult EditGet(int id)
    {
        var entity = _dbContext.PlaninarskeUdruge
            .FirstOrDefault(u => u.IdPlaninarskaUdruga == id && u.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        var model = new PlaninarskaUdrugaEditModel
        {
            OIB = entity.OIB,
            Naziv = entity.Naziv,
            Email = entity.Email,
            BrojTelefona = entity.BrojTelefona,
            Adresa = entity.Adresa,
            PostanskiBroj = entity.PostanskiBroj,
            Grad = entity.Grad,
            Zupanija = entity.Zupanija,
            BrojClanova = entity.BrojClanova
        };

        ViewData["Title"] = "Uredi udrugu";
        return View(model);
    }

    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public IActionResult EditPost(int id, PlaninarskaUdrugaEditModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Uredi udrugu";
            return View(model);
        }

        var entity = _dbContext.PlaninarskeUdruge
            .FirstOrDefault(u => u.IdPlaninarskaUdruga == id && u.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        entity.OIB = model.OIB;
        entity.Naziv = model.Naziv;
        entity.Email = model.Email;
        entity.BrojTelefona = model.BrojTelefona;
        entity.Adresa = model.Adresa;
        entity.PostanskiBroj = model.PostanskiBroj;
        entity.Grad = model.Grad;
        entity.Zupanija = model.Zupanija;
        entity.BrojClanova = model.BrojClanova;

        _dbContext.SaveChanges();
        TempData["Success"] = "Udruga je uspjesno azurirana.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var entity = _dbContext.PlaninarskeUdruge
            .FirstOrDefault(u => u.IdPlaninarskaUdruga == id && u.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        ViewData["Title"] = "Obrisi udrugu";
        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var entity = _dbContext.PlaninarskeUdruge
            .FirstOrDefault(u => u.IdPlaninarskaUdruga == id && u.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        entity.DeletedAt = DateTime.UtcNow;
        _dbContext.SaveChanges();
        TempData["Success"] = "Udruga je uspjesno obrisana.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Details(int id)
    {
        var u = _dbContext.PlaninarskeUdruge
            .Include(x => x.PlaninarskiObjekti)
            .FirstOrDefault(x => x.IdPlaninarskaUdruga == id && x.DeletedAt == null);

        if (u == null) return NotFound();

        var objekti = u.PlaninarskiObjekti
            .Where(o => o.DeletedAt == null)
            .Select(o => new ObjektUdrugeViewModel
            {
                IdPlaninarskiObjekt = o.IdPlaninarskiObjekt,
                Naziv = o.Naziv,
                TipObjekta = FormatirajTipObjekta(o.TipObjekta),
                NadmorskaVisina = o.NadmorskaVisina,
                ImaNocenje = o.ImaNocenje
            })
            .ToList();

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

    private List<PlaninarskaUdrugaIndexViewModel> BuildIndexModel(string? searchTerm)
    {
        var query = _dbContext.PlaninarskeUdruge
            .Where(u => u.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(u =>
                u.Naziv.Contains(term) ||
                u.OIB.Contains(term) ||
                (u.Grad != null && u.Grad.Contains(term)) ||
                (u.Zupanija != null && u.Zupanija.Contains(term)));
        }

        return query
            .OrderBy(u => u.Naziv)
            .Select(u => new PlaninarskaUdrugaIndexViewModel
            {
                IdPlaninarskaUdruga = u.IdPlaninarskaUdruga,
                Naziv = u.Naziv,
                OIB = u.OIB,
                Grad = u.Grad,
                Zupanija = u.Zupanija,
                BrojClanova = u.BrojClanova
            })
            .ToList();
    }
}