# Lab 3 — Kompletni vodič za implementaciju

## Pregled koraka

| Korak | Što se radi | Bodovi |
|-------|-------------|--------|
| 0 | Kreiranje prazne MySQL baze | Preduvjet |
| 1 | NuGet paketi za EF + MySQL | Bod 1 |
| 2 | Prilagodba modela za EF (anotacije, veze) | Bod 1 |
| 3 | DbContext klasa + DI registracija + connection string | Bod 1 |
| 4 | Seed podaci + inicijalna migracija | Bod 1 |
| 5 | Zamjena mock repozitorija EF-om u kontrolerima | Bod 1 |
| 6 | Custom routing — barem 4 akcije | Bod 3 (usmeno) |
| 7 | semantic-model.md | Bod 4 |
| 8 | sitemap.md | Bod 4 |
| 9 | SKILL.md za VS Code Copilot | Bod 5 |

---

# KORAK 0 — Kreiranje prazne baze

Otvori SQLyog ili MySQL Workbench i pokreni:

```sql
CREATE DATABASE PlaninarstvoDb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

Pokreni XAMPP i osiguraj da MySQL servis radi.

---

# KORAK 1 — NuGet paketi

## Kontekst fileovi za agenta
Nema — ovo je terminalna naredba.

## Prompt za Copilot agenta (edit mode)

```
Instaliraj sljedeće NuGet pakete u projekt. Koristi terminalne naredbe `dotnet add package`:

1. Pomelo.EntityFrameworkCore.MySql
2. Microsoft.EntityFrameworkCore
3. Microsoft.EntityFrameworkCore.Tools
4. Microsoft.EntityFrameworkCore.Design

Ako projekt ima više slojeva (Web, Model, DAL), instaliraj:
- Pomelo.EntityFrameworkCore.MySql → u DAL ili Web projekt (onaj koji ima DbContext)
- Microsoft.EntityFrameworkCore → u Model/DAL projekt
- Microsoft.EntityFrameworkCore.Tools i .Design → u Web/startup projekt

Ako je sve u jednom projektu, instaliraj sve u taj jedan projekt.

Pokreni naredbe u terminalu.
```

---

# KORAK 2 — Prilagodba modela za EF

## Kontekst fileovi za agenta
Dodaj u kontekst:
- `finalni_model_planinarska_aplikacija_ispravljeno.md`
- **Sve entitetne klase** iz projekta (Korisnik.cs, Knjizica.cs, Posjet.cs, Fotografija.cs, KontrolnaTocka.cs, Ruta.cs, Podrucje.cs, PlaninarskiObjekt.cs, PlaninarskaUdruga.cs, Medalja.cs, KorisnikMedalja.cs)
- **Sve enum klase** (DozivljajPosjeta.cs, TipKontrolneTocke.cs, TipObjekta.cs, TezinaRute.cs, TipSlike.cs — ako postoje)

## Prompt za Copilot agenta (edit mode)

```
Prilagodi sve entitetne klase u projektu za Entity Framework Core. Koristi datoteku finalni_model_planinarska_aplikacija_ispravljeno.md kao izvor istine za sve atribute, tipove i relacije.

Za svaku klasu napravi sljedeće:

1. PRIMARNI KLJUČ — dodaj [Key] atribut na PK svojstvo (npr. IdKorisnik, IdPosjet, itd.)

2. FOREIGN KEY SVOJSTVA — za svaki FK:
   - Zadrži int FK svojstvo (npr. IdKorisnik u Knjizica)
   - Dodaj [ForeignKey("NavigacijskoSvojstvo")] atribut iznad FK int svojstva
   - Dodaj navigacijsko svojstvo kao: public virtual ReferenciranaKlasa NazivSvojstva { get; set; }

3. KOLEKCIJE (1:N strana) — na "1" strani relacije dodaj:
   - public virtual ICollection<DijeteKlasa> NazivKolekcije { get; set; }

4. NULLABLE SVOJSTVA — svojstva koja su NULL u modelu označi kao nullable (string? za stringove, int? za int, DateTime? za datume)

5. REQUIRED SVOJSTVA — svojstva koja su NN (not null) ostavi bez ? oznake

6. STRING DULJINE — dodaj [MaxLength(X)] prema specifikaciji iz modela (npr. [MaxLength(100)] za Ime, [MaxLength(150)] za Email itd.)

7. UNIQUE — dodaj [Microsoft.EntityFrameworkCore.Index] na klasu za unique polja ili ćemo to riješiti u OnModelCreating

