using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
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
        var model = _dbContext.KorisnikMedalje
            .Include(km => km.Korisnik)
            .Include(km => km.Medalja)
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

        return View(model);
    }

    public IActionResult Details(int id)
    {
        var km = _dbContext.KorisnikMedalje
            .Include(x => x.Korisnik)
            .Include(x => x.Medalja)
            .FirstOrDefault(x => x.IdKorisnikMedalja == id);

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
}