using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace planinarenje.Controllers;

public class MedaljaController : BaseController
{
    private readonly ILogger<MedaljaController> _logger;

    public MedaljaController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db, ILogger<MedaljaController> logger)
        : base(userMgr, db)
    {
        _logger = logger;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        var model = BuildIndexModel(null);
        ViewData["Title"] = "Priznanja i Medalje";
        return View(model);
    }

    [HttpGet]
    public IActionResult Search(string? searchTerm)
    {
        var model = BuildIndexModel(searchTerm);
        return PartialView("_MedaljaListPartial", model);
    }

    [HttpGet]
    public IActionResult AutocompleteSearch(string term)
    {
        var results = Db.Medalje
            .Where(m => m.DeletedAt == null && m.Naziv.Contains(term))
            .OrderBy(m => m.Naziv)
            .Take(15)
            .Select(m => new
            {
                value = m.IdMedalja,
                label = m.Naziv
            })
            .ToList();

        return Json(results);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        ViewData["Title"] = "Nova medalja";
        return View(new MedaljaCreateModel());
    }

    private bool DodajGreskeZaDuplikatMedalje(string naziv, int minimalanBrojKontrolnihTocaka, int minimalanBrojPodrucja, int? excludeId = null)
    {
        var postoji = Db.Medalje.Any(m =>
            m.Naziv == naziv &&
            m.MinimalanBrojKontrolnihTocaka == minimalanBrojKontrolnihTocaka &&
            m.MinimalanBrojPodrucja == minimalanBrojPodrucja &&
            (!excludeId.HasValue || m.IdMedalja != excludeId.Value) &&
            m.DeletedAt == null);

        if (!postoji)
        {
            return false;
        }

        var poruka = "Medalja s istim nazivom, minimalnim brojem KT i minimalnim brojem područja već postoji.";
        ModelState.AddModelError(string.Empty, poruka);
        ViewData["PopupWarning"] = poruka;
        return true;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(MedaljaCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Nova medalja";
            return View(model);
        }

        if (DodajGreskeZaDuplikatMedalje(model.Naziv, model.MinimalanBrojKontrolnihTocaka, model.MinimalanBrojPodrucja))
        {
            ViewData["Title"] = "Nova medalja";
            return View(model);
        }

        var entity = new Medalja
        {
            Naziv = model.Naziv,
            Opis = model.Opis,
            MinimalanBrojKontrolnihTocaka = model.MinimalanBrojKontrolnihTocaka,
            MinimalanBrojPodrucja = model.MinimalanBrojPodrucja
        };

        try
        {
            Db.Medalje.Add(entity);
            await Db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ViewData["PopupWarning"] = "Medalja s istim nazivom, minimalnim brojem KT i minimalnim brojem područja već postoji.";
            ViewData["Title"] = "Nova medalja";
            return View(model);
        }

        _logger.LogInformation("Medalja {IdMedalja} kreirana ({Naziv}).", entity.IdMedalja, entity.Naziv);
        TempData["NewId"] = entity.IdMedalja;
        TempData["Success"] = "Medalja je uspjesno dodana.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [ActionName("Edit")]
    public async Task<IActionResult> EditGet(int id)
    {
        var entity = await Db.Medalje.FirstOrDefaultAsync(m => m.IdMedalja == id && m.DeletedAt == null);
        if (entity == null) return NotFound();

        var model = new MedaljaEditModel
        {
            Naziv = entity.Naziv,
            Opis = entity.Opis,
            MinimalanBrojKontrolnihTocaka = entity.MinimalanBrojKontrolnihTocaka,
            MinimalanBrojPodrucja = entity.MinimalanBrojPodrucja
        };

        ViewData["Title"] = "Uredi medalju";
        return View(model);
    }

    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditPost(int id, MedaljaEditModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Uredi medalju";
            return View(model);
        }

        var entity = await Db.Medalje.FirstOrDefaultAsync(m => m.IdMedalja == id && m.DeletedAt == null);
        if (entity == null) return NotFound();

        if (DodajGreskeZaDuplikatMedalje(model.Naziv, model.MinimalanBrojKontrolnihTocaka, model.MinimalanBrojPodrucja, id))
        {
            ViewData["Title"] = "Uredi medalju";
            return View(model);
        }

        entity.Naziv = model.Naziv;
        entity.Opis = model.Opis;
        entity.MinimalanBrojKontrolnihTocaka = model.MinimalanBrojKontrolnihTocaka;
        entity.MinimalanBrojPodrucja = model.MinimalanBrojPodrucja;

        try
        {
            await Db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ViewData["PopupWarning"] = "Medalja s istim nazivom, minimalnim brojem KT i minimalnim brojem područja već postoji.";
            ViewData["Title"] = "Uredi medalju";
            return View(model);
        }

        _logger.LogInformation("Medalja {IdMedalja} ažurirana.", id);
        TempData["Success"] = "Medalja je uspjesno azurirana.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await Db.Medalje.FirstOrDefaultAsync(m => m.IdMedalja == id && m.DeletedAt == null);
        if (entity == null) return NotFound();

        ViewData["Title"] = "Obrisi medalju";
        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await Db.Medalje.FirstOrDefaultAsync(m => m.IdMedalja == id && m.DeletedAt == null);
        if (entity == null) return NotFound();

        entity.DeletedAt = DateTime.UtcNow;
        await Db.SaveChangesAsync();
        _logger.LogInformation("Medalja {IdMedalja} obrisana (soft delete).", id);
        TempData["Success"] = "Medalja je uspjesno obrisana.";
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    public IActionResult Details(int id)
    {
        var medalja = Db.Medalje.FirstOrDefault(m => m.IdMedalja == id && m.DeletedAt == null);

        if (medalja == null)
            return NotFound();

        ViewData["Title"] = medalja.Naziv;
        return View(medalja);
    }

    private List<MedaljaIndexCardViewModel> BuildIndexModel(string? searchTerm)
    {
        var query = Db.Medalje
            .Where(m => m.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(m =>
                m.Naziv.Contains(term) ||
                (m.Opis != null && m.Opis.Contains(term)));
        }

        return query
            .OrderBy(m => m.MinimalanBrojKontrolnihTocaka)
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