8. ENUMI — ostavi enum tipove kakvi jesu, EF Core ih automatski mapira u int. Ako enum klase ne postoje, kreiraj ih:
   - DozivljajPosjeta: VrloLagano, Lagano, Srednje, Zahtjevno, VrloZahtjevno, KratkoAliTesko, DugoAliLagano, FizickiNaporno, TehnickiZahtjevno
   - TipKontrolneTocke: Vrh, Vidikovac, KontrolnaTocka
   - TipObjekta: Dom, Kuca, Skloniste
   - TezinaRute: Laka, Srednja, Teska  (ne Zahtjevna — koristi točne nazive)
   - TipSlike: Selfie, Oznaka, Krajolik, Mapa, Drugo

Relacije iz modela (STROGO POŠTUJ):

1:1 relacije:
- Korisnik (1) — (1) Knjizica → Knjizica ima FK IdKorisnik s UNIQUE

1:N relacije:
- Korisnik (1) — (N) Posjet
- Knjizica (1) — (N) Posjet
- KontrolnaTocka (1) — (N) Posjet
- Ruta (1) — (N) Posjet
- Posjet (1) — (N) Fotografija
- Podrucje (1) — (N) KontrolnaTocka
- KontrolnaTocka (1) — (N) Ruta
- Podrucje (1) — (N) PlaninarskiObjekt
- PlaninarskaUdruga (1) — (N) PlaninarskiObjekt
- Korisnik (1) — (N) KorisnikMedalja
- Medalja (1) — (N) KorisnikMedalja

Primjer kako treba izgledati klasa Podrucje nakon prilagodbe:

```csharp
using System.ComponentModel.DataAnnotations;

public class Podrucje
{
    [Key]
    public int IdPodrucje { get; set; }

    [Required]
    [MaxLength(150)]
    public string Naziv { get; set; }

    public string? Opis { get; set; }

    [MaxLength(150)]
    public string? Regija { get; set; }

    public int MinimalanBrojKTZaObilazak { get; set; }

    public virtual ICollection<KontrolnaTocka> KontrolneTocke { get; set; }
    public virtual ICollection<PlaninarskiObjekt> PlaninarskiObjekti { get; set; }
}
```

Primjer kako treba izgledati klasa Posjet (ima 4 FK-a):

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Posjet
{
    [Key]
    public int IdPosjet { get; set; }

    [ForeignKey("Korisnik")]
    public int IdKorisnik { get; set; }
    public virtual Korisnik Korisnik { get; set; }

    [ForeignKey("Knjizica")]
    public int IdKnjizica { get; set; }
    public virtual Knjizica Knjizica { get; set; }

    [ForeignKey("KontrolnaTocka")]
    public int IdKontrolnaTocka { get; set; }
    public virtual KontrolnaTocka KontrolnaTocka { get; set; }

    [ForeignKey("Ruta")]
    public int IdRuta { get; set; }
    public virtual Ruta Ruta { get; set; }

    public DateTime DatumVrijemePosjeta { get; set; }
    public int? VrijemeUsponaMin { get; set; }
    public DozivljajPosjeta DozivljajPosjeta { get; set; }
    public string? OpisIskustva { get; set; }

    [Required]
    [MaxLength(100)]
    public string UneseniGUID { get; set; }

    public bool JeLiPotvrdenPosjet { get; set; }
    public DateTime DatumKreiranjaZapisa { get; set; }

    public virtual ICollection<Fotografija> Fotografije { get; set; }
}
```

Primijeni ovaj obrazac na SVE klase u projektu. Ne preskoči nijednu. Dodaj using System.ComponentModel.DataAnnotations i using System.ComponentModel.DataAnnotations.Schema gdje je potrebno.
```

---

# KORAK 3 — DbContext + Connection String + DI

## Kontekst fileovi za agenta
Dodaj u kontekst:
- `Program.cs`
- `appsettings.json`
- Sve ažurirane entitetne klase iz koraka 2

## Prompt za Copilot agenta (edit mode)

```
Napravi sljedeće tri stvari:

## 1. Kreiraj DbContext klasu

Kreiraj novu klasu PlaninarstvoDbContext koja nasljeđuje DbContext.
Ako postoji DAL folder ili projekt, stavi ju tamo. Ako ne, stavi u root projekta ili u novi folder "Data".

Sadržaj klase:

```csharp
using Microsoft.EntityFrameworkCore;

public class PlaninarstvoDbContext : DbContext
{
    public PlaninarstvoDbContext(DbContextOptions<PlaninarstvoDbContext> options) : base(options)
    { }

