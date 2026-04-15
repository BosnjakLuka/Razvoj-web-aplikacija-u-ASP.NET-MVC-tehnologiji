using Microsoft.AspNetCore.Mvc;
using planinarenje.Entiteti;
using planinarenje.Models;
using System;
using System.Linq;

namespace planinarenje.Controllers
{
    public class PosjetController : Controller
    {
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
            if (string.IsNullOrEmpty(absolutePath)) return "/Slike/Dizajn/hpo.jpg";
            
            var idx = absolutePath.IndexOf("Slike", StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                // Ekstrahiraj relativnu putanju
                var relPath = "/" + absolutePath.Substring(idx).Replace("\\", "/");
                
                // Popravi grešku u podacima gdje piše .jpg a datoteka je .jpeg na disku
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
                    return "/Slike/Dizajn/hpo.jpg"; // Placeholder ako ne postoji
                }
                return relPath;
            }
            return "/Slike/Dizajn/hpo.jpg";
        }

        public IActionResult Index()
        {
            var podaci = Lab1PodaciFactory.Kreiraj();

            var model = podaci.Posjeti
                .OrderByDescending(p => p.DatumVrijemePosjeta)
                .Select(p => {
                    var korisnik = podaci.Korisnici.FirstOrDefault(k => k.IdKorisnik == p.IdKorisnik);
                    var kt = podaci.KontrolneTocke.FirstOrDefault(k => k.IdKontrolnaTocka == p.IdKontrolnaTocka);
                    var ruta = podaci.Rute.FirstOrDefault(r => r.IdRuta == p.IdRuta);

                    return new PosjetIndexViewModel
                    {
                        IdPosjet = p.IdPosjet,
                        ImePrezimeKorisnika = korisnik != null ? $"{korisnik.Ime} {korisnik.Prezime}" : "Nepoznati korisnik",
                        NazivKontrolneTocke = kt?.Naziv ?? "Nepoznata točka",
                        NazivRute = ruta?.Naziv ?? "Samostalni posjet",
                        DatumVrijemePosjeta = p.DatumVrijemePosjeta,
                        Dozivljaj = FormatirajDozivljaj(p.DozivljajPosjeta),
                        JeLiPotvrdenPosjet = p.JeLiPotvrdenPosjet
                    };
                }).ToList();

            return View(model);
        }

        public IActionResult Details(int id)
        {
            var podaci = Lab1PodaciFactory.Kreiraj();
            var p = podaci.Posjeti.FirstOrDefault(pos => pos.IdPosjet == id);

            if (p == null) return NotFound();

            var korisnik = podaci.Korisnici.FirstOrDefault(k => k.IdKorisnik == p.IdKorisnik);
            var knjizica = podaci.Knjizice.FirstOrDefault(knj => knj.IdKnjizica == p.IdKnjizica);
            var kt = podaci.KontrolneTocke.FirstOrDefault(k => k.IdKontrolnaTocka == p.IdKontrolnaTocka);
            var ruta = podaci.Rute.FirstOrDefault(r => r.IdRuta == p.IdRuta);
            
            var fotografijePosjeta = podaci.Fotografije
                .Where(f => f.IdPosjet == p.IdPosjet)
                .Select(f => new FotografijaPosjetaViewModel
                {
                    NazivDatoteke = f.NazivDatoteke,
                    PutanjaUrl = FormatirajSliku(f.PutanjaDatoteke),
                    Opis = f.Opis
                }).ToList();

            var model = new PosjetDetailsViewModel
            {
                IdPosjet = p.IdPosjet,
                IdKorisnik = p.IdKorisnik,
                ImePrezimeKorisnika = korisnik != null ? $"{korisnik.Ime} {korisnik.Prezime}" : "Nepoznati korisnik",
                IdKnjizica = p.IdKnjizica,
                NazivKnjizice = "Digitalna planinarska knjižica", // Default for display
                IdKontrolnaTocka = p.IdKontrolnaTocka,
                NazivKontrolneTocke = kt?.Naziv ?? "Nepoznata točka",
                IdRuta = p.IdRuta,
                NazivRute = ruta?.Naziv ?? "Nije definirano",
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