using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Models.ViewModels;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace planinarenje.Controllers;

public class KorisnikMedaljaController : BaseController
{
    public KorisnikMedaljaController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db)
        : base(userMgr, db)
    {
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

    private bool DodajGreskeZaDuplikatDodjele(int idKorisnik, int idMedalja, int? excludeId = null)
    {
        var postoji = Db.KorisnikMedalje.Any(km =>
            km.IdKorisnik == idKorisnik &&
            km.IdMedalja == idMedalja &&
            (!excludeId.HasValue || km.IdKorisnikMedalja != excludeId.Value));

        if (!postoji)
        {
            return false;
        }

        var poruka = "Korisnik već ima ovu medalju. Odaberi drugog korisnika ili drugu medalju.";
        ModelState.AddModelError(string.Empty, poruka);
        ViewData["PopupWarning"] = poruka;
        return true;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        var model = BuildIndexModel(null);
        ViewData["Title"] = "Dodijeljene medalje";
        return View(model);
    }

    [HttpGet]
    public IActionResult Search(string? searchTerm)
    {
        var model = BuildIndexModel(searchTerm);
        return PartialView("_KorisnikMedaljaListPartial", model);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        ViewData["Title"] = "Nova dodjela medalje";
        var model = BuildEligibilityPageModel();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Award(int medaljaId, int korisnikId)
    {
        var medalja = Db.Medalje.FirstOrDefault(m => m.IdMedalja == medaljaId && m.DeletedAt == null);
        if (medalja == null)
        {
            TempData["Error"] = "Odabrana medalja nije valjana.";
            return RedirectToAction(nameof(Create));
        }

        var eligibleUsers = GetEligibleUsersForMedal(medalja).ToList();
        var eligibleUser = eligibleUsers.FirstOrDefault(u => u.IdKorisnik == korisnikId);
        if (eligibleUser == null)
        {
            TempData["Error"] = "Korisnik nije eligible za ovu medalju.";
            return RedirectToAction(nameof(Create));
        }

        var exists = Db.KorisnikMedalje.Any(km =>
            km.DeletedAt == null && km.IdKorisnik == korisnikId && km.IdMedalja == medaljaId);
        if (exists)
        {
            TempData["PopupWarning"] = "Korisnik već ima ovu medalju. Odaberi drugog korisnika ili drugu medalju.";
            TempData["Error"] = "Korisnik već ima ovu medalju.";
            return RedirectToAction(nameof(Create));
        }

        var entity = new KorisnikMedalja
        {
            IdKorisnik = korisnikId,
            IdMedalja = medaljaId,
            DatumDodjele = DateTime.UtcNow,
            Napomena = "Automatski dodijeljeno kroz eligibility stranicu."
        };

        Db.KorisnikMedalje.Add(entity);
        await Db.SaveChangesAsync();
        TempData["NewId"] = entity.IdKorisnikMedalja;
        TempData["MedaljaSuccess"] = true;
        TempData["Success"] = "Medalja je uspjesno dodijeljena.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [ActionName("Edit")]
    public async Task<IActionResult> EditGet(int id)
    {
        var entity = await Db.KorisnikMedalje
            .Include(km => km.Korisnik)
            .Include(km => km.Medalja)
            .FirstOrDefaultAsync(km => km.IdKorisnikMedalja == id && km.DeletedAt == null);
        if (entity == null) return NotFound();

        var model = new KorisnikMedaljaEditModel
        {
            IdKorisnik = entity.IdKorisnik,
            IdMedalja = entity.IdMedalja,
            DatumDodjele = entity.DatumDodjele,
            Napomena = entity.Napomena
        };

        ViewData["KorisnikText"] = entity.Korisnik != null
            ? entity.Korisnik.Ime + " " + entity.Korisnik.Prezime + " (@" + entity.Korisnik.KorisnickoIme + ")"
            : GetKorisnikLabel(model.IdKorisnik);
        ViewData["MedaljaText"] = entity.Medalja?.Naziv ?? GetMedaljaNaziv(model.IdMedalja);
        ViewData["Title"] = "Uredi dodjelu";
        return View(model);
    }

    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditPost(int id, KorisnikMedaljaEditModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["KorisnikText"] = GetKorisnikLabel(model.IdKorisnik);
            ViewData["MedaljaText"] = GetMedaljaNaziv(model.IdMedalja);
            ViewData["Title"] = "Uredi dodjelu";
            return View(model);
        }

        var entity = await Db.KorisnikMedalje
            .FirstOrDefaultAsync(km => km.IdKorisnikMedalja == id && km.DeletedAt == null);
        if (entity == null) return NotFound();

        if (DodajGreskeZaDuplikatDodjele(model.IdKorisnik, model.IdMedalja, id))
        {
            ViewData["KorisnikText"] = GetKorisnikLabel(model.IdKorisnik);
            ViewData["MedaljaText"] = GetMedaljaNaziv(model.IdMedalja);
            ViewData["Title"] = "Uredi dodjelu";
            return View(model);
        }

        entity.IdKorisnik = model.IdKorisnik;
        entity.IdMedalja = model.IdMedalja;
        entity.DatumDodjele = model.DatumDodjele;
        entity.Napomena = model.Napomena;

        try
        {
            await Db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ViewData["PopupWarning"] = "Korisnik već ima ovu medalju. Odaberi drugog korisnika ili drugu medalju.";
            ViewData["KorisnikText"] = GetKorisnikLabel(model.IdKorisnik);
            ViewData["MedaljaText"] = GetMedaljaNaziv(model.IdMedalja);
            ViewData["Title"] = "Uredi dodjelu";
            return View(model);
        }

        TempData["Success"] = "Dodjela je uspjesno azurirana.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await Db.KorisnikMedalje
            .Include(km => km.Korisnik)
            .Include(km => km.Medalja)
            .FirstOrDefaultAsync(km => km.IdKorisnikMedalja == id && km.DeletedAt == null);
        if (entity == null) return NotFound();

        ViewData["Title"] = "Obrisi dodjelu";
        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await Db.KorisnikMedalje
            .FirstOrDefaultAsync(km => km.IdKorisnikMedalja == id && km.DeletedAt == null);
        if (entity == null) return NotFound();

        entity.DeletedAt = DateTime.UtcNow;
        await Db.SaveChangesAsync();
        TempData["Success"] = "Dodjela je uspjesno obrisana.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Details(int id)
    {
        var km = Db.KorisnikMedalje
            .Include(x => x.Korisnik)
            .Include(x => x.Medalja)
            .FirstOrDefault(x => x.IdKorisnikMedalja == id && x.DeletedAt == null);

        if (km == null) return NotFound();

        var model = new KorisnikMedaljaDetailsViewModel
        {
            IdKorisnikMedalja = km.IdKorisnikMedalja,
            IdKorisnik = km.IdKorisnik,
            ImePrezimeKorisnika = km.Korisnik != null ? $"{km.Korisnik.Ime} {km.Korisnik.Prezime}" : "Nepoznati korisnik",
            ProfilnaSlikaUrl = FormatProfileSlika(km.Korisnik?.ProfilnaSlika),
            IdMedalja = km.IdMedalja,
            NazivMedalje = km.Medalja?.Naziv ?? "Nepoznata medalja",
            DatumDodjele = km.DatumDodjele,
            Napomena = km.Napomena
        };

        return View(model);
    }

    private List<KorisnikMedaljaIndexViewModel> BuildIndexModel(string? searchTerm)
    {
        var query = Db.KorisnikMedalje
            .Include(km => km.Korisnik)
            .Include(km => km.Medalja)
            .Where(km => km.DeletedAt == null &&
                         (km.Korisnik == null || km.Korisnik.StatusAktivan) &&
                         (km.Medalja == null || km.Medalja.DeletedAt == null));

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(km =>
                (km.Korisnik != null &&
                 (km.Korisnik.Ime.Contains(term) || km.Korisnik.Prezime.Contains(term))) ||
                (km.Medalja != null && km.Medalja.Naziv.Contains(term)));
        }

        return query
            .OrderByDescending(km => km.DatumDodjele)
            .AsEnumerable()
            .Select(km => new KorisnikMedaljaIndexViewModel
            {
                IdKorisnikMedalja = km.IdKorisnikMedalja,
                IdKorisnik = km.IdKorisnik,
                ImePrezimeKorisnika = km.Korisnik != null ? $"{km.Korisnik.Ime} {km.Korisnik.Prezime}" : "Nepoznati korisnik",
                IdMedalja = km.IdMedalja,
                NazivMedalje = km.Medalja?.Naziv ?? "Nepoznata medalja",
                DatumDodjele = km.DatumDodjele,
                Napomena = km.Napomena
            })
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

    private string? GetMedaljaNaziv(int? id)
    {
        if (!id.HasValue)
        {
            return null;
        }

        return Db.Medalje
            .Where(m => m.DeletedAt == null && m.IdMedalja == id.Value)
            .Select(m => m.Naziv)
            .FirstOrDefault();
    }

    private KorisnikMedaljaEligibilityPageViewModel BuildEligibilityPageModel()
    {
        var userStats = GetEligibleUsersBaseQuery();
        var medals = Db.Medalje
            .Where(m => m.DeletedAt == null)
            .OrderBy(m => m.MinimalanBrojKontrolnihTocaka)
            .ThenBy(m => m.Naziv)
            .ToList();

        var medalCards = medals.Select(medalja => new MedaljaEligibilityCardViewModel
        {
            IdMedalja = medalja.IdMedalja,
            NazivMedalje = medalja.Naziv,
            Opis = medalja.Opis,
            MinimalanBrojKontrolnihTocaka = medalja.MinimalanBrojKontrolnihTocaka,
            MinimalanBrojPodrucja = medalja.MinimalanBrojPodrucja,
            EligibleUsers = GetEligibleUsersForMedal(medalja, userStats).ToList()
        }).ToList();

        return new KorisnikMedaljaEligibilityPageViewModel
        {
            Medalje = medalCards
        };
    }

    private List<KorisnikMedaljaEligibleUserViewModel> GetEligibleUsersBaseQuery()
    {
        return Db.Korisnici
            .Where(k => k.StatusAktivan)
            .Select(k => new KorisnikMedaljaEligibleUserViewModel
            {
                IdKorisnik = k.IdKorisnik,
                ImePrezimeKorisnika = k.Ime + " " + k.Prezime,
                KorisnickoIme = k.KorisnickoIme,
                BrojKontrolnihTocaka = Db.Posjeti
                    .Where(p => p.DeletedAt == null && p.IdKorisnik == k.IdKorisnik)
                    .Select(p => p.IdKontrolnaTocka)
                    .Distinct()
                    .Count(),
                BrojPodrucja = Db.Posjeti
                    .Where(p => p.DeletedAt == null && p.IdKorisnik == k.IdKorisnik)
                    .Select(p => p.KontrolnaTocka != null ? p.KontrolnaTocka.IdPodrucje : 0)
                    .Where(id => id > 0)
                    .Distinct()
                    .Count()
            })
            .ToList();
    }

    private IEnumerable<KorisnikMedaljaEligibleUserViewModel> GetEligibleUsersForMedal(Medalja medalja, List<KorisnikMedaljaEligibleUserViewModel>? cachedStats = null)
    {
        var stats = cachedStats ?? GetEligibleUsersBaseQuery();

        var awardedUserIds = Db.KorisnikMedalje
            .Where(km => km.DeletedAt == null && km.IdMedalja == medalja.IdMedalja)
            .Select(km => km.IdKorisnik)
            .ToHashSet();

        return stats
            .Where(user =>
                !awardedUserIds.Contains(user.IdKorisnik) &&
                user.BrojKontrolnihTocaka >= medalja.MinimalanBrojKontrolnihTocaka &&
                user.BrojPodrucja >= medalja.MinimalanBrojPodrucja)
            .OrderBy(user => user.ImePrezimeKorisnika);
    }
}