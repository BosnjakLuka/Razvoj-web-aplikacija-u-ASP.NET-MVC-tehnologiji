# Edit Form Skill — Planinarska aplikacija

## Opis skilla
Ovaj skill definira pravila za izradu create/edit formi u projektu planinarske aplikacije.

## Kada se aktivira
- Dodavanje Create i Edit akcija u controller
- Kreiranje Create/Edit view-ova
- Uključivanje validacije i osnovnih form kontrola

## Konvencije
- GET akcija vraća View s modelom
- POST akcija provjerava ModelState i sprema promjene
- Koristi [ValidateAntiForgeryToken]
- View koristi postojeći vizualni stil (detail-panel, app-card)

## Workflow: izrada forme

Korak 1 — Dodaj Create akcije
```csharp
public IActionResult Create() => View();

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Create(Entitet model)
{
    if (!ModelState.IsValid) return View(model);
    _dbContext.Add(model);
    _dbContext.SaveChanges();
    return RedirectToAction(nameof(Index));
}
```

Korak 2 — Dodaj Edit akcije
```csharp
public IActionResult Edit(int id)
{
    var entity = _dbContext.Entiteti.Find(id);
    if (entity == null) return NotFound();
    return View(entity);
}

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Edit(int id, Entitet model)
{
    if (id != model.Id) return NotFound();
    if (!ModelState.IsValid) return View(model);
    _dbContext.Update(model);
    _dbContext.SaveChanges();
    return RedirectToAction(nameof(Index));
}
```

Korak 3 — Kreiraj Create/Edit view-ove
- Koristi asp-for, asp-validation-for
- Dodaj button "Spremi" i "Odustani"

## Česte greške koje treba izbjegavati
1. Ne zaboravi [ValidateAntiForgeryToken]
2. Ne preskači ModelState provjeru
3. Ne zaboravi hidden field za Id u Edit formi
4. Ne vraćaj prazan View bez modela u Edit GET
