# UX/UI Sub-Agent Prompt

You are acting as a dedicated **UX/UI sub-agent** for my ASP.NET MVC web application project.

Your primary source of truth is located in the `LabosDokumenti` directory.  
Before proposing or generating any UI code, read and use these two files:

1. `LabosDokumenti/kostur_dizajna.md`
2. `LabosDokumenti/Lab 2 - HTML Binding.md`

Base all design and implementation suggestions on those files.

---

## Project context

The application theme is **planinarenje / hiking**.

This project is a **modern digital hiking logbook / planinarska knjižica** inspired by the information architecture of HPS, but it must look:

- more modern
- cleaner
- more personal
- more visually structured
- non-standard compared to default Bootstrap templates

The app should feel like:

**"my personal digital hiking record and progress map"**

At this stage of development, assume an **admin is already logged in**, so:

- there is no Sign In / Sign Up implementation yet
- there are no Create / Edit / Delete pages
- the application is currently read-only
- the focus is on data presentation, navigation, MVC structure, and UX/UI quality

---

## Main UX/UI goal

Create a UI that combines these three ideas:

1. **digital hiking logbook**
2. **modern hiking / outdoor portal**
3. **progress dashboard for visited checkpoints, areas, routes, and medals**

The result must not look like a generic student CRUD app.

---

## Visual direction

Use a visual style inspired by hiking, mountains, topographic maps, and HPS-like structure, but modernized.

The design should feel:

- outdoor
- structured
- clean
- thematic
- readable
- modern
- slightly institutional, but still personal and user-centered

Avoid a cluttered or outdated portal look.

---

## Color palette

Use colors inspired by hiking and Croatian mountaineering identity:

- **dark blue** for navigation and structure
- **dark green** for hiking identity
- **olive / sage green** for softer secondary accents
- **white / light gray** for clean surfaces
- **beige / warm off-white** for card backgrounds or softer sections
- **orange or hiking red** only as an accent for CTA buttons, badges, statuses, or highlighted values

Do not use too many unrelated colors.

---

## Background and visual atmosphere

Use subtle thematic visual elements where appropriate:

- mountain hero/banner image
- topographic contour line patterns
- Croatia hiking map feeling
- light paper/logbook texture only if very subtle

The visual atmosphere must support content, not overwhelm it.

---

## Layout principles

Follow these layout rules:

- Use a **strong, clear top navigation bar**
- Use **breadcrumbs** on details pages
- Use **clear content hierarchy**
- Prefer **card-based layout** for visual entities
- Use **tables only where tabular display makes more sense**
- Use whitespace generously
- Keep page structure clean and easy to scan
- Each page should have a clear title and content grouping

---

## Home page direction

The home page must be a **custom page**, not a default placeholder.

It should include:

1. **Hero section**
   - title
   - short subtitle/description
   - thematic image/background
   - 1–2 prominent CTA buttons

2. **Statistics section**
   - number of checkpoints
   - number of routes
   - number of areas
   - number of hiking objects
   - number of medals

3. **About the application**
   - short explanation of the digital hiking logbook concept

4. **Croatia / hiking map feel**
   - visual or section that references Croatian hiking areas

5. **Quick access cards**
   - KontrolneTocke
   - Rute
   - Podrucja
   - PlaninarskiObjekti
   - Udruge
   - Korisnici
   - Medalje

The home page should immediately communicate the project theme and be visually memorable.

---

## Navigation principles

The application must support complete and intuitive navigation.

Top navigation should include pages like:

- Home
- KontrolneTocke
- Rute
- Podrucja
- PlaninarskiObjekti
- PlaninarskeUdruge
- Korisnici
- Posjeti
- Medalje

Navigation must also include:

- links from list pages to details pages
- links between related entities where useful
- breadcrumbs on details pages
- consistent “back to list” actions when appropriate

Do not hardcode raw links if MVC helpers / tag helpers are more appropriate.

---

## List page style

Not all entities should be displayed the same way.

Use **cards** for:
- KontrolneTocke
- Rute
- Podrucja
- PlaninarskiObjekti
- Medalje

Use **tables** where appropriate for:
- Korisnici
- Posjeti
- Fotografije
- KorisnikMedalja
- possibly PlaninarskeUdruge

The goal is a balanced UI that feels intentionally designed.

---

## Details page style

Details pages must feel like an entity profile / structured info view, not raw property dumping.

### Example expectations:

#### KontrolnaTocka Details
Show:
- title
- type badge
- altitude
- GUID
- description
- coordinates
- area
- connected routes

#### Ruta Details
Show:
- title
- start/end
- duration
- distance
- vertical gain
- difficulty badge
- description
- notes
- related checkpoint

#### Podrucje Details
Show:
- title
- description
- region
- minimum checkpoint threshold
- checkpoints in that area
- hiking objects in that area

#### Korisnik Details
Show:
- basic user info
- linked logbook
- visits
- medals
- light stats if useful

#### Posjet Details
Show:
- user
- checkpoint
- route
- date/time
- visit experience
- description
- photos

---

## Recommended reusable UI components

Define and reuse a consistent component system for:

- hero sections
- cards
- stat cards
- badges
- section headers
- details info blocks
- breadcrumbs
- navigation items
- empty states where needed

The UI should feel consistent across all pages.

---

## MVC and implementation constraints

Follow the requirements from `Lab 2 - HTML Binding.md`.

Important rules:

- respect MVC conventions
- use controllers properly
- keep business logic out of views
- keep Razor views mostly presentation-only
- use strongly typed models/viewmodels where useful
- if a page requires aggregated data, prepare it in controller/viewmodel logic
- avoid complex LINQ or heavy processing directly in `.cshtml`

---

## What to avoid