    public DbSet<Korisnik> Korisnici { get; set; }
    public DbSet<Knjizica> Knjizice { get; set; }
    public DbSet<Posjet> Posjeti { get; set; }
    public DbSet<Fotografija> Fotografije { get; set; }
    public DbSet<KontrolnaTocka> KontrolneTocke { get; set; }
    public DbSet<Ruta> Rute { get; set; }
    public DbSet<Podrucje> Podrucja { get; set; }
    public DbSet<PlaninarskiObjekt> PlaninarskiObjekti { get; set; }
    public DbSet<PlaninarskaUdruga> PlaninarskeUdruge { get; set; }
    public DbSet<Medalja> Medalje { get; set; }
    public DbSet<KorisnikMedalja> KorisnikMedalje { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique constraint za Knjizica.IdKorisnik (1:1 veza)
        modelBuilder.Entity<Knjizica>()
            .HasIndex(k => k.IdKorisnik)
            .IsUnique();

        // Unique constraint za Korisnik.Email
        modelBuilder.Entity<Korisnik>()
            .HasIndex(k => k.Email)
            .IsUnique();

        // Unique constraint za Korisnik.KorisnickoIme
        modelBuilder.Entity<Korisnik>()
            .HasIndex(k => k.KorisnickoIme)
            .IsUnique();

        // Unique constraint za KontrolnaTocka.GUIDOznaka
        modelBuilder.Entity<KontrolnaTocka>()
            .HasIndex(kt => kt.GUIDOznaka)
            .IsUnique();

        // Unique constraint za PlaninarskaUdruga.OIB
        modelBuilder.Entity<PlaninarskaUdruga>()
            .HasIndex(pu => pu.OIB)
            .IsUnique();
    }
}
```

Dodaj ispravne using direktive za sve entitetne klase i namespace.

## 2. Dodaj connection string u appsettings.json

U appsettings.json dodaj ConnectionStrings sekciju:

```json
"ConnectionStrings": {
    "PlaninarstvoDbContext": "Server=localhost;Port=3306;Database=PlaninarstvoDb;User=root;Password=;"
}
```

Ako sekcija ConnectionStrings već postoji, samo dodaj ključ unutra. NE briši ostale postojeće ključeve.

## 3. Registriraj DbContext u Program.cs

U Program.cs dodaj PRIJE builder.Build():

```csharp
builder.Services.AddDbContext<PlaninarstvoDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("PlaninarstvoDbContext"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("PlaninarstvoDbContext"))
    ));
```

Dodaj using za:
- Microsoft.EntityFrameworkCore
- namespace u kojem se nalazi PlaninarstvoDbContext

VAŽNO: koristimo MySQL (Pomelo provider), NE SqlServer. Nemoj koristiti UseSqlServer.
```

---

# KORAK 4 — Seed podaci + inicijalna migracija

## Kontekst fileovi za agenta
Dodaj u kontekst:
- `PlaninarstvoDbContext.cs` (klasa iz koraka 3)
- `dataset_planinarska_aplikacija.md`

## Prompt za Copilot agenta (edit mode)

