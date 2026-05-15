# ZADATAK: Autocomplete dropdown + Validacija — Planinarska aplikacija (ASP.NET MVC .NET 8)

## KONTEKST

CRUD implementacija za sve entitete je gotova. Svaka forma trenutno koristi obični `<select>`
za FK polja i već ima Data Annotation anotacije na ViewModelima.

Ovaj zadatak nadograđuje postojeće forme na dva načina:
1. Zamjenjuje `<select>` dropdownove za FK polja s custom AJAX autocomplete kontrolom
2. Osigurava da client-side validacija ispravno okida na `blur` i da su poruke vizualno uklopljene

---

## DIO 1 — AUTOCOMPLETE DROPDOWN

### Što treba napraviti

Napraviti **jednu reusable komponentu** kao Partial View koja se koristi na svim formama
gdje je FK dropdown (odabir povezanog entiteta). Kontrola mora:

- Prikazivati tekstni input u koji korisnik tipka
- AJAX pozivom na server dohvaćati odgovarajuće rezultate dok korisnik tipka (debounce 300ms)
- Prikazivati rezultate kao dropdown listu ispod inputa
- Odabirom rezultata: popuniti vidljivi input s nazivom i hidden input s ID-jem
- Na Edit formi: prikazivati već odabranu vrijednost (naziv + ID iz baze)
- Zatvoriti listu kad korisnik klikne izvan kontrole

---

### Partial View — `Views/Shared/_AutocompleteDropdown.cshtml`

Napravi sljedeći partial view koji prima parametre i može se reusati na svim formama:

```cshtml
@model object

@* Parametri koji se prosljeđuju kroz ViewData / Html.Partial *@
@{
    var fieldName    = ViewData["FieldName"]?.ToString();     // npr. "IdKontrolnaTocka"
    var displayName  = ViewData["DisplayName"]?.ToString();   // npr. "Naziv kontrolne točke"
    var searchUrl    = ViewData["SearchUrl"]?.ToString();     // npr. "/KontrolnaTocka/AutocompleteSearch"
    var currentId    = ViewData["CurrentId"]?.ToString();     // npr. "5" (za Edit formu)
    var currentText  = ViewData["CurrentText"]?.ToString();   // npr. "Okić" (za Edit formu)
    var placeholder  = ViewData["Placeholder"]?.ToString() ?? "Pretraži...";
}

<div class="autocomplete-wrapper position-relative" data-field="@fieldName">
    <input type="hidden"
           name="@fieldName"
           id="hidden_@fieldName"
           value="@currentId" />

    <input type="text"
           id="display_@fieldName"
           class="form-control autocomplete-input"
           placeholder="@placeholder"
           value="@currentText"
           autocomplete="off" />

    <ul class="autocomplete-list list-unstyled position-absolute w-100 shadow-sm d-none"
        id="list_@fieldName"></ul>
</div>

<span class="text-danger small field-validation-error" id="val_@fieldName"></span>
```

---

### CSS — dodaj u `wwwroot/css/site.css`

```css
/* Autocomplete dropdown */
.autocomplete-wrapper {
    z-index: 100;
}

.autocomplete-list {
    background: var(--bs-body-bg);
    border: 1px solid rgba(0,0,0,.15);
    border-radius: 6px;
    max-height: 220px;
    overflow-y: auto;
    top: 100%;
    z-index: 200;
}

.autocomplete-list li {
    padding: 8px 14px;
    cursor: pointer;
    font-size: 0.9rem;
    border-bottom: 1px solid rgba(0,0,0,.06);
    transition: background 0.12s;
}

.autocomplete-list li:last-child {
    border-bottom: none;
}

.autocomplete-list li:hover,
.autocomplete-list li.active {
    background: #e8f5e9;
    color: #1b4332;
}

.autocomplete-list li.no-results {
    color: #888;
    cursor: default;
    font-style: italic;
}
```

---

### JavaScript — dodaj u `wwwroot/js/autocomplete.js`

Napravi zasebnu JS datoteku za autocomplete logiku. Uključi je u `_Layout.cshtml`
nakon jquery, prije `@await RenderSectionAsync("Scripts")`.

