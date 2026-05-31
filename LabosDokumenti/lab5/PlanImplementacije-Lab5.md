# Plan implementacije Lab 5 — Planinarska aplikacija

**Projekt:** Razvoj web aplikacija u ASP.NET MVC tehnologiji
**Repo:** https://github.com/BosnjakLuka/Razvoj-web-aplikacija-u-ASP.NET-MVC-tehnologiji
**Predaja:** 12.6.

---

## Analiza zatečenog stanja

Prema repozitoriju i Lab1–Lab4 dokumentaciji, projekt već sadrži:

- **11 entiteta** (Korisnik, Knjizica, Posjet, Fotografija, KontrolnaTocka, Podrucje, Ruta, PlaninarskiObjekt, PlaninarskaUdruga, Medalja, KorisnikMedalja)
- `PlaninarstvoDbContext` (MySQL preko Pomelo, code-first migracije)
- EF repository pattern (zamijenjen mock repository iz Lab3 koraka 5)
- Funkcionalni CRUD za sve entitete, autocomplete dropdown, client+server validacija, flatpickr datepicker, JS animacije (Lab4)
- Postojeća `Fotografija` tablica vezana N:1 na `Posjet` — savršeno za upload datoteka u Lab5
- Soft delete (`DeletedAt`) na većini entiteta iz Lab4
- HasData seed s placeholder `PasswordHash` vrijednostima u `Korisnik`

### Što fali za Lab5 (5 kriterija, ukupno 7 bodova)

| Kriterij | Bodovi |
|---|---|
| Web API kontroleri + DTO | 2 |
| Identity autentikacija + autorizacija | 1 |
| Dropzone upload datoteka | 1 |
| Google/Facebook OAuth | 1 |
| Integracijski testovi za API | 2 |

> ⚠️ **Mapping na našu domenu:** u Lab5 dokumentu sve se referira na "kviz" jer je profesorov primjer Quiz Manager. Kod nas se to **mapira na `Posjet`** (glavni radni entitet) — Dropzone upload veže fotografije na konkretni `Posjet`.

---

## Identificirani rizici (mora se rješavati ili rano ili ciljano)

| # | Rizik | Faza | Mitigacija |
|---|---|---|---|
| R1 | `Korisnik.IdKorisnik` (int) vs `AppUser.Id` (string) — kako ih povezati | F0+F1 | Dodati `Korisnik.AppUserId` nullable string FK; popunjavati u Register/ExternalLogin |
| R2 | `[Authorize]` zaključa vlastite CRUD ekrane ako "vlasnik" helper nije gotov | F2 | Prvo napisati i testirati `IsOwnerAsync`, **tek tada** dodati atribute |
| R3 | API može slučajno izložiti `PasswordHash`, `OIB`, `JMBG` | F1+F3 | Drop `PasswordHash` migracijom; `KorisnikDto` public/admin split; nikad ne izlagati OIB/JMBG kroz API |
| R4 | `Fotografija` nema `ContentType` / `FileSize` — kasnije lomi DTO i testove | F1 | Dodati polja u **istoj migraciji** s Identity-jem |
| R5 | 88 testova nerealno za rok | F6 | Minimum 5 testova × 11 kontrolera = 55; ostali bonus |
| R6 | Pomelo MySQL + Identity = index length problem (key prefix 450 chars × 4 bajta) | F1 | U `OnModelCreating` skratiti Identity ključeve ili koristiti `KeyAttribute`; test na praznoj bazi prije |
| R7 | EF HasData seed s `PasswordHash` neće kompajlirati nakon Identity migracije | F1 | Prebaciti seed korisnika u `IdentitySeed.cs`; ukloniti `PasswordHash` iz HasData |
| R8 | Soft delete `DeletedAt` lako se zaboravi filtrirati u API GET-ovima | F2 | Global query filter u `OnModelCreating` (`HasQueryFilter`) |
| R9 | Dropzone POST bez antiforgery tokena → 400, izgleda kao da ne radi | F4 | Proslijediti token u Dropzone `headers` |
| R10 | Public vs private listanje posjeta — neodlučeno | F3 | Odluka: posjeti **javni za čitanje**, pisanje samo vlastite |

---

## Faza 0 — Priprema (1 dan)

### Odluke koje moraju biti finalne

