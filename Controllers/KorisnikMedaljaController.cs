using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Models.ViewModels;
using System.IO;
using System.Linq;

namespace planinarenje.Controllers;

public class KorisnikMedaljaController : Controller
{
    private readonly PlaninarstvoDbContext _dbContext;

    public KorisnikMedaljaController(PlaninarstvoDbContext dbContext)
    {
        _dbContext = dbContext;
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

    public IActionResult Create()
    {
        ViewData["Title"] = "Nova dodjela medalje";
        return View(new KorisnikMedaljaCreateModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(KorisnikMedaljaCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["KorisnikText"] = GetKorisnikLabel(model.IdKorisnik);
            ViewData["MedaljaText"] = GetMedaljaNaziv(model.IdMedalja);
            ViewData["Title"] = "Nova dodjela medalje";
            return View(model);
        }

        var entity = new KorisnikMedalja
        {
            IdKorisnik = model.IdKorisnik,
            IdMedalja = model.IdMedalja,
            DatumDodjele = model.DatumDodjele,
            Napomena = model.Napomena
        };

        _dbContext.KorisnikMedalje.Add(entity);
        _dbContext.SaveChanges();
        TempData["Success"] = "Medalja je uspjesno dodijeljena.";
        return RedirectToAction(nameof(Index));
    }

    [ActionName("Edit")]
    public IActionResult EditGet(int id)
    {
        var entity = _dbContext.KorisnikMedalje
            .Include(km => km.Korisnik)
            .Include(km => km.Medalja)
            .FirstOrDefault(km => km.IdKorisnikMedalja == id && km.DeletedAt == null);
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
    public IActionResult EditPost(int id, KorisnikMedaljaEditModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["KorisnikText"] = GetKorisnikLabel(model.IdKorisnik);
            ViewData["MedaljaText"] = GetMedaljaNaziv(model.IdMedalja);
            ViewData["Title"] = "Uredi dodjelu";
            return View(model);
        }

        var entity = _dbContext.KorisnikMedalje
            .FirstOrDefault(km => km.IdKorisnikMedalja == id && km.DeletedAt == null);
        if (entity == null) return NotFound();

        entity.IdKorisnik = model.IdKorisnik;
        entity.IdMedalja = model.IdMedalja;
        entity.DatumDodjele = model.DatumDodjele;
        entity.Napomena = model.Napomena;

        _dbContext.SaveChanges();
        TempData["Success"] = "Dodjela je uspjesno azurirana.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var entity = _dbContext.KorisnikMedalje
            .Include(km => km.Korisnik)
            .Include(km => km.Medalja)
            .FirstOrDefault(km => km.IdKorisnikMedalja == id && km.DeletedAt == null);
        if (entity == null) return NotFound();

        ViewData["Title"] = "Obrisi dodjelu";
        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var entity = _dbContext.KorisnikMedalje
            .FirstOrDefault(km => km.IdKorisnikMedalja == id && km.DeletedAt == null);
        if (entity == null) return NotFound();

        entity.DeletedAt = DateTime.UtcNow;
        _dbContext.SaveChanges();
        TempData["Success"] = "Dodjela je uspjesno obrisana.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Details(int id)
    {
        var km = _dbContext.KorisnikMedalje
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
        var query = _dbContext.KorisnikMedalje
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

        return _dbContext.Korisnici
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

        return _dbContext.Medalje
            .Where(m => m.DeletedAt == null && m.IdMedalja == id.Value)
            .Select(m => m.Naziv)
            .FirstOrDefault();
    }
}