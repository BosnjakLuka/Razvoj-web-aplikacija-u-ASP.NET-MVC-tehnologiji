# CLAUDE.md — Planinarska web aplikacija

> Ovo je projektni kontekst za Claude Code. Pročitaj ga prije bilo kakvog rada u repozitoriju i drži ga se. Ako vidiš sukob između ovog filea i nečega drugog (osim eksplicitne upute korisnika u trenutnom razgovoru), ovaj file ima prioritet.

---

## 1. O projektu

**Naziv:** Razvoj web aplikacija u ASP.NET MVC tehnologiji — projekt *Planinarenje*
**Tip:** ASP.NET MVC web aplikacija (.NET 9), single-project solution
**Kolegij:** Razvoj web aplikacija u ASP.NET MVC tehnologiji (prof. Ivan Cesar)
**Repo:** https://github.com/BosnjakLuka/Razvoj-web-aplikacija-u-ASP.NET-MVC-tehnologiji
**Branch:** rad ide na `main` (sve što se ocjenjuje mora biti na `main`)

### Tema u jednoj rečenici
Aplikacija je **digitalna planinarska knjižica** — zamjena za papirnatu bilježnicu u koju planinari skupljaju žigove sa vrhova/vidikovaca. Korisnik u aplikaciji evidentira posjet kontrolnoj točki (unosom GUID/QR oznake sa žiga), bira rutu kojom je došao, upisuje doživljaj posjeta i uploada fotografije kao dokaz obilaska. Sustav prati medalje, područja i objekte (planinarski domovi/kuće/skloništa) te udruge.

### Glavni pojmovi (domenski rječnik — koristi ih doslovno u kodu)
- **Korisnik** — planinarski profil (ime, prezime, datum rođenja, kontakt) — odvojeno od `AppUser` koji drži Identity podatke
- **Knjizica** — digitalna planinarska knjižica, 1:1 sa Korisnikom
- **Posjet** — glavni radni entitet; jedan unos u knjižicu (korisnik + kontrolna točka + ruta + datum + GUID + doživljaj + fotografije)
- **KontrolnaTocka** — vrh / vidikovac / točka sa žigom (`GUIDOznaka` je unikatan kod sa žiga)
- **Ruta** — staza koja vodi do kontrolne točke
- **Podrucje** — planinarska regija (Medvednica, Velebit, Samoborsko gorje, …)
- **PlaninarskiObjekt** — dom/kuća/sklonište
- **PlaninarskaUdruga** — upravlja objektima
- **Medalja** / **KorisnikMedalja** — sustav nagrada za broj obilazaka

> ⚠️ U `LabosDokumenti/lab5/Lab5.md` se profesorov primjer referira na “kviz”. Kod nas se to **mapira na `Posjet`** (npr. Dropzone upload veže fotografije na konkretni `Posjet`, ne na “kviz”).

---

## 2. Tech stack

| Sloj | Tehnologija |
|---|---|
| Runtime | **.NET 9** |
| Framework | **ASP.NET Core MVC** (Razor Views, klasični MVC pattern — ne Razor Pages) |
| ORM | **Entity Framework Core** (code-first) |
| Baza | **MySQL** preko **Pomelo.EntityFrameworkCore.MySql** |
| Auth (Lab5) | **ASP.NET Core Identity** + Google OAuth |
| Frontend | Razor + **Bootstrap 5**, **jQuery**, AJAX |
| JS biblioteke | **flatpickr** (datepicker), **Dropzone** (upload), custom JS animacije |
| Testovi (Lab5) | **xUnit** + `Microsoft.AspNetCore.Mvc.Testing` + EF Core InMemory + FluentAssertions |
| API doc (Lab5) | **Swashbuckle / Swagger** (samo u Development) |

Connection string i provider su u `appsettings.json` / `appsettings.Development.json`.

---

## 3. Struktura repozitorija

