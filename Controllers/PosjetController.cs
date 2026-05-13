using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Models.ViewModels;
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
            var model = BuildIndexModel(null);
            ViewData["Title"] = "Posjeti";
            return View(model);
        }

        [HttpGet]
        public IActionResult Search(string? searchTerm)
        {
            var model = BuildIndexModel(searchTerm);
            return PartialView("_PosjetListPartial", model);
        }

        public IActionResult Create()
        {
            PopulateDropdowns();
            ViewData["Title"] = "Novi posjet";
            return View(new PosjetCreateModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PosjetCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model.IdKorisnik, model.IdKnjizica, model.IdKontrolnaTocka, model.IdRuta);
                ViewData["Title"] = "Novi posjet";
                return View(model);
            }

            var entity = new Posjet
            {
                IdKorisnik = model.IdKorisnik,
                IdKnjizica = model.IdKnjizica,
                IdKontrolnaTocka = model.IdKontrolnaTocka,
                IdRuta = model.IdRuta,
                DatumVrijemePosjeta = model.DatumVrijemePosjeta,
                VrijemeUsponaMin = model.VrijemeUsponaMin,
                DozivljajPosjeta = model.DozivljajPosjeta,
                OpisIskustva = model.OpisIskustva,
                UneseniGUID = model.UneseniGUID,
                JeLiPotvrdenPosjet = false,
                DatumKreiranjaZapisa = DateTime.UtcNow
            };

            _dbContext.Posjeti.Add(entity);
            _dbContext.SaveChanges();
            TempData["Success"] = "Posjet je uspjesno dodan.";
            return RedirectToAction(nameof(Index));
        }

        [ActionName("Edit")]
        public IActionResult EditGet(int id)
        {
            var entity = _dbContext.Posjeti
                .FirstOrDefault(p => p.IdPosjet == id && p.DeletedAt == null);
            if (entity == null)
            {
                return NotFound();
            }

            var model = new PosjetEditModel
            {
                IdKorisnik = entity.IdKorisnik,
                IdKnjizica = entity.IdKnjizica,
                IdKontrolnaTocka = entity.IdKontrolnaTocka,
                IdRuta = entity.IdRuta,
                DatumVrijemePosjeta = entity.DatumVrijemePosjeta,
                VrijemeUsponaMin = entity.VrijemeUsponaMin,
                DozivljajPosjeta = entity.DozivljajPosjeta,
                OpisIskustva = entity.OpisIskustva,
                UneseniGUID = entity.UneseniGUID
            };

            PopulateDropdowns(model.IdKorisnik, model.IdKnjizica, model.IdKontrolnaTocka, model.IdRuta);
            ViewData["Title"] = "Uredi posjet";
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult EditPost(int id, PosjetEditModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model.IdKorisnik, model.IdKnjizica, model.IdKontrolnaTocka, model.IdRuta);
                ViewData["Title"] = "Uredi posjet";
                return View(model);
            }

            var entity = _dbContext.Posjeti
                .FirstOrDefault(p => p.IdPosjet == id && p.DeletedAt == null);
            if (entity == null)
            {
                return NotFound();
            }

            entity.IdKorisnik = model.IdKorisnik;
            entity.IdKnjizica = model.IdKnjizica;
            entity.IdKontrolnaTocka = model.IdKontrolnaTocka;
            entity.IdRuta = model.IdRuta;
            entity.DatumVrijemePosjeta = model.DatumVrijemePosjeta;
            entity.VrijemeUsponaMin = model.VrijemeUsponaMin;
            entity.DozivljajPosjeta = model.DozivljajPosjeta;
            entity.OpisIskustva = model.OpisIskustva;
            entity.UneseniGUID = model.UneseniGUID;

            _dbContext.SaveChanges();
            TempData["Success"] = "Posjet je uspjesno azuriran.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var entity = _dbContext.Posjeti
                .Include(p => p.Korisnik)
                .Include(p => p.KontrolnaTocka)
                .Include(p => p.Ruta)
                .FirstOrDefault(p => p.IdPosjet == id && p.DeletedAt == null);
            if (entity == null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Obrisi posjet";
            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var entity = _dbContext.Posjeti
                .FirstOrDefault(p => p.IdPosjet == id && p.DeletedAt == null);
            if (entity == null)
            {
                return NotFound();
            }

            entity.DeletedAt = DateTime.UtcNow;
            _dbContext.SaveChanges();
            TempData["Success"] = "Posjet je uspjesno obrisan.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            var p = _dbContext.Posjeti
                .Include(x => x.Korisnik)
                .Include(x => x.Knjizica)
                .Include(x => x.KontrolnaTocka)
                .Include(x => x.Ruta)
                .Include(x => x.Fotografije)
                .FirstOrDefault(x => x.IdPosjet == id && x.DeletedAt == null);

            if (p == null) return NotFound();

            var fotografijePosjeta = p.Fotografije
                .Where(f => f.DeletedAt == null)
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

        private List<PosjetIndexViewModel> BuildIndexModel(string? searchTerm)
        {
            var query = _dbContext.Posjeti
                .Include(p => p.Korisnik)
                .Include(p => p.KontrolnaTocka)
                .Include(p => p.Ruta)
                .Where(p => p.DeletedAt == null &&
                            (p.Korisnik == null || p.Korisnik.StatusAktivan) &&
                            (p.KontrolnaTocka == null || p.KontrolnaTocka.DeletedAt == null) &&
                            (p.Ruta == null || p.Ruta.DeletedAt == null));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(p =>
                    (p.Korisnik != null &&
                     (p.Korisnik.Ime.Contains(term) || p.Korisnik.Prezime.Contains(term))) ||
                    (p.KontrolnaTocka != null && p.KontrolnaTocka.Naziv.Contains(term)) ||
                    (p.Ruta != null && p.Ruta.Naziv.Contains(term)) ||
                    p.UneseniGUID.Contains(term));
            }

            return query
                .OrderByDescending(p => p.DatumVrijemePosjeta)
                .AsEnumerable()
                .Select(p => new PosjetIndexViewModel
                {
                    IdPosjet = p.IdPosjet,
                    ImePrezimeKorisnika = p.Korisnik != null ? $"{p.Korisnik.Ime} {p.Korisnik.Prezime}" : "Nepoznati korisnik",
                    NazivKontrolneTocke = p.KontrolnaTocka?.Naziv ?? "Nepoznata tocka",
                    NazivRute = p.Ruta?.Naziv ?? "Samostalni posjet",
                    DatumVrijemePosjeta = p.DatumVrijemePosjeta,
                    Dozivljaj = FormatirajDozivljaj(p.DozivljajPosjeta),
                    JeLiPotvrdenPosjet = p.JeLiPotvrdenPosjet
                })
                .ToList();
        }

        private void PopulateDropdowns(int? korisnikId = null, int? knjizicaId = null, int? kontrolnaTockaId = null, int? rutaId = null)
        {
            var korisnici = _dbContext.Korisnici
                .Where(k => k.StatusAktivan)
                .OrderBy(k => k.Prezime)
                .ThenBy(k => k.Ime)
                .Select(k => new { k.IdKorisnik, Ime = k.Ime + " " + k.Prezime })
                .ToList();

            var knjizice = _dbContext.Knjizice
                .Where(k => k.StatusAktivna)
                .OrderByDescending(k => k.DatumKreiranja)
                .Select(k => new { k.IdKnjizica, Naziv = "Knjizica #" + k.IdKnjizica })
                .ToList();

            var kontrolneTocke = _dbContext.KontrolneTocke
                .Where(k => k.DeletedAt == null)
                .OrderBy(k => k.Naziv)
                .ToList();

            var rute = _dbContext.Rute
                .Where(r => r.DeletedAt == null)
                .OrderBy(r => r.Naziv)
                .ToList();

            ViewBag.Korisnici = new SelectList(korisnici, "IdKorisnik", "Ime", korisnikId);
            ViewBag.Knjizice = new SelectList(knjizice, "IdKnjizica", "Naziv", knjizicaId);
            ViewBag.KontrolneTocke = new SelectList(kontrolneTocke, "IdKontrolnaTocka", "Naziv", kontrolnaTockaId);
            ViewBag.Rute = new SelectList(rute, "IdRuta", "Naziv", rutaId);
        }
    }
}