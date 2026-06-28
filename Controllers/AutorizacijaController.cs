using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.ViewModels;

namespace planinarenje.Controllers;

// Stranica za odobravanje sadržaja koji čeka potvrdu:
// 1) Posjeti koje su evidentirali korisnici s ulogom "Korisnik" (JeLiPotvrdenPosjet == false)
//    — autorizirati smiju i Admin i Planinar.
// 2) Prijedlozi za KontrolnaTocka/Ruta/Podrucje/PlaninarskiObjekt/PlaninarskaUdruga koje su
//    kreirali ili uredili Planinari (JeOdobreno == false), vidi BaseController.MozeUredivatiSadrzaj
//    — odobriti/odbiti smije samo Admin.
[Authorize(Roles = "Admin,Planinar")]
public class AutorizacijaController : BaseController
{
    private readonly ILogger<AutorizacijaController> _logger;

    public AutorizacijaController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db, ILogger<AutorizacijaController> logger)
        : base(userMgr, db)
    {
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var model = await BuildModel();
        ViewData["Title"] = "Autorizacija";
        return View(model);
    }

    private async Task<AutorizacijaViewModel> BuildModel()
    {
        var posjeti = await Db.Posjeti
            .Include(p => p.Korisnik)
            .Include(p => p.KontrolnaTocka)
            .Include(p => p.Ruta)
            .Where(p => p.DeletedAt == null && !p.JeLiPotvrdenPosjet)
            .OrderBy(p => p.DatumKreiranjaZapisa)
            .Select(p => new PosjetNaCekanjuViewModel
            {
                IdPosjet = p.IdPosjet,
                ImePrezimeKorisnika = p.Korisnik != null ? $"{p.Korisnik.Ime} {p.Korisnik.Prezime}" : "Nepoznati korisnik",
                NazivKontrolneTocke = p.KontrolnaTocka != null ? p.KontrolnaTocka.Naziv : "Nepoznata tocka",
                NazivRute = p.Ruta != null ? p.Ruta.Naziv : "Nije definirano",
                DatumVrijemePosjeta = p.DatumVrijemePosjeta,
                DatumKreiranjaZapisa = p.DatumKreiranjaZapisa
            })
            .ToListAsync();

        var entiteti = new List<EntitetNaCekanjuViewModel>();

        if (!IsAdmin)
        {
            return new AutorizacijaViewModel
            {
                PosjetiNaCekanju = posjeti,
                EntitetiNaCekanju = entiteti,
                PrikaziPrijedlogeSadrzaja = false
            };
        }

        entiteti.AddRange(await Db.KontrolneTocke
            .Include(k => k.Kreator)
            .Include(k => k.Podrucje)
            .Where(k => k.DeletedAt == null && !k.JeOdobreno)
            .Select(k => new EntitetNaCekanjuViewModel
            {
                TipEntiteta = "KontrolnaTocka",
                TipEntitetaNaziv = "Kontrolna tocka",
                Id = k.IdKontrolnaTocka,
                Naziv = k.Naziv,
                Podnaslov = k.Podrucje != null ? k.Podrucje.Naziv : null,
                ImePrezimeKreatora = k.Kreator != null ? $"{k.Kreator.Ime} {k.Kreator.Prezime}" : null,
                DatumPrijave = k.DatumPrijave
            })
            .ToListAsync());

        entiteti.AddRange(await Db.Rute
            .Include(r => r.Kreator)
            .Where(r => r.DeletedAt == null && !r.JeOdobreno)
            .Select(r => new EntitetNaCekanjuViewModel
            {
                TipEntiteta = "Ruta",
                TipEntitetaNaziv = "Ruta",
                Id = r.IdRuta,
                Naziv = r.Naziv,
                Podnaslov = $"{r.Pocetak} -> {r.Kraj}",
                ImePrezimeKreatora = r.Kreator != null ? $"{r.Kreator.Ime} {r.Kreator.Prezime}" : null,
                DatumPrijave = r.DatumPrijave
            })
            .ToListAsync());

        entiteti.AddRange(await Db.Podrucja
            .Include(p => p.Kreator)
            .Where(p => p.DeletedAt == null && !p.JeOdobreno)
            .Select(p => new EntitetNaCekanjuViewModel
            {
                TipEntiteta = "Podrucje",
                TipEntitetaNaziv = "Podrucje",
                Id = p.IdPodrucje,
                Naziv = p.Naziv,
                Podnaslov = p.Regija,
                ImePrezimeKreatora = p.Kreator != null ? $"{p.Kreator.Ime} {p.Kreator.Prezime}" : null,
                DatumPrijave = p.DatumPrijave
            })
            .ToListAsync());

        entiteti.AddRange(await Db.PlaninarskiObjekti
            .Include(o => o.Kreator)
            .Include(o => o.Podrucje)
            .Where(o => o.DeletedAt == null && !o.JeOdobreno)
            .Select(o => new EntitetNaCekanjuViewModel
            {
                TipEntiteta = "PlaninarskiObjekt",
                TipEntitetaNaziv = "Planinarski objekt",
                Id = o.IdPlaninarskiObjekt,
                Naziv = o.Naziv,
                Podnaslov = o.Podrucje != null ? o.Podrucje.Naziv : null,
                ImePrezimeKreatora = o.Kreator != null ? $"{o.Kreator.Ime} {o.Kreator.Prezime}" : null,
                DatumPrijave = o.DatumPrijave
            })
            .ToListAsync());

        entiteti.AddRange(await Db.PlaninarskeUdruge
            .Include(u => u.Kreator)
            .Where(u => u.DeletedAt == null && !u.JeOdobreno)
            .Select(u => new EntitetNaCekanjuViewModel
            {
                TipEntiteta = "PlaninarskaUdruga",
                TipEntitetaNaziv = "Planinarska udruga",
                Id = u.IdPlaninarskaUdruga,
                Naziv = u.Naziv,
                Podnaslov = u.Grad,
                ImePrezimeKreatora = u.Kreator != null ? $"{u.Kreator.Ime} {u.Kreator.Prezime}" : null,
                DatumPrijave = u.DatumPrijave
            })
            .ToListAsync());

        return new AutorizacijaViewModel
        {
            PosjetiNaCekanju = posjeti,
            EntitetiNaCekanju = entiteti.OrderBy(e => e.DatumPrijave).ToList(),
            PrikaziPrijedlogeSadrzaja = true
        };
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PotvrdiPosjet(int id)
    {
        var posjet = await Db.Posjeti.FirstOrDefaultAsync(p => p.IdPosjet == id && p.DeletedAt == null);
        if (posjet == null)
        {
            return NotFound();
        }

        posjet.JeLiPotvrdenPosjet = true;
        await Db.SaveChangesAsync();
        _logger.LogInformation("Posjet {IdPosjet} potvrđen od administratora {AppUserId}.", id, AppUserId);
        TempData["Success"] = "Posjet je potvrđen.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OdbijPosjet(int id)
    {
        var posjet = await Db.Posjeti.FirstOrDefaultAsync(p => p.IdPosjet == id && p.DeletedAt == null);
        if (posjet == null)
        {
            return NotFound();
        }

        posjet.DeletedAt = DateTime.UtcNow;
        await Db.SaveChangesAsync();
        _logger.LogInformation("Posjet {IdPosjet} odbijen (soft delete) od administratora {AppUserId}.", id, AppUserId);
        TempData["Success"] = "Posjet je odbijen.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> OdobriEntitet(string tip, int id)
    {
        if (!await PostaviOdobrenje(tip, id, true))
        {
            return NotFound();
        }

        _logger.LogInformation("Prijedlog {Tip} #{Id} odobren od administratora {AppUserId}.", tip, id, AppUserId);
        TempData["Success"] = "Prijedlog je odobren.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> OdbijEntitet(string tip, int id)
    {
        if (!await PostaviOdobrenje(tip, id, false))
        {
            return NotFound();
        }

        _logger.LogInformation("Prijedlog {Tip} #{Id} odbijen (soft delete) od administratora {AppUserId}.", tip, id, AppUserId);
        TempData["Success"] = "Prijedlog je odbijen.";
        return RedirectToAction(nameof(Index));
    }

    // odobri == true -> JeOdobreno = true; odobri == false -> soft delete (DeletedAt).
    private async Task<bool> PostaviOdobrenje(string tip, int id, bool odobri)
    {
        switch (tip)
        {
            case "KontrolnaTocka":
                {
                    var entitet = await Db.KontrolneTocke.FirstOrDefaultAsync(k => k.IdKontrolnaTocka == id && k.DeletedAt == null);
                    if (entitet == null) return false;
                    if (odobri) entitet.JeOdobreno = true; else entitet.DeletedAt = DateTime.UtcNow;
                    break;
                }
            case "Ruta":
                {
                    var entitet = await Db.Rute.FirstOrDefaultAsync(r => r.IdRuta == id && r.DeletedAt == null);
                    if (entitet == null) return false;
                    if (odobri) entitet.JeOdobreno = true; else entitet.DeletedAt = DateTime.UtcNow;
                    break;
                }
            case "Podrucje":
                {
                    var entitet = await Db.Podrucja.FirstOrDefaultAsync(p => p.IdPodrucje == id && p.DeletedAt == null);
                    if (entitet == null) return false;
                    if (odobri) entitet.JeOdobreno = true; else entitet.DeletedAt = DateTime.UtcNow;
                    break;
                }
            case "PlaninarskiObjekt":
                {
                    var entitet = await Db.PlaninarskiObjekti.FirstOrDefaultAsync(o => o.IdPlaninarskiObjekt == id && o.DeletedAt == null);
                    if (entitet == null) return false;
                    if (odobri) entitet.JeOdobreno = true; else entitet.DeletedAt = DateTime.UtcNow;
                    break;
                }
            case "PlaninarskaUdruga":
                {
                    var entitet = await Db.PlaninarskeUdruge.FirstOrDefaultAsync(u => u.IdPlaninarskaUdruga == id && u.DeletedAt == null);
                    if (entitet == null) return false;
                    if (odobri) entitet.JeOdobreno = true; else entitet.DeletedAt = DateTime.UtcNow;
                    break;
                }
            default:
                return false;
        }

        await Db.SaveChangesAsync();
        return true;
    }
}
