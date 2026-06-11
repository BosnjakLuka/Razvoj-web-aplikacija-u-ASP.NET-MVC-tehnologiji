# Lab 5 — Priprema za obranu

> Vodič za usmenu obranu laboratorijske vježbe 5. Objašnjava **što smo dodali u svakoj fazi, što to radi i čemu služi**, povezano sa stvarnim datotekama u projektu. Sve tvrdnje su provjerene u kodu.
>
> Tema vježbe: **Web API + DTO, autentikacija/autorizacija, upload datoteka, Google login, integracijski testovi** (`LabosDokumenti/lab5/Lab5.md`).

---

## 0. Što Lab 5 traži (bodovanje)

| Kriterij | Bodovi | Gdje je kod nas |
|---|---|---|
| Kompletna API podrška za sve entitete (CRUD, DTO) | 2 | `Controllers/Api/`, `Models/Dto/` |
| Autentikacija (lokalni računi) i autorizacija | 1 | Identity u `Program.cs`, `AppUser`, role |
| Upload datoteka (Dropzone) | 1 | `PosjetController.UploadFoto` |
| 3rd-party autentikacija (Google) | 1 | Google OAuth u `Program.cs` |
| Integracijski testovi za API (svi, CRUD) | 2 | projekt `planinarenje.IntegrationTests/` |

Mapiranje faza iz `PlanImplementacije-Lab5.md` → ovaj projekt:

- **Faza 1** — Identity (lokalna registracija/prijava), proširen `AppUser`
- **Faza 2** — Autorizacija (role `Admin`/`Planinar`, vlasništvo, soft delete)
- **Faza 3** — Web API + DTO + Swagger
- **Faza 4** — Dropzone upload fotografija
- **Faza 5** — Google OAuth
- **Faza 6** — Integracijski testovi (55 testova)
- **Faza 7** — finalna verifikacija + ship na `main`

---

## 1. Autentikacija vs. autorizacija (temeljni pojmovi)

Profesor će gotovo sigurno pitati razliku. Nauči je napamet:

- **Autentikacija** = *„Tko si ti?"* — provjera identiteta (prijava emailom i lozinkom, Google login).
- **Autorizacija** = *„Smiješ li ovo napraviti?"* — provjera prava (samo Admin smije brisati, samo vlasnik smije mijenjati svoj posjet).

Primjer iz naše aplikacije: prijava na `luka@planinarenje.hr` je autentikacija; to što taj korisnik **ne smije** obrisati tuđi posjet je autorizacija.

---

## 2. Faza 1 — Identity: lokalna registracija i prijava

### Što je dodano
- **ASP.NET Core Identity** — gotov, provjeren sustav za registraciju, prijavu, odjavu, hashiranje lozinki, role i vanjske providere. Ne pišemo vlastiti login od nule.
- **`AppUser`** (`Entiteti/AppUser.cs`) — naša klasa koja nasljeđuje `IdentityUser` i **proširuje** ga s dva obavezna polja:
  - `OIB` (točno 11 znamenki, samo brojevi)
  - `JMBG` (točno 13 znamenki, samo brojevi)
- **`PlaninarstvoDbContext`** nasljeđuje `IdentityDbContext<AppUser>`, pa Identity tablice (`AspNetUsers`, `AspNetRoles`, …) žive u istoj bazi.

### Konfiguracija (`Program.cs`)
```csharp
builder.Services
    .AddDefaultIdentity<AppUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false; // ne tražimo potvrdu emaila
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 10;           // jaka lozinka
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<PlaninarstvoDbContext>();
```

U pipeline-u (redoslijed je bitan!):
```csharp
app.UseAuthentication();  // prvo: tko si
app.UseAuthorization();   // pa: smiješ li
```

### Ključna razlika u našem modelu (mogući trik-pitanje)
Imamo **dva pojma korisnika**, namjerno odvojena:
- **`AppUser`** — Identity podaci (email, lozinka, OIB, JMBG) → tablica `AspNetUsers`.
- **`Korisnik`** — planinarski profil (ime, prezime, knjižica, posjeti) → naša domenska tablica.

Veza je preko `Korisnik.AppUserId` (string GUID). Tako su sigurnosni podaci (lozinka) odvojeni od domenskih (planinarska aktivnost).

### Email sender
Identity očekuje `IEmailSender`. Mi nemamo pravi SMTP, pa smo registrirali `NoOpEmailSender` (`builder.Services.AddSingleton<IEmailSender, NoOpEmailSender>()`) koji ništa ne šalje. To je legitiman obrazac — vanjska integracija (email) iza interfacea, koja se u testu/devu zamijeni lažnom implementacijom.