1. **AppUser ↔ Korisnik strategija:** Opcija B (odvojene tablice).
   - `AppUser : IdentityUser` (default string Id, GUID)
   - `Korisnik` ostaje s `int IdKorisnik` + dodaje `public string? AppUserId { get; set; }`
   - Nullable jer postojeći seed-ani Korisnici nemaju AppUser dok migracija ne prođe
2. **Seed strategija:** test korisnici se prebacuju iz EF `HasData` u `IdentitySeed.cs`. Stari `HasData` za Korisnike se zadržava ali bez `PasswordHash` polja; `AppUserId` se popunjava u `IdentitySeed.cs` nakon što su `AppUser` zapisi kreirani.
3. **Identity UI:** koristimo scaffoldane Razor Pages (`Areas/Identity/Pages/...`), ne migriramo u MVC kontrolere.
4. **Username strategija:** email = UserName (jednostavnije, jedan login flow).

### Tehničke pripreme

1. Napraviti git branch `lab5-api-auth`; master ostaje stabilan dok sve ne završi.
2. Provjeriti rade li svi postojeći Lab4 ekrani (CRUD, validacija, autocomplete) — Lab5 ih ne smije slomiti.
3. **Backup baze** (export schema + podaci) prije bilo kakve Identity migracije.
4. **Empty DB test setup:** kreirati lokalno drugu MySQL bazu (`planinarstvo_test`) za prvo trčanje Identity migracije. Ako prođe → tek onda na dev bazu.
5. Provjeriti je li HTTPS aktivan u `Properties/launchSettings.json` (treba za OAuth).
6. Provjeriti verziju Pomelo paketa (mora biti kompatibilan s `Microsoft.AspNetCore.Identity.EntityFrameworkCore` za .NET 8).

---

## Faza 1 — ASP.NET Core Identity (lokalna autentikacija)

**Cilj:** uvesti `AppUser`, registraciju, prijavu, odjavu — bez diranja postojećih CRUD ekrana.

### Tasks

