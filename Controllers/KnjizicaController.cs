using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
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
        var model = _dbContext.Knjizice
            .Include(k => k.Korisnik)
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

        return View(model);
    }

    public IActionResult Details(int id)
    {
        var kn = _dbContext.Knjizice
            .Include(k => k.Korisnik)
            .FirstOrDefault(k => k.IdKnjizica == id);
        if (kn == null) return NotFound();

        var posjeti = _dbContext.Posjeti
            .Where(p => p.IdKnjizica == id)
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
}
