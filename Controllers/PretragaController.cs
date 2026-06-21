using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.ViewModels;

namespace planinarenje.Controllers;

// Globalna (cross-entity) pretraga: jedan upit pretražuje više entiteta odjednom
// i vraća grupirane rezultate. Index = puna stranica, Live = JSON za navbar dropdown.
[AllowAnonymous]
public class PretragaController : BaseController
{
    // Koliko stavki po grupi prikazujemo na punoj stranici, a koliko u live dropdownu.
    private const int MaxPoGrupiStranica = 20;
    private const int MaxPoGrupiLive = 5;

    public PretragaController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db)
        : base(userMgr, db)
    {
    }

    [HttpGet]
    [Route("pretraga")]
    [Route("[controller]/[action]")]
    public IActionResult Index(string? q)
    {
        var model = SagradiModel(q, MaxPoGrupiStranica);
        ViewData["Title"] = "Pretraga";
        return View(model);
    }

    [HttpGet]
    [Route("pretraga/live")]
    [Route("[controller]/[action]")]
    public IActionResult Live(string? q)
    {
        var model = SagradiModel(q, MaxPoGrupiLive);

        var payload = new
        {
            upit = model.Upit,
            ukupno = model.UkupnoPoklapanja,
            grupe = model.Grupe.Select(g => new
            {
                naziv = g.Naziv,
                ikona = g.Ikona,
                ukupno = g.Ukupno,
                stavke = g.Stavke.Select(s => new
                {
                    naziv = s.Naziv,
                    podnaslov = s.Podnaslov,
                    url = Url.Action("Details", s.Controller, new { id = s.Id })
                })
            })
        };

        return Json(payload);
    }

    // Gradi grupirani model. Javni entiteti su dostupni svima; Korisnici samo Adminu
    // (i bez izlaganja osjetljivih polja - Email/OIB/JMBG se ne pretražuju ni ne prikazuju).
    private GlobalnaPretragaViewModel SagradiModel(string? q, int maxPoGrupi)
    {
        var model = new GlobalnaPretragaViewModel { Upit = q };
        if (!model.UpitJeValjan)
        {
            return model;
        }

        var term = q!.Trim();

        // --- Područja ---
        var podrucjaQuery = Db.Podrucja.Where(p => p.DeletedAt == null &&
            (p.Naziv.Contains(term) ||
             (p.Regija != null && p.Regija.Contains(term)) ||
             (p.Opis != null && p.Opis.Contains(term))));
        var podrucja = new PretragaGrupa { Naziv = "Područja", Controller = "Podrucje", Ikona = "geo-alt-fill", Ukupno = podrucjaQuery.Count() };
        podrucja.Stavke = podrucjaQuery.OrderBy(p => p.Naziv).Take(maxPoGrupi)
            .Select(p => new { p.IdPodrucje, p.Naziv, p.Regija }).ToList()
            .Select(p => new PretragaStavka { Id = p.IdPodrucje, Naziv = p.Naziv, Podnaslov = p.Regija, Controller = "Podrucje" })
            .ToList();
        DodajGrupu(model, podrucja);

        // --- Kontrolne točke ---
        var ktQuery = Db.KontrolneTocke.Where(k => k.DeletedAt == null &&
            (k.Naziv.Contains(term) ||
             k.GUIDOznaka.Contains(term) ||
             (k.Opis != null && k.Opis.Contains(term)) ||
             (k.Koordinate != null && k.Koordinate.Contains(term))));
        var kt = new PretragaGrupa { Naziv = "Kontrolne točke", Controller = "KontrolnaTocka", Ikona = "flag-fill", Ukupno = ktQuery.Count() };
        kt.Stavke = ktQuery.OrderBy(k => k.Naziv).Take(maxPoGrupi)
            .Select(k => new { k.IdKontrolnaTocka, k.Naziv, k.TipKontrolneTocke, PodrucjeNaziv = k.Podrucje.Naziv }).ToList()
            .Select(k => new PretragaStavka { Id = k.IdKontrolnaTocka, Naziv = k.Naziv, Podnaslov = $"{k.TipKontrolneTocke} · {k.PodrucjeNaziv}", Controller = "KontrolnaTocka" })
            .ToList();
        DodajGrupu(model, kt);

        // --- Rute ---
        var ruteQuery = Db.Rute.Where(r => r.DeletedAt == null &&
            (r.Naziv.Contains(term) ||
             r.Pocetak.Contains(term) ||
             r.Kraj.Contains(term) ||
             (r.OznakaNaTerenu != null && r.OznakaNaTerenu.Contains(term))));
        var rute = new PretragaGrupa { Naziv = "Rute", Controller = "Ruta", Ikona = "signpost-split-fill", Ukupno = ruteQuery.Count() };
        rute.Stavke = ruteQuery.OrderBy(r => r.Naziv).Take(maxPoGrupi)
            .Select(r => new { r.IdRuta, r.Naziv, r.Pocetak, r.Kraj }).ToList()
            .Select(r => new PretragaStavka { Id = r.IdRuta, Naziv = r.Naziv, Podnaslov = $"{r.Pocetak} → {r.Kraj}", Controller = "Ruta" })
            .ToList();
        DodajGrupu(model, rute);

        // --- Planinarski objekti ---
        var objektiQuery = Db.PlaninarskiObjekti.Where(o => o.DeletedAt == null &&
            (o.Naziv.Contains(term) ||
             (o.Adresa != null && o.Adresa.Contains(term)) ||
             (o.ImeOdgovorneOsobe != null && o.ImeOdgovorneOsobe.Contains(term))));
        var objekti = new PretragaGrupa { Naziv = "Planinarski objekti", Controller = "PlaninarskiObjekt", Ikona = "house-fill", Ukupno = objektiQuery.Count() };
        objekti.Stavke = objektiQuery.OrderBy(o => o.Naziv).Take(maxPoGrupi)
            .Select(o => new { o.IdPlaninarskiObjekt, o.Naziv, o.TipObjekta, o.Adresa }).ToList()
            .Select(o => new PretragaStavka { Id = o.IdPlaninarskiObjekt, Naziv = o.Naziv, Podnaslov = string.IsNullOrWhiteSpace(o.Adresa) ? o.TipObjekta.ToString() : o.Adresa, Controller = "PlaninarskiObjekt" })
            .ToList();
        DodajGrupu(model, objekti);

        // --- Planinarske udruge --- (OIB se namjerno NE pretražuje ni ne prikazuje)
        var udrugeQuery = Db.PlaninarskeUdruge.Where(u => u.DeletedAt == null &&
            (u.Naziv.Contains(term) ||
             (u.Grad != null && u.Grad.Contains(term)) ||
             (u.Zupanija != null && u.Zupanija.Contains(term))));
        var udruge = new PretragaGrupa { Naziv = "Planinarske udruge", Controller = "PlaninarskaUdruga", Ikona = "people-fill", Ukupno = udrugeQuery.Count() };
        udruge.Stavke = udrugeQuery.OrderBy(u => u.Naziv).Take(maxPoGrupi)
            .Select(u => new { u.IdPlaninarskaUdruga, u.Naziv, u.Grad }).ToList()
            .Select(u => new PretragaStavka { Id = u.IdPlaninarskaUdruga, Naziv = u.Naziv, Podnaslov = u.Grad, Controller = "PlaninarskaUdruga" })
            .ToList();
        DodajGrupu(model, udruge);

        // --- Medalje ---
        var medaljeQuery = Db.Medalje.Where(m => m.DeletedAt == null &&
            (m.Naziv.Contains(term) ||
             (m.Opis != null && m.Opis.Contains(term))));
        var medalje = new PretragaGrupa { Naziv = "Medalje", Controller = "Medalja", Ikona = "award-fill", Ukupno = medaljeQuery.Count() };
        medalje.Stavke = medaljeQuery.OrderBy(m => m.Naziv).Take(maxPoGrupi)
            .Select(m => new { m.IdMedalja, m.Naziv }).ToList()
            .Select(m => new PretragaStavka { Id = m.IdMedalja, Naziv = m.Naziv, Controller = "Medalja" })
            .ToList();
        DodajGrupu(model, medalje);

        // --- Korisnici --- (samo Admin; bez Email/OIB/JMBG u matchu i prikazu)
        if (IsAdmin)
        {
            var korisniciQuery = Db.Korisnici.Where(k => k.StatusAktivan &&
                (k.Ime.Contains(term) ||
                 k.Prezime.Contains(term) ||
                 k.KorisnickoIme.Contains(term)));
            var korisnici = new PretragaGrupa { Naziv = "Korisnici", Controller = "Korisnik", Ikona = "person-fill", Ukupno = korisniciQuery.Count() };
            korisnici.Stavke = korisniciQuery.OrderBy(k => k.Prezime).ThenBy(k => k.Ime).Take(maxPoGrupi)
                .Select(k => new { k.IdKorisnik, k.Ime, k.Prezime, k.KorisnickoIme }).ToList()
                .Select(k => new PretragaStavka { Id = k.IdKorisnik, Naziv = $"{k.Ime} {k.Prezime}", Podnaslov = "@" + k.KorisnickoIme, Controller = "Korisnik" })
                .ToList();
            DodajGrupu(model, korisnici);
        }

        return model;
    }

    private static void DodajGrupu(GlobalnaPretragaViewModel model, PretragaGrupa grupa)
    {
        if (grupa.Stavke.Count > 0)
        {
            model.Grupe.Add(grupa);
        }
    }
}
