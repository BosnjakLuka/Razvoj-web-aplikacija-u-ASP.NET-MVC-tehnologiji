# ZADATAK: Napredni JavaScript animacije — Planinarska aplikacija (ASP.NET MVC .NET 8)

## KONTEKST

CRUD, autocomplete dropdown i validacija su implementirani.
Ovaj zadatak dodaje animacije koje su u **službi aplikacije** — svaka animacija
mora imati svrhu i poboljšavati UX, ne biti čisto dekorativna.

Animacije se implementiraju globalno (za sve stranice) i lokalno (tamo gdje ima smisla).

---

## ANIMACIJE KOJE TREBA IMPLEMENTIRATI

### 1. Toast notifikacije — globalna zamjena za TempData alert

Trenutno se TempData["Success"] i TempData["Error"] prikazuju kao statični Bootstrap alert.
Zamijeni ih s animiranim toast notifikacijama u donjem desnom kutu ekrana.

**Dodaj u `_Layout.cshtml`** — container za toastove, odmah prije `</body>`:

```html
<div id="toast-container"
     style="position:fixed; bottom:1.5rem; right:1.5rem; z-index:9999; display:flex; flex-direction:column; gap:8px;">
</div>
```

**Dodaj u `wwwroot/js/site.js`** — funkcija za kreiranje toasta:

```javascript
function showToast(message, type) {
    type = type || 'success';
    const colors = {
        success: { bg: '#1b4332', icon: '✓' },
        error:   { bg: '#7f1d1d', icon: '✕' },
        info:    { bg: '#1e3a5f', icon: 'i' }
    };
    const c = colors[type] || colors.success;

    const toast = document.createElement('div');
    toast.style.cssText = [
        'background:' + c.bg,
        'color:#fff',
        'padding:12px 18px',
        'border-radius:8px',
        'font-size:0.9rem',
        'display:flex',
        'align-items:center',
        'gap:10px',
        'min-width:260px',
        'max-width:380px',
        'box-shadow:0 4px 16px rgba(0,0,0,0.25)',
        'opacity:0',
        'transform:translateX(40px)',
        'transition:opacity 0.25s ease, transform 0.25s ease',
        'cursor:pointer'
    ].join(';');

    toast.innerHTML = '<span style="font-weight:700;font-size:1rem">' + c.icon + '</span>' +
                      '<span>' + message + '</span>';

    document.getElementById('toast-container').appendChild(toast);

    // Animacija ulaska
    requestAnimationFrame(function () {
        requestAnimationFrame(function () {
            toast.style.opacity = '1';
            toast.style.transform = 'translateX(0)';
        });
    });

    // Klik za zatvaranje
    toast.addEventListener('click', function () { dismissToast(toast); });

    // Auto-dismiss nakon 4 sekunde
    setTimeout(function () { dismissToast(toast); }, 4000);
}

function dismissToast(toast) {
    toast.style.opacity = '0';
    toast.style.transform = 'translateX(40px)';
    setTimeout(function () {
        if (toast.parentNode) toast.parentNode.removeChild(toast);
    }, 280);
}
```

**Zamijeni TempData alert blok u `_Layout.cshtml`:**

Ukloni statični `@if (TempData["Success"] != null) { <div class="alert..."> }` blok.
Zamijeni ga s inline skriptom koji okida toast čim se stranica učita:

```html
@if (TempData["Success"] != null)
{
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            showToast('@Html.Raw(TempData["Success"])', 'success');
        });
    </script>
}
@if (TempData["Error"] != null)
{
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            showToast('@Html.Raw(TempData["Error"])', 'error');
        });
    </script>
}
```

---

### 2. AJAX pretraga — fade animacija na rezultatima

Svaka Index stranica ima AJAX pretragu. Trenutno se rezultati zamjenjuju bez animacije.
Nadogradi svaki AJAX search poziv s fade efektom.

Pattern koji se već koristi u Index viewovima:

```javascript
// STARO:
$('#resultsContainer').html(html);

// NOVO — zamijeni na svim Index stranicama:
$('#resultsContainer').fadeOut(120, function () {
    $(this).html(html).fadeIn(180);
});
```

Prođi kroz sve Index viewove (`Views/*/Index.cshtml`) i primijeni ovaj pattern svugdje
gdje postoji AJAX search callback.

Dodatno — dodaj loading indikator za vrijeme AJAX poziva:

```javascript
// Dodaj spinner element ispod search inputa u svakom Index viewu:
// <div id="searchSpinner" class="text-center py-2 d-none">
//     <div class="spinner-border spinner-border-sm text-success" role="status"></div>
// </div>

$('#searchInput').on('input', function () {
    const term = $(this).val();
    $('#searchSpinner').removeClass('d-none');

    $.ajax({
        url: searchUrl,
        data: { searchTerm: term },
        success: function (html) {
            $('#searchSpinner').addClass('d-none');
            $('#resultsContainer').fadeOut(120, function () {
                $(this).html(html).fadeIn(180);
            });
        },
        error: function () {
            $('#searchSpinner').addClass('d-none');
            showToast('Greška pri pretrazi. Pokušaj ponovo.', 'error');
        }
    });
});
```

---

### 3. Row highlight — animacija na novo dodanom zapisu