```
U klasi PlaninarstvoDbContext, unutar metode OnModelCreating, NAKON existing koda za unique indexe, dodaj seed podatke koristeći HasData().

Koristi podatke iz dataset_planinarska_aplikacija.md. Dodaj SVE podatke iz dataseta:

### Podrucje — svih 20 zapisa
Primjer prvog zapisa:
modelBuilder.Entity<Podrucje>().HasData(
    new Podrucje { IdPodrucje = 1, Naziv = "Slavonija", Opis = "Nizinsko i brežuljkasto područje istočne Hrvatske s Papukom, Psunjem, Krndijom i drugim slavonskim gorjima.", Regija = "Istočna Hrvatska", MinimalanBrojKTZaObilazak = 2 }
);
Dodaj svih 20 zapisa prema tablici u datasetu.
NAPOMENA: atribut UkupanBrojKT NE postoji u modelu klase Podrucje (to je izvedeni podatak), zato ga NE stavljaj u seed.

### Medalja — svih 7 zapisa
Primjer: new Medalja { IdMedalja = 1, Naziv = "Početnik", Opis = "Osnovna medalja za prvi ispravno evidentirani obilazak područja.", MinimalanBrojKontrolnihTocaka = 1, MinimalanBrojPodrucja = 1 }

### PlaninarskaUdruga — svih 5 zapisa
Koristi podatke iz dataseta. Za NULL vrijednosti koristi null.

### PlaninarskiObjekt — svih 5 zapisa
Koristi podatke iz dataseta. Poveži ispravne FK-ove (IdPodrucje, IdPlaninarskaUdruga).
NAPOMENA: TipObjekta je enum — koristi TipObjekta.Dom, TipObjekta.Kuca, TipObjekta.Skloniste.

### KontrolnaTocka — svih 5 zapisa
Koristi podatke iz dataseta.
NAPOMENA: TipKontrolneTocke je enum — koristi TipKontrolneTocke.Vrh, itd.

### Ruta — svih 5 zapisa
Koristi podatke iz dataseta.
NAPOMENA: TezinaRute je enum — koristi TezinaRute.Laka, TezinaRute.Srednja, TezinaRute.Teska (NE "Zahtjevna").
NAPOMENA: DuljinaKm je decimal.

### Korisnik — 2 zapisa
Koristi podatke iz dataseta. Za ProfilnaSlika koristi relativne putanje ili null.

### Knjizica — 2 zapisa
Koristi podatke iz dataseta.

### Posjet — svih 5 zapisa
NAPOMENA: DozivljajPosjeta je enum — koristi DozivljajPosjeta.VrloLagano, .Lagano, .Srednje, .KratkoAliTesko, .FizickiNaporno prema datasetu.

### Fotografija — svih 5 zapisa
NAPOMENA: TipSlike je enum — koristi TipSlike.Selfie itd.
Za PutanjaDatoteke koristi relativne putanje (npr. "/slike/posjeti/vis_luka_selfie.jpg").

### KorisnikMedalja — 2 zapisa
Koristi podatke iz dataseta.

VAŽNO:
- Za HasData, NE postavljaj navigacijska svojstva (virtual ICollection, virtual Klasa) — samo primitivne tipove i FK int-ove
- Datumi moraju biti u formatu new DateTime(2026, 4, 1, 9, 0, 0)
- Enum vrijednosti moraju koristiti enum tip, ne string
- Nemoj koristiti string za bool — koristi true/false
```

## Nakon što agent završi — pokreni u terminalu:

### Prompt za agenta (edit mode):

```
Pokreni u terminalu sljedeće naredbe za EF migraciju.
Prilagodi putanje prema strukturi mog projekta:

1. Pozicioniraj se u folder projekta koji sadrži DbContext klasu
2. Pokreni: dotnet ef migrations add Initial --startup-project <putanja-do-web-projekta>
3. Pokreni: dotnet ef database update --startup-project <putanja-do-web-projekta>

Ako je sve u jednom projektu, naredbe su jednostavno:
dotnet ef migrations add Initial
dotnet ef database update

Ako dobiješ grešku, ispiši ju i predloži rješenje.
```

---

# KORAK 5 — Zamjena mock repozitorija EF-om

## Kontekst fileovi za agenta
Dodaj u kontekst:
- `PlaninarstvoDbContext.cs`
- `Program.cs`
- **Sve Controller klase** (HomeController.cs, KontrolnaTockaController.cs, RutaController.cs, PodrucjeController.cs, itd.)
- **Sve Mock Repository klase** (ako postoje)

## Prompt za Copilot agenta (edit mode)

