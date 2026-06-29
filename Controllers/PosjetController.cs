using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

namespace planinarenje.Controllers
{
    public class PosjetController : BaseController
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<PosjetController> _logger;
        private readonly planinarenje.Services.IAiUnosService _aiUnos;

        // Lab5 Faza 4: dopuštene ekstenzije i maksimalna veličina za Dropzone upload.
        private static readonly string[] DopusteneEkstenzije = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxVelicinaDatoteke = 5 * 1024 * 1024; // 5 MB

        public PosjetController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db, IWebHostEnvironment env, ILogger<PosjetController> logger, planinarenje.Services.IAiUnosService aiUnos)
            : base(userMgr, db)
        {
            _env = env;
            _logger = logger;
            _aiUnos = aiUnos;
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

            // Lab5 Faza 4: Dropzone uploadi spremaju web-relativnu putanju pod /uploads/.
            if (absolutePath.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            {
                return absolutePath;
            }

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

        [AllowAnonymous]
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

        [HttpGet]
        public IActionResult AutocompleteSearch(string term)
        {
            var query = Db.Posjeti
                .Include(p => p.KontrolnaTocka)
                .Where(p => p.DeletedAt == null);

            if (int.TryParse(term, out var id))
            {
                query = query.Where(p => p.IdPosjet == id);
            }
            else
            {
                query = query.Where(p => p.KontrolnaTocka != null && p.KontrolnaTocka.Naziv.Contains(term));
            }

            var results = query
                .OrderByDescending(p => p.DatumVrijemePosjeta)
                .Take(15)
                .Select(p => new
                {
                    value = p.IdPosjet,
                    label = p.KontrolnaTocka != null
                        ? "Posjet #" + p.IdPosjet + " - " + p.KontrolnaTocka.Naziv
                        : "Posjet #" + p.IdPosjet
                })
                .ToList();

            return Json(results);
        }

        [HttpGet]
        public IActionResult RuteZaKontrolnuTocku(int kontrolnaTockaId)
        {
            var results = Db.Rute
                .Where(r => r.DeletedAt == null && r.JeOdobreno && r.IdKontrolnaTocka == kontrolnaTockaId)
                .OrderBy(r => r.Naziv)
                .Select(r => new
                {
                    value = r.IdRuta,
                    label = r.Naziv + " (" + r.Pocetak + " -> " + r.Kraj + ")"
                })
                .ToList();

            return Json(results);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            PopulateKnjiziceByKorisnik();
            PopulateRuteByKontrolnaTocka(null);
            ViewData["Title"] = "Novi posjet";

            var model = new PosjetCreateModel
            {
                DatumVrijemePosjeta = DateTime.Now
            };

            // Posjet se uvijek evidentira na prijavljenog korisnika (vidi forsiranje vlasništva
            // u Create POST), pa polja Korisnik/Knjizica po defaultu popunjavamo iz prijave —
            // bez AI-a, čisto na temelju trenutne sesije.
            var korisnik = await GetCurrentKorisnikAsync();
            if (korisnik != null)
            {
                model.IdKorisnik = korisnik.IdKorisnik;
                model.IdKnjizica = await Db.Knjizice
                    .Where(k => k.StatusAktivna && k.IdKorisnik == korisnik.IdKorisnik)
                    .Select(k => k.IdKnjizica)
                    .FirstOrDefaultAsync();
                SetPosjetViewTexts(model.IdKorisnik, model.IdKnjizica, model.IdKontrolnaTocka, model.IdRuta);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(PosjetCreateModel model)
        {
            ValidatePosjetSelection(model);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validacija nije uspjela pri kreiranju posjeta za korisnika {AppUserId}.", AppUserId);
                PopulateKnjiziceByKorisnik();
                PopulateRuteByKontrolnaTocka(model.IdKontrolnaTocka, model.IdRuta);
                SetPosjetViewTexts(model.IdKorisnik, model.IdKnjizica, model.IdKontrolnaTocka, model.IdRuta);
                ViewData["Title"] = "Novi posjet";
                return View(model);
            }

            var kontrolnaTocka = Db.KontrolneTocke
                .AsNoTracking()
                .FirstOrDefault(k => k.DeletedAt == null && k.JeOdobreno && k.IdKontrolnaTocka == model.IdKontrolnaTocka);
            if (kontrolnaTocka == null)
            {
                _logger.LogWarning("Pokušaj kreiranja posjeta s nevaljanom kontrolnom točkom {IdKontrolnaTocka}.", model.IdKontrolnaTocka);
                ModelState.AddModelError(nameof(model.IdKontrolnaTocka), "Kontrolna tocka nije valjana.");
                PopulateKnjiziceByKorisnik();
                PopulateRuteByKontrolnaTocka(model.IdKontrolnaTocka, model.IdRuta);
                SetPosjetViewTexts(model.IdKorisnik, model.IdKnjizica, model.IdKontrolnaTocka, model.IdRuta);
                ViewData["Title"] = "Novi posjet";
                return View(model);
            }

            // force owner to current korisnik
            var currentKorisnik = await GetCurrentKorisnikAsync();
            if (currentKorisnik == null)
            {
                _logger.LogWarning("Pokušaj kreiranja posjeta bez povezanog planinarskog profila (AppUserId={AppUserId}).", AppUserId);
                return Forbid();
            }
            model.IdKorisnik = currentKorisnik.IdKorisnik;

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
                UneseniGUID = kontrolnaTocka.GUIDOznaka,
                JeLiPotvrdenPosjet = User.IsInRole("Admin") || User.IsInRole("Planinar"),
                DatumKreiranjaZapisa = DateTime.UtcNow
            };

            Db.Posjeti.Add(entity);
            await Db.SaveChangesAsync();
            _logger.LogInformation("Posjet {IdPosjet} kreiran za korisnika {IdKorisnik} na kontrolnoj točki {IdKontrolnaTocka}.",
                entity.IdPosjet, model.IdKorisnik, model.IdKontrolnaTocka);
            TempData["NewId"] = entity.IdPosjet;
            TempData["PosjetSuccess"] = true;
            TempData["Success"] = "Posjet je uspjesno dodan.";
            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------------------------------------
        // AI integracija — prijedlog podataka o posjetu iz prirodnog jezika
        // ---------------------------------------------------------------------
        // POST /Posjet/AiPrijedlog — prima slobodan opis posjeta, vraća prefill podatke
        // za Create formu. AI je samo "parser": izvuče nazive, a kontroler ih mapira na
        // ID-eve iz baze. GUID i korisnik se NE popunjavaju (ostaju ručni / server-side).
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AiPrijedlog(string upit)
        {
            if (string.IsNullOrWhiteSpace(upit))
            {
                return BadRequest(new { poruka = "Unesite kratak opis posjeta." });
            }

            var prijedlog = await _aiUnos.IzvuciPodatkeAsync(upit);
            var poruke = new List<string>();
            if (!string.IsNullOrWhiteSpace(prijedlog.Napomena))
            {
                poruke.Add(prijedlog.Napomena!);
            }

            // Mapiranje naziva kontrolne točke na ID — normalizirano podudaranje riječi
            // (neosjetljivo na dijakritiku i hrvatsku deklinaciju: Okić/Okića/Okiću/Okicu
            // imaju isti prefiks "okic"), ne literalni Contains koji puca na padežima.
            int? idKontrolnaTocka = null;
            string? ktNaziv = null;
            if (!string.IsNullOrWhiteSpace(prijedlog.KontrolnaTockaNaziv))
            {
                var upitneRijeci = KljucneRijeci(prijedlog.KontrolnaTockaNaziv);
                var kandidati = Db.KontrolneTocke
                    .Where(k => k.DeletedAt == null && k.JeOdobreno)
                    .Select(k => new { k.IdKontrolnaTocka, k.Naziv })
                    .ToList();

                var najbolji = kandidati
                    .Select(k => new { k.IdKontrolnaTocka, k.Naziv, Poeni = Poklapanje(upitneRijeci, KljucneRijeci(k.Naziv)) })
                    .Where(x => x.Poeni > 0)
                    .OrderByDescending(x => x.Poeni)
                    .ThenBy(x => x.Naziv.Length)
                    .FirstOrDefault();

                if (najbolji != null)
                {
                    idKontrolnaTocka = najbolji.IdKontrolnaTocka;
                    ktNaziv = najbolji.Naziv;
                }
                else
                {
                    poruke.Add($"Kontrolnu točku \"{prijedlog.KontrolnaTockaNaziv.Trim()}\" nisam pronašao u bazi — odaberite je ručno.");
                }
            }

            // Ruta se traži samo unutar pronađene kontrolne točke, istim normaliziranim podudaranjem.
            // Ako tekst ne uspije, a kontrolna točka ima samo jednu rutu, ta ruta je jedina razumna
            // pretpostavka pa se automatski odabire.
            int? idRuta = null;
            string? rutaNaziv = null;
            if (idKontrolnaTocka.HasValue)
            {
                var ruteKandidati = Db.Rute
                    .Where(r => r.DeletedAt == null && r.JeOdobreno && r.IdKontrolnaTocka == idKontrolnaTocka.Value)
                    .Select(r => new { r.IdRuta, r.Naziv })
                    .ToList();

                if (!string.IsNullOrWhiteSpace(prijedlog.RutaNaziv))
                {
                    var upitneRijeci = KljucneRijeci(prijedlog.RutaNaziv);
                    var najbolja = ruteKandidati
                        .Select(r => new { r.IdRuta, r.Naziv, Poeni = Poklapanje(upitneRijeci, KljucneRijeci(r.Naziv)) })
                        .Where(x => x.Poeni > 0)
                        .OrderByDescending(x => x.Poeni)
                        .ThenBy(x => x.Naziv.Length)
                        .FirstOrDefault();

                    if (najbolja != null)
                    {
                        idRuta = najbolja.IdRuta;
                        rutaNaziv = najbolja.Naziv;
                    }
                }

                if (idRuta == null && ruteKandidati.Count == 1)
                {
                    idRuta = ruteKandidati[0].IdRuta;
                    rutaNaziv = ruteKandidati[0].Naziv;
                }
                else if (idRuta == null && !string.IsNullOrWhiteSpace(prijedlog.RutaNaziv))
                {
                    poruke.Add($"Rutu \"{prijedlog.RutaNaziv.Trim()}\" nisam pronašao za tu točku — odaberite je ručno.");
                }
            }

            _logger.LogInformation(
                "AI prijedlog posjeta (korisnik {AppUserId}): KT={IdKontrolnaTocka}, Ruta={IdRuta}, Dozivljaj={Dozivljaj}.",
                AppUserId, idKontrolnaTocka, idRuta, prijedlog.DozivljajPosjeta);

            return Json(new
            {
                idKontrolnaTocka,
                kontrolnaTockaNaziv = ktNaziv,
                idRuta,
                rutaNaziv,
                datum = prijedlog.DatumVrijemePosjeta?.ToString("yyyy-MM-ddTHH:mm"),
                dozivljaj = prijedlog.DozivljajPosjeta.HasValue ? (int?)prijedlog.DozivljajPosjeta.Value : null,
                vrijemeUsponaMin = prijedlog.VrijemeUsponaMin,
                opisIskustva = prijedlog.OpisIskustva,
                poruka = poruke.Count > 0 ? string.Join(" ", poruke) : null
            });
        }

        [Authorize]
        [ActionName("Edit")]
        public async Task<IActionResult> EditGet(int id)
        {
            var entity = await Db.Posjeti
                .Include(p => p.Korisnik)
                .Include(p => p.Knjizica)
                .Include(p => p.KontrolnaTocka)
                .Include(p => p.Ruta)
                .FirstOrDefaultAsync(p => p.IdPosjet == id && p.DeletedAt == null);
            if (entity == null)
            {
                return NotFound();
            }

            if (!IsAdmin && !await IsOwnerAsync(entity.IdKorisnik))
                return Forbid();

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

            PopulateKnjiziceByKorisnik();
            PopulateRuteByKontrolnaTocka(model.IdKontrolnaTocka, model.IdRuta);
            SetPosjetViewTexts(model.IdKorisnik, model.IdKnjizica, model.IdKontrolnaTocka, model.IdRuta);
            ViewData["Title"] = "Uredi posjet";
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditPost(int id, PosjetEditModel model)
        {
            ValidatePosjetSelection(model);

            if (!ModelState.IsValid)
            {
                PopulateKnjiziceByKorisnik();
                PopulateRuteByKontrolnaTocka(model.IdKontrolnaTocka, model.IdRuta);
                SetPosjetViewTexts(model.IdKorisnik, model.IdKnjizica, model.IdKontrolnaTocka, model.IdRuta);
                ViewData["Title"] = "Uredi posjet";
                return View(model);
            }

            var kontrolnaTocka = Db.KontrolneTocke
                .AsNoTracking()
                .FirstOrDefault(k => k.DeletedAt == null && k.JeOdobreno && k.IdKontrolnaTocka == model.IdKontrolnaTocka);
            if (kontrolnaTocka == null)
            {
                ModelState.AddModelError(nameof(model.IdKontrolnaTocka), "Kontrolna tocka nije valjana.");
                PopulateKnjiziceByKorisnik();
                PopulateRuteByKontrolnaTocka(model.IdKontrolnaTocka, model.IdRuta);
                SetPosjetViewTexts(model.IdKorisnik, model.IdKnjizica, model.IdKontrolnaTocka, model.IdRuta);
                ViewData["Title"] = "Uredi posjet";
                return View(model);
            }

            var entity = await Db.Posjeti
                .FirstOrDefaultAsync(p => p.IdPosjet == id && p.DeletedAt == null);
            if (entity == null)
            {
                return NotFound();
            }

            // ownership check using original
            var original = await Db.Posjeti.AsNoTracking().FirstOrDefaultAsync(p => p.IdPosjet == id);
            if (original == null) return NotFound();
            if (!IsAdmin && !await IsOwnerAsync(original.IdKorisnik))
            {
                _logger.LogWarning("Korisnik {AppUserId} bez prava pristupa pokušao je urediti posjet {IdPosjet}.", AppUserId, id);
                return Forbid();
            }

            // force owner to original
            model.IdKorisnik = original.IdKorisnik;

            entity.IdKorisnik = model.IdKorisnik;
            entity.IdKnjizica = model.IdKnjizica;
            entity.IdKontrolnaTocka = model.IdKontrolnaTocka;
            entity.IdRuta = model.IdRuta;
            entity.DatumVrijemePosjeta = model.DatumVrijemePosjeta;
            entity.VrijemeUsponaMin = model.VrijemeUsponaMin;
            entity.DozivljajPosjeta = model.DozivljajPosjeta;
            entity.OpisIskustva = model.OpisIskustva;
            entity.UneseniGUID = kontrolnaTocka.GUIDOznaka;

            await Db.SaveChangesAsync();
            _logger.LogInformation("Posjet {IdPosjet} ažuriran (korisnik {AppUserId}).", id, AppUserId);
            TempData["Success"] = "Posjet je uspjesno azuriran.";
            return RedirectToAction(nameof(Index));
        }

        private string? GetKontrolnaTockaNaziv(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            return Db.KontrolneTocke
                .Where(k => k.DeletedAt == null && k.IdKontrolnaTocka == id.Value)
                .Select(k => k.Naziv)
                .FirstOrDefault();
        }

        private string? GetKorisnikLabel(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            return Db.Korisnici
                .Where(k => k.StatusAktivan && k.IdKorisnik == id.Value)
                .Select(k => k.Ime + " " + k.Prezime + " (@" + k.KorisnickoIme + ")")
                .FirstOrDefault();
        }

        private string? GetKnjizicaLabel(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            return Db.Knjizice
                .Include(k => k.Korisnik)
                .Where(k => k.StatusAktivna && k.IdKnjizica == id.Value)
                .Select(k => "Knjizica #" + k.IdKnjizica + " - " +
                             (k.Korisnik != null ? k.Korisnik.Ime + " " + k.Korisnik.Prezime : ""))
                .FirstOrDefault();
        }

        private string? GetRutaLabel(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            return Db.Rute
                .Where(r => r.DeletedAt == null && r.IdRuta == id.Value)
                .Select(r => r.Naziv + " (" + r.Pocetak + " -> " + r.Kraj + ")")
                .FirstOrDefault();
        }

        private string? GetKontrolnaTockaGuid(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            return Db.KontrolneTocke
                .Where(k => k.DeletedAt == null && k.IdKontrolnaTocka == id.Value)
                .Select(k => k.GUIDOznaka)
                .FirstOrDefault();
        }

        private void PopulateRuteByKontrolnaTocka(int? kontrolnaTockaId, int? selectedRutaId = null)
        {
            var routes = new List<SelectListItem>();

            if (kontrolnaTockaId.HasValue)
            {
                routes = Db.Rute
                    .Where(r => r.DeletedAt == null && r.JeOdobreno && r.IdKontrolnaTocka == kontrolnaTockaId.Value)
                    .OrderBy(r => r.Naziv)
                    .Select(r => new SelectListItem
                    {
                        Value = r.IdRuta.ToString(),
                        Text = r.Naziv + " (" + r.Pocetak + " -> " + r.Kraj + ")",
                        Selected = selectedRutaId.HasValue && r.IdRuta == selectedRutaId.Value
                    })
                    .ToList();
            }

            ViewBag.RuteOptions = routes;
        }

        private void SetPosjetViewTexts(int idKorisnik, int idKnjizica, int idKontrolnaTocka, int idRuta)
        {
            ViewData["KorisnikText"] = GetKorisnikLabel(idKorisnik);
            ViewData["KnjizicaText"] = GetKnjizicaLabel(idKnjizica);
            ViewData["KontrolnaTockaText"] = GetKontrolnaTockaNaziv(idKontrolnaTocka);
            ViewData["RutaText"] = GetRutaLabel(idRuta);
        }

        private void ValidatePosjetSelection(PosjetCreateModel model)
        {
            var kontrolnaTocka = Db.KontrolneTocke
                .AsNoTracking()
                .FirstOrDefault(k => k.DeletedAt == null && k.JeOdobreno && k.IdKontrolnaTocka == model.IdKontrolnaTocka);

            if (kontrolnaTocka == null)
            {
                return;
            }

            var ruta = Db.Rute
                .AsNoTracking()
                .FirstOrDefault(r => r.DeletedAt == null && r.JeOdobreno && r.IdRuta == model.IdRuta);

            if (ruta == null || ruta.IdKontrolnaTocka != model.IdKontrolnaTocka)
            {
                ModelState.AddModelError(nameof(model.IdRuta), "Odabrana ruta mora pripadati odabranoj kontrolnoj tocki.");
            }

            var uneseniGuid = model.UneseniGUID?.Trim();
            if (string.IsNullOrWhiteSpace(uneseniGuid))
            {
                ModelState.AddModelError(nameof(model.UneseniGUID), "GUID je obavezan.");
                return;
            }

            if (!string.Equals(uneseniGuid, kontrolnaTocka.GUIDOznaka, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(model.UneseniGUID), "GUID mora odgovarati odabranoj kontrolnoj tocki.");
            }
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await Db.Posjeti
                .Include(p => p.Korisnik)
                .Include(p => p.KontrolnaTocka)
                .Include(p => p.Ruta)
                .FirstOrDefaultAsync(p => p.IdPosjet == id && p.DeletedAt == null);
            if (entity == null)
            {
                return NotFound();
            }

            if (!IsAdmin && !await IsOwnerAsync(entity.IdKorisnik))
                return Forbid();

            ViewData["Title"] = "Obrisi posjet";
            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await Db.Posjeti
                .FirstOrDefaultAsync(p => p.IdPosjet == id && p.DeletedAt == null);
            if (entity == null)
            {
                return NotFound();
            }

            if (!IsAdmin && !await IsOwnerAsync(entity.IdKorisnik))
            {
                _logger.LogWarning("Korisnik {AppUserId} bez prava pristupa pokušao je obrisati posjet {IdPosjet}.", AppUserId, id);
                return Forbid();
            }

            entity.DeletedAt = DateTime.UtcNow;
            await Db.SaveChangesAsync();
            _logger.LogInformation("Posjet {IdPosjet} obrisan (soft delete) od korisnika {AppUserId}.", id, AppUserId);
            TempData["Success"] = "Posjet je uspjesno obrisan.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var p = await Db.Posjeti
                .Include(x => x.Korisnik)
                .Include(x => x.Knjizica)
                .Include(x => x.KontrolnaTocka)
                .Include(x => x.Ruta)
                .Include(x => x.Fotografije)
                .FirstOrDefaultAsync(x => x.IdPosjet == id && x.DeletedAt == null);

            if (p == null) return NotFound();

            // Lab5 Faza 4: samo vlasnik posjeta ili Admin smiju uploadati/brisati fotografije.
            var mozeUredivati = IsAdmin || await IsOwnerAsync(p.IdKorisnik);

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
                Fotografije = fotografijePosjeta,
                MozeUredivati = mozeUredivati
            };

            return View(model);
        }

        // ---------------------------------------------------------------------
        // Lab5 Faza 4 — Dropzone upload fotografija na Posjet
        // ---------------------------------------------------------------------

        // POST /Posjet/UploadFoto — prima jednu datoteku iz Dropzonea.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> UploadFoto(int idPosjet, IFormFile? file, TipSlike? tip)
        {
            var posjet = await Db.Posjeti
                .FirstOrDefaultAsync(p => p.IdPosjet == idPosjet && p.DeletedAt == null);
            if (posjet == null)
            {
                return NotFound();
            }

            // Vlasništvo: fotografije smije dodavati samo vlasnik posjeta ili Admin.
            if (!IsAdmin && !await IsOwnerAsync(posjet.IdKorisnik))
            {
                return Forbid();
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest(new { success = false, error = "Datoteka nije priložena." });
            }

            if (file.Length > MaxVelicinaDatoteke)
            {
                return BadRequest(new { success = false, error = "Datoteka je prevelika (maksimalno 5 MB)." });
            }

            var ekstenzija = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!DopusteneEkstenzije.Contains(ekstenzija))
            {
                return BadRequest(new { success = false, error = "Dopušteni su samo formati: JPG, PNG, WEBP." });
            }

            if (string.IsNullOrEmpty(file.ContentType) ||
                !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { success = false, error = "Datoteka mora biti slika." });
            }

            var webRoot = ResolveWebRoot();
            var apsolutniFolder = Path.Combine(webRoot, "uploads", "posjeti", idPosjet.ToString());
            Directory.CreateDirectory(apsolutniFolder);

            var spremljenoIme = $"{Guid.NewGuid()}{ekstenzija}";
            var apsolutnaPutanja = Path.Combine(apsolutniFolder, spremljenoIme);

            using (var stream = new FileStream(apsolutnaPutanja, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var entity = new Fotografija
            {
                IdPosjet = idPosjet,
                NazivDatoteke = Path.GetFileName(file.FileName),
                PutanjaDatoteke = $"/uploads/posjeti/{idPosjet}/{spremljenoIme}",
                TipSlike = tip ?? TipSlike.Krajolik,
                ContentType = file.ContentType,
                FileSize = file.Length,
                DatumUploada = DateTime.UtcNow
            };

            Db.Fotografije.Add(entity);
            await Db.SaveChangesAsync();

            return Json(new { success = true, id = entity.IdFotografija });
        }

        // GET /Posjet/GetFotografije — AJAX popis fotografija jednog posjeta (partial).
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetFotografije(int idPosjet)
        {
            var posjet = await Db.Posjeti
                .FirstOrDefaultAsync(p => p.IdPosjet == idPosjet && p.DeletedAt == null);
            if (posjet == null)
            {
                return NotFound();
            }

            var fotografije = await Db.Fotografije
                .Where(f => f.IdPosjet == idPosjet && f.DeletedAt == null)
                .OrderByDescending(f => f.DatumUploada)
                .ToListAsync();

            var model = fotografije
                .Select(f => new FotografijaPosjetaViewModel
                {
                    IdFotografija = f.IdFotografija,
                    NazivDatoteke = f.NazivDatoteke,
                    PutanjaUrl = FormatirajSliku(f.PutanjaDatoteke),
                    Opis = f.Opis
                })
                .ToList();

            ViewBag.MozeUredivati = IsAdmin || await IsOwnerAsync(posjet.IdKorisnik);
            return PartialView("_PosjetFotografijeListPartial", model);
        }

        // POST /Posjet/DeleteFoto — brisanje fotografije (s diska + soft delete u bazi).
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteFoto(int id)
        {
            var foto = await Db.Fotografije
                .Include(f => f.Posjet)
                .FirstOrDefaultAsync(f => f.IdFotografija == id && f.DeletedAt == null);
            if (foto == null)
            {
                return NotFound();
            }

            if (!IsAdmin && !await IsOwnerAsync(foto.Posjet.IdKorisnik))
            {
                return Forbid();
            }

            // Fizički ukloni datoteku s diska samo za Dropzone uploade (web putanja pod /uploads/).
            if (foto.PutanjaDatoteke.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            {
                var webRoot = ResolveWebRoot();
                var relativni = foto.PutanjaDatoteke.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var apsolutna = Path.Combine(webRoot, relativni);
                if (System.IO.File.Exists(apsolutna))
                {
                    System.IO.File.Delete(apsolutna);
                }
            }

            // U bazi koristimo soft delete (konvencija iz CLAUDE.md) — zapis ostaje kao trag.
            foto.DeletedAt = DateTime.UtcNow;
            await Db.SaveChangesAsync();

            return Json(new { success = true });
        }

        // Robustno razrješava wwwroot i kad se app pokreće iz bin/Debug.
        private string ResolveWebRoot()
        {
            if (!string.IsNullOrEmpty(_env.WebRootPath))
            {
                return _env.WebRootPath;
            }

            return Path.Combine(_env.ContentRootPath, "wwwroot");
        }

        private List<PosjetIndexViewModel> BuildIndexModel(string? searchTerm)
        {
            var query = Db.Posjeti
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
                    JeLiPotvrdenPosjet = p.JeLiPotvrdenPosjet,
                    AppUserId = p.Korisnik?.AppUserId
                })
                .ToList();
        }

        // Riječi koje se ne koriste za podudaranje naziva (prečesto se javljaju u različitim
        // zapisima pa bi davale lažna podudaranja).
        private static readonly HashSet<string> StopRijeciNaziva = new(StringComparer.Ordinal)
        {
            "vrh", "dom", "kuca", "kucica", "skloniste", "staza", "ruta", "tocka",
            "vidikovac", "planinarski", "planinarska", "kontrolna", "podrucje",
            "preko", "prema", "pod", "od", "do", "na", "kod", "pl"
        };

        // Razbija naziv na "ključne" riječi (min. 4 znaka, bez stop-riječi) za podudaranje
        // neosjetljivo na hrvatsku deklinaciju (Okić/Okića/Okiću dijele prefiks "okic").
        // Normalizacija dijakritike (planinarenje.Helpers.HrvatskiTekst) je zajednička s globalnom pretragom.
        private static List<string> KljucneRijeci(string naziv)
        {
            var normalizirano = planinarenje.Helpers.HrvatskiTekst.Normaliziraj(naziv);
            return Regex.Split(normalizirano, "[^a-z0-9]+")
                .Where(t => t.Length >= 4 && !StopRijeciNaziva.Contains(t))
                .ToList();
        }

        // Broji koliko se riječi iz AI upita poklapa (prefiksom od min. 4 znaka, što
        // apsorbira hrvatske padežne nastavke) s riječima naziva iz baze.
        private static int Poklapanje(List<string> upitneRijeci, List<string> nazivRijeci)
        {
            var brojac = 0;
            foreach (var u in upitneRijeci)
            {
                foreach (var n in nazivRijeci)
                {
                    var duljina = Math.Min(4, Math.Min(u.Length, n.Length));
                    if (duljina > 0 && u.Substring(0, duljina) == n.Substring(0, duljina))
                    {
                        brojac++;
                        break;
                    }
                }
            }
            return brojac;
        }

        private void PopulateKnjiziceByKorisnik()
        {
            var knjiziceByKorisnik = Db.Knjizice
                .Include(k => k.Korisnik)
                .Where(k => k.StatusAktivna)
                .OrderByDescending(k => k.DatumKreiranja)
                .GroupBy(k => k.IdKorisnik)
                .Select(g => new
                {
                    IdKorisnik = g.Key,
                    IdKnjizica = g.First().IdKnjizica,
                    Label = "Knjizica #" + g.First().IdKnjizica + " - " +
                            (g.First().Korisnik != null ? g.First().Korisnik.Ime + " " + g.First().Korisnik.Prezime : "")
                })
                .ToList();

            ViewBag.KnjiziceByKorisnik = knjiziceByKorisnik;
        }
    }
}