```javascript
(function () {
    'use strict';

    function initAutocomplete(wrapper) {
        const fieldName   = wrapper.dataset.field;
        const hiddenInput = document.getElementById('hidden_' + fieldName);
        const textInput   = document.getElementById('display_' + fieldName);
        const list        = document.getElementById('list_' + fieldName);
        const searchUrl   = wrapper.dataset.searchUrl;
        const valSpan     = document.getElementById('val_' + fieldName);

        let debounceTimer = null;
        let currentRequest = null;

        // Tipkanje — AJAX pretraga s debouncingom
        textInput.addEventListener('input', function () {
            const term = this.value.trim();

            // Ako korisnik briše tekst, resetiraj hidden value
            if (term.length === 0) {
                hiddenInput.value = '';
                hideList();
                return;
            }

            if (term.length < 2) return;

            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(function () {
                if (currentRequest) currentRequest.abort();

                currentRequest = $.ajax({
                    url: searchUrl,
                    data: { term: term },
                    success: function (results) {
                        renderList(results);
                    }
                });
            }, 300);
        });

        // Blur validacija — okini kad korisnik napusti polje
        textInput.addEventListener('blur', function () {
            setTimeout(function () {
                // Ako je hidden value prazan ali input nije, znači nije odabrana vrijednost
                if (textInput.value.trim() !== '' && hiddenInput.value === '') {
                    hiddenInput.value = '';
                    textInput.value = '';
                }
                validateField();
                hideList();
            }, 200);
        });

        function validateField() {
            if (!valSpan) return;
            if (hiddenInput.required && hiddenInput.value === '') {
                valSpan.textContent = 'Ovo polje je obavezno.';
            } else {
                valSpan.textContent = '';
            }
        }

        function renderList(results) {
            list.innerHTML = '';

            if (!results || results.length === 0) {
                const li = document.createElement('li');
                li.className = 'no-results';
                li.textContent = 'Nema rezultata.';
                list.appendChild(li);
                showList();
                return;
            }

            results.forEach(function (item) {
                const li = document.createElement('li');
                li.textContent = item.label;
                li.dataset.id = item.value;

                li.addEventListener('mousedown', function (e) {
                    e.preventDefault(); // spriječi blur prije mouseup
                    selectItem(item.value, item.label);
                });

                list.appendChild(li);
            });

            showList();
        }

        function selectItem(id, label) {
            hiddenInput.value = id;
            textInput.value   = label;
            if (valSpan) valSpan.textContent = '';
            hideList();
        }

        function showList() { list.classList.remove('d-none'); }
        function hideList() { list.classList.add('d-none'); }

        // Klik izvan — zatvori listu
        document.addEventListener('click', function (e) {
            if (!wrapper.contains(e.target)) hideList();
        });
    }

    // Inicijaliziraj sve autocomplete wrappere na stranici
    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.autocomplete-wrapper[data-search-url]').forEach(initAutocomplete);
    });
})();
```

---

### Uključivanje JS datoteke u `_Layout.cshtml`

Dodaj **nakon** jquery i bootstrap, **prije** `@await RenderSectionAsync("Scripts")`:

```html
<script src="~/js/autocomplete.js" asp-append-version="true"></script>
```

---

### Server-side Search Endpointovi — po jedan za svaki FK entitet

Svaki endpoint vraća `JsonResult` s listom `{ value, label }` objekata.
Vraćaj maksimalno **15 rezultata**. Filtriraj soft-deleted zapise.

Primjer za `KontrolnaTockaController`:

```csharp
[HttpGet]
public IActionResult AutocompleteSearch(string term)
{
    var results = _context.KontrolneTocke
        .Where(x => x.DeletedAt == null &&
               x.Naziv.Contains(term))
        .OrderBy(x => x.Naziv)
        .Take(15)
        .Select(x => new {
            value = x.IdKontrolnaTocka,
            label = x.Naziv
        })
        .ToList();

    return Json(results);
}
```

Napravi `AutocompleteSearch` endpoint u sljedećim controllerima:

| Controller | Traži po polju | Label prikazuje |
|---|---|---|
| `KontrolnaTockaController` | `Naziv` | `Naziv` |
| `RutaController` | `Naziv` | `Naziv + " (" + Pocetak + " → " + Kraj + ")"` |
| `KorisnikController` | `Ime`, `Prezime`, `KorisnickoIme` | `Ime + " " + Prezime + " (@" + KorisnickoIme + ")"` |
| `KnjizicaController` | `IdKnjizica` | `"Knjižica #" + IdKnjizica + " – " + Korisnik.Ime` (include Korisnik) |
| `PodrucjeController` | `Naziv` | `Naziv` |
| `PlaninarskaUdrugaController` | `Naziv` | `Naziv` |
| `PosjetController` | traži po `IdPosjet` | `"Posjet #" + IdPosjet + " – " + KontrolnaTocka.Naziv` (include KT) |
| `MedaljaController` | `Naziv` | `Naziv` |

---

### Korištenje autocomplete kontrole na formama

Svaki obični `<select>` za FK polja zamijeni s pozivom partial viewa.