```
Prebaci sve kontrolere u projektu s mock repozitorija na Entity Framework (PlaninarstvoDbContext).

Za svaki controller:

1. UKLONI stari mock repository iz konstruktora
2. DODAJ PlaninarstvoDbContext kao dependency:
   ```csharp
   private readonly PlaninarstvoDbContext _dbContext;
   
   public XxxController(PlaninarstvoDbContext dbContext)
   {
       _dbContext = dbContext;
   }
   ```

3. U Index akcijama zamijeni mock pozive s EF upitima:
   - Umjesto: var data = _mockRepo.GetAll();
   - Koristi: var data = _dbContext.NazivDbSeta.ToList();

4. U Details akcijama koristi .Include() za navigacijska svojstva koja se prikazuju u view-u:
   - Primjer za KontrolnaTockaController:
     ```csharp
     var kt = _dbContext.KontrolneTocke
         .Include(k => k.Podrucje)
         .Include(k => k.Rute)
         .FirstOrDefault(k => k.IdKontrolnaTocka == id);
     ```
   - Primjer za PosjetController:
     ```csharp
     var posjet = _dbContext.Posjeti
         .Include(p => p.Korisnik)
         .Include(p => p.KontrolnaTocka)
         .Include(p => p.Ruta)
         .Include(p => p.Fotografije)
         .FirstOrDefault(p => p.IdPosjet == id);
     ```
   - Primjer za KorisnikController:
     ```csharp
     var korisnik = _dbContext.Korisnici
         .Include(k => k.Knjizica)
         .Include(k => k.Posjeti)
         .Include(k => k.KorisnikMedalje).ThenInclude(km => km.Medalja)
         .FirstOrDefault(k => k.IdKorisnik == id);
     ```
   - Primjer za PodrucjeController:
     ```csharp
     var podrucje = _dbContext.Podrucja
         .Include(p => p.KontrolneTocke)
         .Include(p => p.PlaninarskiObjekti)
         .FirstOrDefault(p => p.IdPodrucje == id);
     ```

5. Dodaj using Microsoft.EntityFrameworkCore; u svaki controller (za Include)

6. NE MIJENJAJ view datoteke — samo controller logiku

7. U Program.cs UKLONI registracije mock repozitorija (builder.Services.AddSingleton<MockRepo>) ako postoje. DbContext je već registriran iz koraka 3.

8. HomeController — ako koristi statistike (broj KT, broj ruta, itd.), zamijeni s:
   ```csharp
   ViewBag.BrojKT = _dbContext.KontrolneTocke.Count();
   ViewBag.BrojRuta = _dbContext.Rute.Count();
   ViewBag.BrojPodrucja = _dbContext.Podrucja.Count();
   // itd.
   ```
   Ili koristi odgovarajući ViewModel.

Primijeni ove promjene na SVE kontrolere u projektu.
```

---

# KORAK 6 — Custom routing (barem 4 akcije)

## Kontekst fileovi za agenta
Dodaj u kontekst:
- `Program.cs`
- **Sve Controller klase** (nakon koraka 5)

## Prompt za Copilot agenta (edit mode)

```
Dodaj custom routing na barem 4 akcije kontrolera koristeći [Route] atribute.
Zadrži i postojeći default routing u Program.cs da stari linkovi nastave raditi.

Implementiraj sljedeće custom rute:

### Ruta 1: Kontrolna točka po hrvatskom URL-u
Na KontrolnaTockaController dodaj:
- [Route("vrh/{id:int}")] na Details akciju
- To znači da URL /vrh/3 prikazuje detalje kontrolne točke s ID-jem 3
- Zadrži i standardni /KontrolnaTocka/Details/3 da radi

### Ruta 2: Područje s kontrolnim točkama
Na PodrucjeController dodaj NOVU akciju:
```csharp
[Route("podrucje/{id:int}/tocke")]
public IActionResult KontrolneTockePodrucja(int id)
{
    var tocke = _dbContext.KontrolneTocke
        .Where(kt => kt.IdPodrucje == id)
        .ToList();
    ViewBag.Podrucje = _dbContext.Podrucja.Find(id);
    return View(tocke);
}
```
Kreiraj i odgovarajući View: Views/Podrucje/KontrolneTockePodrucja.cshtml
koji prikazuje listu kontrolnih točaka za to područje (koristi isti stil kartica kao Index).
URL: /podrucje/5/tocke prikazuje KT iz Samoborskog gorja.

### Ruta 3: Korisnik profil
Na KorisnikController dodaj:
- [Route("planinar/{id:int}")] na Details akciju
- URL: /planinar/1 prikazuje profil korisnika

### Ruta 4: Rute filtrirane po težini
Na RutaController dodaj NOVU akciju:
```csharp
[Route("rute/tezina/{tezina}")]
public IActionResult PoTezini(string tezina)
{
    if (Enum.TryParse<TezinaRute>(tezina, true, out var tezinaEnum))
    {
        var filtrirane = _dbContext.Rute
            .Include(r => r.KontrolnaTocka)
            .Where(r => r.TezinaRute == tezinaEnum)
            .ToList();
        ViewBag.Tezina = tezina;
        return View(filtrirane);
    }
    return NotFound();
}
```
Kreiraj View: Views/Ruta/PoTezini.cshtml
URL: /rute/tezina/Laka, /rute/tezina/Srednja, /rute/tezina/Teska

### Ruta 5 (bonus): Naslovnica na hrvatski URL
Na HomeController dodaj:
- [Route("naslovnica")] na Index akciju (uz [Route("/")] i [Route("")])
- URL: /naslovnica prikazuje home page

Za svaku novu akciju koja ima View, kreiraj odgovarajući .cshtml u ispravnom folderu.
Koristi isti vizualni stil kao postojeći viewovi.
```

---

# KORAK 7 — semantic-model.md

