using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace planinarenje.Controllers;

public class PlaninarskaUdrugaController : BaseController
{
    private readonly ILogger<PlaninarskaUdrugaController> _logger;

    public PlaninarskaUdrugaController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db, ILogger<PlaninarskaUdrugaController> logger)
        : base(userMgr, db)
    {
        _logger = logger;
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

    [AllowAnonymous]
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
        var results = Db.PlaninarskeUdruge
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

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        ViewData["Title"] = "Nova udruga";
        return View(new PlaninarskaUdrugaCreateModel());
    }

    private bool DodajGreskeZaDuplikatUdruge(string oib, int? excludeId = null)
    {
        var postoji = Db.PlaninarskeUdruge.Any(u =>
            u.OIB == oib && (!excludeId.HasValue || u.IdPlaninarskaUdruga != excludeId.Value));

        if (!postoji)
        {
            return false;
        }

        var poruka = "Udruga s ovim OIB-om već postoji. Promijeni OIB ili uređuj postojeću udrugu.";
        ModelState.AddModelError(nameof(PlaninarskaUdrugaCreateModel.OIB), poruka);
        ViewData["PopupWarning"] = poruka;
        return true;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(PlaninarskaUdrugaCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Nova udruga";
            return View(model);
        }

        if (DodajGreskeZaDuplikatUdruge(model.OIB))
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

        try
        {
            Db.PlaninarskeUdruge.Add(entity);
            await Db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ViewData["PopupWarning"] = "Udruga s ovim OIB-om već postoji. Promijeni OIB ili uređuj postojeću udrugu.";
            ViewData["Title"] = "Nova udruga";
            return View(model);
        }

        _logger.LogInformation("Planinarska udruga {IdPlaninarskaUdruga} kreirana ({Naziv}).", entity.IdPlaninarskaUdruga, entity.Naziv);
        TempData["NewId"] = entity.IdPlaninarskaUdruga;
        TempData["Success"] = "Udruga je uspjesno dodana.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [ActionName("Edit")]
    public async Task<IActionResult> EditGet(int id)
    {
        var entity = await Db.PlaninarskeUdruge
            .FirstOrDefaultAsync(u => u.IdPlaninarskaUdruga == id && u.DeletedAt == null);
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditPost(int id, PlaninarskaUdrugaEditModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Uredi udrugu";
            return View(model);
        }

        var entity = await Db.PlaninarskeUdruge
            .FirstOrDefaultAsync(u => u.IdPlaninarskaUdruga == id && u.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        if (DodajGreskeZaDuplikatUdruge(model.OIB, id))
        {
            ViewData["Title"] = "Uredi udrugu";
            return View(model);
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

        try
        {
            await Db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ViewData["PopupWarning"] = "Udruga s ovim OIB-om već postoji. Promijeni OIB ili uređuj postojeću udrugu.";
            ViewData["Title"] = "Uredi udrugu";
            return View(model);
        }

        _logger.LogInformation("Planinarska udruga {IdPlaninarskaUdruga} ažurirana.", id);
        TempData["Success"] = "Udruga je uspjesno azurirana.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await Db.PlaninarskeUdruge
            .FirstOrDefaultAsync(u => u.IdPlaninarskaUdruga == id && u.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        ViewData["Title"] = "Obrisi udrugu";
        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await Db.PlaninarskeUdruge
            .FirstOrDefaultAsync(u => u.IdPlaninarskaUdruga == id && u.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        entity.DeletedAt = DateTime.UtcNow;
        await Db.SaveChangesAsync();
        _logger.LogInformation("Planinarska udruga {IdPlaninarskaUdruga} obrisana (soft delete).", id);
        TempData["Success"] = "Udruga je uspjesno obrisana.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Details(int id)
    {
        var u = Db.PlaninarskeUdruge
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
        var query = Db.PlaninarskeUdruge
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