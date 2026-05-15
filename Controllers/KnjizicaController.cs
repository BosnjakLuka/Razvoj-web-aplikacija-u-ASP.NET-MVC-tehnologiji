using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Models.ViewModels;
using System.IO;
using System.Linq;

namespace planinarenje.Controllers;

public class KnjizicaController : Controller
{
    private readonly PlaninarstvoDbContext _dbContext;

    public KnjizicaController(PlaninarstvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IActionResult Index()
    {
        var model = BuildIndexModel(null);
        ViewData["Title"] = "E-Knjizica";
        return View(model);
    }

    [HttpGet]
    public IActionResult Search(string? searchTerm)
    {
        var model = BuildIndexModel(searchTerm);
        return PartialView("_KnjizicaListPartial", model);
    }

    [HttpGet]
    public IActionResult AutocompleteSearch(string term)
    {
        var query = _dbContext.Knjizice
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

    public IActionResult Create()
    {
        ViewData["Title"] = "Nova knjizica";
        return View(new KnjizicaCreateModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(KnjizicaCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["KorisnikText"] = GetKorisnikLabel(model.IdKorisnik);
            ViewData["Title"] = "Nova knjizica";
            return View(model);
        }

        var entity = new Knjizica
        {
            IdKorisnik = model.IdKorisnik,
            Napomena = model.Napomena,
            DatumKreiranja = DateTime.UtcNow,
            StatusAktivna = true
        };

        _dbContext.Knjizice.Add(entity);
        _dbContext.SaveChanges();
        TempData["NewId"] = entity.IdKnjizica;
        TempData["Success"] = "Knjizica je uspjesno dodana.";
        return RedirectToAction(nameof(Index));
    }

    [ActionName("Edit")]
    public IActionResult EditGet(int id)
    {
        var entity = _dbContext.Knjizice
            .Include(k => k.Korisnik)
            .FirstOrDefault(k => k.IdKnjizica == id && k.StatusAktivna);
        if (entity == null)
        {
            return NotFound();
        }

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
    public IActionResult EditPost(int id, KnjizicaEditModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["KorisnikText"] = GetKorisnikLabel(model.IdKorisnik);
            ViewData["Title"] = "Uredi knjizicu";
            return View(model);
        }

        var entity = _dbContext.Knjizice
            .FirstOrDefault(k => k.IdKnjizica == id && k.StatusAktivna);
        if (entity == null)
        {
            return NotFound();
        }

        entity.IdKorisnik = model.IdKorisnik;
        entity.Napomena = model.Napomena;

        _dbContext.SaveChanges();
        TempData["Success"] = "Knjizica je uspjesno azurirana.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var entity = _dbContext.Knjizice
            .Include(k => k.Korisnik)
            .FirstOrDefault(k => k.IdKnjizica == id && k.StatusAktivna);
        if (entity == null)
        {
            return NotFound();
        }

        ViewData["Title"] = "Obrisi knjizicu";
        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var entity = _dbContext.Knjizice
            .FirstOrDefault(k => k.IdKnjizica == id && k.StatusAktivna);
        if (entity == null)
        {
            return NotFound();
        }

        entity.StatusAktivna = false;
        _dbContext.SaveChanges();
        TempData["Success"] = "Knjizica je uspjesno obrisana.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Details(int id)
    {
        var kn = _dbContext.Knjizice
            .Include(k => k.Korisnik)
            .FirstOrDefault(k => k.IdKnjizica == id && k.StatusAktivna);
        if (kn == null) return NotFound();

        var posjeti = _dbContext.Posjeti
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

    private List<KnjizicaIndexViewModel> BuildIndexModel(string? searchTerm)
    {
        var query = _dbContext.Knjizice
            .Include(k => k.Korisnik)
            .Where(k => k.StatusAktivna && k.Korisnik != null && k.Korisnik.StatusAktivan);

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

        return _dbContext.Korisnici
            .Where(k => k.StatusAktivan && k.IdKorisnik == id.Value)
            .Select(k => k.Ime + " " + k.Prezime + " (@" + k.KorisnickoIme + ")")
            .FirstOrDefault();
    }
}
