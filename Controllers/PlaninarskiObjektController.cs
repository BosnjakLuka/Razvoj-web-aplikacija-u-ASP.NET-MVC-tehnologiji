using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Models.ViewModels;

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
        var model = BuildIndexModel(null);
        ViewData["Title"] = "Objekti";
        return View(model);
    }

    [HttpGet]
    public IActionResult Search(string? searchTerm)
    {
        var model = BuildIndexModel(searchTerm);
        return PartialView("_PlaninarskiObjektListPartial", model);
    }

    public IActionResult Create()
    {
        PopulatePodrucjaSelectList();
        PopulateUdrugeSelectList();
        ViewData["Title"] = "Novi objekt";
        return View(new PlaninarskiObjektCreateModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(PlaninarskiObjektCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            PopulatePodrucjaSelectList(model.IdPodrucje);
            PopulateUdrugeSelectList(model.IdPlaninarskaUdruga);
            ViewData["Title"] = "Novi objekt";
            return View(model);
        }

        var entity = new PlaninarskiObjekt
        {
            IdPodrucje = model.IdPodrucje,
            IdPlaninarskaUdruga = model.IdPlaninarskaUdruga,
            Naziv = model.Naziv,
            TipObjekta = model.TipObjekta,
            NadmorskaVisina = model.NadmorskaVisina,
            Kapacitet = model.Kapacitet,
            Opis = model.Opis,
            ImeOdgovorneOsobe = model.ImeOdgovorneOsobe,
            Telefon = model.Telefon,
            Email = model.Email,
            Adresa = model.Adresa,
            ImaNocenje = model.ImaNocenje,
            ImaHranu = model.ImaHranu,
            RadnoVrijemeOpis = model.RadnoVrijemeOpis
        };

        _dbContext.PlaninarskiObjekti.Add(entity);
        _dbContext.SaveChanges();
        TempData["Success"] = "Objekt je uspjesno dodan.";
        return RedirectToAction(nameof(Index));
    }

    [ActionName("Edit")]
    public IActionResult EditGet(int id)
    {
        var entity = _dbContext.PlaninarskiObjekti
            .FirstOrDefault(po => po.IdPlaninarskiObjekt == id && po.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        var model = new PlaninarskiObjektEditModel
        {
            IdPodrucje = entity.IdPodrucje,
            IdPlaninarskaUdruga = entity.IdPlaninarskaUdruga,
            Naziv = entity.Naziv,
            TipObjekta = entity.TipObjekta,
            NadmorskaVisina = entity.NadmorskaVisina,
            Kapacitet = entity.Kapacitet,
            Opis = entity.Opis,
            ImeOdgovorneOsobe = entity.ImeOdgovorneOsobe,
            Telefon = entity.Telefon,
            Email = entity.Email,
            Adresa = entity.Adresa,
            ImaNocenje = entity.ImaNocenje,
            ImaHranu = entity.ImaHranu,
            RadnoVrijemeOpis = entity.RadnoVrijemeOpis
        };

        PopulatePodrucjaSelectList(model.IdPodrucje);
        PopulateUdrugeSelectList(model.IdPlaninarskaUdruga);
        ViewData["Title"] = "Uredi objekt";
        return View(model);
    }

    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public IActionResult EditPost(int id, PlaninarskiObjektEditModel model)
    {
        if (!ModelState.IsValid)
        {
            PopulatePodrucjaSelectList(model.IdPodrucje);
            PopulateUdrugeSelectList(model.IdPlaninarskaUdruga);
            ViewData["Title"] = "Uredi objekt";
            return View(model);
        }

        var entity = _dbContext.PlaninarskiObjekti
            .FirstOrDefault(po => po.IdPlaninarskiObjekt == id && po.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        entity.IdPodrucje = model.IdPodrucje;
        entity.IdPlaninarskaUdruga = model.IdPlaninarskaUdruga;
        entity.Naziv = model.Naziv;
        entity.TipObjekta = model.TipObjekta;
        entity.NadmorskaVisina = model.NadmorskaVisina;
        entity.Kapacitet = model.Kapacitet;
        entity.Opis = model.Opis;
        entity.ImeOdgovorneOsobe = model.ImeOdgovorneOsobe;
        entity.Telefon = model.Telefon;
        entity.Email = model.Email;
        entity.Adresa = model.Adresa;
        entity.ImaNocenje = model.ImaNocenje;
        entity.ImaHranu = model.ImaHranu;
        entity.RadnoVrijemeOpis = model.RadnoVrijemeOpis;

        _dbContext.SaveChanges();
        TempData["Success"] = "Objekt je uspjesno azuriran.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var entity = _dbContext.PlaninarskiObjekti
            .Include(po => po.Podrucje)
            .Include(po => po.PlaninarskaUdruga)
            .FirstOrDefault(po => po.IdPlaninarskiObjekt == id && po.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        ViewData["Title"] = "Obrisi objekt";
        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var entity = _dbContext.PlaninarskiObjekti
            .FirstOrDefault(po => po.IdPlaninarskiObjekt == id && po.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        entity.DeletedAt = DateTime.UtcNow;
        _dbContext.SaveChanges();
        TempData["Success"] = "Objekt je uspjesno obrisan.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Details(int id)
    {
        var objekt = _dbContext.PlaninarskiObjekti
            .Include(po => po.Podrucje)
            .Include(po => po.PlaninarskaUdruga)
            .FirstOrDefault(po => po.IdPlaninarskiObjekt == id && po.DeletedAt == null);

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

    private List<PlaninarskiObjektIndexCardViewModel> BuildIndexModel(string? searchTerm)
    {
        var query = _dbContext.PlaninarskiObjekti
            .Include(po => po.Podrucje)
            .Include(po => po.PlaninarskaUdruga)
            .Where(po => po.DeletedAt == null &&
                         (po.Podrucje == null || po.Podrucje.DeletedAt == null) &&
                         (po.PlaninarskaUdruga == null || po.PlaninarskaUdruga.DeletedAt == null));

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(po =>
                po.Naziv.Contains(term) ||
                (po.Podrucje != null && po.Podrucje.Naziv.Contains(term)) ||
                (po.PlaninarskaUdruga != null && po.PlaninarskaUdruga.Naziv.Contains(term)));
        }

        return query
            .OrderBy(po => po.Naziv)
            .AsEnumerable()
            .Select(po => new PlaninarskiObjektIndexCardViewModel
            {
                IdPlaninarskiObjekt = po.IdPlaninarskiObjekt,
                Naziv = po.Naziv,
                TipObjektaNaziv = DajNazivTipa(po.TipObjekta).ToUpper(),
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
    }

    private void PopulatePodrucjaSelectList(int? selectedId = null)
    {
        var podrucja = _dbContext.Podrucja
            .Where(p => p.DeletedAt == null)
            .OrderBy(p => p.Naziv)
            .ToList();

        ViewBag.Podrucja = new SelectList(podrucja, "IdPodrucje", "Naziv", selectedId);
    }

    private void PopulateUdrugeSelectList(int? selectedId = null)
    {
        var udruge = _dbContext.PlaninarskeUdruge
            .Where(u => u.DeletedAt == null)
            .OrderBy(u => u.Naziv)
            .ToList();

        ViewBag.Udruge = new SelectList(udruge, "IdPlaninarskaUdruga", "Naziv", selectedId);
    }
}