## Kontekst fileovi za agenta
Dodaj u kontekst:
- `finalni_model_planinarska_aplikacija_ispravljeno.md`
- `PlaninarstvoDbContext.cs`
- Sve entitetne klase

## Prompt za Copilot agenta (edit mode)

```
Kreiraj datoteku semantic-model.md u root folderu projekta (na istoj razini kao .csproj ili .sln).

Sadržaj treba biti sažeti pregled baze podataka. Format:

# Semantički model baze podataka — Planinarska aplikacija

## Pregled tablica

| Tablica | Opis | PK |
|---------|------|----|
| Korisnik | Registrirani korisnik aplikacije | IdKorisnik |
| Knjizica | Digitalna planinarska knjižica korisnika | IdKnjizica |
| ... (sve tablice) |

## Detalji po tablicama

### Korisnik
| Atribut | Tip | Ograničenja |
|---------|-----|-------------|
| IdKorisnik | int | PK, AI |
| Ime | string(100) | Required |
| Email | string(150) | Required, Unique |
| ... |

Relacije:
- Korisnik 1:1 Knjizica (FK: Knjizica.IdKorisnik)
- Korisnik 1:N Posjet (FK: Posjet.IdKorisnik)
- Korisnik 1:N KorisnikMedalja (FK: KorisnikMedalja.IdKorisnik)

(ponovi za SVE tablice)

### Knjizica
...

### Posjet
...

(itd. za svih 11 tablica)

## Enum tipovi

### DozivljajPosjeta
VrloLagano, Lagano, Srednje, Zahtjevno, VrloZahtjevno, KratkoAliTesko, DugoAliLagano, FizickiNaporno, TehnickiZahtjevno

### TipKontrolneTocke
Vrh, Vidikovac, KontrolnaTocka

(itd. za sve enume)

## Dijagram relacija (tekstualni)

Korisnik 1──1 Knjizica
Korisnik 1──N Posjet
Knjizica 1──N Posjet
KontrolnaTocka 1──N Posjet
Ruta 1──N Posjet
Posjet 1──N Fotografija
Podrucje 1──N KontrolnaTocka
KontrolnaTocka 1──N Ruta
Podrucje 1──N PlaninarskiObjekt
PlaninarskaUdruga 1──N PlaninarskiObjekt
Korisnik N──N Medalja (preko KorisnikMedalja)

Napiši kompletni semantic-model.md sa svim tablicama, svim atributima i svim relacijama.
```

---

# KORAK 8 — sitemap.md

## Kontekst fileovi za agenta
Dodaj u kontekst:
- `Program.cs`
- **Sve Controller klase** (nakon koraka 6 — s custom rutama)
- Folder strukturu Views/ direktorija

## Prompt za Copilot agenta (edit mode)

```
Kreiraj datoteku sitemap.md u root folderu projekta (na istoj razini kao semantic-model.md).

Sadržaj treba opisivati SVE dostupne URL-ove u aplikaciji.
Aplikacija radi na http://localhost:5041.

Format:

# Sitemap — Planinarska aplikacija

## Pregled svih URL-ova

| URL | HTTP | Controller | Akcija | View | Routing tip |
|-----|------|------------|--------|------|-------------|
| / | GET | HomeController | Index | Views/Home/Index.cshtml | Default |
| /Home/Index | GET | HomeController | Index | Views/Home/Index.cshtml | Default |
| /naslovnica | GET | HomeController | Index | Views/Home/Index.cshtml | Custom [Route] |
| /KontrolnaTocka | GET | KontrolnaTockaController | Index | Views/KontrolnaTocka/Index.cshtml | Default |
| /KontrolnaTocka/Details/3 | GET | KontrolnaTockaController | Details | Views/KontrolnaTocka/Details.cshtml | Default |
| /vrh/3 | GET | KontrolnaTockaController | Details | Views/KontrolnaTocka/Details.cshtml | Custom [Route] |
| /Ruta | GET | RutaController | Index | Views/Ruta/Index.cshtml | Default |
| /Ruta/Details/1 | GET | RutaController | Details | Views/Ruta/Details.cshtml | Default |
| /rute/tezina/Laka | GET | RutaController | PoTezini | Views/Ruta/PoTezini.cshtml | Custom [Route] |
| /Podrucje | GET | PodrucjeController | Index | Views/Podrucje/Index.cshtml | Default |
| /Podrucje/Details/5 | GET | PodrucjeController | Details | Views/Podrucje/Details.cshtml | Default |
| /podrucje/5/tocke | GET | PodrucjeController | KontrolneTockePodrucja | Views/Podrucje/KontrolneTockePodrucja.cshtml | Custom [Route] |
| /Korisnik | GET | KorisnikController | Index | Views/Korisnik/Index.cshtml | Default |
| /Korisnik/Details/1 | GET | KorisnikController | Details | Views/Korisnik/Details.cshtml | Default |
| /planinar/1 | GET | KorisnikController | Details | Views/Korisnik/Details.cshtml | Custom [Route] |
... (nastavi za SVE kontrolere i SVE akcije)

Pregledaj SVE kontrolere i SVE akcije u projektu i upiši SVE URL-ove.
Uključi i default rute i custom rute.
Za svaku akciju napiši koji View se renderira.

## Custom rute — pregled

| Custom URL | Opis |
|------------|------|
| /vrh/{id} | Prikazuje detalje kontrolne točke po hrvatskom URL-u |
| /podrucje/{id}/tocke | Prikazuje kontrolne točke unutar određenog područja |
| /planinar/{id} | Prikazuje profil korisnika |
| /rute/tezina/{tezina} | Filtrira rute po težini |
| /naslovnica | Početna stranica aplikacije |

## Routing konfiguracija

Default ruta u Program.cs:
pattern: "{controller=Home}/{action=Index}/{id?}"

Custom rute su definirane pomoću [Route] atributa na kontrolerima i akcijama.
```

