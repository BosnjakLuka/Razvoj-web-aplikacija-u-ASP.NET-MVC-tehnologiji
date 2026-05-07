---

# Sitemap — Planinarska aplikacija

## Svi URL-ovi aplikacije

| # | URL | Metoda | Controller | Akcija | View datoteka | Tip rute |
|---|-----|--------|------------|--------|---------------|----------|
| 1 | / | GET | HomeController | Index | Views/Home/Index.cshtml | Custom [Route] |
| 2 | /naslovnica | GET | HomeController | Index | Views/Home/Index.cshtml | Custom [Route] |
| 3 | /Home/Index | GET | HomeController | Index | Views/Home/Index.cshtml | Custom [Route] |
| 4 | /Home/Privacy | GET | HomeController | Privacy | Views/Home/Privacy.cshtml | Default |
| 5 | /Home/Error | GET | HomeController | Error | Views/Shared/Error.cshtml | Default |
| 6 | /KontrolnaTocka | GET | KontrolnaTockaController | Index | Views/KontrolnaTocka/Index.cshtml | Default |
| 7 | /KontrolnaTocka/Details/{id} | GET | KontrolnaTockaController | Details | Views/KontrolnaTocka/Details.cshtml | Custom [Route] |
| 8 | /vrh/{id} | GET | KontrolnaTockaController | Details | Views/KontrolnaTocka/Details.cshtml | Custom [Route] |
| 9 | /Podrucje | GET | PodrucjeController | Index | Views/Podrucje/Index.cshtml | Default |
| 10 | /Podrucje/Details/{id} | GET | PodrucjeController | Details | Views/Podrucje/Details.cshtml | Default |
| 11 | /podrucje/{id}/tocke | GET | PodrucjeController | KontrolneTockePodrucja | Views/Podrucje/KontrolneTockePodrucja.cshtml | Custom [Route] |
| 12 | /Ruta | GET | RutaController | Index | Views/Ruta/Index.cshtml | Default |
| 13 | /Ruta/Details/{id} | GET | RutaController | Details | Views/Ruta/Details.cshtml | Default |
| 14 | /rute/tezina/{tezina} | GET | RutaController | PoTezini | Views/Ruta/PoTezini.cshtml | Custom [Route] |
| 15 | /Korisnik | GET | KorisnikController | Index | Views/Korisnik/Index.cshtml | Default |
| 16 | /Korisnik/Details/{id} | GET | KorisnikController | Details | Views/Korisnik/Details.cshtml | Custom [Route] |
| 17 | /planinar/{id} | GET | KorisnikController | Details | Views/Korisnik/Details.cshtml | Custom [Route] |
| 18 | /Posjet | GET | PosjetController | Index | Views/Posjet/Index.cshtml | Default |
| 19 | /Posjet/Details/{id} | GET | PosjetController | Details | Views/Posjet/Details.cshtml | Default |
| 20 | /Fotografija | GET | FotografijaController | Index | Views/Fotografija/Index.cshtml | Default |
| 21 | /Fotografija/Details/{id} | GET | FotografijaController | Details | Views/Fotografija/Details.cshtml | Default |
| 22 | /Medalja | GET | MedaljaController | Index | Views/Medalja/Index.cshtml | Default |
| 23 | /Medalja/Details/{id} | GET | MedaljaController | Details | Views/Medalja/Details.cshtml | Default |
| 24 | /PlaninarskaUdruga | GET | PlaninarskaUdrugaController | Index | Views/PlaninarskaUdruga/Index.cshtml | Default |
| 25 | /PlaninarskaUdruga/Details/{id} | GET | PlaninarskaUdrugaController | Details | Views/PlaninarskaUdruga/Details.cshtml | Default |
| 26 | /PlaninarskiObjekt | GET | PlaninarskiObjektController | Index | Views/PlaninarskiObjekt/Index.cshtml | Default |
| 27 | /PlaninarskiObjekt/Details/{id} | GET | PlaninarskiObjektController | Details | Views/PlaninarskiObjekt/Details.cshtml | Default |
| 28 | /Knjizica | GET | KnjizicaController | Index | Views/Knjizica/Index.cshtml | Default |
| 29 | /Knjizica/Details/{id} | GET | KnjizicaController | Details | Views/Knjizica/Details.cshtml | Default |
| 30 | /KorisnikMedalja | GET | KorisnikMedaljaController | Index | Views/KorisnikMedalja/Index.cshtml | Default |
| 31 | /KorisnikMedalja/Details/{id} | GET | KorisnikMedaljaController | Details | Views/KorisnikMedalja/Details.cshtml | Default |

## Custom rute — pregled

| Custom URL | Opis | Controller | Akcija |
|------------|------|------------|--------|
| /vrh/{id} | Detalji kontrolne točke | KontrolnaTockaController | Details |
| /podrucje/{id}/tocke | KT unutar područja | PodrucjeController | KontrolneTockePodrucja |
| /planinar/{id} | Profil korisnika | KorisnikController | Details |
| /rute/tezina/{tezina} | Rute po težini | RutaController | PoTezini |
| /naslovnica | Početna stranica | HomeController | Index |
| / | Početna stranica | HomeController | Index |
| /Home/Index | Početna stranica | HomeController | Index |
| /KontrolnaTocka/Details/{id} | Detalji kontrolne točke | KontrolnaTockaController | Details |
| /Korisnik/Details/{id} | Profil korisnika | KorisnikController | Details |

## Routing konfiguracija

### Default ruta (Program.cs)

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```
