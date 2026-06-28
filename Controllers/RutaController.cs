using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Models.ViewModels;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace planinarenje.Controllers;

public class RutaController : BaseController
{
    private readonly ILogger<RutaController> _logger;

    public RutaController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db, ILogger<RutaController> logger)
        : base(userMgr, db)
    {
        _logger = logger;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var model = await BuildIndexModel(null);
        ViewData["Title"] = "Rute";
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Search(string? searchTerm)
    {
        var model = await BuildIndexModel(searchTerm);
        return PartialView("_RutaListPartial", model);
    }

    [HttpGet]
    public IActionResult AutocompleteSearch(string term)
    {
        var results = Db.Rute
            .Where(r => r.DeletedAt == null && r.JeOdobreno && r.Naziv.Contains(term))
            .OrderBy(r => r.Naziv)
            .Take(15)
            .Select(r => new
            {
                value = r.IdRuta,
                label = r.Naziv + " (" + r.Pocetak + " -> " + r.Kraj + ")"
            })
            .ToList();

        return Json(results);
    }

    [Authorize(Roles = "Admin,Planinar")]
    public IActionResult Create()
    {
        ViewData["Title"] = "Nova ruta";
        return View(new RutaCreateModel());
    }

    private bool ValidirajKontrolnuTocku(int idKontrolnaTocka)
    {
        var postoji = Db.KontrolneTocke.Any(k => k.IdKontrolnaTocka == idKontrolnaTocka && k.DeletedAt == null && k.JeOdobreno);
        if (postoji)
        {
            return true;
        }

        ModelState.AddModelError(nameof(RutaCreateModel.IdKontrolnaTocka), "Odabrana kontrolna točka više ne postoji ili je obrisana.");
        ViewData["PopupWarning"] = "Odabrana kontrolna točka više ne postoji ili je obrisana.";
        return false;
    }

    private bool DodajGreskeZaDuplikatRute(RutaCreateModel model, int? excludeId = null)
    {
        var postoji = Db.Rute
            .Where(r => r.DeletedAt == null)
            .Any(r => r.Naziv == model.Naziv &&
                      r.IdKontrolnaTocka == model.IdKontrolnaTocka &&
                      r.Pocetak == model.Pocetak &&
                      r.Kraj == model.Kraj &&
                      r.VrijemeHodaMin == model.VrijemeHodaMin &&
                      r.DuljinaKm == model.DuljinaKm &&
                      (!excludeId.HasValue || r.IdRuta != excludeId.Value));

        if (!postoji)
        {
            return false;
        }

        var poruka = "Ruta s istim nazivom, kontrolnom točkom, početkom, krajem, trajanjem hoda i duljinom km već postoji.";
        ModelState.AddModelError(string.Empty, poruka);
        ViewData["PopupWarning"] = poruka;
        return true;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Planinar")]
    public async Task<IActionResult> Create(RutaCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["KontrolnaTockaText"] = GetKontrolnaTockaNaziv(model.IdKontrolnaTocka);
            ViewData["Title"] = "Nova ruta";
            return View(model);
        }

        if (!ValidirajKontrolnuTocku(model.IdKontrolnaTocka))
        {
            ViewData["KontrolnaTockaText"] = GetKontrolnaTockaNaziv(model.IdKontrolnaTocka);
            ViewData["Title"] = "Nova ruta";
            return View(model);
        }

        if (DodajGreskeZaDuplikatRute(model))
        {
            ViewData["KontrolnaTockaText"] = GetKontrolnaTockaNaziv(model.IdKontrolnaTocka);
            ViewData["Title"] = "Nova ruta";
            return View(model);
        }

        var kreator = await GetCurrentKorisnikAsync();
        var entity = new Ruta
        {
            IdKontrolnaTocka = model.IdKontrolnaTocka,
            Naziv = model.Naziv,
            Pocetak = model.Pocetak,
            Kraj = model.Kraj,
            VrijemeHodaMin = model.VrijemeHodaMin,
            DuljinaKm = model.DuljinaKm,
            VisinskaRazlikaM = model.VisinskaRazlikaM,
            Opis = model.Opis,
            OznakaNaTerenu = model.OznakaNaTerenu,
            GodinaObnove = model.GodinaObnove,
            Napomena = model.Napomena,
            TezinaRute = model.TezinaRute,
            GPXPath = model.GPXPath,
            JeOdobreno = IsAdmin,
            IdKreator = kreator?.IdKorisnik,
            DatumPrijave = IsAdmin ? null : DateTime.UtcNow
        };

        try
        {
            Db.Rute.Add(entity);
            await Db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ViewData["PopupWarning"] = "Ruta s istim nazivom, kontrolnom točkom, početkom, krajem, trajanjem hoda i duljinom km već postoji.";
            ViewData["KontrolnaTockaText"] = GetKontrolnaTockaNaziv(model.IdKontrolnaTocka);
            ViewData["Title"] = "Nova ruta";
            return View(model);
        }

        _logger.LogInformation("Ruta {IdRuta} kreirana ({Naziv}).", entity.IdRuta, entity.Naziv);
        TempData["NewId"] = entity.IdRuta;
        TempData["Success"] = IsAdmin
            ? "Ruta je uspjesno dodana."
            : "Ruta je poslana na odobravanje administratoru.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Planinar")]
    [ActionName("Edit")]
    public async Task<IActionResult> EditGet(int id)
    {
        var entity = await Db.Rute
            .Include(r => r.KontrolnaTocka)
            .FirstOrDefaultAsync(r => r.IdRuta == id && r.DeletedAt == null);
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

        var model = new RutaEditModel
        {
            IdKontrolnaTocka = entity.IdKontrolnaTocka,
            Naziv = entity.Naziv,
            Pocetak = entity.Pocetak,
            Kraj = entity.Kraj,
            VrijemeHodaMin = entity.VrijemeHodaMin,
            DuljinaKm = entity.DuljinaKm,
            VisinskaRazlikaM = entity.VisinskaRazlikaM,
            Opis = entity.Opis,
            OznakaNaTerenu = entity.OznakaNaTerenu,
            GodinaObnove = entity.GodinaObnove,
            Napomena = entity.Napomena,
            TezinaRute = entity.TezinaRute,
            GPXPath = entity.GPXPath
        };

        ViewData["KontrolnaTockaText"] = entity.KontrolnaTocka?.Naziv ?? GetKontrolnaTockaNaziv(model.IdKontrolnaTocka);
        ViewData["Title"] = "Uredi rutu";
        return View(model);
    }

    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Planinar")]
    public async Task<IActionResult> EditPost(int id, RutaEditModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["KontrolnaTockaText"] = GetKontrolnaTockaNaziv(model.IdKontrolnaTocka);
            ViewData["Title"] = "Uredi rutu";
            return View(model);
        }

        var entity = await Db.Rute
            .FirstOrDefaultAsync(r => r.IdRuta == id && r.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        var korisnik = await GetCurrentKorisnikAsync();
        if (!IsAdmin && !entity.JeOdobreno && entity.IdKreator != korisnik?.IdKorisnik)
        {
            return Forbid();
        }

        if (!ValidirajKontrolnuTocku(model.IdKontrolnaTocka))
        {
            ViewData["KontrolnaTockaText"] = GetKontrolnaTockaNaziv(model.IdKontrolnaTocka);
            ViewData["Title"] = "Uredi rutu";
            return View(model);
        }

        if (DodajGreskeZaDuplikatRute(model, id))
        {
            ViewData["KontrolnaTockaText"] = GetKontrolnaTockaNaziv(model.IdKontrolnaTocka);
            ViewData["Title"] = "Uredi rutu";
            return View(model);
        }

        entity.IdKontrolnaTocka = model.IdKontrolnaTocka;
        entity.Naziv = model.Naziv;
        entity.Pocetak = model.Pocetak;
        entity.Kraj = model.Kraj;
        entity.VrijemeHodaMin = model.VrijemeHodaMin;
        entity.DuljinaKm = model.DuljinaKm;
        entity.VisinskaRazlikaM = model.VisinskaRazlikaM;
        entity.Opis = model.Opis;
        entity.OznakaNaTerenu = model.OznakaNaTerenu;
        entity.GodinaObnove = model.GodinaObnove;
        entity.Napomena = model.Napomena;
        entity.TezinaRute = model.TezinaRute;
        entity.GPXPath = model.GPXPath;

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
            ViewData["PopupWarning"] = "Ruta s istim nazivom, kontrolnom točkom, početkom, krajem, trajanjem hoda i duljinom km već postoji.";
            ViewData["KontrolnaTockaText"] = GetKontrolnaTockaNaziv(model.IdKontrolnaTocka);
            ViewData["Title"] = "Uredi rutu";
            return View(model);
        }

        _logger.LogInformation("Ruta {IdRuta} ažurirana.", id);
        TempData["Success"] = IsAdmin
            ? "Ruta je uspjesno azurirana."
            : "Izmjena je poslana na odobravanje administratoru.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await Db.Rute
            .Include(r => r.KontrolnaTocka)
            .FirstOrDefaultAsync(r => r.IdRuta == id && r.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        ViewData["Title"] = "Obrisi rutu";
        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await Db.Rute
            .FirstOrDefaultAsync(r => r.IdRuta == id && r.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        entity.DeletedAt = DateTime.UtcNow;
        await Db.SaveChangesAsync();
        _logger.LogInformation("Ruta {IdRuta} obrisana (soft delete).", id);
        TempData["Success"] = "Ruta je uspjesno obrisana.";
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var ruta = Db.Rute
            .Include(r => r.KontrolnaTocka)
            .FirstOrDefault(r => r.IdRuta == id && r.DeletedAt == null);

        if (ruta == null)
            return NotFound();

        if (!ruta.JeOdobreno && !IsAdmin)
        {
            var korisnik = await GetCurrentKorisnikAsync();
            if (ruta.IdKreator != korisnik?.IdKorisnik)
            {
                return NotFound();
            }
        }

        ViewData["Title"] = ruta.Naziv;
        return View(ruta);
    }

    [Route("rute/tezina/{tezina}")]
    public IActionResult PoTezini(string tezina)
    {
        if (!Enum.TryParse<TezinaRute>(tezina, true, out var tezinaEnum))
            return NotFound();

        var filtrirane = Db.Rute
            .Include(r => r.KontrolnaTocka)
            .Where(r => r.TezinaRute == tezinaEnum && r.DeletedAt == null && r.JeOdobreno)
            .ToList();

        ViewBag.Tezina = tezina;
        return View(filtrirane);
    }

    private static string FormatTrajanje(int minute)
    {
        if (minute < 60) return $"{minute} min";
        int h = minute / 60;
        int m = minute % 60;
        return m > 0 ? $"{h}h {m}min" : $"{h}h";
    }

    private static string DajNazivTezine(TezinaRute t)
    {
        return t switch
        {
            TezinaRute.Laka => "Laka",
            TezinaRute.Srednja => "Srednja",
            TezinaRute.Teska => "Teška",
            _ => "Nepoznato"
        };
    }

    private static string DajCssKlasuObziromNaTezinu(TezinaRute t)
    {
        return t switch
        {
            TezinaRute.Laka => "success",
            TezinaRute.Srednja => "warning",
            TezinaRute.Teska => "danger",
            _ => "subtle"
        };
    }

    private static string? TrimOpis(string? opis, int max)
    {
        if (string.IsNullOrWhiteSpace(opis)) return null;
        var o = opis.Trim();
        if (o.Length <= max) return o;
        return o[..max].TrimEnd() + "...";
    }

    private async Task<List<RutaIndexCardViewModel>> BuildIndexModel(string? searchTerm)
    {
        var korisnik = await GetCurrentKorisnikAsync();
        var idKorisnik = korisnik?.IdKorisnik;

        var query = Db.Rute
            .Include(r => r.KontrolnaTocka)
            .Where(r => r.DeletedAt == null && (r.KontrolnaTocka == null || r.KontrolnaTocka.DeletedAt == null));

        if (!IsAdmin)
        {
            query = query.Where(r => r.JeOdobreno || (idKorisnik.HasValue && r.IdKreator == idKorisnik.Value));
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(r =>
                r.Naziv.Contains(term) ||
                r.Pocetak.Contains(term) ||
                r.Kraj.Contains(term) ||
                (r.KontrolnaTocka != null && r.KontrolnaTocka.Naziv.Contains(term)));
        }

        return query
            .OrderBy(r => r.Naziv)
            .AsEnumerable()
            .Select(r => new RutaIndexCardViewModel
            {
                IdRuta = r.IdRuta,
                Naziv = r.Naziv,
                PovezanaKontrolnaTocka = r.KontrolnaTocka?.Naziv ?? "Nije prijavljena KT",
                Pocetak = r.Pocetak,
                Kraj = r.Kraj,
                Trajanje = FormatTrajanje(r.VrijemeHodaMin),
                DuljinaKm = r.DuljinaKm,
                VisinskaRazlikaM = r.VisinskaRazlikaM,
                TezinaTekst = DajNazivTezine(r.TezinaRute),
                TezinaCssClass = DajCssKlasuObziromNaTezinu(r.TezinaRute),
                OpisPreview = TrimOpis(r.Opis, 150),
                JeOdobreno = r.JeOdobreno
            })
            .ToList();
    }

    private string? GetKontrolnaTockaNaziv(int? id)
    {
        if (!id.HasValue)
        {
            return null;
        }

        return Db.KontrolneTocke
            .Where(k => k.DeletedAt == null && k.IdKontrolnaTocka == id.Value)
            .Select(k => k.Naziv)
            .FirstOrDefault();
    }
}