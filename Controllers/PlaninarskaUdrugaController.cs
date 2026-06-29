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
    public async Task<IActionResult> Index()
    {
        var model = await BuildIndexModel(null);
        ViewData["Title"] = "Udruge";
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Search(string? searchTerm)
    {
        var model = await BuildIndexModel(searchTerm);
        return PartialView("_PlaninarskaUdrugaListPartial", model);
    }

    [HttpGet]
    public IActionResult AutocompleteSearch(string term)
    {
        var results = Db.PlaninarskeUdruge
            .Where(u => u.DeletedAt == null && u.JeOdobreno && u.Naziv.Contains(term))
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

    [Authorize(Roles = "Admin,Planinar")]
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
    [Authorize(Roles = "Admin,Planinar")]
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

        var kreator = await GetCurrentKorisnikAsync();
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
            BrojClanova = model.BrojClanova,
            JeOdobreno = IsAdmin,
            IdKreator = kreator?.IdKorisnik,
            DatumPrijave = IsAdmin ? null : DateTime.UtcNow
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
        TempData["Success"] = IsAdmin
            ? "Udruga je uspjesno dodana."
            : "Udruga je poslana na odobravanje administratoru.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Planinar")]
    [ActionName("Edit")]
    public async Task<IActionResult> EditGet(int id)
    {
        var entity = await Db.PlaninarskeUdruge
            .FirstOrDefaultAsync(u => u.IdPlaninarskaUdruga == id && u.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        if (!IsAdmin)
        {
            var korisnik = await GetCurrentKorisnikAsync();
            if (!entity.JeOdobreno && entity.IdKreator != korisnik?.IdKorisnik)
            {
                return Forbid();
            }
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
    [Authorize(Roles = "Admin,Planinar")]
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

        var korisnik = await GetCurrentKorisnikAsync();
        if (!IsAdmin && !entity.JeOdobreno && entity.IdKreator != korisnik?.IdKorisnik)
        {
            return Forbid();
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

        if (IsAdmin)
        {
            entity.JeOdobreno = true;
        }
        else
        {
            entity.JeOdobreno = false;
            entity.IdKreator = korisnik?.IdKorisnik;
            entity.DatumPrijave = DateTime.UtcNow;
        }

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
        TempData["Success"] = IsAdmin
            ? "Udruga je uspjesno azurirana."
            : "Izmjena je poslana na odobravanje administratoru.";
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

    public async Task<IActionResult> Details(int id)
    {
        var u = Db.PlaninarskeUdruge
            .Include(x => x.PlaninarskiObjekti)
            .FirstOrDefault(x => x.IdPlaninarskaUdruga == id && x.DeletedAt == null);

        if (u == null) return NotFound();

        if (!u.JeOdobreno && !IsAdmin)
        {
            var korisnikVeza = await GetCurrentKorisnikAsync();
            if (u.IdKreator != korisnikVeza?.IdKorisnik)
            {
                return NotFound();
            }
        }

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

    private async Task<List<PlaninarskaUdrugaIndexViewModel>> BuildIndexModel(string? searchTerm)
    {
        var korisnik = await GetCurrentKorisnikAsync();
        var idKorisnik = korisnik?.IdKorisnik;

        var query = Db.PlaninarskeUdruge
            .Where(u => u.DeletedAt == null);

        if (!IsAdmin)
        {
            query = query.Where(u => u.JeOdobreno || (idKorisnik.HasValue && u.IdKreator == idKorisnik.Value));
        }

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
                BrojClanova = u.BrojClanova,
                JeOdobreno = u.JeOdobreno
            })
            .ToList();
    }
}