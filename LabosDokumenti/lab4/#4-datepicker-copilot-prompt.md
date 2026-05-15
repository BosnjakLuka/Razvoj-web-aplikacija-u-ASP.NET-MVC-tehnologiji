# ZADATAK: Datumska kontrola kao Partial View — Planinarska aplikacija (ASP.NET MVC .NET 8)

## KONTEKST

CRUD, autocomplete, validacija i JS animacije su implementirani.
Ovaj zadatak implementira custom datumsku kontrolu (date + time picker) kao reusable Partial View
koja se primjenjuje na **sva mjesta u aplikaciji gdje postoji datum ili datetime polje**.

## ZAHTJEVI IZ VJEŽBE (obavezno poštivati)

- Partial View pristup — jedna komponenta, koristi se svugdje
- Koristiti **flatpickr** JS plugin — NE native `<input type="date">` ili `<input type="datetime-local">`
- Podržavati **hr format** (`dd.MM.yyyy HH:mm`) i **en-US format** (`MM/dd/yyyy HH:mm`)
  ovisno o postavkama preglednika (browser language)
- Ispravno prepopulirati vrijednost na **Edit** formi
- Ispravno slati vrijednost na server u formatu koji ASP.NET MVC može parsirati
- Raditi bez grešaka za nullable datumska polja

---

## KORAK 1 — Konfiguracija višejezičnosti u `Program.cs`

Dodaj lokalizaciju **prije** `app.MapControllerRoute`:

```csharp
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var supportedCultures = new[]
{
    new CultureInfo("hr"),
    new CultureInfo("en-US")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture    = new RequestCulture("hr"),
    SupportedCultures        = supportedCultures,
    SupportedUICultures      = supportedCultures
});

// app.MapControllerRoute(...) dolazi NAKON ovog bloka
```

---

## KORAK 2 — Uključi flatpickr u `_Layout.cshtml`

Dodaj u `<head>` (CSS):

```html
<link rel="stylesheet"
      href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" />
```

Dodaj u `<body>` nakon bootstrap JS, **prije** `autocomplete.js` i `site.js`:

```html
<script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
<script src="https://cdn.jsdelivr.net/npm/flatpickr/dist/l10n/hr.js"></script>
```

---

## KORAK 3 — Partial View `Views/Shared/_DateTimePicker.cshtml`

```cshtml
@{
    var fieldName    = ViewData["FieldName"]?.ToString();
    var displayLabel = ViewData["Label"]?.ToString() ?? fieldName;
    var currentValue = ViewData["Value"]?.ToString();
    var includeTime  = (bool)(ViewData["IncludeTime"] ?? true);
    var isRequired   = (bool)(ViewData["Required"] ?? false);
    var htmlClass    = ViewData["Class"]?.ToString() ?? "form-control";
}

<div class="mb-3 datepicker-wrapper">
    <label for="dp_@fieldName" class="form-label fw-semibold">@displayLabel</label>

    @* Vidljivi input koji flatpickr kontrolira *@
    <input type="text"
           id="dp_@fieldName"
           class="@htmlClass datepicker-input"
           placeholder="@(includeTime ? "dd.MM.yyyy HH:mm" : "dd.MM.yyyy")"
           autocomplete="off"
           @(isRequired ? "required" : "") />

    @* Hidden input koji se šalje na server — uvijek ISO format *@
    <input type="hidden"
           id="hidden_@fieldName"
           name="@fieldName"
           value="@currentValue" />

    <span class="text-danger small" id="val_@fieldName"></span>
</div>
```

---

## KORAK 4 — JavaScript inicijalizacija flatpickra

Dodaj u `wwwroot/js/site.js` (na kraju datoteke):

```javascript
// Datepicker inicijalizacija
(function () {
    'use strict';

    function getBrowserLocale() {
        var lang = (navigator.languages && navigator.languages[0]) ||
                   navigator.language || 'hr';
        return lang.toLowerCase().startsWith('en') ? 'en' : 'hr';
    }

    function initDatepickers() {
        document.querySelectorAll('.datepicker-input').forEach(function (input) {
            var wrapperId  = input.id;                                   // dp_FieldName
            var fieldName  = wrapperId.replace('dp_', '');
            var hiddenEl   = document.getElementById('hidden_' + fieldName);
            var valSpan    = document.getElementById('val_' + fieldName);
            var withTime   = input.placeholder.indexOf('HH:mm') !== -1;
            var locale     = getBrowserLocale();

            // Format za prikaz korisniku
            var displayFmt = locale === 'hr'
                ? (withTime ? 'd.m.Y H:i' : 'd.m.Y')
                : (withTime ? 'm/d/Y H:i' : 'm/d/Y');

            // Prepopulacija: ako hidden input ima vrijednost, parsiraj je i prikaži
            var defaultDate = null;
            if (hiddenEl && hiddenEl.value) {
                defaultDate = new Date(hiddenEl.value);
            }

            var fp = flatpickr(input, {
                locale:      locale === 'hr' ? 'hr' : 'default',
                enableTime:  withTime,
                dateFormat:  displayFmt,
                defaultDate: defaultDate,
                time_24hr:   true,

                onChange: function (selectedDates, dateStr) {
                    if (selectedDates.length > 0) {
                        // Spremi ISO string u hidden input za server
                        hiddenEl.value = selectedDates[0].toISOString();
                        if (valSpan) valSpan.textContent = '';
                    } else {
                        hiddenEl.value = '';
                    }
                },

                onClose: function () {
                    // Blur validacija
                    if (valSpan && input.hasAttribute('required') && !hiddenEl.value) {
                        valSpan.textContent = 'Datum je obavezan.';
                    }
                }
            });
        });
    }

    document.addEventListener('DOMContentLoaded', initDatepickers);
})();
```