1. **NuGet paketi:**
   - `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
   - `Microsoft.AspNetCore.Identity.UI`
   - `Microsoft.Extensions.Identity.Stores`

2. **Nova klasa `Entiteti/AppUser.cs`:**
   ```csharp
   public class AppUser : IdentityUser {
       [Required, StringLength(11, MinimumLength = 11)]
       [RegularExpression("^[0-9]*$")]
       public string OIB { get; set; } = string.Empty;

       [Required, StringLength(13, MinimumLength = 13)]
       [RegularExpression("^[0-9]*$")]
       public string JMBG { get; set; } = string.Empty;
   }
   ```

3. **Izmjene u `Entiteti/Korisnik.cs`:**
   - **Ukloniti** `PasswordHash` polje (više se ne koristi — Identity ima svoje).
   - **Dodati** `public string? AppUserId { get; set; }` + navigacijsko svojstvo:
     ```csharp
     [ForeignKey(nameof(AppUserId))]
     public virtual AppUser? AppUser { get; set; }
     ```

4. **Izmjene u `Entiteti/Fotografija.cs` (R4):**
   - `public string? ContentType { get; set; }`
   - `public long FileSize { get; set; }`

5. **`PlaninarstvoDbContext`:**
   - Promijeniti baznu klasu iz `DbContext` u `IdentityDbContext<AppUser>`.
   - U `OnModelCreating`: **prvo** `base.OnModelCreating(modelBuilder)`, **tek onda** postojeća konfiguracija.
   - **MySQL key length fix (R6):** dodati globalnu konvenciju za string ključeve:
     ```csharp
     foreach (var entity in modelBuilder.Model.GetEntityTypes()) {
         foreach (var prop in entity.GetProperties()
                  .Where(p => p.ClrType == typeof(string) && p.IsKey())) {
             prop.SetMaxLength(255);  // sigurno ispod InnoDB limita
         }
     }
     ```
   - **Global query filter za soft delete (R8):** za svaki entitet koji ima `DeletedAt`:
     ```csharp
     modelBuilder.Entity<Podrucje>().HasQueryFilter(p => p.DeletedAt == null);
     modelBuilder.Entity<KontrolnaTocka>().HasQueryFilter(k => k.DeletedAt == null);
     // … za svih 5-6 entiteta s DeletedAt
     ```

6. **Migracija (jedna, kombinirana):**
   ```bash
   dotnet ef migrations add Lab5_AddIdentityAndFotoMetadata
   dotnet ef database update --connection "<test-baza>"   # prvo na test bazi (R6)
   ```
   Migracija mora:
   - Dodati sve AspNet* tablice
   - Dropati `PasswordHash` kolonu iz `Korisnik` (R3)
   - Dodati `AppUserId` u `Korisnik`
   - Dodati `ContentType`, `FileSize` u `Fotografija` (R4)
   - **NE smije** dirati postojeće FK-ove, indekse, ili seed podatke

   Ako migracija prođe na test bazi → primijeniti na dev:
   ```bash
   dotnet ef database update
   ```

7. **`Program.cs` — Identity setup:**
   ```csharp
   builder.Services
       .AddDefaultIdentity<AppUser>(o => {
           o.SignIn.RequireConfirmedAccount = false;
           o.Password.RequiredLength = 6;
           o.Password.RequireNonAlphanumeric = false;  // labavije za development
       })
       .AddRoles<IdentityRole>()
       .AddEntityFrameworkStores<PlaninarstvoDbContext>();

   builder.Services.AddRazorPages();  // KRITIČNO za Identity stranice
   ```

   U pipeline (redoslijed je obavezan):
   ```csharp
   app.UseAuthentication();   // PRIJE UseAuthorization
   app.UseAuthorization();

   app.MapControllerRoute(...);
   app.MapRazorPages();       // KRITIČNO, bez ovog Identity stranice = 404
   ```

8. **Scaffold Identity stranica** (VS → Add → New Scaffolded Item → Identity):
   - `Account/Register`
   - `Account/Login`
   - `Account/Logout`
   - `Account/ExternalLogin` (potreban za Fazu 5)
   - `Account/Manage/Index` (osobni podaci — tu se vide OIB/JMBG)
   - Odabrati postojeći `PlaninarstvoDbContext`. **Obavezno provjeriti** je li scaffolder generirao duplikat `ApplicationDbContext` u `Areas/Identity/Data` — ako jest, obrisati ga.

9. **Proširiti `Register.cshtml(.cs)`** poljima `OIB`, `JMBG`:
   - InputModel: dodati polja s istim validacijskim anotacijama kao na `AppUser`.
   - `OnPostAsync`: kod kreiranja `var user = new AppUser { ... }` proslijediti i `OIB`, `JMBG`.
   - **Odmah nakon** `await _userManager.CreateAsync(user, Input.Password)` (ako uspije), kreirati i pripadajući `Korisnik` zapis:
     ```csharp
     var korisnik = new Korisnik {
         Ime = Input.Ime,
         Prezime = Input.Prezime,
         Email = Input.Email,
         KorisnickoIme = Input.Email,
         DatumRegistracije = DateTime.UtcNow,
         StatusAktivan = true,
         AppUserId = user.Id
     };
     _context.Korisnici.Add(korisnik);
     await _context.SaveChangesAsync();
     ```
     Bez ovog koraka korisnik je prijavljen ali nema planinarski profil → CRUD ekrani pucaju.

10. **`_Layout.cshtml`:** ubaciti `@await Html.PartialAsync("_LoginPartial")` u navbar (Login/Register linkovi za anonimne, korisničko ime + Logout za prijavljene). Tipično ide desno od main navigacije.

### Acceptance kriteriji za Fazu 1

- [ ] `/Identity/Account/Register` se otvara i prihvaća validni email + OIB + JMBG
- [ ] Nakon registracije korisnik je auto-loginan i preusmjeren na `/`
- [ ] U bazi postoji zapis u `AspNetUsers` **i** u `Korisnik` (s popunjenim `AppUserId`)
- [ ] `/Identity/Account/Login` radi s tim računom
- [ ] `/Identity/Account/Logout` briše cookie
- [ ] `/Identity/Account/Manage` prikazuje OIB i JMBG
- [ ] Postojeći CRUD ekrani (Posjet, KontrolnaTocka, …) **nastavljaju raditi** kao prije
- [ ] Migracija prošla bez warninga o key length-u

---

## Faza 2 — Autorizacija (role + zaštita akcija)

### Tasks

1. **Definirati role:** `Admin`, `Planinar` (Lab5 traži Admin + bar još jednu).

2. **`Data/IdentitySeed.cs` — seed rola, admina i test planinara:**
   ```csharp
   public static async Task SeedAsync(IServiceProvider sp) {
       var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole>>();
       var userMgr = sp.GetRequiredService<UserManager<AppUser>>();
       var db = sp.GetRequiredService<PlaninarstvoDbContext>();

       foreach (var r in new[] {"Admin", "Planinar"})
           if (!await roleMgr.RoleExistsAsync(r))
               await roleMgr.CreateAsync(new IdentityRole(r));

       // Admin
       if (await userMgr.FindByEmailAsync("admin@planinarenje.hr") is null) {
           var admin = new AppUser { UserName = "admin@planinarenje.hr",
                                     Email = "admin@planinarenje.hr",
                                     EmailConfirmed = true,
                                     OIB = "00000000000", JMBG = "0000000000000" };
           await userMgr.CreateAsync(admin, "Admin123!");
           await userMgr.AddToRoleAsync(admin, "Admin");
           // poveži s postojećim seed-anim Korisnikom (R7) ili kreiraj novi
       }

       // Planinar (test korisnik)
       // … isto za "luka@planinarenje.hr" + role "Planinar"
   }
   ```
   Pozvati iz `Program.cs` nakon `app.Build()` kroz scope.

3. **`Controllers/BaseController.cs` — helper za vlasništvo (R2):**
   ```csharp
   public abstract class BaseController : Controller {
       protected readonly UserManager<AppUser> UserMgr;
       protected readonly PlaninarstvoDbContext Db;

       protected BaseController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db) {
           UserMgr = userMgr; Db = db;
       }

       protected string? AppUserId => UserMgr.GetUserId(User);

       protected async Task<Korisnik?> GetCurrentKorisnikAsync() {
           var id = AppUserId;
           if (id is null) return null;
           return await Db.Korisnici.FirstOrDefaultAsync(k => k.AppUserId == id);
       }

       protected async Task<bool> IsOwnerAsync(int idKorisnik) {
           var k = await GetCurrentKorisnikAsync();
           return k != null && k.IdKorisnik == idKorisnik;
       }
   }
   ```

4. **Test helpera RUČNO prije nego diraš `[Authorize]`:** login kao Luka, otvori bilo koju akciju koja zove `IsOwnerAsync(lukaId)`, debug confirm true. **Tek tada** kreni s `[Authorize]` atributima (R2).

5. **Autorizacijska pravila** po kontroleru:

   | Entitet | Index/Details | Create | Edit | Delete |
   |---|---|---|---|---|
   | KontrolnaTocka, Ruta, Podrucje, PlaninarskiObjekt, PlaninarskaUdruga, Medalja | `[AllowAnonymous]` | Admin | Admin | Admin |
   | Posjet | `[AllowAnonymous]` (Index, Details) | Planinar+Admin | vlasnik ili Admin | vlasnik ili Admin |
   | Knjizica | vlasnik ili Admin | auto pri Registraciji | vlasnik | Admin |
   | Korisnik | Admin | Admin | vlasnik ili Admin | Admin |
   | Fotografija | preko Posjet vlasnika | vlasnik posjeta | vlasnik posjeta | vlasnik ili Admin |
   | KorisnikMedalja | čitanje javno | Admin (auto) | Admin | Admin |

6. **UI prilagodba** — sakriti gumbe koje korisnik ionako ne smije:
   ```cshtml
   @if (User.IsInRole("Admin")) {
       <a class="btn btn-danger" asp-action="Delete">Obriši</a>
   }
   ```

### Acceptance kriteriji za Fazu 2

- [ ] Admin može sve, anonimni može samo čitati javne ekrane
- [ ] Planinar može kreirati Posjet, editirati **samo svoje**, drugima vraća 403
- [ ] Soft-deletani zapisi se ne prikazuju ni u jednoj listi (R8)
- [ ] Gumbi koje korisnik ne smije nisu vidljivi

---

## Faza 3 — Web API (CRUD + DTO)

### Pristup

Lab5 izričito kaže: **prvo napraviti jedan API kontroler kvalitetno, testirati ga, pa AI-em replicirati**. Krenuti s `PosjetApiController` jer je najsloženiji.

### Struktura

```
Controllers/Api/
  PosjetApiController.cs
  KontrolnaTockaApiController.cs
  RutaApiController.cs
  PodrucjeApiController.cs
  PlaninarskiObjektApiController.cs
  PlaninarskaUdrugaApiController.cs
  KorisnikApiController.cs
  KnjizicaApiController.cs
  MedaljaApiController.cs
  KorisnikMedaljaApiController.cs
  FotografijaApiController.cs