```
/
├── .github/                       # GitHub konfiguracija
├── Controllers/                   # MVC kontroleri (NazivEntitetaController.cs)
│   └── Api/                       # API kontroleri (Lab5): NazivEntitetaApiController.cs
├── Data/                          # PlaninarstvoDbContext.cs, IdentitySeed.cs
├── Entiteti/                      # ⚠️ EF entitet klase su OVDJE (ne "Models/Entities")
│   ├── Korisnik.cs, Knjizica.cs, Posjet.cs, Fotografija.cs,
│   ├── KontrolnaTocka.cs, Ruta.cs, Podrucje.cs,
│   ├── PlaninarskiObjekt.cs, PlaninarskaUdruga.cs,
│   ├── Medalja.cs, KorisnikMedalja.cs, AppUser.cs (Lab5)
│   └── enums: DozivljajPosjeta, TipKontrolneTocke, TezinaRute, TipObjekta, TipSlike
├── Migrations/                    # EF Core migracije
├── Models/                        # ViewModels + DTO klase
│   ├── ViewModels/                # NazivEntitetaCreateModel, NazivEntitetaEditModel
│   └── Dto/                       # Read/Create/Update DTO klase za API (Lab5)
├── Repositories/                  # EF repository sloj
├── Properties/                    # launchSettings.json itd.
├── Slike/                         # Statički sadržaj (slike entiteta)
├── Views/                         # Razor view-ovi
│   ├── Shared/                    # _Layout, _ValidationScriptsPartial, partial views
│   └── NazivEntiteta/             # Index, Details, Create, Edit, Delete, _*Partial
├── wwwroot/                       # Statički web sadržaj (css, js, lib, uploads/)
│   └── uploads/posjeti/{id}/      # Dropzone destinacija (Lab5)
├── lab-1/                         # Log AI agenta iz Lab1 (zahtjev kolegija)
├── LabosDokumenti/                # 📚 IZVOR ISTINE — pročitaj prije rada (vidi sekciju 5)
├── Program.cs                     # DI, middleware pipeline, EF + Identity konfiguracija
├── appsettings.json
├── appsettings.Development.json   # connection string (lokalni MySQL)
├── planinarenje.csproj
├── planinarenje.slnx              # solution file (novi slnx format)
├── semantic-model.md              # Sažeti EF model (Lab3 deliverable)
├── sitemap.md                     # Routing model (Lab3 deliverable)
└── README.md
```

---

## 4. Stanje projekta po laboratorijima

| Lab | Stanje | Što sadrži |
|---|---|---|
| **Lab 1** | ✅ Predan | Objektni model, LINQ upiti, log AI agenta u `/lab-1/` |
| **Lab 2** | ✅ Predan | HTML binding, Razor view-ovi, _Layout, kostur dizajna (outdoor/hiking, tamno-plava + tamno-zelena paleta) |
| **Lab 3** | ✅ Predan | EF Core + MySQL, migracije, custom routing (≥4 akcije), `semantic-model.md`, `sitemap.md` |
| **Lab 4** | ✅ Predan | Kompletan CRUD za 11 entiteta, AJAX autocomplete dropdown, client+server validacija, flatpickr datepicker (partial view), JS animacije |
| **Lab 5** | ✅ Predan | Web API + DTO, Identity (lokalna + Google OAuth), Dropzone upload, integracijski testovi za API — detaljan plan u `LabosDokumenti/lab5/PlanImplementacije-Lab5.md` |

Sve lab vježbe (1–5) su predane. Aktualni rad je na **projektnim/seminarskim kriterijima** iznad lab vježbi (AI integracija, global search, MCP expose, logging — gotovo; deploy, potpuna pokrivenost testovima, responsive polish, "okvirni dojam" i usmeno ispitivanje — još otvoreno). Izvor istine za to je `LabosDokumenti/Ocjenjivanje-seminar.md` (checkbox tablica bodova) — provjeri je prije nego procijeniš što je "gotovo". Za usmeni ispit pripremu pogledaj `LabosDokumenti/lab5/Lab5-Priprema-Obrana.md`.

---

## 5. Izvor istine — `LabosDokumenti/` (OBVEZNO PROČITATI PRIJE RADA)