---

## KORAK 5 — Korištenje partial viewa na formama

Svaki obični `<input asp-for="DatumXYZ">` zamijeni s pozivom partial viewa.

**Staro (obrisati):**
```html
<div class="mb-3">
    <label asp-for="DatumVrijemePosjeta" class="form-label fw-semibold"></label>
    <input asp-for="DatumVrijemePosjeta" class="form-control" />
    <span asp-validation-for="DatumVrijemePosjeta" class="text-danger small"></span>
</div>
```

**Novo — Create forma:**
```html
@await Html.PartialAsync("_DateTimePicker", null, new ViewDataDictionary(ViewData) {
    { "FieldName",   "DatumVrijemePosjeta" },
    { "Label",       "Datum i vrijeme posjeta" },
    { "IncludeTime", true },
    { "Required",    true }
})
```

**Novo — Edit forma (s postojećom vrijednošću):**
```html
@await Html.PartialAsync("_DateTimePicker", null, new ViewDataDictionary(ViewData) {
    { "FieldName",   "DatumVrijemePosjeta" },
    { "Label",       "Datum i vrijeme posjeta" },
    { "Value",       Model.DatumVrijemePosjeta.ToString("o") },
    { "IncludeTime", true },
    { "Required",    true }
})
```

> `ToString("o")` generira ISO 8601 format (`2026-04-15T10:30:00`) koji flatpickr
> i `new Date()` mogu pouzdano parsirati bez obzira na kulturu.

---

## KORAK 6 — Server-side parsiranje datuma

Na serveru, hiddenInput šalje ISO string. ASP.NET MVC ga automatski parsira u `DateTime`
ako je ViewModel tip `DateTime` ili `DateTime?`. Ne treba posebna konfiguracija.

Ako parsiranje ne radi, dodaj u `Program.cs` **ispred** `builder.Build()`:

```csharp
builder.Services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options => {
    options.ModelBindingMessageProvider
           .SetValueMustNotBeNullAccessor(_ => "Datum je obavezan.");
});
```

---

## KORAK 7 — Sva datumska polja koja trebaju picker

Primijeni `_DateTimePicker` partial na **sva** ova polja u Create i Edit formama:

| Entitet | Polje | IncludeTime | Required |
|---|---|---|---|
| `Posjet` | `DatumVrijemePosjeta` | `true` | `true` |
| `Posjet` | `DatumKreiranjaZapisa` | `true` | — automatski, ne na formi |
| `Korisnik` | `DatumRodenja` | `false` | `false` |
| `Korisnik` | `DatumRegistracije` | `true` | — automatski, ne na formi |
| `Knjizica` | `DatumKreiranja` | `true` | — automatski, ne na formi |
| `Fotografija` | `DatumUploada` | `true` | — automatski, ne na formi |
| `KorisnikMedalja` | `DatumDodjele` | `true` | `true` |

> Polja označena s "automatski, ne na formi" ne pojavljuju se u ViewModel formi —
> ne trebaju picker, server ih postavlja sam.

---

## KORAK 8 — ViewModel tipovi za datumska polja

Provjeri da datumska polja u ViewModelima imaju ispravan tip:

```csharp
// Obavezno datumsko polje
[Required(ErrorMessage = "Datum posjeta je obavezan.")]
public DateTime DatumVrijemePosjeta { get; set; }

// Opcionalno datumsko polje
public DateTime? DatumRodenja { get; set; }
```

---

## REDOSLIJED IMPLEMENTACIJE

| Korak | Što raditi |
|---|---|
| 1 | Dodaj `UseRequestLocalization` blok u `Program.cs` |
| 2 | Dodaj flatpickr CSS link u `<head>` layouta |
| 3 | Dodaj flatpickr JS i hr locale skriptu u `<body>` layouta |
| 4 | Napravi `Views/Shared/_DateTimePicker.cshtml` |
| 5 | Dodaj JS inicijalizaciju flatpickra na kraj `site.js` |
| 6 | Primijeni picker na `Posjet/Create` za `DatumVrijemePosjeta` |
| 7 | **Testiraj** — odabir datuma, Edit forma prepopulira, server prihvaća, hr i en format |
| 8 | Primijeni picker na `Posjet/Edit` za `DatumVrijemePosjeta` |
| 9 | Primijeni picker na `Korisnik/Create` i `Edit` za `DatumRodenja` |
| 10 | Primijeni picker na `KorisnikMedalja/Create` i `Edit` za `DatumDodjele` |

## ŠTO NE IMPLEMENTIRAŠ U OVOM KORAKU

- ❌ Ne koristi native `<input type="date">` ni `<input type="datetime-local">` nigdje
- ❌ Ne koristi jQuery UI Datepicker — koristi isključivo flatpickr
- ✅ Polja koja server postavlja automatski (`DatumKreiranjaZapisa`, `DatumUploada`, itd.)
     ne trebaju picker na formi — ostavi ih izvan forme
