# List Page Skill — Planinarska aplikacija

## Opis skilla
Ovaj skill definira pravila za izradu "list" (index) stranica u projektu planinarske aplikacije. Koristi se kada treba napraviti preglednu stranicu s popisom entiteta.

## Kada se aktivira
- Kreiranje nove Index akcije i view-a
- Proširenje postojećeg popisa na list stranici
- Dodavanje filtriranja, sortiranja ili osnovnih akcija (detalji, edit)

## Konvencije
- Index akcija dohvaća podatke kroz EF i vraća listu u View
- Sortiranje se radi po smislenom polju (npr. Datum, Naziv)
- View koristi postojeći vizualni stil (app-card, badge, section-title)
- Svaka kartica ima link na Details akciju

## Workflow: izrada list stranice

Korak 1 — Dodaj Index akciju u controller
```csharp
public IActionResult Index()
{
    var model = _dbContext.Entiteti
        .OrderBy(e => e.Naziv)
        .ToList();

    return View(model);
}
```

Korak 2 — Kreiraj View: Views/Entitet/Index.cshtml
- Dodaj naslov i kratak opis
- Prikaži kartice ili tablicu
- Dodaj link na Details

Korak 3 — Dodaj navigacijski link u _Layout.cshtml (ako je nova cjelina)

## Česte greške koje treba izbjegavati
1. Ne vraćaj IQueryable u View
2. Ne zaboravi sortiranje
3. Ne preskači link na Details
4. Ne koristi drugačiji dizajn od postojećih list stranica