U `/LabosDokumenti/` postoji set markdown filea koji opisuju **što projekt mora biti i kako se to implementira**. Ovo nisu “nice to read”, **ovo je obveza kolegija**. `LabosDokumenti/` je reorganiziran u podfoldere po labu (`lab1/`–`lab5/`); root foldera sadrži samo presječne dokumente. Prije bilo kakve veće izmjene, otvori relevantne:

### Trenutno stanje i ocjenjivanje (provjeri prvo ovo)
- `Ocjenjivanje-seminar.md` — **checkbox tablica bodova za cijeli seminar/projekt** (deploy, AI integracija, testovi, responsive, MCP, usmeni ispit...). Ovo je izvor istine za "što je gotovo" — ne pretpostavljaj na temelju koda.
- `lab5/Lab5-Priprema-Obrana.md` — priprema za usmeno ispitivanje (40 bodova, najveća stavka)
- `lab5/Checklist-Faza3.md`, `Checklist-Faza4.md`, `Checklist-Faza5.md` — checkliste odrađenih faza Lab5 implementacije

### Domenski model i podaci
- `semantic-model.md` (root) — **autoritativan sažetak modela baze** (klase, svojstva, veze); zamjenjuje stari `finalni_model_planinarska_aplikacija_ispravljeno.md` koji je uklonjen iz repozitorija
- `lab1/dataset_planinarska_aplikacija.md` — konkretni seed podaci (područja Hrvatske, kontrolne točke, udruge, objekti — usklađeno s HPS izvorima)
- `dataset_prosireni_kontrolne_tocke_i_rute.md` (root) — proširen seed dataset kontrolnih točaka i ruta
- `lab1/linq_upiti.md` — primjeri LINQ upita nad modelom
- `lab1/Plan-Aplikacije-Planinarenje.md`, `lab3/Plan-Aplikacije-Planinarenje.md` — opći opis aplikacije (dupli sadržaj, postoji u oba foldera)

### Lab specifikacije
- `lab1/Lab-1.md`, `lab2/Lab 2 - HTML Binding.md`, `lab3/Lab3.md`, `lab4/Lab4.md`, `lab5/Lab5.md` — što svaki lab traži, kriteriji bodovanja
- `lab3/lab3-implementacija-vodic.md` — korak-po-korak vodič za Lab3 (EF + routing setup)
- `lab5/PlanImplementacije-Lab5.md` — **detaljan plan za Lab5** (faze 0–7, što je dodano, redoslijed, autorizacijska matrica) — Lab5 je predan, file je historijska referenca

### Dizajn i UX
- `lab2/kostur_dizajna.md` — vizualni identitet (outdoor/hiking, HPS inspiracija, modernizirana paleta), struktura početne stranice, pravila lista (kartice vs tablice), Details stranica
- `lab2/ux-subagent-prompt.md`, `lab2/plan-uxSubagent.prompt.md` — UX smjernice u kratkom obliku
- `lab2/dokazMockRepositoryDI.md`, `lab2/dokazPrompta.md` — dokazi AI/prompt rada za Lab2 (deliverable, ne dirati)

### Copilot promptovi (referentni — može se koristiti kao šablona za nove dijelove)
- `lab4/#1-crud-copilot-prompt.md` — kako radi CRUD (soft delete, ViewModels, AJAX search, TryUpdateModelAsync pattern, validacijske anotacije)
- `lab4/#2-autocomplete-validacija-copilot-prompt.md` — autocomplete dropdown + validacija
- `lab4/#3-js-animacije-copilot-prompt.md` — JS animacije
- `lab4/#4-datepicker-copilot-prompt.md` — flatpickr partial view
- `lab4/summit-animacija-copilot-prompt.md`, `lab4/medal-animacija-copilot-prompt.md` — specifične animacije

### Mali deliverable fileovi u rootu
- `semantic-model.md` — sažeti popis klasa, svojstava, veza (Lab3 bod)
- `sitemap.md` — za svaki URL: koji controller, koja akcija, koji view (Lab3 bod)