**Staro (obrisati):**
```html
<select asp-for="IdKontrolnaTocka" asp-items="ViewBag.KontrolneTocke" class="form-select">
    <option value="">-- Odaberi --</option>
</select>
```

**Novo:**
```html
@await Html.PartialAsync("_AutocompleteDropdown", null, new ViewDataDictionary(ViewData) {
    { "FieldName",   "IdKontrolnaTocka" },
    { "DisplayName", "Kontrolna točka" },
    { "SearchUrl",   Url.Action("AutocompleteSearch", "KontrolnaTocka") },
    { "CurrentId",   Model?.IdKontrolnaTocka.ToString() },
    { "CurrentText", Model?.KontrolnaTocka?.Naziv }
})
```

> Na **Create** formi `CurrentId` i `CurrentText` su `null` — kontrola se ponaša kao prazan input.
> Na **Edit** formi `CurrentId` i `CurrentText` dolaze iz baze — kontrola prikazuje postojeću vrijednost.

---

### Koje forme trebaju autocomplete (zamijeni `<select>` → autocomplete)

| Forma | FK polje | Endpoint |
|---|---|---|
| `Posjet/Create` i `Edit` | `IdKontrolnaTocka` | `KontrolnaTocka/AutocompleteSearch` |
| `Posjet/Create` i `Edit` | `IdRuta` | `Ruta/AutocompleteSearch` |
| `Posjet/Create` i `Edit` | `IdKorisnik` | `Korisnik/AutocompleteSearch` |
| `Posjet/Create` i `Edit` | `IdKnjizica` | `Knjizica/AutocompleteSearch` |
| `KontrolnaTocka/Create` i `Edit` | `IdPodrucje` | `Podrucje/AutocompleteSearch` |
| `Ruta/Create` i `Edit` | `IdKontrolnaTocka` | `KontrolnaTocka/AutocompleteSearch` |
| `PlaninarskiObjekt/Create` i `Edit` | `IdPodrucje` | `Podrucje/AutocompleteSearch` |
| `PlaninarskiObjekt/Create` i `Edit` | `IdPlaninarskaUdruga` | `PlaninarskaUdruga/AutocompleteSearch` |
| `Knjizica/Create` i `Edit` | `IdKorisnik` | `Korisnik/AutocompleteSearch` |
| `Fotografija/Create` i `Edit` | `IdPosjet` | `Posjet/AutocompleteSearch` |
| `KorisnikMedalja/Create` i `Edit` | `IdKorisnik` | `Korisnik/AutocompleteSearch` |
| `KorisnikMedalja/Create` i `Edit` | `IdMedalja` | `Medalja/AutocompleteSearch` |

> Enum polja (`TipKontrolneTocke`, `DozivljajPosjeta`, `TezinaRute`, itd.) ostaju kao obični
> `<select asp-items="Html.GetEnumSelectList<...>">` — autocomplete se koristi samo za FK polja.

---

### Ukloni ViewBag SelectList iz controllera

Nakon zamjene `<select>` → autocomplete, ukloni iz controller akcija `Create GET`,
`Create POST`, `Edit GET`, `Edit POST` sve `ViewBag.XYZ = new SelectList(...)` linije
koje su služile FK dropdownovima. Enum dropdownovi ne koriste ViewBag, pa ih ne diraj.

---

## DIO 2 — VALIDACIJA

### Provjera anotacija na ViewModelima

Provjeri da svaki ViewModel u `Models/ViewModels/` ima ispravne anotacije.
Ako neka nedostaju, dodaj ih prema ovim pravilima:

**Obavezna polja** — svako `NN` polje iz modela baze:
```csharp
[Required(ErrorMessage = "Naziv je obavezan.")]
```

**Duljina stringa** — prema VARCHAR duljini iz modela:
```csharp
[StringLength(150, MinimumLength = 2, ErrorMessage = "Naziv mora imati između 2 i 150 znakova.")]
```

**Numerički raspon:**
```csharp
[Range(1, 9999, ErrorMessage = "Vrijednost mora biti između 1 i 9999.")]
```

**Email format:**
```csharp
[EmailAddress(ErrorMessage = "Unesite ispravnu e-mail adresu.")]
```

**OIB (PlaninarskaUdruga):**
```csharp
[StringLength(11, MinimumLength = 11, ErrorMessage = "OIB mora imati točno 11 znakova.")]
[RegularExpression(@"^\d{11}$", ErrorMessage = "OIB smije sadržavati samo znamenke.")]
```

**Decimal polje (DuljinaKm na Ruta):**
```csharp
[Range(0.1, 999.99, ErrorMessage = "Duljina mora biti između 0.1 i 999.99 km.")]
```

