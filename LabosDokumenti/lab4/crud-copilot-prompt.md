# ZADATAK: Implementacija CRUD operacija — Planinarska aplikacija (ASP.NET MVC .NET 8)

## KONTEKST PROJEKTA

Radim na ASP.NET MVC .NET 8 web aplikaciji — digitalnoj planinarskoj knjižici.
Do sada su implementirani:

- EF Core s MSSQL bazom i svim entitetima
- Index i Details stranice za sve entitete (read-only, bez CRUD-a)
- Custom UX dizajn (outdoor/hiking tematika, tamno-zelena + tamno-plava paleta)
- Dependency Injection i DbContext konfiguracija u Program.cs

## ŠTO TREBA IMPLEMENTIRATI

Implementiraj kompletni CRUD (Create, Read, Edit, Delete) za sljedeće entitete:

```
Podrucje, KontrolnaTocka, Ruta, PlaninarskiObjekt, PlaninarskaUdruga,
Korisnik, Knjizica, Posjet, Fotografija, Medalja, KorisnikMedalja
```

---

## OBVEZNE ARHITEKTURNE ODLUKE — PRATI IH TOČNO

### 1. SOFT DELETE

Za entitete koji imaju logičko brisanje, **NE pozivaj `_context.Remove()`**.

- `Korisnik` i `Knjizica` već imaju bool `StatusAktivan` — kod brisanja postavi `StatusAktivan = false`
- Za ostale entitete dodaj nullable polje `DeletedAt` tipa `DateTime?` direktno u EF entitet klasu

U **svakom** dohvatu liste (Index, dropdownovi, search endpointovi) obavezno filtriraj:

```csharp
.Where(x => x.DeletedAt == null)   // za entitete s DeletedAt
.Where(x => x.StatusAktivan)       // za Korisnik i Knjizica
```

Nakon dodavanja `DeletedAt` polja, generiraj EF migraciju:

```
Add-Migration AddSoftDeleteFields
Update-Database
```

---

### 2. VIEWMODELI ZA FORME — obvezno, nikad direktno EF entitet na formi

Za svaki entitet napravi zasebne ViewModel klase u folderu `Models/ViewModels/`:

```
Models/ViewModels/
  PodrucjeViewModels.cs              → PodrucjeCreateModel, PodrucjeEditModel
  KontrolnaTockaViewModels.cs        → KontrolnaTockaCreateModel, KontrolnaTockaEditModel
  RutaViewModels.cs                  → RutaCreateModel, RutaEditModel
  PlaninarskiObjektViewModels.cs     → PlaninarskiObjektCreateModel, PlaninarskiObjektEditModel
  PlaninarskaUdrugaViewModels.cs     → PlaninarskaUdrugaCreateModel, PlaninarskaUdrugaEditModel
  KorisnikViewModels.cs              → KorisnikCreateModel, KorisnikEditModel
  KnjizicaViewModels.cs              → KnjizicaCreateModel, KnjizicaEditModel
  PosjetViewModels.cs                → PosjetCreateModel, PosjetEditModel
  FotografijaViewModels.cs           → FotografijaCreateModel, FotografijaEditModel
  MedaljaViewModels.cs               → MedaljaCreateModel, MedaljaEditModel
  KorisnikMedaljaViewModels.cs       → KorisnikMedaljaCreateModel, KorisnikMedaljaEditModel
```

ViewModel smije sadržavati **samo** polja koja korisnik smije vidjeti i mijenjati:

- `KorisnikEditModel` **NE smije** imati: `PasswordHash`, `DatumRegistracije`
- `PosjetCreateModel` **NE smije** imati: `DatumKreiranjaZapisa` (setira se automatski na serveru)

Svaki ViewModel mora imati Data Annotation validacije (vidi sekciju VALIDACIJA).

---

### 3. AJAX PRETRAGA NA SVAKOM INDEX-U

Na **svakoj** Index stranici mora biti search box koji radi bez page reload-a.

**Controller — dodaj Search akciju (primjer za Podrucje):**

```csharp
[HttpGet]
public IActionResult Search(string searchTerm)
{
    var results = _context.Podrucja
        .Where(x => x.DeletedAt == null &&
               (string.IsNullOrEmpty(searchTerm) ||
                x.Naziv.Contains(searchTerm) ||
                x.Regija.Contains(searchTerm)))
        .ToList();
    return PartialView("_PodrucjeListPartial", results);
}
```