> **Pravilo:** Ako mijenjaš shemu, model, ili kompletan novi modul — prvo otvori odgovarajući file iz `LabosDokumenti/` i potvrdi konzistentnost. Ne improviziraj nazive entiteta, polja ni enuma; autoritativna referenca je `semantic-model.md`.

---

## 6. Konvencije koda

### Jezik i imenovanje
- **Imena entiteta, atributa, kontrolera, akcija — na hrvatskom**, kao u finalnom modelu (`KontrolnaTocka`, `IdPosjet`, `DozivljajPosjeta`, `PodrucjeController.Index`). Ne prevoditi u engleski.
- **Code keywords** (`public`, `class`, …) i framework pojmovi (`Controller`, `Index`, `Create`, `Edit`, `Delete`) — engleski (standard .NET).
- **Komentari i UI tekst** — hrvatski.
- **Pascal case** za klase, svojstva, metode i akcije. **Camel case** za lokalne varijable i parametre.

### MVC pravila
- Controller za pojam *Xyz* → `XyzController` (npr. `PosjetController`). View-ovi u `Views/Xyz/`.
- View-ovi ostaju prezentacijski. **Bez business logike u `.cshtml`** — agregirati u controlleru ili ViewModelu.
- API kontroleri idu u `Controllers/Api/`, naziv `XyzApiController`, sa `[ApiController]` i `[Route("api/xyz")]`.

### EF i model
- PK svojstva imaju format `IdNazivKlase` (npr. `IdKorisnik`, `IdPosjet`). Dodati `[Key]` atribut.
- FK svojstva: `int IdNeke`, uz `[ForeignKey("NavigationProperty")]`. Navigacijska svojstva su `public virtual NazivKlase NazivSvojstva { get; set; }`.
- Kolekcije: `public virtual ICollection<NazivKlase> NazivPlural { get; set; }`, inicijalizirati u konstruktoru.
- Soft delete: koristi `DeletedAt` (`DateTime?`) tamo gdje je dodano migracijom; `Korisnik` i `Knjizica` imaju `StatusAktivan` (bool). Pri Delete akciji **ne pozivati `_context.Remove()`** — postaviti flag i pozvati `SaveChangesAsync()`.
- Svaki query liste mora filtrirati: `.Where(x => x.DeletedAt == null)` ili `.Where(x => x.StatusAktivan)`.

### Forme i ViewModeli
- **Nikada ne bindati EF entitet direktno na formu.** Svaka entitet ima `XyzCreateModel` i `XyzEditModel` u `Models/ViewModels/`.
- ViewModel sadrži samo polja koja korisnik smije vidjeti i mijenjati (`PasswordHash`, `DatumRegistracije`, `DatumKreiranjaZapisa` ne idu na formu).
- Validacija: Data Annotations na ViewModelu (`[Required]`, `[StringLength]`, `[Range]`, `[RegularExpression]`, `[EmailAddress]` itd.) — uvijek sa hrvatskim `ErrorMessage`.
- Client-side validacija: jQuery Unobtrusive Validation; server-side: `if (!ModelState.IsValid) return View(model);`.

### CRUD pattern (Lab4 standard)
- GET Edit: dohvat entiteta → mapiranje u `EditModel` → vraćanje view-a
- POST Edit: validacija → dohvat originalnog entiteta → ručno mapiranje polja → `SaveChangesAsync()` → `TempData["Success"]` + redirect na Index
- Search/Filter: AJAX endpoint koji vraća `PartialView("_NazivListPartial", results)`
- Enum polja: `<select asp-items="Html.GetEnumSelectList<NazivEnuma>()">`
- FK dropdownovi: `SelectList` u `ViewBag`, sortirano po `Naziv`, filtrirano za soft delete

