using Microsoft.AspNetCore.Mvc;
using planinarenje.Entiteti;
using planinarenje.Models;
using System.Linq;

namespace planinarenje.Controllers;

public class KorisnikMedaljaController : Controller
{
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
        var podaci = Lab1PodaciFactory.Kreiraj();

        var model = podaci.KorisnikMedalje
            .OrderByDescending(km => km.DatumDodjele)
            .Select(km => {
                var korisnik = podaci.Korisnici.FirstOrDefault(k => k.IdKorisnik == km.IdKorisnik);
                var medalja = podaci.Medalje.FirstOrDefault(m => m.IdMedalja == km.IdMedalja);

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
        var podaci = Lab1PodaciFactory.Kreiraj();
        var km = podaci.KorisnikMedalje.FirstOrDefault(x => x.IdKorisnikMedalja == id);

        if (km == null) return NotFound();

        var korisnik = podaci.Korisnici.FirstOrDefault(k => k.IdKorisnik == km.IdKorisnik);
        var medalja = podaci.Medalje.FirstOrDefault(m => m.IdMedalja == km.IdMedalja);

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