---

# KORAK 9 — SKILL.md za VS Code Copilot

## Kontekst fileovi za agenta
Nema — ovo piše agent od nule.

## Prompt za Copilot agenta (edit mode)

```
Kreiraj VS Code Copilot Agent skill za Entity Framework.

Kreiraj folder strukturu: .github/copilot-skills/ef-skill/
Unutra kreiraj datoteku SKILL.md sa sljedećim sadržajem:

# Entity Framework Skill — Planinarska aplikacija

## Kada koristiti ovaj skill
Koristi ovaj skill kad:
- Trebaš dodati novi entitet/tablicu u bazu podataka
- Trebaš dodati novo svojstvo u postojeći entitet
- Trebaš promijeniti ili dodati relaciju između entiteta
- Trebaš generirati EF migraciju
- Trebaš dodati seed podatke
- Trebaš napisati EF upit s Include/Where/Select

## Pravila projekta

### Struktura projekta
- DbContext klasa: PlaninarstvoDbContext
- Baza podataka: MySQL (Pomelo.EntityFrameworkCore.MySql)
- Connection string ključ: "PlaninarstvoDbContext" u appsettings.json

### Konvencije imenovanja
- Primarni ključ: Id + NazivEntiteta (npr. IdKorisnik, IdPosjet)
- Foreign key: Id + NazivReferenciraneKlase (npr. IdPodrucje u KontrolnaTocka)
- Navigacijsko svojstvo: ime klase (npr. public virtual Podrucje Podrucje { get; set; })
- Kolekcija: množina imena klase (npr. public virtual ICollection<Posjet> Posjeti { get; set; })

### Anotacije
- [Key] na svaki primarni ključ
- [ForeignKey("NavigacijskoSvojstvo")] na svaki FK int
- [Required] na not-null string svojstva
- [MaxLength(X)] na string svojstva s ograničenom duljinom
- Unique constrainti se definiraju u OnModelCreating s HasIndex().IsUnique()

### Navigacijska svojstva
- "1" strana relacije: virtual ICollection<T>
- "N" strana relacije: virtual T (+ int FK svojstvo s [ForeignKey])
- 1:1 relacija: virtual T na obje strane, UNIQUE na FK

### Enum tipovi u projektu
- DozivljajPosjeta: VrloLagano, Lagano, Srednje, Zahtjevno, VrloZahtjevno, KratkoAliTesko, DugoAliLagano, FizickiNaporno, TehnickiZahtjevno
- TipKontrolneTocke: Vrh, Vidikovac, KontrolnaTocka
- TipObjekta: Dom, Kuca, Skloniste
- TezinaRute: Laka, Srednja, Teska
- TipSlike: Selfie, Oznaka, Krajolik, Mapa, Drugo

### Migracije
Nakon svake promjene modela pokreni:
```
dotnet ef migrations add <OpisPromjene>
dotnet ef database update
```

### Seed podaci
Seed podaci se dodaju u OnModelCreating metodom HasData().
Ne koristiti navigacijska svojstva u seed podacima — samo primitivne tipove.

## Primjer: dodavanje novog entiteta

### 1. Kreiraj klasu
```csharp
public class NoviEntitet
{
    [Key]
    public int IdNoviEntitet { get; set; }