### API + DTO (Lab5)
- Nikada ne izlagati entitet direktno; uvijek `XyzDto` (read), `XyzCreateDto`, `XyzUpdateDto`
- `PasswordHash`, OIB, JMBG ne izlažu se u javnim API endpointima
- Standardni status kodovi: 200 / 201 (Created) / 204 (NoContent) / 400 / 401 / 403 / 404
- Query parametri za filtriranje (npr. `GET /api/posjet?korisnikId=1&datumOd=...`)
- **PodrucjeDto mora imati i `MinimalanBrojKTZaObilazak` (prag za medalju) I `BrojKontrolnihTocaka` (stvarni ukupni broj KT u tom području).** To su dva različita polja — jedno je uvjet, drugo je stvarno stanje. Nakon POST/PUT uvijek reloadaj entitet s `.Include(p => p.KontrolneTocke)` da se count ispravno vrati u odgovoru.
- GET lista s filterom koji ne pronađe ništa vraća **200 s praznim `[]`** — to nije greška. 404 se vraća samo za `GET /{id}` kad konkretan zapis ne postoji.

### Autorizacija (Lab5)
- Role: `Admin` i `Planinar`
- Public read (`[AllowAnonymous]`) za pregled javnih entiteta (KontrolnaTocka, Ruta, Podrucje, Objekti, Udruga, Medalja)
- `[Authorize(Roles = "Admin")]` za sve Create/Edit/Delete osim Posjeta
- Posjet/Fotografija — vlasništvo provjeravati ručno (`posjet.IdKorisnik == UserId` ili `User.IsInRole("Admin")`)

---

## 7. Komande

### Build / run
```bash
dotnet restore
dotnet build
dotnet run
```

### EF Core migracije
Pomelo MySQL provider, code-first. Iz root foldera (gdje je `planinarenje.csproj`):
```bash
dotnet ef migrations add NazivMigracije
dotnet ef database update
dotnet ef migrations remove          # samo ako migracija još nije primijenjena
```

Ako `dotnet ef` nije instaliran globalno:
```bash
dotnet tool install --global dotnet-ef
```

### Testovi (kad bude test projekt iz Lab5)
```bash
dotnet test
```

### User secrets (za Google OAuth — Lab5)
```bash
dotnet user-secrets init
dotnet user-secrets set "Authentication:Google:ClientId" "..."
dotnet user-secrets set "Authentication:Google:ClientSecret" "..."
```

---

## 8. Što Claude NE smije dirati

### Nikako bez eksplicitnog odobrenja
- **`appsettings.json` connection string** — ne mijenjati lozinke, hostove, baze. Ako nešto fali za development, reci mi u chatu.
- **`appsettings.Development.json`** kao gore.
- **User secrets** (`dotnet user-secrets`) — ne snimati ključeve u kod, nikada ne commitati `appsettings.*.json` sa pravim ključevima Google/FB OAuth.
- **`.github/`** workflow fileovi — ne mijenjati osim ako se eksplicitno traži.
- **`lab-1/`** — sadrži arhivski log AI agenta za Lab1 (deliverable), čisto historijski; ne dirati.
- **`LabosDokumenti/`** — ovo su materijali kolegija i deliverabli. Pročitati smije i mora; mijenjati samo ako se eksplicitno traži dorada nekog .md filea koji je vlastiti deliverable (`semantic-model.md`, `sitemap.md`, `LabosDokumenti/lab5/PlanImplementacije-Lab5.md`, `LabosDokumenti/Ocjenjivanje-seminar.md`).
- **Postojeće migracije u `Migrations/`** — nikada ne editirati datoteku migracije koja je već primijenjena na bazu. Ako treba korekcija, napravi novu migraciju.
- **`.vs/`** Visual Studio cache — ignorirati.
- **`temp.txt`** u rootu — ignorirati (privremena bilješka).

### Stvari koje treba uskladiti sa mnom prije promjene
- Schema baze (dodavanje/uklanjanje tablica i kolona) — predloži migraciju, ali ne pokreni `database update` bez potvrde
- Promjene u autorizacijskoj matrici (tko smije što) — provjeri prema `LabosDokumenti/lab5/PlanImplementacije-Lab5.md` Faza 2
- Promjene u routingu (`Program.cs` ili `[Route]` atributima) — može slomiti deliverable `sitemap.md`
- Dodavanje novih NuGet paketa — ok, ali javi koji i zašto u commit poruci

