using Microsoft.AspNetCore.Mvc;
using planinarenje.Entiteti;
using planinarenje.Models;
using System.IO;

namespace planinarenje.Controllers;

public class FotografijaController : Controller
{
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
        if (string.IsNullOrEmpty(apsolutnaPutanja)) return "/Slike/Dizajn/hpo.jpg";
        
        var idx = apsolutnaPutanja.IndexOf("Slike", StringComparison.OrdinalIgnoreCase);
        if (idx >= 0)
        {
            // Ekstrahiraj relativnu putanju
            var relPath = "/" + apsolutnaPutanja.Substring(idx).Replace("\\", "/");
            
            // Popravi grešku u podacima gdje piše .jpg a datoteka je .jpeg na disku
            if (!System.IO.File.Exists(apsolutnaPutanja))
            {
                if (apsolutnaPutanja.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                {
                    var altPath = apsolutnaPutanja.Substring(0, apsolutnaPutanja.Length - 4) + ".jpeg";
                    if (System.IO.File.Exists(altPath))
                    {
                        return relPath.Substring(0, relPath.Length - 4) + ".jpeg";
                    }
                }
            }
            return relPath;
        }

        return "/Slike/Dizajn/hpo.jpg";
    }

    public IActionResult Index()
    {
        var podaci = Lab1PodaciFactory.Kreiraj();

        var model = podaci.Fotografije
            .OrderByDescending(f => f.DatumUploada)
            .Select(f => {
                var posjet = podaci.Posjeti.FirstOrDefault(p => p.IdPosjet == f.IdPosjet);
                var kt = posjet != null ? podaci.KontrolneTocke.FirstOrDefault(k => k.IdKontrolnaTocka == posjet.IdKontrolnaTocka) : null;
                
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
        var podaci = Lab1PodaciFactory.Kreiraj();
        var f = podaci.Fotografije.FirstOrDefault(fot => fot.IdFotografija == id);

        if (f == null) return NotFound();

        var posjet = podaci.Posjeti.FirstOrDefault(p => p.IdPosjet == f.IdPosjet);
        var kt = posjet != null ? podaci.KontrolneTocke.FirstOrDefault(k => k.IdKontrolnaTocka == posjet.IdKontrolnaTocka) : null;

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