Models/Dto/
  Posjet/  → PosjetDto, PosjetCreateDto, PosjetUpdateDto, PosjetSummaryDto
  Korisnik/ → KorisnikPublicDto, KorisnikAdminDto, KorisnikSummaryDto  (R3)
  … (jedan folder po entitetu)
```

### DTO pravila (R3 + R10)

- **Read DTO**: nested summary DTO-ovi za navigacijska svojstva (npr. `PosjetDto` ima `KontrolnaTockaSummaryDto`).
- **Create/Update DTO**: samo primitivni tipovi + FK ID-evi. Bez nested objekata.
- **`KorisnikPublicDto`** (default `[AllowAnonymous]` GET): samo `IdKorisnik, Ime, Prezime, KorisnickoIme, ProfilnaSlika, DatumRegistracije`.
- **`KorisnikAdminDto`** (samo `[Authorize(Roles="Admin")]`): dodaje `Email`, `BrojMobitela`, `StatusAktivan`. **NIKAD ne sadrži OIB, JMBG, AppUserId** — ta polja nisu izložena ni preko API-ja.
- **`PasswordHash`** ne postoji više u `Korisnik` modelu nakon F1 migracije, pa nije rizik (R3).

### Public vs private listing (R10)

Odluka: `GET /api/posjet` je javno (svi vide tuđe posjete kao na društvenoj mreži). `POST/PUT/DELETE` traži autentikaciju + vlasništvo.

### Standardni obrazac (kopirati za 10 ostalih)

```csharp
[Route("api/posjet")]
[ApiController]
public class PosjetApiController : ControllerBase {
    private readonly PlaninarstvoDbContext _db;
    public PosjetApiController(PlaninarstvoDbContext db) { _db = db; }

