using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Models.ViewModels;
using System.IO;

namespace planinarenje.Controllers;

public class FotografijaController : Controller
{
    private readonly PlaninarstvoDbContext _dbContext;

    public FotografijaController(PlaninarstvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private string FormatirajTipSlike(TipSlike tip)
    {
        return tip switch
        {
            TipSlike.Selfie => "Selfie",
            TipSlike.Oznaka => "Oznaka",
            TipSlike.Krajolik => "Krajolik",
            TipSlike.Mapa => "Mapa",
            TipSlike.Drugo => "Drugo",
            _ => "Nepoznato"
        };
    }

    // Helper za Dohvaćanje slika
    private string FormatirajSliku(string? apsolutnaPutanja)
    {
        if (string.IsNullOrWhiteSpace(apsolutnaPutanja)) return "/Slike/Dizajn/hpo.jpg";

        if (apsolutnaPutanja.StartsWith("/Slike/Fotografije/", StringComparison.OrdinalIgnoreCase))
        {
            return apsolutnaPutanja;
        }

        if (apsolutnaPutanja.Contains("\\Slike\\Fotografije\\", StringComparison.OrdinalIgnoreCase) ||
            apsolutnaPutanja.Contains("/Slike/Fotografije/", StringComparison.OrdinalIgnoreCase))
        {
            return "/Slike/Fotografije/" + Path.GetFileName(apsolutnaPutanja);
        }

        return "/Slike/Dizajn/hpo.jpg";
    }

    public IActionResult Index()
    {
        var model = BuildIndexModel(null);
        ViewData["Title"] = "Fotografije";
        return View(model);
    }

    [HttpGet]
    public IActionResult Search(string? searchTerm)
    {
        var model = BuildIndexModel(searchTerm);
        return PartialView("_FotografijaListPartial", model);
    }

    public IActionResult Create()
    {
        ViewData["Title"] = "Nova fotografija";
        return View(new FotografijaCreateModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(FotografijaCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PosjetText"] = GetPosjetLabel(model.IdPosjet);
            ViewData["Title"] = "Nova fotografija";
            return View(model);
        }

        var entity = new Fotografija
        {
            IdPosjet = model.IdPosjet,
            NazivDatoteke = model.NazivDatoteke,
            PutanjaDatoteke = model.PutanjaDatoteke,
            TipSlike = model.TipSlike,
            Opis = model.Opis,
            DatumUploada = DateTime.UtcNow
        };

        _dbContext.Fotografije.Add(entity);
        _dbContext.SaveChanges();
        TempData["NewId"] = entity.IdFotografija;
        TempData["Success"] = "Fotografija je uspjesno dodana.";
        return RedirectToAction(nameof(Index));
    }

    [ActionName("Edit")]
    public IActionResult EditGet(int id)
    {
        var entity = _dbContext.Fotografije
            .Include(f => f.Posjet)
                .ThenInclude(p => p.KontrolnaTocka)
            .FirstOrDefault(f => f.IdFotografija == id && f.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        var model = new FotografijaEditModel
        {
            IdPosjet = entity.IdPosjet,
            NazivDatoteke = entity.NazivDatoteke,
            PutanjaDatoteke = entity.PutanjaDatoteke,
            TipSlike = entity.TipSlike,
            Opis = entity.Opis
        };

        ViewData["PosjetText"] = entity.Posjet?.KontrolnaTocka != null
            ? "Posjet #" + entity.Posjet.IdPosjet + " - " + entity.Posjet.KontrolnaTocka.Naziv
            : GetPosjetLabel(model.IdPosjet);
        ViewData["Title"] = "Uredi fotografiju";
        return View(model);
    }

    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public IActionResult EditPost(int id, FotografijaEditModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PosjetText"] = GetPosjetLabel(model.IdPosjet);
            ViewData["Title"] = "Uredi fotografiju";
            return View(model);
        }

        var entity = _dbContext.Fotografije
            .FirstOrDefault(f => f.IdFotografija == id && f.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        entity.IdPosjet = model.IdPosjet;
        entity.NazivDatoteke = model.NazivDatoteke;
        entity.PutanjaDatoteke = model.PutanjaDatoteke;
        entity.TipSlike = model.TipSlike;
        entity.Opis = model.Opis;

        _dbContext.SaveChanges();
        TempData["Success"] = "Fotografija je uspjesno azurirana.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var entity = _dbContext.Fotografije
            .Include(f => f.Posjet)
                .ThenInclude(p => p.KontrolnaTocka)
            .FirstOrDefault(f => f.IdFotografija == id && f.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        ViewData["Title"] = "Obrisi fotografiju";
        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var entity = _dbContext.Fotografije
            .FirstOrDefault(f => f.IdFotografija == id && f.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        entity.DeletedAt = DateTime.UtcNow;
        _dbContext.SaveChanges();
        TempData["Success"] = "Fotografija je uspjesno obrisana.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Details(int id)
    {
        var f = _dbContext.Fotografije
            .Include(x => x.Posjet)
                .ThenInclude(p => p.KontrolnaTocka)
            .FirstOrDefault(x => x.IdFotografija == id && x.DeletedAt == null);

        if (f == null) return NotFound();

        var kt = f.Posjet?.KontrolnaTocka;

        var model = new FotografijaDetailsViewModel
        {
            IdFotografija = f.IdFotografija,
            NazivDatoteke = f.NazivDatoteke,
            PutanjaDatoteke = FormatirajSliku(f.PutanjaDatoteke),
            DatumUploada = f.DatumUploada,
            TipSlike = FormatirajTipSlike(f.TipSlike),
            Opis = f.Opis,
            IdPosjet = f.IdPosjet,
            PosjetNaslov = kt != null ? $"KT: {kt.Naziv}" : $"Posjet #{f.IdPosjet}"
        };

        return View(model);
    }

    private List<FotografijaIndexViewModel> BuildIndexModel(string? searchTerm)
    {
        var query = _dbContext.Fotografije
            .Include(f => f.Posjet)
                .ThenInclude(p => p.KontrolnaTocka)
            .Where(f => f.DeletedAt == null && (f.Posjet == null || f.Posjet.DeletedAt == null));

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(f =>
                f.NazivDatoteke.Contains(term) ||
                (f.Posjet != null && f.Posjet.KontrolnaTocka != null && f.Posjet.KontrolnaTocka.Naziv.Contains(term)));
        }

        return query
            .OrderByDescending(f => f.DatumUploada)
            .AsEnumerable()
            .Select(f =>
            {
                var kt = f.Posjet?.KontrolnaTocka;
                return new FotografijaIndexViewModel
                {
                    IdFotografija = f.IdFotografija,
                    NazivDatoteke = f.NazivDatoteke,
                    IdPosjet = f.IdPosjet,
                    PosjetNaslov = kt != null ? $"KT: {kt.Naziv}" : $"Posjet #{f.IdPosjet}",
                    DatumUploada = f.DatumUploada,
                    TipSlike = FormatirajTipSlike(f.TipSlike)
                };
            })
            .ToList();
    }

    private string? GetPosjetLabel(int? id)
    {
        if (!id.HasValue)
        {
            return null;
        }

        return _dbContext.Posjeti
            .Include(p => p.KontrolnaTocka)
            .Where(p => p.DeletedAt == null && p.IdPosjet == id.Value)
            .Select(p => p.KontrolnaTocka != null
                ? "Posjet #" + p.IdPosjet + " - " + p.KontrolnaTocka.Naziv
                : "Posjet #" + p.IdPosjet)
            .FirstOrDefault();
    }
}