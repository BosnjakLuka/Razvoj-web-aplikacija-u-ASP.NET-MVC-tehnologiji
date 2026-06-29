using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace planinarenje.Controllers;

public class PlaninarskiObjektController : BaseController
{
    private readonly ILogger<PlaninarskiObjektController> _logger;

    public PlaninarskiObjektController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db, ILogger<PlaninarskiObjektController> logger)
        : base(userMgr, db)
    {
        _logger = logger;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var model = await BuildIndexModel(null);
        ViewData["Title"] = "Objekti";
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Search(string? searchTerm)
    {
        var model = await BuildIndexModel(searchTerm);
        return PartialView("_PlaninarskiObjektListPartial", model);
    }

    [Authorize(Roles = "Admin,Planinar")]
    public IActionResult Create()
    {
        ViewData["Title"] = "Novi objekt";
        return View(new PlaninarskiObjektCreateModel());
    }

    private bool ValidirajUdruguIPodrucje(PlaninarskiObjektCreateModel model)
    {
        var imaPodrucje = Db.Podrucja.Any(p => p.IdPodrucje == model.IdPodrucje && p.DeletedAt == null && p.JeOdobreno);
        var imaUdrugu = Db.PlaninarskeUdruge.Any(u => u.IdPlaninarskaUdruga == model.IdPlaninarskaUdruga && u.DeletedAt == null && u.JeOdobreno);

        if (imaPodrucje && imaUdrugu)
        {
            return true;
        }

        if (!imaPodrucje)
        {
            ModelState.AddModelError(nameof(PlaninarskiObjektCreateModel.IdPodrucje), "Odabrano područje više ne postoji ili je obrisano.");
        }

        if (!imaUdrugu)
        {
            ModelState.AddModelError(nameof(PlaninarskiObjektCreateModel.IdPlaninarskaUdruga), "Odabrana udruga više ne postoji ili je obrisana.");
        }

        ViewData["PopupWarning"] = "Odabrana udruga ili područje nisu valjani. Provjeri unos i pokušaj ponovno.";
        return false;
    }

    private bool DodajGreskeZaDuplikatObjekta(string naziv, int idPodrucje, int idPlaninarskaUdruga, int? excludeId = null)
    {
        var postoji = Db.PlaninarskiObjekti.Any(po =>
            po.Naziv == naziv &&
            po.IdPodrucje == idPodrucje &&
            po.IdPlaninarskaUdruga == idPlaninarskaUdruga &&
            (!excludeId.HasValue || po.IdPlaninarskiObjekt != excludeId.Value) &&
            po.DeletedAt == null);

        if (!postoji)
        {
            return false;
        }

        var poruka = "Planinarski objekt s istim nazivom, područjem i udrugom već postoji.";
        ModelState.AddModelError(nameof(PlaninarskiObjektCreateModel.Naziv), poruka);
        ViewData["PopupWarning"] = poruka;
        return true;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Planinar")]
    public async Task<IActionResult> Create(PlaninarskiObjektCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PodrucjeText"] = GetPodrucjeNaziv(model.IdPodrucje);
            ViewData["UdrugaText"] = GetUdrugaNaziv(model.IdPlaninarskaUdruga);
            ViewData["Title"] = "Novi objekt";
            return View(model);
        }

        if (!ValidirajUdruguIPodrucje(model))
        {
            ViewData["PodrucjeText"] = GetPodrucjeNaziv(model.IdPodrucje);
            ViewData["UdrugaText"] = GetUdrugaNaziv(model.IdPlaninarskaUdruga);
            ViewData["Title"] = "Novi objekt";
            return View(model);
        }

        if (DodajGreskeZaDuplikatObjekta(model.Naziv, model.IdPodrucje, model.IdPlaninarskaUdruga))
        {
            ViewData["PodrucjeText"] = GetPodrucjeNaziv(model.IdPodrucje);
            ViewData["UdrugaText"] = GetUdrugaNaziv(model.IdPlaninarskaUdruga);
            ViewData["Title"] = "Novi objekt";
            return View(model);
        }

        var kreator = await GetCurrentKorisnikAsync();
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
            RadnoVrijemeOpis = model.RadnoVrijemeOpis,
            JeOdobreno = IsAdmin,
            IdKreator = kreator?.IdKorisnik,
            DatumPrijave = IsAdmin ? null : DateTime.UtcNow
        };

        try
        {
            Db.PlaninarskiObjekti.Add(entity);
            await Db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ViewData["PopupWarning"] = "Odabrana udruga ili područje nisu valjani ili objekt s istim podacima već postoji.";
            ViewData["PodrucjeText"] = GetPodrucjeNaziv(model.IdPodrucje);
            ViewData["UdrugaText"] = GetUdrugaNaziv(model.IdPlaninarskaUdruga);
            ViewData["Title"] = "Novi objekt";
            return View(model);
        }

        _logger.LogInformation("Planinarski objekt {IdPlaninarskiObjekt} kreiran ({Naziv}).", entity.IdPlaninarskiObjekt, entity.Naziv);
        TempData["NewId"] = entity.IdPlaninarskiObjekt;
        TempData["Success"] = IsAdmin
            ? "Objekt je uspjesno dodan."
            : "Objekt je poslan na odobravanje administratoru.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Planinar")]
    [ActionName("Edit")]
    public async Task<IActionResult> EditGet(int id)
    {
        var entity = await Db.PlaninarskiObjekti
            .Include(po => po.Podrucje)
            .Include(po => po.PlaninarskaUdruga)
            .FirstOrDefaultAsync(po => po.IdPlaninarskiObjekt == id && po.DeletedAt == null);
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

        ViewData["PodrucjeText"] = entity.Podrucje?.Naziv ?? GetPodrucjeNaziv(model.IdPodrucje);
        ViewData["UdrugaText"] = entity.PlaninarskaUdruga?.Naziv ?? GetUdrugaNaziv(model.IdPlaninarskaUdruga);
        ViewData["Title"] = "Uredi objekt";
        return View(model);
    }

    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Planinar")]
    public async Task<IActionResult> EditPost(int id, PlaninarskiObjektEditModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PodrucjeText"] = GetPodrucjeNaziv(model.IdPodrucje);
            ViewData["UdrugaText"] = GetUdrugaNaziv(model.IdPlaninarskaUdruga);
            ViewData["Title"] = "Uredi objekt";
            return View(model);
        }

        var entity = await Db.PlaninarskiObjekti
            .FirstOrDefaultAsync(po => po.IdPlaninarskiObjekt == id && po.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        var korisnik = await GetCurrentKorisnikAsync();
        if (!IsAdmin && !entity.JeOdobreno && entity.IdKreator != korisnik?.IdKorisnik)
        {
            return Forbid();
        }

        if (!ValidirajUdruguIPodrucje(model))
        {
            ViewData["PodrucjeText"] = GetPodrucjeNaziv(model.IdPodrucje);
            ViewData["UdrugaText"] = GetUdrugaNaziv(model.IdPlaninarskaUdruga);
            ViewData["Title"] = "Uredi objekt";
            return View(model);
        }

        if (DodajGreskeZaDuplikatObjekta(model.Naziv, model.IdPodrucje, model.IdPlaninarskaUdruga, id))
        {
            ViewData["PodrucjeText"] = GetPodrucjeNaziv(model.IdPodrucje);
            ViewData["UdrugaText"] = GetUdrugaNaziv(model.IdPlaninarskaUdruga);
            ViewData["Title"] = "Uredi objekt";
            return View(model);
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
            ViewData["PopupWarning"] = "Odabrana udruga ili područje nisu valjani ili objekt s istim podacima već postoji.";
            ViewData["PodrucjeText"] = GetPodrucjeNaziv(model.IdPodrucje);
            ViewData["UdrugaText"] = GetUdrugaNaziv(model.IdPlaninarskaUdruga);
            ViewData["Title"] = "Uredi objekt";
            return View(model);
        }

        _logger.LogInformation("Planinarski objekt {IdPlaninarskiObjekt} ažuriran.", id);
        TempData["Success"] = IsAdmin
            ? "Objekt je uspjesno azuriran."
            : "Izmjena je poslana na odobravanje administratoru.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await Db.PlaninarskiObjekti
            .Include(po => po.Podrucje)
            .Include(po => po.PlaninarskaUdruga)
            .FirstOrDefaultAsync(po => po.IdPlaninarskiObjekt == id && po.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        ViewData["Title"] = "Obrisi objekt";
        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await Db.PlaninarskiObjekti
            .FirstOrDefaultAsync(po => po.IdPlaninarskiObjekt == id && po.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        entity.DeletedAt = DateTime.UtcNow;
        await Db.SaveChangesAsync();
        _logger.LogInformation("Planinarski objekt {IdPlaninarskiObjekt} obrisan (soft delete).", id);
        TempData["Success"] = "Objekt je uspjesno obrisan.";
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var objekt = Db.PlaninarskiObjekti
            .Include(po => po.Podrucje)
            .Include(po => po.PlaninarskaUdruga)
            .FirstOrDefault(po => po.IdPlaninarskiObjekt == id && po.DeletedAt == null);

        if (objekt == null)
            return NotFound();

        if (!objekt.JeOdobreno && !IsAdmin)
        {
            var korisnik = await GetCurrentKorisnikAsync();
            if (objekt.IdKreator != korisnik?.IdKorisnik)
            {
                return NotFound();
            }
        }

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

    private async Task<List<PlaninarskiObjektIndexCardViewModel>> BuildIndexModel(string? searchTerm)
    {
        var korisnik = await GetCurrentKorisnikAsync();
        var idKorisnik = korisnik?.IdKorisnik;

        var query = Db.PlaninarskiObjekti
            .Include(po => po.Podrucje)
            .Include(po => po.PlaninarskaUdruga)
            .Where(po => po.DeletedAt == null &&
                         (po.Podrucje == null || po.Podrucje.DeletedAt == null) &&
                         (po.PlaninarskaUdruga == null || po.PlaninarskaUdruga.DeletedAt == null));

        if (!IsAdmin)
        {
            query = query.Where(po => po.JeOdobreno || (idKorisnik.HasValue && po.IdKreator == idKorisnik.Value));
        }

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
                OpisPreview = TrimOpis(po.Opis, 140),
                JeOdobreno = po.JeOdobreno
            })
            .ToList();
    }

    private string? GetPodrucjeNaziv(int? id)
    {
        if (!id.HasValue)
        {
            return null;
        }

        return Db.Podrucja
            .Where(p => p.DeletedAt == null && p.IdPodrucje == id.Value)
            .Select(p => p.Naziv)
            .FirstOrDefault();
    }

    private string? GetUdrugaNaziv(int? id)
    {
        if (!id.HasValue)
        {
            return null;
        }

        return Db.PlaninarskeUdruge
            .Where(u => u.DeletedAt == null && u.IdPlaninarskaUdruga == id.Value)
            .Select(u => u.Naziv)
            .FirstOrDefault();
    }
}