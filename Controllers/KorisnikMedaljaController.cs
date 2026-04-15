using Microsoft.AspNetCore.Mvc;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Repositories;
using System.Linq;

namespace planinarenje.Controllers;

public class KorisnikMedaljaController : Controller
{
    private readonly IKorisnikMedaljaMockRepository _korisnikMedaljaRepository;
    private readonly IKorisnikMockRepository _korisnikRepository;
    private readonly IMedaljaMockRepository _medaljaRepository;

    public KorisnikMedaljaController(
        IKorisnikMedaljaMockRepository korisnikMedaljaRepository,
        IKorisnikMockRepository korisnikRepository,
        IMedaljaMockRepository medaljaRepository)
    {
        _korisnikMedaljaRepository = korisnikMedaljaRepository;
        _korisnikRepository = korisnikRepository;
        _medaljaRepository = medaljaRepository;
    }

    private string? FormatProfileSlika(string? absolutePath)
    {
        if (string.IsNullOrEmpty(absolutePath)) return null;
        var idx = absolutePath.IndexOf("Slike", StringComparison.OrdinalIgnoreCase);
        if (idx >= 0)
        {
            var relPath = "/" + absolutePath.Substring(idx).Replace("\\", "/");
            if (!System.IO.File.Exists(absolutePath))
            {
                if (absolutePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                {
                    var altPath = absolutePath.Substring(0, absolutePath.Length - 4) + ".jpeg";
                    if (System.IO.File.Exists(altPath))
                    {
                        return relPath.Substring(0, relPath.Length - 4) + ".jpeg";
                    }
                }
                return null; 
            }
            return relPath;
        }
        return null;
    }

    public IActionResult Index()
    {
        var model = _korisnikMedaljaRepository.GetAll()
            .OrderByDescending(km => km.DatumDodjele)
            .Select(km => {
                var korisnik = _korisnikRepository.GetById(km.IdKorisnik);
                var medalja = _medaljaRepository.GetById(km.IdMedalja);

                return new KorisnikMedaljaIndexViewModel
                {
                    IdKorisnikMedalja = km.IdKorisnikMedalja,
                    IdKorisnik = km.IdKorisnik,
                    ImePrezimeKorisnika = korisnik != null ? $"{korisnik.Ime} {korisnik.Prezime}" : "Nepoznati korisnik",
                    IdMedalja = km.IdMedalja,
                    NazivMedalje = medalja?.Naziv ?? "Nepoznata medalja",
                    DatumDodjele = km.DatumDodjele,
                    Napomena = km.Napomena
                };
            }).ToList();

        return View(model);
    }

    public IActionResult Details(int id)
    {
        var km = _korisnikMedaljaRepository.GetById(id);

        if (km == null) return NotFound();

        var korisnik = _korisnikRepository.GetById(km.IdKorisnik);
        var medalja = _medaljaRepository.GetById(km.IdMedalja);

        var model = new KorisnikMedaljaDetailsViewModel
        {
            IdKorisnikMedalja = km.IdKorisnikMedalja,
            IdKorisnik = km.IdKorisnik,
            ImePrezimeKorisnika = korisnik != null ? $"{korisnik.Ime} {korisnik.Prezime}" : "Nepoznati korisnik",
            ProfilnaSlikaUrl = FormatProfileSlika(korisnik?.ProfilnaSlika),
            IdMedalja = km.IdMedalja,
            NazivMedalje = medalja?.Naziv ?? "Nepoznata medalja",
            DatumDodjele = km.DatumDodjele,
            Napomena = km.Napomena
        };

        return View(model);
    }
}