**View — Search box i AJAX poziv:**

```html
<input type="text" id="searchInput" class="form-control" placeholder="Pretraži..." />
<div id="resultsContainer">
    @await Html.PartialAsync("_PodrucjeListPartial", Model)
</div>

@section Scripts {
<script>
    $('#searchInput').on('input', function () {
        const term = $(this).val();
        $.ajax({
            url: '@Url.Action("Search", "Podrucje")',
            data: { searchTerm: term },
            success: function (html) {
                $('#resultsContainer').fadeOut(150, function () {
                    $(this).html(html).fadeIn(200);
                });
            }
        });
    });
</script>
}
```

**Partial View** `_NazivEntitetaListPartial.cshtml` — sadrži samo `<table>` ili kartice s podacima. Svaki redak mora imati linkove: **Edit**, **Details**, **Delete**.

---

### 4. CONTROLLER PATTERN — Edit koristi TryUpdateModelAsync

```csharp
// GET: Edit
[ActionName("Edit")]
public async Task<IActionResult> EditGet(int id)
{
    var entitet = await _context.Podrucja.FindAsync(id);
    if (entitet == null) return NotFound();

    var model = new PodrucjeEditModel
    {
        Naziv = entitet.Naziv,
        Regija = entitet.Regija,
        Opis = entitet.Opis,
        MinimalanBrojKTZaObilazak = entitet.MinimalanBrojKTZaObilazak
    };
    return View(model);
}

// POST: Edit
[HttpPost, ActionName("Edit")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EditPost(int id, PodrucjeEditModel model)
{
    if (!ModelState.IsValid) return View(model);

    var entitet = await _context.Podrucja.FindAsync(id);
    if (entitet == null) return NotFound();

    // mapiranje — samo dopuštena polja
    entitet.Naziv = model.Naziv;
    entitet.Regija = model.Regija;
    entitet.Opis = model.Opis;
    entitet.MinimalanBrojKTZaObilazak = model.MinimalanBrojKTZaObilazak;

    await _context.SaveChangesAsync();
    TempData["Success"] = "Područje je uspješno ažurirano.";
    return RedirectToAction(nameof(Index));
}
```

---

### 5. DELETE — soft delete pattern

```csharp
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var entitet = await _context.Podrucja.FindAsync(id);
    if (entitet == null) return NotFound();

    entitet.DeletedAt = DateTime.UtcNow;
    await _context.SaveChangesAsync();
    TempData["Success"] = "Područje je uspješno obrisano.";
    return RedirectToAction(nameof(Index));
}
```

Za `Korisnik` i `Knjizica`:

```csharp
korisnik.StatusAktivan = false;
```

---

### 6. CREATE — automatska polja na serveru, ne na formi

Polja koja korisnik ne unosi postavi u controller:

```csharp
var noviPosjet = new Posjet
{
    DatumKreiranjaZapisa = DateTime.UtcNow,
    JeLiPotvrdenPosjet   = false,
    IdKorisnik           = model.IdKorisnik,
    // ostala polja iz ViewModela...
};
_context.Posjeti.Add(noviPosjet);
await _context.SaveChangesAsync();
TempData["Success"] = "Posjet je uspješno dodan.";
return RedirectToAction(nameof(Index));
```

---

## VALIDACIJA — na svakom ViewModelu

Primjer anotacija koje obavezno dodaj:

```csharp
public class PodrucjeCreateModel
{
    [Required(ErrorMessage = "Naziv je obavezan.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Naziv mora imati između 2 i 150 znakova.")]
    public string Naziv { get; set; }

    [StringLength(500)]
    public string? Opis { get; set; }

    [StringLength(150)]
    public string? Regija { get; set; }

    [Required(ErrorMessage = "Minimalan broj KT je obavezan.")]
    [Range(1, 100, ErrorMessage = "Mora biti između 1 i 100.")]
    public int MinimalanBrojKTZaObilazak { get; set; }
}
```

Na **svim** formama:

- Datumska polja ostavi kao `<input asp-for="..." class="form-control" />` — date picker dolazi u sljedećem koraku
- Enum polja renderaj kao `<select asp-for="..." asp-items="Html.GetEnumSelectList<NazivEnuma>()" class="form-select">`
- FK polja renderaj kao `<select>` s liste iz baze:

```csharp
// U controlleru — Create GET i Edit GET:
ViewBag.Podrucja = new SelectList(
    _context.Podrucja.Where(x => x.DeletedAt == null).OrderBy(x => x.Naziv),
    "IdPodrucje", "Naziv"
);
```

```html
<!-- U viewu: -->
<select asp-for="IdPodrucje" asp-items="ViewBag.Podrucja" class="form-select">
    <option value="">-- Odaberi područje --</option>
</select>
```

> Autocomplete dropdown za FK polja dolazi kao zasebni korak, za sada je obični `<select>` ispravan.

Na svakom polju u viewu obavezno dodaj validation span:

```html
<span asp-validation-for="Naziv" class="text-danger small"></span>
```

---

## STRUKTURA FOLDERA — VIEWS

```
Views/
  Podrucje/
    Index.cshtml                   ← lista s AJAX search boxom
    _PodrucjeListPartial.cshtml    ← samo tablica/kartice (za AJAX refresh)
    Details.cshtml                 ← već postoji, ne diraj
    Create.cshtml
    Edit.cshtml
    Delete.cshtml                  ← confirm stranica
  KontrolnaTocka/
    (isti pattern)
  Ruta/
    (isti pattern)
  PlaninarskiObjekt/
    (isti pattern)
  PlaninarskaUdruga/
    (isti pattern)
  Korisnik/
    (isti pattern)
  Knjizica/
    (isti pattern)
  Posjet/
    (isti pattern)
  Fotografija/
    (isti pattern)
  Medalja/
    (isti pattern)
  KorisnikMedalja/
    (isti pattern)
```

---

## ENTITETI I NJIHOVA SPECIFIČNOST

### Podrucje — POČNI S OVIM (najjednostavniji, bez FK-ova prema gore)

- Polja na formi: `Naziv`, `Opis`, `Regija`, `MinimalanBrojKTZaObilazak`
- Soft delete: dodaj `DeletedAt`
- FK na: ništa (root entitet)

---

### KontrolnaTocka

- Polja na formi: `Naziv`, `TipKontrolneTocke` (enum), `NadmorskaVisina`, `Opis`, `Koordinate`, `OpisZiga`, `GUIDOznaka`
- FK dropdown: `IdPodrucje` → SelectList Podrucja
- Soft delete: dodaj `DeletedAt`

---

### Ruta

- Polja na formi: `Naziv`, `Pocetak`, `Kraj`, `VrijemeHodaMin`, `DuljinaKm`, `VisinskaRazlikaM`, `Opis`, `OznakaNaTerenu`, `GodinaObnove`, `Napomena`, `TezinaRute` (enum), `GPXPath`
- FK dropdown: `IdKontrolnaTocka` → SelectList KontrolnihTocaka
- Soft delete: dodaj `DeletedAt`

---

### PlaninarskaUdruga

- Polja na formi: `OIB`, `Naziv`, `Email`, `BrojTelefona`, `Adresa`, `PostanskiBroj`, `Grad`, `Zupanija`, `BrojClanova`
- Soft delete: dodaj `DeletedAt`
- Validacija: `OIB` → `[StringLength(11, MinimumLength = 11, ErrorMessage = "OIB mora imati točno 11 znakova.")]`

---

### PlaninarskiObjekt

- Polja na formi: `Naziv`, `TipObjekta` (enum), `NadmorskaVisina`, `Kapacitet`, `Opis`, `ImeOdgovorneOsobe`, `Telefon`, `Email`, `Adresa`, `ImaNocenje`, `ImaHranu`, `RadnoVrijemeOpis`
- FK dropdownovi: `IdPodrucje`, `IdPlaninarskaUdruga`
- Soft delete: dodaj `DeletedAt`

---

### Korisnik

- Polja na Create formi: `Ime`, `Prezime`, `Email`, `KorisnickoIme`, `BrojMobitela`, `DatumRodenja`
- `PasswordHash` na Create: postavi placeholder `"ChangeMe123!"` direktno u controller, ne na formi
- Edit forma **NE smije** imati: `PasswordHash`, `DatumRegistracije`
- Soft delete: postavi `StatusAktivan = false` (polje već postoji)
- `DatumRegistracije` na Create: postavi `DateTime.UtcNow` u controlleru

