using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
using System;
using System.IO;
using System.Linq;

namespace planinarenje.Controllers
{
    public class PosjetController : Controller
    {
        private readonly PlaninarstvoDbContext _dbContext;

        public PosjetController(PlaninarstvoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Helper za hrvatski opis doživljaja
        private string FormatirajDozivljaj(DozivljajPosjeta dozivljaj)
        {
            return dozivljaj switch
            {
                DozivljajPosjeta.VrloLagano => "Vrlo lagano",
                DozivljajPosjeta.Lagano => "Lagano",
                DozivljajPosjeta.Srednje => "Srednje težine",
                DozivljajPosjeta.Zahtjevno => "Zahtjevno",
                DozivljajPosjeta.VrloZahtjevno => "Vrlo zahtjevno",
                DozivljajPosjeta.KratkoAliTesko => "Kratko, ali teško",
                DozivljajPosjeta.DugoAliLagano => "Dugo, ali lagano",
                DozivljajPosjeta.FizickiNaporno => "Fizičko naporno",
                DozivljajPosjeta.TehnickiZahtjevno => "Tehnički zahtjevno",
                _ => dozivljaj.ToString()
            };
        }

        // Helper za Dohvaćanje slika
        private string FormatirajSliku(string? absolutePath)
        {
            if (string.IsNullOrWhiteSpace(absolutePath)) return "/Slike/Dizajn/hpo.jpg";

            if (absolutePath.StartsWith("/Slike/Fotografije/", StringComparison.OrdinalIgnoreCase))
            {
                return absolutePath;
            }

            if (absolutePath.Contains("\\Slike\\Fotografije\\", StringComparison.OrdinalIgnoreCase) ||
                absolutePath.Contains("/Slike/Fotografije/", StringComparison.OrdinalIgnoreCase))
            {
                return "/Slike/Fotografije/" + Path.GetFileName(absolutePath);
            }

            return "/Slike/Dizajn/hpo.jpg";
        }

        public IActionResult Index()
        {
            var model = _dbContext.Posjeti
                .Include(p => p.Korisnik)
                .Include(p => p.KontrolnaTocka)
                .Include(p => p.Ruta)
                .OrderByDescending(p => p.DatumVrijemePosjeta)
                .AsEnumerable()
                .Select(p => new PosjetIndexViewModel
                {
                    IdPosjet = p.IdPosjet,
                    ImePrezimeKorisnika = p.Korisnik != null ? $"{p.Korisnik.Ime} {p.Korisnik.Prezime}" : "Nepoznati korisnik",
                    NazivKontrolneTocke = p.KontrolnaTocka?.Naziv ?? "Nepoznata točka",
                    NazivRute = p.Ruta?.Naziv ?? "Samostalni posjet",
                    DatumVrijemePosjeta = p.DatumVrijemePosjeta,
                    Dozivljaj = FormatirajDozivljaj(p.DozivljajPosjeta),
                    JeLiPotvrdenPosjet = p.JeLiPotvrdenPosjet
                })
                .ToList();

            return View(model);
        }

        public IActionResult Details(int id)
        {
            var p = _dbContext.Posjeti
                .Include(x => x.Korisnik)
                .Include(x => x.Knjizica)
                .Include(x => x.KontrolnaTocka)
                .Include(x => x.Ruta)
                .Include(x => x.Fotografije)
                .FirstOrDefault(x => x.IdPosjet == id);

            if (p == null) return NotFound();

            var fotografijePosjeta = p.Fotografije
                .Select(f => new FotografijaPosjetaViewModel
                {
                    IdFotografija = f.IdFotografija,
                    NazivDatoteke = f.NazivDatoteke,
                    PutanjaUrl = FormatirajSliku(f.PutanjaDatoteke),
                    Opis = f.Opis
                })
                .ToList();

            var model = new PosjetDetailsViewModel
            {
                IdPosjet = p.IdPosjet,
                IdKorisnik = p.IdKorisnik,
                ImePrezimeKorisnika = p.Korisnik != null ? $"{p.Korisnik.Ime} {p.Korisnik.Prezime}" : "Nepoznati korisnik",
                IdKnjizica = p.IdKnjizica,
                NazivKnjizice = "Digitalna planinarska knjižica", // Default for display
                IdKontrolnaTocka = p.IdKontrolnaTocka,
                NazivKontrolneTocke = p.KontrolnaTocka?.Naziv ?? "Nepoznata točka",
                IdRuta = p.IdRuta,
                NazivRute = p.Ruta?.Naziv ?? "Nije definirano",
                DatumVrijemePosjeta = p.DatumVrijemePosjeta,
                VrijemeUsponaMin = p.VrijemeUsponaMin,
                Dozivljaj = FormatirajDozivljaj(p.DozivljajPosjeta),
                OpisIskustva = p.OpisIskustva,
                UneseniGUID = string.IsNullOrEmpty(p.UneseniGUID) ? "Nije evidentiran" : p.UneseniGUID,
                JeLiPotvrdenPosjet = p.JeLiPotvrdenPosjet,
                DatumKreiranjaZapisa = p.DatumKreiranjaZapisa,
                Fotografije = fotografijePosjeta
            };

            return View(model);
        }
    }
}