# Implementacija dodatnih seminarskih cjelina

> Ovaj dokument objašnjava **kako su implementirane** seminarske cjeline koje nadilaze lab vježbe 1–5 (vidi `Ocjenjivanje-seminar.md` za bodovnu tablicu). Redoslijed odgovara kronologiji rada na projektu (vidi git log): MCP → Global search → AI integracija → Testovi za sve endpointe → Deploy → Logging je ugrađen od ranije i nadograđivan usput.

Pokriveno:
1. [Expose MCP i pristup kroz agentic IDE](#1-expose-mcp-i-pristup-kroz-agentic-ide)
2. [Global search](#2-global-search)
3. [AI integracija — unos podataka putem AI upita](#3-ai-integracija--unos-podataka-putem-ai-upita)
4. [Kreiranje testova za sve endpointe](#4-kreiranje-testova-za-sve-endpointe)
5. [Implementacija logging mehanizma](#5-implementacija-logging-mehanizma)
6. [Deploy na cloud provider (Azure)](#6-deploy-na-cloud-provider-azure)

---

## 1. Expose MCP i pristup kroz agentic IDE

### Što je cilj
Izložiti domenske podatke aplikacije (Korisnik, Posjet, KontrolnaTocka, Ruta, Podrucje, ...) kao **MCP (Model Context Protocol)** servere, tako da agentic IDE (Claude Code, Cursor) može čitati stvarne podatke iz baze tijekom rada na projektu, bez ručnog kopiranja SQL rezultata u chat.

### Arhitektura
MCP server je ugrađen direktno u istu ASP.NET Core aplikaciju (paket `ModelContextProtocol.AspNetCore`), izložen preko HTTP transporta na `/mcp` endpointu.

**`Program.cs`** — registracija (sekcija s `AddMcpServer`):
```csharp
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<KontrolnaTockaMcpTools>()
    .WithTools<KorisnikMcpTools>()
    .WithTools<KnjizicaMcpTools>()
    .WithTools<PosjetMcpTools>()
    .WithTools<FotografijaMcpTools>()
    .WithTools<RutaMcpTools>()
    .WithTools<PodrucjeMcpTools>()
    .WithTools<PlaninarskiObjektMcpTools>()
    .WithTools<PlaninarskaUdrugaMcpTools>()
    .WithTools<MedaljaMcpTools>()
    .WithTools<KorisnikMedaljaMcpTools>();
...
app.MapMcp("/mcp");
```

Svih **11 entiteta** ima svoju tool klasu u `/Mcp/` (`KontrolnaTockaMcpTools.cs`, `KorisnikMcpTools.cs`, `KnjizicaMcpTools.cs`, `PosjetMcpTools.cs`, `FotografijaMcpTools.cs`, `RutaMcpTools.cs`, `PodrucjeMcpTools.cs`, `PlaninarskiObjektMcpTools.cs`, `PlaninarskaUdrugaMcpTools.cs`, `MedaljaMcpTools.cs`, `KorisnikMedaljaMcpTools.cs`).

### Pattern unutar jedne tool klase
Svaka klasa je obilježena `[McpServerToolType]` i nudi dvije metode po obrascu **search + get by id**, npr. `PosjetMcpTools`:

```csharp
[McpServerToolType]
public class PosjetMcpTools
{
    [McpServerTool, Description("Pretražuje zabilježene posjete...")]
    public async Task<List<PosjetDto>> PretraziPosjete(
        int? idKorisnik = null, int? idKontrolnaTocka = null,
        DateTime? datumOd = null, DateTime? datumDo = null)
    {
        var upit = _db.Posjeti.Where(p => p.DeletedAt == null);
        // ... uvjetni filteri po proslijeđenim parametrima
        return (await upit.OrderByDescending(p => p.DatumVrijemePosjeta).ToListAsync())
            .Select(ToDto).ToList();
    }

    [McpServerTool, Description("Dohvaća jedan posjet po ID-u")]
    public async Task<PosjetDto?> DohvatiPosjet(int id) { ... }
}
```

`[Description(...)]` atributi na parametrima služe kao dokumentacija koju agent vidi prije pozivanja toola — bitno je da su opisni jer agent na temelju njih odlučuje koje parametre poslati.

### Sigurnosne mjere
- **Read-only**: nijedan tool ne radi POST/PUT/DELETE — samo pretraga i dohvat po ID-u.
- **Soft delete se respektira**: svi upiti imaju `.Where(x => x.DeletedAt == null)`, obrisani zapisi se nikad ne vraćaju.
- **DTO sloj, ne entiteti**: tool metode vraćaju DTO objekte (npr. `PosjetDto`), ne EF entitete — osjetljiva polja (`PasswordHash`, `AppUserId`, OIB/JMBG) se ne izlažu.
- MCP server u dev okruženju nema dodatnu autentikaciju (sluša na `localhost`) — namijenjen je razvojnom radnom toku, ne produkcijskom javnom pristupu.

### Konfiguracija na strani klijenta
Repo sadrži `.mcp.json` u rootu koji Claude Code/Cursor automatski pokupi:
```json
{
  "mcpServers": {
    "planinarenje": {
      "type": "http",
      "url": "http://localhost:5041/mcp"
    }
  }
}
```
Uvjet: aplikacija mora biti pokrenuta lokalno (`dotnet run`) da bi agent mogao pozivati toolove.

---

## 2. Global search

### Što je cilj
Jedna tražilica koja pretražuje **svih 7 relevantnih entiteta** odjednom (Područja, Kontrolne točke, Rute, Planinarski objekti, Planinarske udruge, Medalje, Korisnici), grupira rezultate i sortira ih po relevantnosti — plus brzi "typeahead" dropdown u navigaciji.

### Kontroler
`Controllers/PretragaController.cs` — centralna logika u privatnoj metodi `SagradiModel()`. Tri ulazne točke:

| Ruta | Svrha | Limit po grupi |
|---|---|---|
| `GET /pretraga?q=...` | Puna stranica rezultata | 20 |
| `GET /pretraga/live?q=...` | JSON za typeahead dropdown u navbaru | 5 |
| Pojedinačni `AutocompleteSearch` po kontroleru (npr. `PosjetController`) | Specifična pretraga unutar jednog entiteta (za dropdown na formama) | 15 |

### Tok pretrage
1. Provjera minimalne duljine upita (`UpitJeValjan`, min. 2 znaka) — kraći upiti se ignoriraju da se ne preplavi baza upitima na svaki tipkani znak.
2. Za svaki entitet: `.Where(DeletedAt == null)` filter, zatim `.ToList()` u memoriju (jer treba hrvatska normalizacija teksta koju EF/SQL ne znaju prevesti u upit), zatim filtriranje po `HrvatskiTekst.SadrziNormalizirano(...)` na relevantnim poljima (naziv, opis, GUID oznaka, itd.).
3. Vidljivost po entitetu: `VidljivoZaSve(jeOdobreno, idKreator)` — javno odobreni zapisi vidljivi svima, neodobreni samo Adminu ili vlasniku-kreatoru. Korisnici kao entitet se prikazuju **samo Adminu** i bez osjetljivih polja.
4. Svaki pogodak dobiva **skor relevantnosti** preko `IzracunajSkor(naziv, term)`:
   - 100 — točan match (normalizirano)
   - 80 — naziv počinje s upitom
   - 60 — upit se javlja kao cijela riječ unutar naziva
   - 40 — upit je bilo gdje u nazivu (substring)
   - 15 — fallback (stigao kroz drugi filter, npr. opis ili GUID)
5. Grupe rezultata se sortiraju opadajuće po najvišem skoru unutar grupe, a unutar grupe stavke po vlastitom skoru.

### Hrvatska dijakritika
`Helpers/HrvatskiTekst.cs` mapira `č/ć→c, đ→d, š→s, ž→z` i lowercase, tako da upit `"okic"` pogodi i `"Okić"` i `"Okića"` i `"Okiću"` (deklinacije). Ovo je ključno jer korisnici ne moraju znati tipkati dijakritičke znakove ni pogoditi padež.

### Zašto in-memory filtriranje, a ne SQL `LIKE`
Pomelo/MySQL provider ne zna prevesti `HrvatskiTekst.Normaliziraj()` (custom C# metoda) u SQL izraz, pa se entiteti prvo materijaliziraju (`.ToList()`) pa filtriraju u .NET-u. Za trenutni obujam podataka (desetci/stotine zapisa po entitetu) to je prihvatljivo; za puno veći dataset bi trebalo razmotriti generated/computed kolonu s normaliziranim tekstom u bazi.

---

## 3. AI integracija — unos podataka putem AI upita

### Što je cilj
Korisnik upiše rečenicu prirodnim jezikom (npr. *"Bio sam na Okiću 15.6. preko staze sa sjevera, bilo je srednje teško, prekrasan pogled"*) i AI predloži popunjena polja forme za novi `Posjet` — korisnik ih pregleda/ispravi pa potvrdi. AI **nikad** ne sprema podatak direktno, samo predlaže.

### Provider i razlog odabira
Google **Gemini 2.5 Flash** (vidi memoriju `ai-unos-provider.md`) — besplatan tier dovoljan za studentski projekt, dovoljno dobar za strukturirano izvlačenje podataka iz kratkog teksta.

### Arhitektura — tok podataka
```
UI (forma Posjet/Create) → POST /Posjet/AiPrijedlog (upit: string)
    → GeminiAiUnosService.IzvuciPodatkeAsync(upit)
        → HTTP POST prema generativelanguage.googleapis.com (model gemini-2.5-flash)
          s responseSchema koja forsira strukturirani JSON izlaz
        ← JSON: { kontrolnaTockaNaziv, rutaNaziv, datum, dozivljaj, vrijemeUsponaMin, opisIskustva }
    → PosjetController mapira NAZIVE (tekst) na ID-eve u bazi
        koristeći HrvatskiTekst normalizaciju + IzracunajSkor ranking
        (isti pristup kao kod global search-a — ponovna upotreba helpera)
    ← JSON odgovor klijentu s predloženim ID-evima koji se upisuju u <select>/<input> polja forme
```

- `Services/IAiUnosService.cs` — sučelje (apstrakcija, omogućuje da se provider zamijeni bez diranja kontrolera).
- `Services/GeminiAiUnosService.cs` — implementacija specifična za Gemini.
- `Models/Ai/AiPosjetPrijedlog.cs` — DTO rezultata.

### Zašto enum vrijednosti idu dinamički u upit
`GeminiAiUnosService` čita `Enum.GetNames<DozivljajPosjeta>()` i ubacuje ih u JSON schema koju šalje modelu, umjesto da su hardkodirane u promptu. Ako se enum `DozivljajPosjeta` ikad proširi novom vrijednošću, AI servis je automatski usklađen bez dodatne izmjene koda — manje mjesta za desinkronizaciju domene i AI sloja.

### Sigurnosne mjere (server-side forcing) — **najbitniji dio**
Ovo je izravno definirano u `ai-unos-arhitektura.md` memoriji i potvrđeno u kodu:
- **GUID oznaka žiga AI nikad ne popunjava** — korisnik mora ručno upisati GUID sa stvarnog žiga na terenu. To je dokaz fizičkog posjeta; ako bi AI mogao izmisliti/popuniti GUID, cijeli koncept "dokaza obilaska" bi propao.
- **Korisnik/Knjižica vlasništvo se nikad ne uzima iz AI inputa** — server u `Create` POST akciji forsira `IdKorisnik`/`IdKnjizica` iz autentificiranog `GetCurrentKorisnikAsync()`, nikad iz tijela zahtjeva. Ovo sprječava da netko preko AI prompta (prompt injection ili izmijenjen request) upiše posjet u tuđe ime.
- Predložena kontrolna točka/ruta moraju imati `JeOdobreno == true` da bi AI mogao predložiti match — neodobreni/korisnički predloženi zapisi se ne nude kao prijedlog.

### Graceful degradation
Ako `Gemini:ApiKey` nije postavljen (user secrets/env varijabla), `GeminiAiUnosService.JeDostupan` vraća `false`, UI sakriva AI gumb, a ručni unos forme radi posve normalno. Aplikacija nikad ne ovisi o tome da je AI servis dostupan.

---

## 4. Kreiranje testova za sve endpointe

### Što je cilj
Integracijski testovi koji pokrivaju i **API kontrolere** (Lab5, `Controllers/Api/`) i **MVC kontrolere** (CRUD ekrani) za svih 11 entiteta, s pravom autentikacijom/autorizacijom (Admin vs Planinar vs Anonimno) umjesto mockiranja svega.

### Projekt i tehnologije
`planinarenje.IntegrationTests/` — zaseban xUnit projekt, `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`) + EF Core **InMemory** provider (ne mockira se cijeli DbContext, koristi se prava EF Core logika nad in-memory bazom).

### Ključna infrastruktura

| Fajl | Uloga |
|---|---|
| `CustomWebAppFactory.cs` | Diže aplikaciju u memoriji; u `ConfigureWebHost` zamjenjuje MySQL s `UseInMemoryDatabase(Guid.NewGuid())` (svaki test fixture = izolirana baza) i zamjenjuje Identity auth shemu s `TestAuthHandler` |
| `Helpers/TestAuthHandler.cs` | Lažna auth shema — čita `X-Test-UserId` i `X-Test-Roles` HTTP zaglavlja i iz njih gradi `ClaimsPrincipal`. Bez `X-Test-UserId` zaglavlja → `AuthenticateResult.Fail` (simulira anonimnog korisnika) |
| `Helpers/AuthHelper.cs` | Factory metode `CreateAnonymousClient`, `CreatePlaninarClient`, `CreateAdminClient` — vraćaju `HttpClient` s odgovarajućim test-zaglavljima predpostavljenim |
| `Helpers/TestData.cs` | Konstante (ID-evi, AppUserId vrijednosti) korištene konzistentno kroz sve testove |
| `Helpers/TestDataSeeder.cs` | Seedira in-memory bazu s konzistentnim testnim podacima prije svakog testa |
| `Helpers/AntiForgeryHelper.cs` | Pomoć za CSRF token u MVC POST testovima (`[ValidateAntiForgeryToken]` se ne zaobilazi, nego se token stvarno dohvaća i šalje) |

### Pattern testova — "prvo jedan, pa replikacija"
U skladu s pravilom iz `CLAUDE.md` ("Lab5 dokument upozorava na AI slop ako se odmah krene paralelno na sve"), prvo je do kraja napravljen `PosjetApiTests.cs` (222 linije) kao template, pa repliciran isti obrazac na ostalih 10 entiteta.

Standardni scenariji po API kontroleru (5+ testova):
1. `GetAll_Returns200_AndNonEmptyList`
2. `GetById_Returns200_WhenExists`
3. `GetById_Returns404_WhenMissing`
4. `Post_Returns201_AndCreatesEntity`
5. `Post_Returns400_WhenModelInvalid`
6. Autorizacijski testovi specifični za entitet, npr. za `Posjet`: `Post_Returns401_WhenAnonymous`, `Post_Returns403_WhenNotOwner` (vlasništvo se provjerava ručno, ne kroz role — u skladu s autorizacijskom pravilom iz `CLAUDE.md` sekcija 6).

Isti pattern (`IClassFixture<CustomWebAppFactory>` + `IAsyncLifetime.InitializeAsync` koji re-seeda bazu) ponavlja se za svih **11 `*ApiTests.cs`** klasa (Posjet, KontrolnaTocka, Knjizica, Korisnik, Fotografija, Ruta, Podrucje, Medalja, KorisnikMedalja, PlaninarskaUdruga, PlaninarskiObjekt).

Paralelno postoji **`*ControllerTests.cs`** set (12 klasa) koji testira MVC ekrane (Index, Create GET/POST, Edit GET/POST, Delete GET/POST, Details, autorizacijske provjere) na isti način — kroz prave HTTP zahtjeve, ne unit-testirajući akcije izolirano.

### Zašto InMemory provider, a ne stvarni MySQL u testovima
- Testovi se mogu pokretati offline, bez Azure/lokalnog MySQL servera, što ubrzava CI i lokalni razvoj.
- Svaki test fixture dobiva svoju izoliranu bazu (`Guid.NewGuid()` naziv) — testovi se mogu pokretati paralelno bez međusobnog zagađivanja podataka.
- Napomena/ograničenje: InMemory provider ne provjerava relacijske constraint-e (FK, unique index) kao prava SQL baza — testovi provjeravaju **logiku aplikacije**, ne integritet sheme baze (to se provjerava kroz migracije i ručno testiranje na pravoj bazi).

### Pokretanje
```bash
dotnet test
```
Stanje na zadnju provjeru: svi testovi prolaze (zeleno) — ukupan broj raste kako se dodaju entiteti/scenariji, trenutno pokriva sve API i MVC kontrolere za svih 11 entiteta.

---

## 5. Implementacija logging mehanizma

### Tehnologija
**Serilog** — strukturirani logger, konfiguriran u `Program.cs` na startu aplikacije (prije `WebApplication.CreateBuilder`), s dva sink-a: Console (dev) i File.

### Konfiguracija
`Program.cs` gradi `Log.Logger` iz `appsettings.json`/`appsettings.{Environment}.json` preko `ReadFrom.Configuration(...)`, te dodaje `WriteTo.File(...)` s putanjom `Logs/log-{dd-MM-yyyy_HH-mm-ss}.txt` — **jedan log fajl po pokretanju aplikacije** (ne po danu), s formatom:
```
{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}
```

`appsettings.json` sekcija `Serilog`:
- `MinimumLevel.Default = Information`, s override na `Warning` za `Microsoft.AspNetCore` i `Microsoft.EntityFrameworkCore` šum (da se ne zatrpa log frameworkovim internim porukama).
- `Enrich: [FromLogContext, WithMachineName]`.

### Automatski request logging
`app.UseSerilogRequestLogging()` middleware loguje **svaki** HTTP zahtjev (metoda, putanja, status kod, vrijeme odziva) bez ručnog pisanja koda po kontroleru.

### Eksplicitni logovi u kodu
Kontroleri injektiraju `ILogger<T>` i bilježe domenski bitne događaje, npr. u `PosjetApiController`:
```csharp
_logger.LogWarning("POST /api/posjet - neispravan DTO od korisnika {AppUserId}.", AppUserId);
_logger.LogInformation("POST /api/posjet - posjet {IdPosjet} kreiran za korisnika {IdKorisnik}.",
    entity.IdPosjet, entity.IdKorisnik);
```
Isti obrazac u `PosjetController` (npr. za AI prijedlog: `LogInformation("AI prijedlog posjeta (korisnik {AppUserId}): KT={IdKontrolnaTocka}, ...")`) — bitno je da se loguju **identifikatori** (ID-evi), ne osjetljivi sadržaj korisničkog unosa.

### Redakcija osjetljivih podataka (`agent_log.txt`)
Odvojeno od Serilog mehanizma, postoji hook koji logira AI agent promptove u `lab-1/agent_log.txt` (Lab1 deliverable, vidi `CLAUDE.md` sekciju 8 — taj file se ne dira). Commit `f52e907` ("Redact secret patterns before logging agent input") je uveo redakciju da se ključevi/lozinke koje korisnik eventualno zalijepi u prompt ne snime u plain textu u taj log file prije commita. Vidi memoriju `feedback-ne-paste-secrets-u-chat.md`.

### Razina/format loga — primjeri
```
2026-06-21 17:36:51.765 +02:00 [INF] Now listening on: https://localhost:7187
2026-06-21 17:36:55.021 +02:00 [INF] HTTP GET / responded 200 in 554.5787 ms
2026-06-21 17:38:01.128 +02:00 [ERR] HTTP GET /Pretraga/Live responded 500 in 4.1603 ms
```

---

## 6. Deploy na cloud provider (Azure)

### Topologija (vidi i memoriju `deploy-azure-planinarenje.md`)
- **Azure App Service**: `planinarenje-app`, resource group `rg-planinarenje`, regija Austria East.
- **Produkcijski URL**: `https://planinarenje-app-h5gbahh5b3afasfq.austriaeast-01.azurewebsites.net`
- **Baza**: MySQL (Pomelo provider), connection string odvojen po environmentu (`appsettings.Development.json` lokalno; produkcijski connection string se ne commita).
- Deploy je **ručan** preko Azure CLI (`az webapp deploy`), nije postavljen GitHub Actions CI/CD pipeline.

### Koraci deploya (dokumentirano u `README.md`)
1. **Preduvjet**: prijava u Azure CLI (`az login`).
2. **Publish samo glavnog projekta** — eksplicitno:
   ```bash
   dotnet publish planinarenje.csproj -c Release -o ./publish-output
   ```
   Mora se navesti `planinarenje.csproj`, **ne** `planinarenje.slnx` — jer bi publish cijelog solution fajla pokupio i `planinarenje.IntegrationTests` projekt u output, što nije poželjno za produkciju.
3. **Pakiranje u zip s normaliziranim separatorima putanje**:
   ```powershell
   $sourceDir = (Resolve-Path "./publish-output").Path
   $zipPath = Join-Path (Get-Location).Path "deploy.zip"
   # ... System.IO.Compression.ZipArchive, putanje s Replace('\','/')
   ```
   **Zašto ne `Compress-Archive`**: PowerShell-ov `Compress-Archive` upisuje Windows backslash (`\`) separatore putanja unutar zip entry-ja, a Linux App Service (Kudu/zip deploy) ne raspakira takve unose ispravno — fajlovi završe na pogrešnim putanjama ili se ne raspakiraju. Rješenje je ručno graditi zip preko `System.IO.Compression.ZipArchive` i normalizirati sve separatore na `/`.
4. **Deploy na Azure**:
   ```bash
   az webapp deploy --resource-group rg-planinarenje --name planinarenje-app --src-path ./deploy.zip --type zip
   ```
5. **EF migracije** (samo ako su dodane nove od zadnjeg deploya) — pokreću se ručno protiv produkcijske baze, **nema automatskog `Database.Migrate()` na startupu** aplikacije (svjesna odluka — sprječava da app pri svakom restartu/scale-outu pokuša mijenjati shemu baze bez nadzora):
   ```bash
   dotnet ef database update --connection "<produkcijski MySQL connection string>"
   ```

### Specifičnost projekta — `Slike/` folder
`planinarenje.csproj` ima eksplicitan `<Content Include="Slike\**" CopyToPublishDirectory="PreserveNewest" />` jer je `Slike/` folder izvan `wwwroot/` (statičke slike entiteta, vidi strukturu repozitorija u `CLAUDE.md`), pa se default ASP.NET Core publish pravilima ne bi uključio — morao se commit "Uvrsti Slike/ folder u publish output" da slike preživu deploy (povezano s ranijim bugom "naslovnica slike nisu vidljive", vidi git log `7812974`).

### Sigurnosne mjere
- Connection string i Google OAuth ključevi se ne commitaju — čitaju se iz Azure App Service Application Settings (environment varijable) u produkciji, iz user secrets lokalno.
- `appsettings.Development.json` (lokalni connection string) je u `.gitignore`/ne dira se eksplicitno (vidi `CLAUDE.md` sekcija 8).

---

## Sažetak — kako se sve povezuje

| Cjelina | Ulazna točka | Tehnologija | Status |
|---|---|---|---|
| MCP | `GET/POST /mcp` | `ModelContextProtocol.AspNetCore`, 11 tool klasa u `/Mcp/` | ✅ |
| Global search | `GET /pretraga`, `/pretraga/live` | LINQ in-memory + `HrvatskiTekst` normalizacija + scoring | ✅ |
| AI integracija | `POST /Posjet/AiPrijedlog` | Gemini 2.5 Flash, `IAiUnosService` apstrakcija | ✅ |
| Testovi | `dotnet test` | xUnit + `WebApplicationFactory` + EF InMemory, 11 API + 12 MVC test klasa | ✅ |
| Logging | Middleware + `_logger.LogXxx()` | Serilog (Console + File sink), `Logs/log-{timestamp}.txt` | ✅ |
| Deploy | `az webapp deploy` (ručno) | Azure App Service `planinarenje-app`, MySQL | ✅ |

Sve cjeline su ugrađene u **istu** ASP.NET Core (.NET 9) aplikaciju — nema mikroservisne arhitekture niti vanjskih backend servisa osim Azure infrastrukture i (opcionalnog) Gemini API-ja. AI i MCP slojevi su svjesno dizajnirani kao **dodatak** postojećim CRUD ekranima iz Lab1–5, ne kao zamjena — ručni unos i postojeći tokovi rade identično i kad su AI/MCP nedostupni.