---

### Knjizica

- Polja na formi: `Napomena`
- `DatumKreiranja`: automatski `DateTime.UtcNow` na Create
- Soft delete: postavi `StatusAktivna = false`
- FK dropdown: `IdKorisnik` → SelectList aktivnih Korisnika

---

### Posjet — najkompleksniji entitet

- Polja na Create formi: `IdKorisnik`, `IdKnjizica`, `IdKontrolnaTocka`, `IdRuta`, `DatumVrijemePosjeta`, `VrijemeUsponaMin`, `DozivljajPosjeta` (enum), `OpisIskustva`, `UneseniGUID`
- Automatski u controlleru: `DatumKreiranjaZapisa = DateTime.UtcNow`, `JeLiPotvrdenPosjet = false`
- FK dropdownovi: `IdKorisnik`, `IdKnjizica`, `IdKontrolnaTocka`, `IdRuta`
- Soft delete: dodaj `DeletedAt`

---

### Fotografija

- Polja na formi: `NazivDatoteke`, `PutanjaDatoteke`, `TipSlike` (enum), `Opis`
- Automatski: `DatumUploada = DateTime.UtcNow`
- FK dropdown: `IdPosjet` → SelectList Posjeta
- Soft delete: dodaj `DeletedAt`

---

### Medalja

- Polja na formi: `Naziv`, `Opis`, `MinimalanBrojKontrolnihTocaka`, `MinimalanBrojPodrucja`
- Soft delete: dodaj `DeletedAt`

---

### KorisnikMedalja

- Polja na formi: `IdKorisnik`, `IdMedalja`, `DatumDodjele`, `Napomena`
- FK dropdownovi: `IdKorisnik`, `IdMedalja`
- Soft delete: dodaj `DeletedAt`

---

## TempData FLASH PORUKE

Postavi `TempData["Success"]` u svakoj POST akciji i prikaži je na vrhu stranice ili u `_Layout.cshtml`:

```html
@if (TempData["Success"] != null)
{
    <div class="alert alert-success alert-dismissible fade show" role="alert">
        @TempData["Success"]
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </div>
}
```

---

## NAPOMENE O UX-U

- Zadrži postojeći outdoor/hiking dizajn — ne mijenjaj layout, navbar, boje
- Delete stranica treba biti confirm stranica (ne inline, ne modal):
  `"Jesi li siguran da želiš obrisati [naziv]?"` s gumbima **Potvrdi** i **Odustani**
- Na Index stranicama Edit i Delete gumbi neka budu uz svaki redak tablice:

```html
<a asp-action="Edit" asp-route-id="@item.IdPodrucje" class="btn btn-sm btn-outline-secondary">
    <i class="bi bi-pencil"></i>
</a>
<a asp-action="Delete" asp-route-id="@item.IdPodrucje" class="btn btn-sm btn-outline-danger">
    <i class="bi bi-trash"></i>
</a>
```

---

## REDOSLIJED IMPLEMENTACIJE

| Korak | Što raditi |
|-------|-----------|
| 1 | Dodaj `DeletedAt` svim relevantnim EF entitetima → `Add-Migration AddSoftDeleteFields` → `Update-Database` |
| 2 | Kreiraj sve ViewModel klase s validacijama u `Models/ViewModels/` |
| 3 | Implementiraj **Podrucje** CRUD potpuno (uključujući AJAX Search partial) |
| 4 | Testiraj Podrucje — provjeri Create, Edit, Delete (soft), AJAX search |
| 5 | Repliciraj isti pattern: `PlaninarskaUdruga` → `KontrolnaTocka` → `Ruta` → `PlaninarskiObjekt` → `Korisnik` → `Knjizica` → `Medalja` → `Posjet` → `Fotografija` → `KorisnikMedalja` |

---

## ŠTO NE IMPLEMENTIRAŠ U OVOM KORAKU

- ❌ AJAX autocomplete dropdown za FK polja — dolazi u sljedećem koraku
- ❌ Custom date picker — dolazi u sljedećem koraku
- ❌ JS animacije — dolaze u sljedećem koraku
- ✅ FK polja za sada ostaju kao obični `<select>` HTML elementi