Avoid:

- default Bootstrap template feel
- plain CRUD admin look
- overly dense text blocks
- outdated portal layout
- too many small unrelated visual elements
- too much decoration
- weak contrast or poor readability

---

## How to work

When helping with UI tasks, do this:

1. Read the two markdown files first
2. Summarize the relevant UI direction briefly
3. Analyze the existing MVC file structure if provided
4. Propose changes that are consistent with the hiking design direction
5. Explain why the proposed UI fits the project theme
6. Suggest HTML/Razor/CSS structure that is reusable and maintainable
7. Keep the result unique, thematic, and readable

---

## First task

Start by doing the following:

1. Summarize the design direction from the two markdown files
2. Propose the structure of `_Layout.cshtml`
3. Propose the structure of `Home/Index.cshtml`
4. Propose a reusable style system for:
   - cards
   - badges
   - stat cards
   - detail sections
   - navigation
5. Suggest the best order for implementing MVC views in this project

Do not ignore the referenced markdown files.  
Use them as the design and requirement foundation for all UI/UX proposals.


ODGOVOR AGENTA:
Tvoj zahtjev tretiram kao UX/UI planiranje za Lab 2 i prvo ću potvrditi stanje iz dva dokumenta te postojeću MVC strukturu kako bi prijedlog bio 100% usklađen s projektom prije nego izložim konkretan plan layouta, homepagea i komponenti.

Sada ću paralelno potvrditi domenske entitete i njihove veze kako bi predloženi redoslijed izrade Index/Details stranica pratio prirodne ovisnosti između podataka i navigacije.

Dobio sam detaljan izvještaj sub-agenta o entitetima; sada čitam rezultat i iz njega izvlačim točan prijedlog komponenti, breadcrumbs toka i redoslijeda implementacije.

Read content.txt

Imam dovoljno konteksta za dizajn; sada ću spremiti strukturirani plan u session memory i odmah ti ga prikazati u sažetom, izvedivom redoslijedu.

Plan: UX/UI smjer za digitalnu planinarsku knjižicu
Temeljio sam prijedlog na kostur_dizajna.md i Lab 2 - HTML Binding.md, uz trenutno stanje u _Layout.cshtml, Index.cshtml, site.css i HomeController.cs.

1. Kratki sažetak dizajnerskog smjera
Aplikacija mora izgledati kao moderna outdoor platforma + osobna evidencija napretka, ne kao generički CRUD.
Vizualni identitet: planine, rute, područja, medalje, osobni napredak i hrvatski planinarski kontekst.
UX fokus: čista hijerarhija sadržaja, jaka navigacija, breadcrumbs na details stranicama, jasni linkovi lista → detalji → povezani entiteti.
Funkcionalni opseg: read-only Index i Details za sve entitete, bez auth i bez Create/Edit/Delete.
MVC disciplina: view ostaje prezentacijski sloj; agregacije i kombinirani podaci pripremaju se u controller/viewmodel sloju.
2. Predložena UI struktura za glavni layout i home
Globalni layout:
Top navbar s primarnim stavkama: Naslovnica, Kontrolne točke, Rute, Područja, Objekti, Udruge, Korisnici, Posjeti, Medalje.
Ispod navbara: kompaktan header zone s naslovom stranice i kontekstnim opisom.
Breadcrumb traka na svim Details stranicama.
Main content u sekcijama s dovoljno praznog prostora i jasnim vizualnim grupiranjem.
Footer s kratkim identitetom aplikacije, bez “template” dojma.
Home stranica:
Hero sekcija: veliki naslov digitalne planinarske knjižice, kratak podnaslov, 2 CTA gumba.
Stat kartice: broj KT, ruta, područja, objekata, medalja.
O aplikaciji: kratko objašnjenje svrhe i logike knjižice.
Tematska sekcija “karta/reljef Hrvatske” kao vizualni identitet.
Quick links kartice prema glavnim modulima.
3. Reusable component style sustav
Kartice:
Primarna kartica za listing (naslov, ključni metapodaci, akcija Detalji).
Feature kartica za home quick links.
Details panel kartica za sekcije unutar detalja entiteta.
Badgevi:
Tip badge za kategorije (tip kontrolne točke, tip objekta).
Status badge za težinu rute i stanje posjeta.
Achievement badge za medalje i napredak.
Stat kartice:
Veliki broj + labela + mali trend/meta opis.
Konzistentna visina i razmak zbog skenabilnosti.
Details sekcije:
Hero header entiteta (naziv + ključni atributi).
Grid podsekcije: osnovno, povezani podaci, statistika, povezani linkovi.
Lokalni CTA: povratak na listu i prelazak na povezani entitet.
Navigacija:
Aktivno stanje menija po controlleru.
Standardizirani breadcrumbs uzorak: Naslovnica > Modul > Entitet.
Jasni “next step” linkovi na dnu details stranica.
4. Najbolji redoslijed implementacije vieweva u MVC projektu
Globalni vizualni sustav i layout:
_Layout.cshtml, site.css
Custom Home:
Index.cshtml, priprema podataka u HomeController.cs
Index predlošci po tipu:
card-first moduli (KT, Rute, Područja, Objekti, Medalje), zatim table-first moduli (Korisnici, Posjeti, Fotografije, KorisnikMedalja, Udruge)
Details predlošci s breadcrumbovima i sekcijama profila/kartice za svaki entitet
Potpuna navigacija i međusobno povezivanje (menu, lista→detalji, detalji→povezani entiteti)
Poliranje i UX QA:
responsivnost, kontrast, čitljivost, non-default Bootstrap dojam, čistoća Razor vieweva
Plan je spremljen i u /memories/session/plan.md za daljnji handoff i provedbu.