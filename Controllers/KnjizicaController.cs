using Microsoft.AspNetCore.Mvc;
using planinarenje.Entiteti;
using planinarenje.Models;
using System.Linq;

namespace planinarenje.Controllers;

public class KnjizicaController : Controller
{
    public IActionResult Index()
    {
        var podaci = Lab1PodaciFactory.Kreiraj();
        var model = podaci.Knjizice.Select(k => new KnjizicaIndexViewModel
        {
            IdKnjizica = k.IdKnjizica,
            IdKorisnik = k.IdKorisnik,
            ImePrezimeKorisnika = podaci.Korisnici.FirstOrDefault(u => u.IdKorisnik == k.IdKorisnik)?.Ime + " " + podaci.Korisnici.FirstOrDefault(u => u.IdKorisnik == k.IdKorisnik)?.Prezime ?? "Nepoznat planer",
            DatumKreiranja = k.DatumKreiranja,
            StatusAktivna = k.StatusAktivna
        }).OrderByDescending(k => k.DatumKreiranja).ToList();

        return View(model);
    }

    public IActionResult Details(int id)
    {
        var podaci = Lab1PodaciFactory.Kreiraj();
        var kn = podaci.Knjizice.FirstOrDefault(k => k.IdKnjizica == id);
        if (kn == null) return NotFound();

        var korisnik = podaci.Korisnici.FirstOrDefault(u => u.IdKorisnik == kn.IdKorisnik);
        
        var posjeti = podaci.Posjeti
            .Where(p => p.IdKnjizica == id)
            .OrderByDescending(p => p.DatumVrijemePosjeta)
            .Select(p => new KnjizicaPosjetViewModel
            {
                IdPosjet = p.IdPosjet,
                IdKontrolnaTocka = p.IdKontrolnaTocka,
                NazivKontrolneTocke = podaci.KontrolneTocke.FirstOrDefault(kt => kt.IdKontrolnaTocka == p.IdKontrolnaTocka)?.Naziv ?? "Nepoznato",
                DatumVrijemePosjeta = p.DatumVrijemePosjeta,
                JeLiPotvrdenPosjet = p.JeLiPotvrdenPosjet
            }).ToList();

        var model = new KnjizicaDetailsViewModel
        {
            IdKnjizica = kn.IdKnjizica,
            IdKorisnik = kn.IdKorisnik,
            ImePrezimeKorisnika = korisnik != null ? (korisnik.Ime + " " + korisnik.Prezime) : "Nepoznato",
            KorisnickoIme = korisnik?.KorisnickoIme ?? "nepoznato_ime",
            ProfilnaSlikaUrl = FormatProfileSlika(korisnik?.ProfilnaSlika),
            DatumKreiranja = kn.DatumKreiranja,
            StatusAktivna = kn.StatusAktivna,
            Napomena = kn.Napomena,
            Posjeti = posjeti
        };

        return View(model);
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
            }
            return relPath;
        }
        return null;
    }
}