---

### Provjera `asp-validation-for` spanova u svim formama

Na **svakom** polju u svim Create i Edit viewovima mora postojati validation span:

```html
<div class="mb-3">
    <label asp-for="Naziv" class="form-label fw-semibold"></label>
    <input asp-for="Naziv" class="form-control" />
    <span asp-validation-for="Naziv" class="text-danger small"></span>
</div>
```

Provjeri sve Create.cshtml i Edit.cshtml datoteke i dodaj `<span asp-validation-for="...">` svugdje gdje nedostaje, uključujući enum `<select>` polja i datumska polja.

---

### Client-side validacija — okida na blur

Provjeri da je u `_Layout.cshtml` uključen `_ValidationScriptsPartial`:

```html
<partial name="_ValidationScriptsPartial" />
```

Taj partial uključuje `jquery.validate` i `jquery.validate.unobtrusive` koji automatski
okidaju validaciju na `blur` (kad korisnik napusti polje).

Ako `_ValidationScriptsPartial` već postoji u layoutu, nema potrebe za dodatnim kodom.
Ako ne postoji, dodaj ga **unutar** `<body>` **nakon** jquery i bootstrap skripti.

---

### Server-side validacija — provjera u svim POST akcijama

Provjeri da svaka `Create POST` i `Edit POST` akcija u svim controllerima ima provjeru:

```csharp
if (!ModelState.IsValid)
{
    // Za forme s autocomplete: ViewBag ne treba puniti (nema <select>)
    // Za forme s enum <select>: nema ViewBag-a, enum SelectList radi automatski
    return View(model);
}
```

Ako POST akcija vraća `View(model)` bez `ModelState.IsValid` provjere — dodaj je.

---

### Validacijska poruka za autocomplete polja — server side

Autocomplete kontrola koristi `hidden input` čija se vrijednost šalje na server.
Da bi server-side validacija radila za FK polja, ViewModel mora imati:

```csharp
[Required(ErrorMessage = "Kontrolna točka je obavezna.")]
public int IdKontrolnaTocka { get; set; }
```

Ako je FK polje nullable (opcionalno), koristi `int?` bez `[Required]`.

---

### Vizualni stil validacijskih poruka — dodaj u `site.css`

```css
/* Validacijske poruke */
.field-validation-error,
.text-danger.small {
    font-size: 0.82rem;
    margin-top: 3px;
    display: block;
}

.input-validation-error {
    border-color: #dc3545 !important;
    box-shadow: 0 0 0 0.15rem rgba(220, 53, 69, 0.15) !important;
}

.field-validation-valid {
    display: none;
}
```

---

## REDOSLIJED IMPLEMENTACIJE

| Korak | Što raditi |
|---|---|
| 1 | Napravi `Views/Shared/_AutocompleteDropdown.cshtml` partial view |
| 2 | Napravi `wwwroot/js/autocomplete.js` s inicijalizacijom i AJAX logikom |
| 3 | Dodaj CSS stilove autocomplete liste u `wwwroot/css/site.css` |
| 4 | Uključi `autocomplete.js` u `_Layout.cshtml` |
| 5 | Dodaj `AutocompleteSearch` endpoint u `KontrolnaTockaController` |
| 6 | Zamijeni `<select>` → autocomplete na `Posjet/Create` i `Posjet/Edit` za IdKontrolnaTocka |
| 7 | **Testiraj** — provjeri tipkanje, odabir, Edit formu (prepopulira), blur validaciju |
| 8 | Dodaj `AutocompleteSearch` u sve ostale controllere iz tablice iznad |
| 9 | Zamijeni sve ostale `<select>` FK dropdownove → autocomplete prema tablici iznad |
| 10 | Ukloni ViewBag SelectList linije iz svih controller akcija |
| 11 | Provjeri `asp-validation-for` spanove u svim formama — dodaj gdje nedostaju |
| 12 | Provjeri `ModelState.IsValid` u svim POST akcijama — dodaj gdje nedostaje |
| 13 | Dodaj validacijske CSS stilove u `site.css` |
| 14 | Provjeri da `_ValidationScriptsPartial` je u `_Layout.cshtml` |

---

## ŠTO NE IMPLEMENTIRAŠ U OVOM KORAKU

- ❌ Custom date picker — dolazi u sljedećem koraku
- ❌ JS animacije (toast, fade, spinner) — dolaze u sljedećem koraku
- ✅ Enum `<select>` ostaju kao obični `<select asp-items="Html.GetEnumSelectList<...>">` — ne mijenjaj ih
