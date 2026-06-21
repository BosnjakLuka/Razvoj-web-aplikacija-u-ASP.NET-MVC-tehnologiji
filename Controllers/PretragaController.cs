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

    // Gradi grupirani model s case-insensitive pretraživanjem i rangiranjem po relevantnosti.
    // Javni entiteti su dostupni svima; Korisnici samo Adminu (bez Email/OIB/JMBG u matchu).
    private GlobalnaPretragaViewModel SagradiModel(string? q, int maxPoGrupi)
    {
        var model = new GlobalnaPretragaViewModel { Upit = q };
        if (!model.UpitJeValjan)
        {
            return model;
        }

        var term = q!.Trim();

        // --- Područja ---
        var podrucjaRezultati = Db.Podrucja.Where(p => p.DeletedAt == null)
            .Select(p => new { p.IdPodrucje, p.Naziv, p.Regija, p.Opis }).ToList()
            .Where(p => p.Naziv.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                       (p.Regija != null && p.Regija.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                       (p.Opis != null && p.Opis.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        var podrucja = new PretragaGrupa { Naziv = "Područja", Controller = "Podrucje", Ikona = "geo-alt-fill", Ukupno = podrucjaRezultati.Count() };
        podrucja.Stavke = podrucjaRezultati
            .Select(p => Stavka("Područje", "Podrucje", p.IdPodrucje, p.Naziv, p.Regija, term,
                ("Regija", p.Regija), ("Opis", Skrati(p.Opis))))
            .OrderByDescending(s => s.Skor).ThenBy(s => s.Naziv)
            .Take(maxPoGrupi)
            .ToList();
        DodajGrupu(model, podrucja);

        // --- Kontrolne točke ---
        var ktRezultati = Db.KontrolneTocke.Where(k => k.DeletedAt == null)
            .Select(k => new { k.IdKontrolnaTocka, k.Naziv, k.TipKontrolneTocke, k.GUIDOznaka, k.Opis, k.Koordinate, k.Podrucje }).ToList()
            .Where(k => k.Naziv.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                       k.GUIDOznaka.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                       (k.Opis != null && k.Opis.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                       (k.Koordinate != null && k.Koordinate.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        var kt = new PretragaGrupa { Naziv = "Kontrolne točke", Controller = "KontrolnaTocka", Ikona = "flag-fill", Ukupno = ktRezultati.Count() };
        kt.Stavke = ktRezultati
            .Select(k => Stavka("Kontrolna točka", "KontrolnaTocka", k.IdKontrolnaTocka, k.Naziv, $"{k.TipKontrolneTocke} · {k.Podrucje.Naziv}", term,
                ("Područje", k.Podrucje.Naziv), ("GUID oznaka", k.GUIDOznaka), ("Koordinate", k.Koordinate), ("Opis", Skrati(k.Opis))))
            .OrderByDescending(s => s.Skor).ThenBy(s => s.Naziv)
            .Take(maxPoGrupi)
            .ToList();
        DodajGrupu(model, kt);

        // --- Rute ---
        var ruteRezultati = Db.Rute.Where(r => r.DeletedAt == null)
            .Select(r => new { r.IdRuta, r.Naziv, r.Pocetak, r.Kraj, r.OznakaNaTerenu }).ToList()
            .Where(r => r.Naziv.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                       r.Pocetak.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                       r.Kraj.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                       (r.OznakaNaTerenu != null && r.OznakaNaTerenu.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        var rute = new PretragaGrupa { Naziv = "Rute", Controller = "Ruta", Ikona = "signpost-split-fill", Ukupno = ruteRezultati.Count() };
        rute.Stavke = ruteRezultati
            .Select(r => Stavka("Ruta", "Ruta", r.IdRuta, r.Naziv, $"{r.Pocetak} → {r.Kraj}", term,
                ("Početak", r.Pocetak), ("Kraj", r.Kraj), ("Oznaka na terenu", r.OznakaNaTerenu)))
            .OrderByDescending(s => s.Skor).ThenBy(s => s.Naziv)
            .Take(maxPoGrupi)
            .ToList();
        DodajGrupu(model, rute);

        // --- Planinarski objekti ---
        var objektiRezultati = Db.PlaninarskiObjekti.Where(o => o.DeletedAt == null)
            .Select(o => new { o.IdPlaninarskiObjekt, o.Naziv, o.TipObjekta, o.Adresa, o.ImeOdgovorneOsobe }).ToList()
            .Where(o => o.Naziv.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                       (o.Adresa != null && o.Adresa.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                       (o.ImeOdgovorneOsobe != null && o.ImeOdgovorneOsobe.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        var objekti = new PretragaGrupa { Naziv = "Planinarski objekti", Controller = "PlaninarskiObjekt", Ikona = "house-fill", Ukupno = objektiRezultati.Count() };
        objekti.Stavke = objektiRezultati
            .Select(o => Stavka("Planinarski objekt", "PlaninarskiObjekt", o.IdPlaninarskiObjekt, o.Naziv, string.IsNullOrWhiteSpace(o.Adresa) ? o.TipObjekta.ToString() : o.Adresa, term,
                ("Tip", o.TipObjekta.ToString()), ("Adresa", o.Adresa), ("Odgovorna osoba", o.ImeOdgovorneOsobe)))
            .OrderByDescending(s => s.Skor).ThenBy(s => s.Naziv)
            .Take(maxPoGrupi)
            .ToList();
        DodajGrupu(model, objekti);

        // --- Planinarske udruge ---
        var udrugeRezultati = Db.PlaninarskeUdruge.Where(u => u.DeletedAt == null)
            .Select(u => new { u.IdPlaninarskaUdruga, u.Naziv, u.Grad, u.Zupanija }).ToList()
            .Where(u => u.Naziv.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                       (u.Grad != null && u.Grad.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                       (u.Zupanija != null && u.Zupanija.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        var udruge = new PretragaGrupa { Naziv = "Planinarske udruge", Controller = "PlaninarskaUdruga", Ikona = "people-fill", Ukupno = udrugeRezultati.Count() };
        udruge.Stavke = udrugeRezultati
            .Select(u => Stavka("Planinarska udruga", "PlaninarskaUdruga", u.IdPlaninarskaUdruga, u.Naziv, u.Grad, term,
                ("Grad", u.Grad), ("Županija", u.Zupanija)))
            .OrderByDescending(s => s.Skor).ThenBy(s => s.Naziv)
            .Take(maxPoGrupi)
            .ToList();
        DodajGrupu(model, udruge);

        // --- Medalje ---
        var medaljeRezultati = Db.Medalje.Where(m => m.DeletedAt == null)
            .Select(m => new { m.IdMedalja, m.Naziv, m.Opis }).ToList()
            .Where(m => m.Naziv.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                       (m.Opis != null && m.Opis.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        var medalje = new PretragaGrupa { Naziv = "Medalje", Controller = "Medalja", Ikona = "award-fill", Ukupno = medaljeRezultati.Count() };
        medalje.Stavke = medaljeRezultati
            .Select(m => Stavka("Medalja", "Medalja", m.IdMedalja, m.Naziv, null, term,
                ("Opis", Skrati(m.Opis))))
            .OrderByDescending(s => s.Skor).ThenBy(s => s.Naziv)
            .Take(maxPoGrupi)
            .ToList();
        DodajGrupu(model, medalje);

        // --- Korisnici --- (samo Admin; bez Email/OIB/JMBG)
        if (IsAdmin)
        {
            var korisniciRezultati = Db.Korisnici.Where(k => k.StatusAktivan)
                .Select(k => new { k.IdKorisnik, k.Ime, k.Prezime, k.KorisnickoIme }).ToList()
                .Where(k => k.Ime.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                           k.Prezime.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                           k.KorisnickoIme.Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
            var korisnici = new PretragaGrupa { Naziv = "Korisnici", Controller = "Korisnik", Ikona = "person-fill", Ukupno = korisniciRezultati.Count() };
            korisnici.Stavke = korisniciRezultati
                .Select(k => Stavka("Korisnik", "Korisnik", k.IdKorisnik, $"{k.Ime} {k.Prezime}", "@" + k.KorisnickoIme, term,
                    ("Korisničko ime", k.KorisnickoIme)))
                .OrderByDescending(s => s.Skor).ThenBy(s => s.Naziv)
                .Take(maxPoGrupi)
                .ToList();
            DodajGrupu(model, korisnici);
        }

        // Grupe poredane po jačini pogotka (najbolja grupa - najviši max skor - idepredana).
        model.Grupe = model.Grupe
            .OrderByDescending(g => g.Stavke.Count > 0 ? g.Stavke.Max(s => s.Skor) : 0)
            .ThenBy(g => g.Naziv)
            .ToList();

        return model;
    }

    // Tvori stavku i odmah izračuna relevantnost na temelju naziva i upita.
    // polja = sva ostala pretraživana svojstva entiteta (prikazana u rezultatu da je
    // vidljivo po čemu je zapis pogodio upit, ne samo po Nazivu).
    private static PretragaStavka Stavka(string tip, string controller, int id, string naziv, string? podnaslov, string term,
        params (string Label, string? Vrijednost)[] polja)
    {
        var stavka = new PretragaStavka
        {
            Tip = tip,
            Controller = controller,
            Id = id,
            Naziv = naziv,
            Podnaslov = podnaslov,
            Skor = IzracunajSkor(naziv, term)
        };

        foreach (var (label, vrijednost) in polja)
        {
            if (!string.IsNullOrWhiteSpace(vrijednost))
            {
                stavka.Polja.Add(new KeyValuePair<string, string>(label, vrijednost));
            }
        }

        return stavka;
    }

    // Skraćuje duži tekst (npr. Opis) na pregledan isječak za prikaz u rezultatu.
    private static string? Skrati(string? tekst, int maxDuljina = 90)
    {
        if (string.IsNullOrWhiteSpace(tekst)) return null;
        tekst = tekst.Trim();
        return tekst.Length <= maxDuljina ? tekst : tekst[..maxDuljina].TrimEnd() + "…";
    }

    // Heuristika relevantnosti (all case-insensitive).
    private static int IzracunajSkor(string naziv, string term)
    {
        if (string.IsNullOrEmpty(naziv)) return 0;
        var cmp = StringComparison.OrdinalIgnoreCase;
        if (string.Equals(naziv, term, cmp)) return 100;
        if (naziv.StartsWith(term, cmp)) return 80;
        if (naziv.Contains(" " + term, cmp)) return 60;
        if (naziv.Contains(term, cmp)) return 40;
        return 15;
    }

    private static void DodajGrupu(GlobalnaPretragaViewModel model, PretragaGrupa grupa)
    {
        if (grupa.Stavke.Count > 0)
        {
            model.Grupe.Add(grupa);
        }
    }
}