### Pitanja za obranu
- *„Zašto niste pisali vlastiti login?"* → Identity već rješava hashiranje + salt, lockout, reset lozinke, 2FA. Vlastiti sustav je sigurnosni rizik.
- *„Gdje je proširen AppUser?"* → `Entiteti/AppUser.cs`, polja `OIB` i `JMBG` s validacijom.

---

## 3. Faza 2 — Autorizacija: role i vlasništvo

### Role
Aplikacija ima **dvije role**: `Admin` i `Planinar` (zahtjev kolegija je „Admin + barem još jedna rola"). Role i testni korisnici se **seedaju** pri pokretanju u `Data/IdentitySeed.cs`:

| Korisnik | Email | Lozinka | Rola |
|---|---|---|---|
| Admin | `admin@planinarenje.hr` | `Admin@2026!` | `Admin` |
| Planinar | `luka@planinarenje.hr` | `Planinar@26!` | `Planinar` |

Seed je idempotentan (kreira samo ako ne postoji) i svakom korisniku stvara povezani `Korisnik` profil.

### Tri razine zaštite akcija
1. **`[AllowAnonymous]`** — javno čitanje (pregled kontrolnih točaka, ruta, područja…). Bilo tko.
2. **`[Authorize(Roles = "Admin")]`** ili `"Admin,Planinar"` — kreiranje/izmjena/brisanje traži prijavu i točnu rolu.
3. **Provjera vlasništva u kodu** — za `Posjet` i `Fotografija` nije dovoljna rola; provjerava se da zapis pripada baš prijavljenom korisniku.

### Vlasništvo (`ApiBaseController`)
Zajednička logika je u baznom razredu `Controllers/Api/ApiBaseController.cs`:
```csharp
protected bool IsAdmin => User.IsInRole("Admin");

protected async Task<Korisnik?> GetCurrentKorisnikAsync()
    => await Db.Korisnici.FirstOrDefaultAsync(k => k.AppUserId == AppUserId);

protected async Task<bool> IsOwnerOrAdminAsync(int idKorisnik)
{
    if (IsAdmin) return true;
    var k = await GetCurrentKorisnikAsync();
    return k != null && k.IdKorisnik == idKorisnik;
}
```
Primjer u akciji (brisanje posjeta): ako nisi Admin i nisi vlasnik → `Forbid()` (403).

### Soft delete
Brisanje **ne uklanja red iz baze** — postavlja `DeletedAt = DateTime.UtcNow`. Global query filter osigurava da soft-deletani zapisi nikad ne izlaze u listama. Tako čuvamo povijest i izbjegavamo rušenje relacija.

### Pitanja za obranu
- *„Koje role imate i tko ih dodjeljuje?"* → `Admin` i `Planinar`, seedaju se u `IdentitySeed.cs`.
- *„Kako spriječavate da korisnik mijenja tuđi posjet?"* → `IsOwnerOrAdminAsync` u `ApiBaseController`, vraća 403 ako nije vlasnik ni Admin.
- *„Što je soft delete?"* → logičko brisanje preko `DeletedAt`, zapis ostaje u bazi ali se filtrira van.

---

## 4. Faza 3 — Web API, DTO i Swagger (2 boda, najveći dio)

### 4.1 Što je Web API i čemu služi
**Web API** je sloj kojim aplikacija izlaže **podatke i operacije** drugim klijentima u strojno-čitljivom obliku (**JSON**), umjesto HTML stranica. Dok klasični MVC controller vraća cijeli `View` (HTML), API controller vraća podatke.

Tko zove API:
- JavaScript u pregledniku (AJAX)
- mobilne/desktop aplikacije
- drugi serveri (server-to-server)
- **naši integracijski testovi** (Faza 6)

API koristi HTTP metode po značenju:
- `GET` — dohvat, `POST` — kreiranje, `PUT` — izmjena, `DELETE` — brisanje

I vraća **HTTP status kod** koji govori ishod:
- `200 OK`, `201 Created`, `204 No Content`
- `400 Bad Request` (nevaljan model), `401 Unauthorized` (nisi prijavljen), `403 Forbidden` (nemaš pravo), `404 Not Found`

### 4.2 Što su API controlleri i čemu služe
API controller prima HTTP zahtjev na nekoj ruti, izvrši operaciju nad bazom i vrati podatke + status kod. Kod nas:

- **11 API kontrolera** u `Controllers/Api/`, jedan po entitetu: `PosjetApiController`, `KontrolnaTockaApiController`, `RutaApiController`, `PodrucjeApiController`, `PlaninarskiObjektApiController`, `PlaninarskaUdrugaApiController`, `MedaljaApiController`, `KorisnikApiController`, `KnjizicaApiController`, `KorisnikMedaljaApiController`, `FotografijaApiController`.
- Svi nasljeđuju **`ApiBaseController`** koji nosi `[ApiController]` atribut i zajedničke helpere (vlasništvo, trenutni korisnik).
- Ruta se definira atributom, npr. `[Route("api/posjet")]`. Rute su sve oblika `api/<entitet>` (malim slovima).

**Čemu služi `[ApiController]`?** (često pitanje) — donosi API ponašanja:
- traži attribute routing,
- automatski vraća `400 Bad Request` kad je model nevaljan,
- bolji model binding za JSON,
- API-friendly odgovori za greške.

**Zašto `ControllerBase`, a ne `Controller`?** — API-ju ne trebaju View funkcionalnosti, pa koristimo lakši `ControllerBase`.

Referentni kontroler je **`PosjetApiController`** (glavni entitet). Ima svih 5 CRUD metoda + **query parametre** za filtriranje:
```
GET /api/posjet?korisnikId=1&kontrolnaTockaId=3&datumOd=...&datumDo=...&dozivljaj=Srednje
```
Bitan sigurnosni detalj: kod `POST` se **vlasnik postavlja iz prijavljenog korisnika, nikad iz DTO-a**, a `JeLiPotvrdenPosjet` se izračuna usporedbom unesenog GUID-a sa `GUIDOznaka` kontrolne točke (digitalni žig).

### 4.3 Što je DTO i čemu služi
**DTO (Data Transfer Object)** je klasa koja definira **točan oblik podataka koji API prima/vraća** — odvojena od EF entiteta.

Zašto ne vraćamo entitet direktno:
- entitet ima **interna/osjetljiva polja** koja ne smiju van (npr. `OIB`, `JMBG`, `AppUserId`, `PasswordHash`),
- navigacijska svojstva izazivaju **prevelik ili ciklički JSON**,
- API model je **stabilniji** od sheme baze (možemo mijenjati bazu bez lomljenja klijenata),
- precizno kontroliramo **što klijent vidi**.

Kod nas, po entitetu, postoje tri vrste DTO-a u `Models/Dto/`:
- **`XyzDto`** — *read* (što API vraća),
- **`XyzCreateDto`** — ulaz za `POST`,
- **`XyzUpdateDto`** — ulaz za `PUT`.

Primjer (`PosjetDto`) ima i **ugniježđeni** DTO za povezane podatke (`List<FotografijaSummaryDto>`) — tako fotografije dolaze unutar posjeta, ali samo kao sažetak, bez cijelog entiteta. Mapiranje entitet→DTO radi privatna metoda `ToDto(...)` u kontroleru (jednostavno ručno mapiranje, bez AutoMappera — dovoljno za vježbu).

> **Pravilo koje smo poštivali:** DTO **nikad** ne izlaže `OIB`, `JMBG`, `AppUserId` ni `PasswordHash`.

### 4.4 Što je Swagger i zašto smo ga implementirali
**Swagger / OpenAPI** je alat koji **automatski generira interaktivnu dokumentaciju** svih API endpointa. Otvoriš stranicu u pregledniku, vidiš popis svih ruta, koje parametre primaju, koji JSON vraćaju, i možeš ih **isprobati uživo** (klikneš „Try it out", pošalješ zahtjev, vidiš odgovor).

Čemu služi kod nas:
- **dokaz na obrani** — možeš profesoru pokazati sve API rute na jednom mjestu i pozvati ih bez Postmana,
- razvojni alat za ručno testiranje API-ja.

Konfiguracija (`Program.cs`):
```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // dokumentiraj SAMO Web API rute (api/...), ne MVC stranice (Home/Index itd.)
    options.DocInclusionPredicate((_, apiDesc) =>
        apiDesc.RelativePath is not null && apiDesc.RelativePath.StartsWith("api/"));
});
```
Swagger UI je namjerno **samo u Development okruženju** (ne izlažemo dokumentaciju u produkciji):
```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(...); // dostupno na /swagger
}
```
**Demo:** pokreni app i otvori `https://localhost:<port>/swagger`.

> Napomena o izgovoru: alat se zove **Swagger** (ne „swapper").

### 4.5 Pitanja za obranu (Faza 3)
- *„Razlika MVC i API kontrolera?"* → MVC vraća View (HTML), API vraća podatke (JSON) + status kod.
- *„Zašto DTO, a ne entitet?"* → skrivanje osjetljivih polja, izbjegavanje cikličkog JSON-a, stabilnost API-ja.
- *„Što radi `[ApiController]`?"* → attribute routing, auto 400 na nevaljan model, bolji binding.
- *„Što je 201 Created?"* → status nakon uspješnog `POST`; vraćamo ga preko `CreatedAtAction` s lokacijom novog resursa.
- *„Što ako GET filter ne nađe ništa?"* → `200` s praznim `[]` (nije greška); `404` je samo za `GET /{id}` kad zapis ne postoji.

---

## 5. Faza 4 — Upload datoteka (Dropzone)

### Što je dodano i čemu služi
**Dropzone** je JavaScript komponenta za **asinkroni upload** datoteka (povuci-i-pusti). U našoj aplikaciji planinar na **Edit stranici posjeta** uploada fotografije kao dokaz obilaska. Upload je vezan uz **konkretni `Posjet`** (kod profesora „kviz" = kod nas `Posjet`).

### Tok (server strana — `PosjetController`)
1. **`UploadFoto(int idPosjet, IFormFile file, TipSlike tip)`** — `[HttpPost]`, `[ValidateAntiForgeryToken]`, `[Authorize]`:
   - provjeri da posjet postoji i da je korisnik **vlasnik ili Admin** (inače 403),
   - **validira datoteku**: nije prazna, ≤ 5 MB, ekstenzija JPG/PNG/WEBP, `ContentType` počinje s `image/`,
   - **sprema na disk** u `wwwroot/uploads/posjeti/{idPosjet}/` pod **GUID imenom** (sprječava koliziju/preplitanje imena),
   - **u bazu** sprema metapodatke + putanju (`Fotografija`: `NazivDatoteke`, `PutanjaDatoteke`, `TipSlike`, `ContentType`, `FileSize`, `DatumUploada`),
   - vrati `Json(new { success = true, id = ... })`.
2. **`GetFotografije(int idPosjet)`** — AJAX `GET`, vraća **partial view** s popisom fotografija (osvježava se nakon svakog uploada).
3. **`DeleteFoto(int id)`** — `[HttpPost]`, `[ValidateAntiForgeryToken]`, `[Authorize]`: provjeri vlasništvo, **obriše datoteku s diska** i napravi **soft delete** u bazi.

### Gdje se datoteke spremaju i zašto na disk
Za vježbu: lokalni disk (`wwwroot/uploads/posjeti/{id}/`). U bazu ide **samo putanja + metapodaci**, ne sam sadržaj. (Za produkciju bi se koristio Azure Blob / S3 zbog skaliranja na više instanci — dobro je to spomenuti.)

### Pitanja za obranu
- *„Što ide na disk, a što u bazu?"* → datoteka na disk, putanja + metapodaci u bazu.
- *„Zašto upload tek na Edit, ne Create?"* → na Create posjet još nema ID, pa se datoteka nema na što vezati.
- *„Antiforgery?"* → `[ValidateAntiForgeryToken]` štiti POST od CSRF; token se šalje uz Dropzone zahtjev.
- *„Kako se osvježava popis?"* → AJAX poziva `GetFotografije` koji vraća partial, bez reloada cijele stranice.

---

## 6. Faza 5 — Google OAuth (3rd-party login)

### Što je OAuth i čemu služi
**OAuth** omogućuje prijavu preko vanjskog servisa (Google) **bez da naša aplikacija ikad vidi korisnikovu Google lozinku**. Korisnik klikne „Prijava Googleom", potvrdi identitet kod Googlea, a Google nas obavijesti tko je. Mi onda kreiramo lokalnu prijavu.

### Pojednostavljeni tok
1. Korisnik klikne „Login with Google".
2. Aplikacija ga preusmjeri na Google.
3. Korisnik se prijavi **kod Googlea** (ne kod nas).
4. Google vrati korisnika na naš callback s `authorization code`.
5. Aplikacija server-to-server provjeri taj code kod Googlea.
6. Ako je valjan → kreira se lokalna prijava (cookie).

`ClientId` identificira našu aplikaciju kod Googlea; `ClientSecret` je tajna kojom dokazujemo da smijemo provjeriti code.

### Konfiguracija (`Program.cs`)
```csharp
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
if (!string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret))
{
    builder.Services.AddAuthentication()
        .AddGoogle(options =>
        {
            options.ClientId = googleClientId;
            options.ClientSecret = googleClientSecret;
        });
}
```
Dva ključna detalja za obranu:
- **Tajne se NE drže u kodu.** `ClientId`/`ClientSecret` se čitaju iz **user secrets** (development), nikad se ne commitaju u repozitorij.
  ```bash
  dotnet user-secrets set "Authentication:Google:ClientId" "..."
  dotnet user-secrets set "Authentication:Google:ClientSecret" "..."
  ```
- **Graceful fallback** — Google provider se registrira **samo ako su oba ključa postavljena**. Ako nisu (npr. na tuđem računalu), aplikacija normalno radi s lokalnom prijavom i ne ruši se na startupu.

OAuth traži **HTTPS** (zato `app.UseHttpsRedirection()` i HTTPS profil u `launchSettings.json`).

### Pitanja za obranu
- *„Vidi li vaša aplikacija Google lozinku?"* → Ne, nikad. Korisnik se autentificira kod Googlea; mi dobijemo samo potvrdu identiteta.
- *„Gdje su ClientId/Secret?"* → u user secrets, ne u kodu i ne u gitu.
- *„Što ako ključevi nisu postavljeni?"* → Google login se ne registrira, lokalna prijava i dalje radi.

---

## 7. Faza 6 — Integracijski testovi (2 boda)

### 7.1 Što je `planinarenje.IntegrationTests/` i čemu služi
To je **zaseban test projekt** (xUnit) koji **automatski provjerava da svih 11 Web API kontrolera radi kroz stvarni HTTP sloj**: ruta → model binding → validacija → autorizacija → baza → JSON odgovor. Pokreće se s `dotnet test`.

Cilj nije „nabrijati" testove koji samo prolaze, nego **regresijska zaštita**: ako kasnije nešto slomimo, testovi to odmah pokažu.

### 7.2 Što se nalazi u projektu
```
planinarenje.IntegrationTests/
├── CustomWebAppFactory.cs       ← podiže aplikaciju u testnom načinu
├── GlobalUsings.cs              ← zajednički using-i (Xunit, FluentAssertions…)
├── Helpers/
│   ├── TestAuthHandler.cs       ← lažna autentikacija (preko HTTP zaglavlja)
│   ├── AuthHelper.cs            ← tvori Admin/Planinar/anonimnog HTTP klijenta
│   ├── TestData.cs             ← konstante ID-eva za testove
│   └── TestDataSeeder.cs       ← povezuje seed Korisnike s testnim AppUserId
├── PosjetApiTests.cs            ← referentna klasa (5 testova)
└── …još 10 *ApiTests.cs        ← po jedna klasa za svaki API kontroler
```

### 7.3 Kako radi (ključne tehnologije)
- **`WebApplicationFactory<Program>`** — pokreće cijelu pravu aplikaciju u memoriji i daje `HttpClient` kojim zovemo endpointe kao pravi klijent. (Zato smo na dno `Program.cs` dodali `public partial class Program { }` — da factory „vidi" tip.)
- **EF Core InMemory** — umjesto MySQL koristi se baza u memoriji. Testovi **ne trebaju MySQL ni internet**, svaki factory dobije svoju izoliranu bazu (`Guid.NewGuid()`).
- **`CustomWebAppFactory`** — u testnom hostu **zamijeni** dvije stvari:
  1. MySQL `DbContext` → InMemory provider,
  2. pravu Identity autentikaciju → `TestAuthHandler`.
- **`TestAuthHandler`** — lažna auth shema koja čita zaglavlja `X-Test-UserId` i `X-Test-Roles` iz zahtjeva i od njih napravi prijavljenog korisnika. Tako u testu **glumimo** Admina ili Planinara bez pravog logina/Googlea. `AuthHelper` priprema tri klijenta: Admin, Planinar, anoniman.
- **FluentAssertions** — čitljive tvrdnje: `response.StatusCode.Should().Be(HttpStatusCode.OK);`
- **`IClassFixture<CustomWebAppFactory>` + `IAsyncLifetime`** — dijeljeni host po test klasi + `InitializeAsync` seeda bazu prije svakog testa.

### 7.4 Što se testira (5 testova × 11 kontrolera = 55)
Za svaki API kontroler:
1. `GET all` → `200` + neprazna lista
2. `GET /{id}` → `200` kad zapis postoji
3. `GET /{id}` → `404` kad ne postoji
4. `POST` → `201` + zapis stvarno u bazi
5. `POST` → `400` za nevaljan model

Pokrivamo **uspješne scenarije, nepostojeće ID-eve i validacijske greške** — točno kako Lab5 traži.

### 7.5 Zašto InMemory, a ne prava baza (mogući prigovor)
InMemory je brz i izoliran, ne ovisi o lokalnom MySQL-u ni stanju razvojne baze. **Mana:** ne provjerava sve relacijske constraint-ove kao prava SQL baza — to treba razumjeti i spomenuti. Za fokus na *API ponašanje* je dovoljan.

### 7.6 Kako pokrenuti (demo na obrani)
```bash
dotnet test
```
Rezultat: `Passed! - Failed: 0, Passed: 55, Skipped: 0, Total: 55`.
Za ispis imena svih testova: `dotnet test --logger "console;verbosity=detailed"`.

> Savjet: pokreni jednom prije obrane da se zbuilda, pa drugo pokretanje ide gotovo instant (sami testovi <1 s).

### 7.7 Pitanja za obranu
- *„Što je integracijski test (vs. unit test)?"* → integracijski provjerava više slojeva zajedno kroz pravi HTTP poziv; unit test izolira jednu metodu.
- *„Kako testirate zaštićene endpointe bez pravog logina?"* → `TestAuthHandler` čita `X-Test-UserId`/`X-Test-Roles` i glumi prijavljenog korisnika.
- *„Zašto ne dirate pravu bazu?"* → InMemory provider, izolirana baza po testu, nema ovisnosti o MySQL-u.
- *„Što ako test treba Admina?"* → `AuthHelper.CreateAdminClient(...)` postavi zaglavlja s rolom `Admin`.

---

## 8. Faza 7 — Finalna verifikacija (checklista za predaju)

Prije mergea na `main` proletjeti:

- [x] 11 API kontrolera, 5 CRUD metoda + query parametri
- [x] DTO ne izlaže `OIB`/`JMBG`/`AppUserId`/`PasswordHash`
- [x] Registracija/prijava/odjava rade lokalno
- [x] Google login radi end-to-end
- [x] Role `Admin` + `Planinar`, seed kreira oba korisnika
- [x] Dropzone upload (disk + DB), AJAX osvježavanje, brisanje
- [x] Antiforgery na Dropzone POST-u
- [x] Soft-delete global filter aktivan
- [x] Min. 55 testova, svi zeleni
- [ ] `dotnet build` bez warninga, Lab1–4 ekrani i dalje rade *(provjeriti prije predaje)*

---

## 9. Brza šalabahter-tablica pojmova

| Pojam | Jedna rečenica |
|---|---|
| **Autentikacija** | Tko si ti (prijava). |
| **Autorizacija** | Smiješ li ovo (prava/role). |
| **Web API** | Sloj koji izlaže podatke kao JSON drugim klijentima. |
| **API controller** | Klasa koja obrađuje `api/...` rute i vraća podatke + status kod. |
| **`[ApiController]`** | Daje attribute routing, auto-400, bolji binding. |
| **DTO** | Oblik podataka za API; skriva osjetljiva polja entiteta. |
| **Swagger** | Auto-generirana interaktivna dokumentacija API-ja (`/swagger`). |
| **Identity** | Gotov sustav za registraciju, prijavu, role, lozinke. |
| **`AppUser`** | Naš Identity korisnik proširen s `OIB`/`JMBG`. |
| **OAuth / Google** | Prijava preko Googlea bez da vidimo lozinku. |
| **Dropzone** | JS komponenta za asinkroni upload datoteka. |
| **Soft delete** | Logičko brisanje preko `DeletedAt`, red ostaje u bazi. |
| **Integracijski test** | Provjera cijelog API toka kroz pravi HTTP poziv. |
| **`WebApplicationFactory`** | Pokreće aplikaciju u memoriji za testove. |
| **EF InMemory** | Baza u memoriji umjesto MySQL-a u testovima. |
| **`TestAuthHandler`** | Lažna autentikacija preko HTTP zaglavlja u testovima. |
| **Status 201** | Created — uspješan `POST`. |
| **Status 401 / 403** | Nisi prijavljen / nemaš pravo. |
| **Status 404** | Zapis ne postoji. |

---

*Dokument je study-aid za obranu; sve tvrdnje provjerene u kodu projekta (stanje: Faza 6 gotova, 55/55 testova prolazi).*