Nakon Create akcije, korisnik se preusmjerava na Index. Novi zapis treba biti vizualno
istaknut nekoliko sekundi da korisnik odmah vidi što je dodano.

**U controller Create POST akciji** — dodaj ID novog zapisa u TempData:

```csharp
await _context.SaveChangesAsync();
TempData["Success"] = "Zapis je uspješno dodan.";
TempData["NewId"] = noviEntitet.IdPodrucje;  // prilagodi naziv ID polja po entitetu
return RedirectToAction(nameof(Index));
```

**U svakom `_NazivEntitetaListPartial.cshtml`** — dodaj `data-id` atribut na svaki redak:

```html
<tr data-id="@item.IdPodrucje">
    ...
</tr>
```

**U svaki `Index.cshtml`** — dodaj JS koji highlighta novi redak:

```javascript
@if (TempData["NewId"] != null)
{
    <script>
    document.addEventListener('DOMContentLoaded', function () {
        const newId = @TempData["NewId"];
        const row = document.querySelector('tr[data-id="' + newId + '"]');
        if (row) {
            row.style.transition = 'background 0.4s ease';
            row.style.background = '#d1fae5';
            setTimeout(function () {
                row.style.background = '';
            }, 2500);
        }
    });
    </script>
}
```

---

### 4. Confirm animacija na Delete stranici

Trenutna Delete confirm stranica je statična. Dodaj vizualni efekt koji naglašava
destruktivnost akcije.

**U `Views/*/Delete.cshtml`** svim entitetima — dodaj shake animaciju na confirm gumb
i pulsiranje ikone upozorenja:

Dodaj u `site.css`:

```css
/* Delete confirm animacije */
@keyframes shake {
    0%, 100% { transform: translateX(0); }
    20%       { transform: translateX(-6px); }
    40%       { transform: translateX(6px); }
    60%       { transform: translateX(-4px); }
    80%       { transform: translateX(4px); }
}

@keyframes pulse-warning {
    0%, 100% { transform: scale(1);    opacity: 1; }
    50%       { transform: scale(1.08); opacity: 0.85; }
}

.delete-warning-icon {
    animation: pulse-warning 1.6s ease-in-out infinite;
    display: inline-block;
    font-size: 2.5rem;
    color: #dc3545;
}

.btn-delete-confirm:hover {
    animation: shake 0.4s ease;
}
```

**U svakom `Delete.cshtml`** zamijeni statični sadržaj s:

```html
<div class="text-center py-4">
    <div class="delete-warning-icon">⚠️</div>
    <h4 class="mt-3 mb-1">Brisanje zapisa</h4>
    <p class="text-muted">Jesi li siguran da želiš obrisati <strong>@Model.Naziv</strong>?</p>
    <p class="text-muted small">Ova akcija se ne može poništiti.</p>

    <form asp-action="Delete" method="post" class="mt-4 d-flex justify-content-center gap-3">
        <input type="hidden" asp-for="@(/* ID polje entiteta */)" />
        @Html.AntiForgeryToken()
        <button type="submit" class="btn btn-danger btn-delete-confirm px-4">
            Potvrdi brisanje
        </button>
        <a asp-action="Index" class="btn btn-outline-secondary px-4">Odustani</a>
    </form>
</div>
```

---

### 5. Smooth scroll na navigaciji

Dodaj u `site.js` — smooth scroll na sve interne ankore:

```javascript
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('a[href^="#"]').forEach(function (anchor) {
        anchor.addEventListener('click', function (e) {
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                e.preventDefault();
                target.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        });
    });
});
```

---

### 6. Navbar active state animacija

Dodaj podvlaku s animacijom na aktivan link u navigaciji. Dodaj u `site.css`:

```css
.navbar-nav .nav-link {
    position: relative;
}

.navbar-nav .nav-link::after {
    content: '';
    position: absolute;
    bottom: -2px;
    left: 50%;
    right: 50%;
    height: 2px;
    background: #52b788;
    transition: left 0.2s ease, right 0.2s ease;
}

.navbar-nav .nav-link.active::after,
.navbar-nav .nav-link:hover::after {
    left: 8px;
    right: 8px;
}
```

---

## REDOSLIJED IMPLEMENTACIJE

| Korak | Što raditi |
|---|---|
| 1 | Dodaj `showToast` i `dismissToast` funkcije u `site.js` |
| 2 | Dodaj toast container u `_Layout.cshtml`, zamijeni TempData alert blokove |
| 3 | Prođi sve Index viewove — dodaj `fadeOut/fadeIn` i `searchSpinner` na AJAX callback |
| 4 | Dodaj `data-id` na table row u svim List partial viewovima |
| 5 | Dodaj TempData["NewId"] u sve Create POST akcije |
| 6 | Dodaj row highlight JS na sve Index viewove |
| 7 | Dodaj CSS animacije za Delete stranicu u `site.css` |
| 8 | Primijeni animirani Delete layout na sve Delete.cshtml stranice |
| 9 | Dodaj smooth scroll i navbar animaciju u `site.js` / `site.css` |

## ŠTO NE IMPLEMENTIRAŠ U OVOM KORAKU

- ❌ Custom date picker — dolazi u sljedećem (zadnjem) koraku
- ✅ Ne dodavaj animacije koje nemaju svrhu (npr. rotiranje logotipa, bounce efekti na karticama)