    [HttpGet, AllowAnonymous]
    public async Task<ActionResult<List<PosjetDto>>> Get(
        [FromQuery] int? korisnikId,
        [FromQuery] int? kontrolnaTockaId,
        [FromQuery] DateTime? datumOd,
        [FromQuery] DateTime? datumDo)
    { /* filter, Include, .Select(ToDto), ToListAsync */ }

    [HttpGet("{id}"), AllowAnonymous]
    public async Task<ActionResult<PosjetDto>> GetById(int id) { /* 404 ako null */ }

    [HttpPost, Authorize(Roles="Admin,Planinar")]
    public async Task<ActionResult<PosjetDto>> Post([FromBody] PosjetCreateDto dto) {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        // postaviti IdKorisnik iz trenutnog Identity korisnika, ne iz DTO-a
    }

    [HttpPut("{id}"), Authorize(Roles="Admin,Planinar")]
    public async Task<ActionResult<PosjetDto>> Put(int id, [FromBody] PosjetUpdateDto dto)
    { /* provjeri vlasništvo, vrati 403 ako ne odgovara */ }

    [HttpDelete("{id}"), Authorize(Roles="Admin,Planinar")]
    public async Task<IActionResult> Delete(int id) { /* isto, vlasnik ili Admin */ }
}
```

### Tasks

1. Dodati Swagger (`Swashbuckle.AspNetCore`) + `app.UseSwagger(); app.UseSwaggerUI();` u Development.
2. **`PosjetApiController` do kraja** + DTO mapiranje + svi query filteri + Include navigacijskih svojstava.
3. Testirati u Swaggeru — status kodovi 200/201/204/400/404/401/403.
4. Tek tada AI-em replicirati identičan obrazac na ostalih 10 kontrolera.
5. **Query parametri po kontroleru:**
   - Posjet: `korisnikId, kontrolnaTockaId, datumOd, datumDo, dozivljaj`
   - KontrolnaTocka: `podrucjeId, tip, naziv` (LIKE)
   - Ruta: `tezina, kontrolnaTockaId`
   - Podrucje: `naziv` (LIKE)
   - Korisnik: `query` (po imenu/prezimenu/usernameu)
6. **Mapiranje:** privatne `ToDto(entity)` metode u kontroleru. AutoMapper nije nužan za vježbu.

---

## Faza 4 — Dropzone upload fotografija na Posjet

### Tasks

1. **`Fotografija` entitet već ima `ContentType` i `FileSize`** (dodano u F1, R4). Nije potrebna nova migracija.

2. **Dropzone u `Posjet/Edit.cshtml` i `Posjet/Details.cshtml`** — CDN:
   ```html
   <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/dropzone@5/dist/min/dropzone.min.css">
   <script src="https://cdn.jsdelivr.net/npm/dropzone@5/dist/min/dropzone.min.js"></script>
   ```

3. **Dropzone init s antiforgery tokenom (R9):**
   ```html
   @Html.AntiForgeryToken()
   <form id="fotoDropzone" class="dropzone"
         action="@Url.Action("UploadFoto","Posjet")"></form>
   <script>
     Dropzone.options.fotoDropzone = {
       paramName: "file",
       maxFilesize: 5,
       acceptedFiles: ".jpg,.jpeg,.png,.webp",
       headers: {
         "RequestVerificationToken":
           document.querySelector('input[name="__RequestVerificationToken"]').value
       },
       params: { idPosjet: @Model.IdPosjet },
       success: function() { loadFotografije(); }
     };
   </script>
   ```

4. **`PosjetController.UploadFoto(int idPosjet, IFormFile file)`:**
   - `[HttpPost, ValidateAntiForgeryToken, Authorize]`
   - Provjeriti `IsOwnerAsync` za `posjet.IdKorisnik` (preko helpera iz F2).
   - Validacija: ekstenzija + veličina (max 5MB) + ContentType startsWith "image/".
   - Path: `wwwroot/uploads/posjeti/{idPosjet}/{Guid}.{ext}`.
   - U bazu: `Fotografija { IdPosjet, NazivDatoteke, PutanjaDatoteke, ContentType, FileSize, DatumUploada, TipSlike }`.
   - Vratiti `Json(new { success = true, id = foto.IdFotografija })`.

5. **AJAX popis fotografija** — `_FotografijaList.cshtml` partial + `GetFotografije(int idPosjet)` akcija. U viewu:
   ```js
   function loadFotografije() {
     $("#fotoList").load("@Url.Action("GetFotografije","Posjet")?idPosjet=@Model.IdPosjet");
   }
   $(function() { loadFotografije(); });
   ```

6. **Brisanje** — `[HttpPost, ValidateAntiForgeryToken, Authorize] DeleteFoto(int id)`:
   - Provjera vlasništva.
   - Fizičko brisanje s diska.
   - Brisanje zapisa iz baze.
   - Refresh AJAX-om.

### Acceptance kriteriji za Fazu 4

- [ ] Upload prolazi 200 (ne 400 zbog tokena)
- [ ] Datoteka stigla na `wwwroot/uploads/posjeti/{id}/...`
- [ ] U `Fotografija` tablici postoji red s točnim metapodacima
- [ ] Popis se osvježi automatski nakon uploada
- [ ] Tuđi posjet vraća 403 (ne 200)
- [ ] Brisanje skida i s diska i iz baze

---

## Faza 5 — Google OAuth

**Preporuka:** Google (jednostavniji setup od Facebooka).

### Tasks

1. **Google Cloud Console** → New Project → APIs & Services → Credentials → OAuth 2.0 Client ID (Web application):
   - Authorized redirect URI: `https://localhost:7xxx/signin-google`
   - Dobiti `ClientId` + `ClientSecret`.