    [Required]
    [MaxLength(100)]
    public string Naziv { get; set; }

    [ForeignKey("Podrucje")]
    public int IdPodrucje { get; set; }
    public virtual Podrucje Podrucje { get; set; }
}
```

### 2. Dodaj DbSet u PlaninarstvoDbContext
```csharp
public DbSet<NoviEntitet> NoviEntiteti { get; set; }
```

### 3. Dodaj ICollection na povezanu klasu (ako 1:N)
```csharp
// U klasi Podrucje:
public virtual ICollection<NoviEntitet> NoviEntiteti { get; set; }
```

### 4. Dodaj seed podatke u OnModelCreating
```csharp
modelBuilder.Entity<NoviEntitet>().HasData(
    new NoviEntitet { IdNoviEntitet = 1, Naziv = "Primjer", IdPodrucje = 1 }
);
```

### 5. Generiraj i primijeni migraciju
```
dotnet ef migrations add DodanNoviEntitet
dotnet ef database update
```

## Primjer: EF upit s Include
```csharp
var posjet = _dbContext.Posjeti
    .Include(p => p.Korisnik)
    .Include(p => p.KontrolnaTocka)
        .ThenInclude(kt => kt.Podrucje)
    .Include(p => p.Ruta)
    .Include(p => p.Fotografije)
    .FirstOrDefault(p => p.IdPosjet == id);
```

## Postojeće tablice u bazi
Korisnik, Knjizica, Posjet, Fotografija, KontrolnaTocka, Ruta, Podrucje, PlaninarskiObjekt, PlaninarskaUdruga, Medalja, KorisnikMedalja
```

Dodatno, registriraj skill u VS Code konfiguraciji.
Ako projekt koristi .github/copilot-instructions.md, dodaj referencu na skill.
Ako koristi .vscode/settings.json za Copilot skills, konfiguriraj tamo.
Provjeri VS Code Copilot dokumentaciju: https://code.visualstudio.com/docs/copilot/customization/agent-skills
```

---

# NAKON SVIH KORAKA — Git commit

```
git add .
git commit -m "Lab 3: EF konfiguracija, MySQL, custom routing, semantic model, sitemap, EF skill"
git push
```

---

# PROVJERA — jesam li sve napravio?

| Zahtjev | Korak | ✅ |
|---------|-------|---|
| EF anotacije na modelu ([Key], [ForeignKey], virtual) | 2 | ☐ |
| ICollection<> za 1:N veze | 2 | ☐ |
| Baza podataka instalirana (MySQL/XAMPP) | 0 | ☐ |
| Connection string u appsettings.json | 3 | ☐ |
| DbContext klasa s DbSet<> | 3 | ☐ |
| DI registracija u Program.cs | 3 | ☐ |
| Inicijalna migracija generirana | 4 | ☐ |
| Seed podaci u OnModelCreating | 4 | ☐ |
| Mock repozitoriji zamijenjeni EF-om | 5 | ☐ |
| Aplikacija radi s bazom podataka | 5 | ☐ |
| Barem 4 custom rute ([Route] atributi) | 6 | ☐ |
| semantic-model.md kreiran | 7 | ☐ |
| sitemap.md kreiran | 8 | ☐ |
| SKILL.md kreiran | 9 | ☐ |
| Sve commitano na Git | - | ☐ |

---

# ZA USMENO — Što moraš znati objasniti

## EF koncepti:
1. Što je DbContext? → Klasa koja predstavlja sesiju s bazom, prati promjene
2. Što je DbSet? → Kolekcija koja predstavlja tablicu u bazi
3. Što radi [Key]? → Označava primarni ključ
4. Što radi [ForeignKey]? → Povezuje FK int s navigacijskim svojstvom
5. Zašto virtual? → Omogućava lazy loading
6. Što radi Include()? → Eager loading — dohvaća povezane podatke odmah
7. Što je migracija? → Skripta koja usklađuje model i bazu
8. Što radi SaveChanges()? → Sprema sve promjene u bazu (commit)
9. Što je seed data? → Inicijalni podaci koji se unesu pri kreiranju baze

## Routing koncepti:
1. Kako default ruta parsira URL? → {controller}/{action}/{id?}
2. Što je [Route] atribut? → Definira custom URL za akciju
3. Što znači {id:int}? → Route constraint — id mora biti integer
4. Razlika convention vs attribute routing? → Convention je u Program.cs, attribute je na kontroleru/akciji
5. Mogu li oba postojati istovremeno? → Da, attribute ima prioritet
