using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
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
        var model = _dbContext.Fotografije
            .Include(f => f.Posjet)
                .ThenInclude(p => p.KontrolnaTocka)
            .OrderByDescending(f => f.DatumUploada)
            .AsEnumerable()
            .Select(f => {
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
            }).ToList();

        return View(model);
    }

    public IActionResult Details(int id)
    {
        var f = _dbContext.Fotografije
            .Include(x => x.Posjet)
                .ThenInclude(p => p.KontrolnaTocka)
            .FirstOrDefault(x => x.IdFotografija == id);

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
}