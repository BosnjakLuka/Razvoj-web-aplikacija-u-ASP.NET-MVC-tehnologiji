using Microsoft.AspNetCore.Mvc;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Repositories;
using System.IO;

namespace planinarenje.Controllers;

public class FotografijaController : Controller
{
    private readonly IFotografijaMockRepository _fotografijaRepository;
    private readonly IPosjetMockRepository _posjetRepository;
    private readonly IKontrolnaTockaMockRepository _kontrolnaTockaRepository;

    public FotografijaController(
        IFotografijaMockRepository fotografijaRepository,
        IPosjetMockRepository posjetRepository,
        IKontrolnaTockaMockRepository kontrolnaTockaRepository)
    {
        _fotografijaRepository = fotografijaRepository;
        _posjetRepository = posjetRepository;
        _kontrolnaTockaRepository = kontrolnaTockaRepository;
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
        var model = _fotografijaRepository.GetAll()
            .OrderByDescending(f => f.DatumUploada)
            .Select(f => {
                var posjet = _posjetRepository.GetById(f.IdPosjet);
                var kt = posjet != null ? _kontrolnaTockaRepository.GetById(posjet.IdKontrolnaTocka) : null;
                
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
        var f = _fotografijaRepository.GetById(id);

        if (f == null) return NotFound();

        var posjet = _posjetRepository.GetById(f.IdPosjet);
        var kt = posjet != null ? _kontrolnaTockaRepository.GetById(posjet.IdKontrolnaTocka) : null;

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