### Stvari koje slobodno radi bez pitanja
- View i CSS dorade unutar postojećih konvencija dizajna iz `LabosDokumenti/lab2/kostur_dizajna.md`
- ViewModel i DTO refaktoriranja (sve dok ne lome bind)
- Dodavanje LINQ upita, search filtera, novih akcija u postojeće kontrolere
- Bugfixovi (ali objasni što i zašto)
- Komentari, XML doc komentari, README dijelovi (ne LabosDokumenti)

---

## 9. Kako raditi sa mnom

- **Hrvatski jezik** u svim odgovorima i kodu (UI tekst, ErrorMessage, komentari).
- **Pitaj kad nije jasno** — ne pogađaj domenu. Ako se pojam ne nalazi u `semantic-model.md`, pitaj prije nego ga uvedeš.
- **Granularno po taskovima** — kolegij eksplicitno boduje “granularno izvođenje agenta po taskovima” (Lab1 kriterij). Ne diraj 50 stvari u jednom commitu; radije jedan task, jedan commit.
- **Prvo jedan, pa replikacija** — kad treba dodati nešto za svih 11 entiteta (npr. API kontrolere, integracijske testove), prvo napraviti `PosjetApiController` / `PosjetApiTests` do kraja, testirati, pa po tom obrascu replicirati ostale. Lab5 dokument upozorava na “AI slop” ako se odmah krene paralelno na sve.
- **Reci što nisi siguran** — bolje pitati nego izmišljati polja ili relacije.
- **Ne lomi postojeće Lab1-5 ekrane** — bilo koja nova izmjena (AI integracija, MCP, global search...) mora ostaviti netaknutim CRUD, validaciju, autocomplete, datepicker, API i Identity/OAuth.

---

## 10. Korisni quick reference

### 11 entiteta i njihove enum vrijednosti
- `Korisnik`, `Knjizica`, `Posjet`, `Fotografija`, `KontrolnaTocka`, `Ruta`, `Podrucje`, `PlaninarskiObjekt`, `PlaninarskaUdruga`, `Medalja`, `KorisnikMedalja`
- `DozivljajPosjeta`: VrloLagano, Lagano, Srednje, Zahtjevno, VrloZahtjevno, KratkoAliTesko, DugoAliLagano, FizickiNaporno, TehnickiZahtjevno
- `TipKontrolneTocke`: Vrh, Vidikovac, KontrolnaTocka
- `TezinaRute`: Laka, Srednja, Teska
- `TipObjekta`: Dom, Kuca, Skloniste
- `TipSlike`: Selfie, Oznaka, Krajolik, Mapa, Drugo

### Ključne relacije (skraćeno)
- `Korisnik` 1:1 `Knjizica`
- `Korisnik` 1:N `Posjet`, `Knjizica` 1:N `Posjet`
- `Posjet` 1:N `Fotografija`
- `Podrucje` 1:N `KontrolnaTocka`, `KontrolnaTocka` 1:N `Ruta`, `Ruta` 1:N `Posjet`
- `Podrucje` 1:N `PlaninarskiObjekt`, `PlaninarskaUdruga` 1:N `PlaninarskiObjekt`
- `Korisnik` N:N `Medalja` preko `KorisnikMedalja`

### Vizualni identitet (kratko)
Outdoor / hiking portal, tamno-plava (navigacija) + tamno-zelena (akcent) + maslinasta (sekundarno) + bež (kartice) + bijela/svijetlo siva (površine) + narančasta/planinarska crvena (CTA). **Ne izgleda kao default Bootstrap.** Detalji u `LabosDokumenti/lab2/kostur_dizajna.md`.

---

*Ako ti nešto u ovom fileu nije jasno ili djeluje zastarjelo s obzirom na trenutno stanje koda — pitaj prije nego pretpostaviš. Ovaj file je živ, ažurirat će se kako projekt napreduje.*
