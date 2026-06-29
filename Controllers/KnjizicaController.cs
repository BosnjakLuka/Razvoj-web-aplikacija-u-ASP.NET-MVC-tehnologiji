using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Models.ViewModels;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace planinarenje.Controllers;

public class KnjizicaController : BaseController
{
    private readonly ILogger<KnjizicaController> _logger;

    public KnjizicaController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db, ILogger<KnjizicaController> logger)
        : base(userMgr, db)
    {
        _logger = logger;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "E-Knjizica";

        var current = await GetCurrentKorisnikAsync();
        ViewData["CanCreateOwn"] = current != null &&
            !Db.Knjizice.Any(k => k.IdKorisnik == current.IdKorisnik && k.StatusAktivna);

        if (IsAdmin || IsPlaninar)
        {
            return View(BuildIndexModel(null));
        }

        if (current == null) return Forbid();

        return View(BuildIndexModel(null, current.IdKorisnik));
    }

    [HttpGet]
    public async Task<IActionResult> Search(string? searchTerm)
    {
        if (IsAdmin || IsPlaninar)
        {
            return PartialView("_KnjizicaListPartial", BuildIndexModel(searchTerm));
        }

        var current = await GetCurrentKorisnikAsync();
        if (current == null) return Forbid();

        return PartialView("_KnjizicaListPartial", BuildIndexModel(searchTerm, current.IdKorisnik));
    }

    [HttpGet]
    public IActionResult AutocompleteSearch(string term)
    {
        var query = Db.Knjizice
            .Include(k => k.Korisnik)
            .Where(k => k.StatusAktivna && k.Korisnik != null && k.Korisnik.StatusAktivan);

        if (int.TryParse(term, out var id))
        {
            query = query.Where(k => k.IdKnjizica == id);
        }

        var results = query
            .OrderByDescending(k => k.DatumKreiranja)
            .Take(15)
            .Select(k => new
            {
                value = k.IdKnjizica,
                label = "Knjizica #" + k.IdKnjizica + " - " + (k.Korisnik != null ? k.Korisnik.Ime : "")
            })
            .ToList();

        return Json(results);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        ViewData["Title"] = "Nova knjizica";
        var model = BuildEligibilityPageModel();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Dodijeli(int korisnikId)
    {
        var eligibleIds = GetEligibleKorisniciZaKnjizicu().Select(k => k.IdKorisnik).ToHashSet();
        if (!eligibleIds.Contains(korisnikId))
        {
            TempData["Error"] = "Korisnik nije eligible za dodjelu knjižice.";
            return RedirectToAction(nameof(Create));
        }

        var entity = await KreirajIliReaktivirajKnjizicuAsync(korisnikId);
        if (entity == null)
        {
            TempData["PopupWarning"] = "Knjizica za ovog korisnika već postoji.";
            TempData["Error"] = "Knjizica za ovog korisnika već postoji.";
            return RedirectToAction(nameof(Create));
        }

        _logger.LogInformation("Knjizica {IdKnjizica} dodijeljena korisniku {IdKorisnik} (admin {AppUserId}).", entity.IdKnjizica, korisnikId, AppUserId);
        TempData["NewId"] = entity.IdKnjizica;
        TempData["Success"] = "Knjizica je uspjesno dodana.";
        return RedirectToAction(nameof(Index));
    }

    // Bilo koji prijavljeni korisnik (Korisnik, Planinar ili Admin) smije sam sebi kreirati
    // knjižicu ako je još nema - ne treba čekati admina da je dodijeli.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> KreirajVlastitu()
    {
        var current = await GetCurrentKorisnikAsync();
        if (current == null) return Forbid();

        var entity = await KreirajIliReaktivirajKnjizicuAsync(current.IdKorisnik);
        if (entity == null)
        {
            TempData["Error"] = "Već imaš aktivnu knjižicu.";
            return RedirectToAction(nameof(Index));
        }

        _logger.LogInformation("Knjizica {IdKnjizica} kreirana samostalno za korisnika {IdKorisnik}.", entity.IdKnjizica, current.IdKorisnik);
        TempData["NewId"] = entity.IdKnjizica;
        TempData["Success"] = "Knjizica je uspjesno kreirana.";
        return RedirectToAction(nameof(Index));
    }

    // Knjizice.IdKorisnik ima jedinstveni indeks, pa soft-obrisana knjizica (StatusAktivna = false)
    // i dalje zauzima taj indeks. Zato ne smijemo ubaciti novi red - ako za korisnika postoji
    // ranije obrisana knjizica, samo je reaktiviramo; inace kreiramo novu. Vraca null ako korisnik
    // već ima aktivnu knjižicu.
    private async Task<Knjizica?> KreirajIliReaktivirajKnjizicuAsync(int idKorisnik)
    {
        var postojeca = await Db.Knjizice.FirstOrDefaultAsync(k => k.IdKorisnik == idKorisnik);
        if (postojeca != null)
        {
            if (postojeca.StatusAktivna)
            {
                return null;
            }

            postojeca.StatusAktivna = true;
            postojeca.DatumKreiranja = DateTime.UtcNow;
            postojeca.Napomena = null;
            await Db.SaveChangesAsync();
            return postojeca;
        }

        var entity = new Knjizica
        {
            IdKorisnik = idKorisnik,
            DatumKreiranja = DateTime.UtcNow,
            StatusAktivna = true
        };

        Db.Knjizice.Add(entity);
        await Db.SaveChangesAsync();
        return entity;
    }

    [Authorize]
    [ActionName("Edit")]
    public async Task<IActionResult> EditGet(int id)
    {
        var entity = await Db.Knjizice
            .Include(k => k.Korisnik)
            .FirstOrDefaultAsync(k => k.IdKnjizica == id && k.StatusAktivna);
        if (entity == null)
        {
            return NotFound();
        }

        if (!IsAdmin && !await IsOwnerAsync(entity.IdKorisnik))
            return Forbid();

        var model = new KnjizicaEditModel
        {
            IdKorisnik = entity.IdKorisnik,
            Napomena = entity.Napomena
        };

        ViewData["KorisnikText"] = entity.Korisnik != null ? entity.Korisnik.Ime + " " + entity.Korisnik.Prezime + " (@" + entity.Korisnik.KorisnickoIme + ")" : GetKorisnikLabel(model.IdKorisnik);
        ViewData["Title"] = "Uredi knjizicu";
        return View(model);
    }

    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> EditPost(int id, KnjizicaEditModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["KorisnikText"] = GetKorisnikLabel(model.IdKorisnik);
            ViewData["Title"] = "Uredi knjizicu";
            return View(model);
        }

        var entity = await Db.Knjizice
            .FirstOrDefaultAsync(k => k.IdKnjizica == id && k.StatusAktivna);
        if (entity == null)
        {
            return NotFound();
        }

        if (DodajGreskeZaDuplikatKnjizice(model.IdKorisnik, id))
        {
            ViewData["KorisnikText"] = GetKorisnikLabel(model.IdKorisnik);
            ViewData["Title"] = "Uredi knjizicu";
            return View(model);
        }

        // ownership check and force owner
        var original = await Db.Knjizice.AsNoTracking().FirstOrDefaultAsync(k => k.IdKnjizica == id);
        if (original == null) return NotFound();
        if (!IsAdmin && !await IsOwnerAsync(original.IdKorisnik))
        {
            _logger.LogWarning("Korisnik {AppUserId} bez prava pristupa pokušao je urediti knjižicu {IdKnjizica}.", AppUserId, id);
            return Forbid();
        }

        model.IdKorisnik = original.IdKorisnik;
        entity.IdKorisnik = model.IdKorisnik;
        entity.Napomena = model.Napomena;

        try
        {
            await Db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ViewData["PopupWarning"] = "Knjizica za ovog korisnika već postoji. Odaberi drugog korisnika ili uređuj postojeću knjižicu.";
            ViewData["KorisnikText"] = GetKorisnikLabel(model.IdKorisnik);
            ViewData["Title"] = "Uredi knjizicu";
            return View(model);
        }

        _logger.LogInformation("Knjizica {IdKnjizica} ažurirana.", id);
        TempData["Success"] = "Knjizica je uspjesno azurirana.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await Db.Knjizice
            .Include(k => k.Korisnik)
            .FirstOrDefaultAsync(k => k.IdKnjizica == id && k.StatusAktivna);
        if (entity == null)
        {
            return NotFound();
        }

        ViewData["Title"] = "Obrisi knjizicu";
        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await Db.Knjizice
            .FirstOrDefaultAsync(k => k.IdKnjizica == id && k.StatusAktivna);
        if (entity == null)
        {
            return NotFound();
        }

        entity.StatusAktivna = false;
        await Db.SaveChangesAsync();
        _logger.LogInformation("Knjizica {IdKnjizica} obrisana (soft delete).", id);
        TempData["Success"] = "Knjizica je uspjesno obrisana.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        var kn = await Db.Knjizice
            .Include(k => k.Korisnik)
            .FirstOrDefaultAsync(k => k.IdKnjizica == id && k.StatusAktivna);
        if (kn == null) return NotFound();

        if (!IsAdmin && !await IsOwnerAsync(kn.IdKorisnik))
            return Forbid();

        var posjeti = Db.Posjeti
            .Where(p => p.IdKnjizica == id && p.DeletedAt == null)
            .Include(p => p.KontrolnaTocka)
            .OrderByDescending(p => p.DatumVrijemePosjeta)
            .Select(p => new KnjizicaPosjetViewModel
            {
                IdPosjet = p.IdPosjet,
                IdKontrolnaTocka = p.IdKontrolnaTocka,
                NazivKontrolneTocke = p.KontrolnaTocka != null ? p.KontrolnaTocka.Naziv : "Nepoznato",
                DatumVrijemePosjeta = p.DatumVrijemePosjeta,
                JeLiPotvrdenPosjet = p.JeLiPotvrdenPosjet
            })
            .ToList();

        var model = new KnjizicaDetailsViewModel
        {
            IdKnjizica = kn.IdKnjizica,
            IdKorisnik = kn.IdKorisnik,
            ImePrezimeKorisnika = kn.Korisnik != null ? kn.Korisnik.Ime + " " + kn.Korisnik.Prezime : "Nepoznato",
            KorisnickoIme = kn.Korisnik?.KorisnickoIme ?? "nepoznato_ime",
            ProfilnaSlikaUrl = FormatProfileSlika(kn.Korisnik?.ProfilnaSlika),
            DatumKreiranja = kn.DatumKreiranja,
            StatusAktivna = kn.StatusAktivna,
            Napomena = kn.Napomena,
            Posjeti = posjeti
        };

        return View(model);
    }

    private string? FormatProfileSlika(string? absolutePath)
    {
        if (string.IsNullOrWhiteSpace(absolutePath)) return null;

        if (absolutePath.StartsWith("/Slike/Profil/", StringComparison.OrdinalIgnoreCase))
        {
            return absolutePath;
        }

        if (absolutePath.Contains("\\Slike\\Profil\\", StringComparison.OrdinalIgnoreCase) ||
            absolutePath.Contains("/Slike/Profil/", StringComparison.OrdinalIgnoreCase))
        {
            return "/Slike/Profil/" + Path.GetFileName(absolutePath);
        }

        return absolutePath;
    }

    private bool DodajGreskeZaDuplikatKnjizice(int idKorisnik, int? excludeId = null)
    {
        var postoji = Db.Knjizice.Any(k =>
            k.IdKorisnik == idKorisnik && k.StatusAktivna && (!excludeId.HasValue || k.IdKnjizica != excludeId.Value));

        if (!postoji)
        {
            return false;
        }

        var poruka = "Knjizica za ovog korisnika već postoji. Odaberi drugog korisnika ili uređuj postojeću knjižicu.";
        ModelState.AddModelError(nameof(KnjizicaCreateModel.IdKorisnik), poruka);
        ViewData["PopupWarning"] = poruka;
        return true;
    }

    private KnjizicaEligibilityPageViewModel BuildEligibilityPageModel()
    {
        return new KnjizicaEligibilityPageViewModel
        {
            EligibleUsers = GetEligibleKorisniciZaKnjizicu()
        };
    }

    private List<KnjizicaEligibleUserViewModel> GetEligibleKorisniciZaKnjizicu()
    {
        var korisniciSaKnjizicom = Db.Knjizice
            .Where(k => k.StatusAktivna)
            .Select(k => k.IdKorisnik)
            .ToHashSet();

        return Db.Korisnici
            .Where(k => k.StatusAktivan)
            .AsEnumerable()
            .Where(k => !korisniciSaKnjizicom.Contains(k.IdKorisnik))
            .Select(k => new KnjizicaEligibleUserViewModel
            {
                IdKorisnik = k.IdKorisnik,
                ImePrezimeKorisnika = k.Ime + " " + k.Prezime,
                KorisnickoIme = k.KorisnickoIme
            })
            .OrderBy(k => k.ImePrezimeKorisnika)
            .ToList();
    }

    private List<KnjizicaIndexViewModel> BuildIndexModel(string? searchTerm, int? scopeToKorisnikId = null)
    {
        var query = Db.Knjizice
            .Include(k => k.Korisnik)
            .Where(k => k.StatusAktivna && k.Korisnik != null && k.Korisnik.StatusAktivan);

        if (scopeToKorisnikId.HasValue)
        {
            query = query.Where(k => k.IdKorisnik == scopeToKorisnikId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(k =>
                (k.Korisnik != null &&
                 (k.Korisnik.Ime.Contains(term) ||
                  k.Korisnik.Prezime.Contains(term) ||
                  k.Korisnik.KorisnickoIme.Contains(term))));
        }

        return query
            .Select(k => new KnjizicaIndexViewModel
            {
                IdKnjizica = k.IdKnjizica,
                IdKorisnik = k.IdKorisnik,
                ImePrezimeKorisnika = k.Korisnik != null ? k.Korisnik.Ime + " " + k.Korisnik.Prezime : "Nepoznat planer",
                DatumKreiranja = k.DatumKreiranja,
                StatusAktivna = k.StatusAktivna
            })
            .OrderByDescending(k => k.DatumKreiranja)
            .ToList();
    }

    private string? GetKorisnikLabel(int? id)
    {
        if (!id.HasValue)
        {
            return null;
        }

        return Db.Korisnici
            .Where(k => k.StatusAktivan && k.IdKorisnik == id.Value)
            .Select(k => k.Ime + " " + k.Prezime + " (@" + k.KorisnickoIme + ")")
            .FirstOrDefault();
    }
}