2. **User secrets:**
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "Authentication:Google:ClientId" "…"
   dotnet user-secrets set "Authentication:Google:ClientSecret" "…"
   ```

3. **NuGet:** `Microsoft.AspNetCore.Authentication.Google`

4. **`Program.cs`:**
   ```csharp
   builder.Services.AddAuthentication()
       .AddGoogle(o => {
           o.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
           o.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
       });
   ```
   ⚠️ `UseAuthentication()` **mora** biti prije `UseAuthorization()` u pipeline (već postavljeno u F1).

5. **`Login.cshtml`** — scaffoldana stranica automatski prikaže Google gumb kad je provider registriran. Provjeriti samo da je "External logins" sekcija u viewu.

6. **`ExternalLogin.cshtml.cs`** (prva prijava preko Googlea) — dodati OIB/JMBG polja **iste kao u Register**. Pri kreiranju `AppUser`-a popuniti ih, i odmah kreirati pripadajući `Korisnik` zapis (kao u F1 task 9).

7. **HTTPS** obavezan — provjeriti `app.UseHttpsRedirection()`.

### Acceptance kriteriji za Fazu 5

- [ ] Klik na "Google" preusmjerava na Google login
- [ ] Prva prijava traži OIB+JMBG i kreira AppUser + Korisnik
- [ ] Sljedeća prijava istog Google računa odmah logira

---

## Faza 6 — Integracijski testovi

### Strategija pokrivenosti

**Minimum (cilj za prolaz):** 5 testova × 11 kontrolera = **55 testova** (R5).
**Stretch (ako vrijeme):** +3 po kontroleru = 88.

**Obavezni minimum po kontroleru:**
1. `GET all` → 200 + nije prazna kolekcija (sa seed-om)
2. `GET by id` → 200 kad postoji
3. `GET by id` → 404 kad ne postoji
4. `POST` → 201 + zapis je u bazi
5. `POST` → 400 za invalid model

**Stretch:**
6. `PUT` → 200 / 7. `PUT` → 404 / 8. `DELETE` → 200 / 9. `DELETE` → 404 / 10. Anonimni `POST` → 401

### Setup

1. **Novi test projekt:** `Razvoj-web-aplikacija.IntegrationTests` (xUnit).
   ```
   IntegrationTests/
     CustomWebAppFactory.cs
     Helpers/TestDataSeeder.cs
     Helpers/AuthHelper.cs        ← za testove koji zahtijevaju autorizaciju
     PosjetApiTests.cs
     KontrolnaTockaApiTests.cs
     … (jedna klasa po API kontroleru)
   ```

2. **NuGet:** `Microsoft.AspNetCore.Mvc.Testing`, `Microsoft.EntityFrameworkCore.InMemory`, `xunit`, `xunit.runner.visualstudio`, `FluentAssertions`.

3. **`Program.cs`** — dodati na dno:
   ```csharp
   public partial class Program {}
   ```
   (omogućava `WebApplicationFactory<Program>`).

### CustomWebAppFactory

- Zamijeni MySQL `PlaninarstvoDbContext` s InMemory providerom.
- Svaka test klasa koristi unikatni naziv InMemory baze (`Guid.NewGuid().ToString()`) za izolaciju.
- Mockaj `IEmailSender` ako Identity scaffolder ga registrira (`FakeEmailSender`).

### Obrazac

```csharp
public class PosjetApiTests : IClassFixture<CustomWebAppFactory> {
    [Fact] public async Task GetAll_Returns200() { ... }
    [Fact] public async Task GetById_Returns200_WhenExists() { ... }
    [Fact] public async Task GetById_Returns404_WhenMissing() { ... }
    [Fact] public async Task Post_Returns201_AndCreatesEntity() { ... }
    [Fact] public async Task Post_Returns400_WhenModelInvalid() { ... }
}
```

Prvi `PosjetApiTests` ručno do kraja, ostali AI-em po istom obrascu.

---

## Faza 7 — Finalna verifikacija

- [ ] Sva 11 API kontrolera imaju 5 osnovnih CRUD metoda + query parametre
- [ ] DTO nikad ne izlaže `OIB`, `JMBG`, `AppUserId` kroz javni endpoint
- [ ] Registracija/Login/Logout rade lokalno
- [ ] Google login radi end-to-end
- [ ] Role `Admin` + `Planinar` postoje, seed kreira oba test korisnika
- [ ] Dropzone uploada na disk + DB, AJAX refresh radi, brisanje radi
- [ ] Antiforgery token prolazi (nema 400 na Dropzone POST-u)
- [ ] Soft-deletani zapisi nikad ne izlaze van (global query filter aktivan)
- [ ] Test projekt — minimum 55 testova, svi zeleni
- [ ] `dotnet build` bez warninga, Lab1–4 ekrani i dalje rade

---

## Vremenska linija do 12.6.

| Tjedan | Fokus | Glavni rizik |
|---|---|---|
| T1 | Faza 0 + Faza 1 (Identity, migracija prošla, Register/Login rade) | R1, R6, R7 |
| T2 | Faza 2 (vlasnik helper + role + soft delete filter) | R2, R8 |
| T3 | Faza 3 — `PosjetApiController` do kraja, pa AI ostali | R3, R10 |
| T4 | Faza 4 (Dropzone) + Faza 5 (Google) | R9 |
| T5 | Faza 6 — 55 testova minimum | R5 |
| Predaja | Faza 7 verifikacija | — |
