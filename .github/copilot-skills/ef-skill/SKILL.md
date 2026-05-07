# Entity Framework Skill — Planinarska aplikacija

## Opis skilla
Ovaj skill definira pravila za rad s Entity Framework Core u projektu planinarske aplikacije.
Koristi se kad treba dodati, izmijeniti ili obrisati entitete, svojstva, relacije, migracije ili seed podatke.

## Kada se aktivira
- Dodavanje novog entiteta/tablice
- Dodavanje ili izmjena svojstva u postojećem entitetu
- Dodavanje ili izmjena relacije (1:1, 1:N, N:N)
- Generiranje EF migracije
- Dodavanje seed podataka
- Pisanje EF upita (LINQ s Include, Where, Select...)

## Tehnički stack
- Framework: .NET + Entity Framework Core 9.x
- Baza: MySQL (Pomelo.EntityFrameworkCore.MySql 9.0.0)
- DbContext klasa: PlaninarstvoDbContext
- Connection string ključ: "PlaninarstvoDbContext" (u appsettings.json)

## Konvencije imenovanja

### Primarni ključevi
Format: Id + NazivEntiteta (npr. IdKorisnik, IdPosjet, IdKontrolnaTocka)
Anotacija: [Key]

### Foreign key svojstva
Format: Id + NazivReferenciraneKlase (npr. IdKorisnik u Posjet, IdPodrucje u KontrolnaTocka)
Anotacija: [ForeignKey("NazivNavigacijskogSvojstva")]
Uvijek dodaj i int FK svojstvo i virtual navigacijsko svojstvo.

### Navigacijska svojstva
- "1" strana (roditelj): public virtual ICollection<DijeteKlasa> ImeKolekcije { get; set; }
- "N" strana (dijete): public virtual RoditeljKlasa ImeSvojstva { get; set; }

### String svojstva
- [Required] za not-null
- [MaxLength(X)] prema specifikaciji

## Enum tipovi u projektu
- DozivljajPosjeta: VrloLagano, Lagano, Srednje, Zahtjevno, VrloZahtjevno, KratkoAliTesko, DugoAliLagano, FizickiNaporno, TehnickiZahtjevno
- TipKontrolneTocke: Vrh, Vidikovac, KontrolnaTocka
- TipObjekta: Dom, Kuca, Skloniste
- TezinaRute: Laka, Srednja, Teska
- TipSlike: Selfie, Oznaka, Krajolik, Mapa, Drugo

## Workflow: dodavanje novog entiteta

### Korak 1 — Kreiraj klasu

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

### Korak 2 — Dodaj DbSet u PlaninarstvoDbContext

```csharp
public DbSet<NoviEntitet> NoviEntiteti { get; set; }
```

### Korak 3 — Dodaj ICollection na povezanu klasu (ako 1:N)

```csharp
// U klasi Podrucje dodaj:
public virtual ICollection<NoviEntitet> NoviEntiteti { get; set; }
```

### Korak 4 — Unique ili posebna pravila u OnModelCreating

```csharp
modelBuilder.Entity<NoviEntitet>()
    .HasIndex(e => e.Naziv)
    .IsUnique();
```

### Korak 5 — Seed podaci (opcionalno)

```csharp
modelBuilder.Entity<NoviEntitet>().HasData(
    new NoviEntitet { IdNoviEntitet = 1, Naziv = "Primjer", IdPodrucje = 1 }
);
```

NAPOMENA: u HasData nikad ne stavljaj navigacijska svojstva — samo primitivne tipove.

### Korak 6 — Migracija

```
dotnet ef migrations add DodanNoviEntitet
dotnet ef database update
```

## Workflow: EF upit u kontroleru

### Dohvat liste s uključenim relacijama

```csharp
var posjeti = _dbContext.Posjeti
    .Include(p => p.Korisnik)
    .Include(p => p.KontrolnaTocka)
    .Include(p => p.Ruta)
    .OrderByDescending(p => p.DatumVrijemePosjeta)
    .ToList();
```

### Dohvat jednog zapisa s duboko ugniježđenim relacijama

```csharp
var posjet = _dbContext.Posjeti
    .Include(p => p.Korisnik)
    .Include(p => p.KontrolnaTocka)
        .ThenInclude(kt => kt.Podrucje)
    .Include(p => p.Ruta)
    .Include(p => p.Fotografije)
    .FirstOrDefault(p => p.IdPosjet == id);
```

### Filtriranje s Where

```csharp
var lakeRute = _dbContext.Rute
    .Include(r => r.KontrolnaTocka)
    .Where(r => r.TezinaRute == TezinaRute.Laka)
    .ToList();
```

## Postojeće tablice u bazi
Korisnik, Knjizica, Posjet, Fotografija, KontrolnaTocka, Ruta, Podrucje, PlaninarskiObjekt, PlaninarskaUdruga, Medalja, KorisnikMedalja

## Česte greške koje treba izbjegavati
1. Ne zaboravi [ForeignKey] na FK int svojstvo
2. Ne stavljaj navigacijska svojstva u HasData seed
3. Uvijek pokreni migraciju nakon promjene modela
4. Koristi Include() u kontroleru za podatke koji se prikazuju u View-u
5. Za decimal svojstva definiraj HasPrecision() u OnModelCreating
