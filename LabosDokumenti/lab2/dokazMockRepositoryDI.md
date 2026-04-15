# Lab 2 - Dokaz za Mock Repository + DI

## Sto je trazeno u vjezbi
U Lab 2 je trazeno da se podaci ne citaju direktno iz baze, nego iz mock repository sloja sa statickim podacima (Lab 1 dataset), te da se repository ovisnosti ubrizgavaju kroz Dependency Injection.

To pokriva dvije stvari:
1. Controller ne zna odakle podaci dolaze (samo trazi podatke).
2. Kasnije se mock sloj moze zamijeniti pravim (npr. EF Core) bez velikih promjena u controllerima.

## Sto je napravljeno u projektu
Implementiran je puni mock repository sloj i DI registracija.

### 1) Repository sloj
Datoteka:
- `Repositories/MockRepositories.cs`

Sadrzi:
- `ILab1DataStore` + `Lab1DataStore` (centralni izvor statickih podataka iz `Lab1PodaciFactory.Kreiraj()`)
- zasebne repository interface + implementacije za glavne entitete

Primjer uzorka koji se koristi svugdje:
- `GetAll()` vraca sve zapise za Index
- `GetById(int id)` vraca jedan zapis za Details

## 2) Dependency Injection registracija
Datoteka:
- `Program.cs`

Dodane su `AddSingleton` registracije za:
- data store
- sve mock repository klase

Time ASP.NET automatski ubrizgava repository objekte u controllere.

## 3) Controlleri prebaceni na constructor injection
Controlleri vise ne zovu direktno `Lab1PodaciFactory.Kreiraj()`.

Umjesto toga:
- primaju repository kroz konstruktor
- koriste repository metode u `Index` i `Details` akcijama

Primjeri controllera koji su prebaceni:
- `Controllers/KorisnikController.cs`
- `Controllers/PosjetController.cs`
- `Controllers/PodrucjeController.cs`
- `Controllers/RutaController.cs`
- `Controllers/MedaljaController.cs`
- `Controllers/KnjizicaController.cs`
- `Controllers/FotografijaController.cs`
- `Controllers/KorisnikMedaljaController.cs`
- `Controllers/PlaninarskiObjektController.cs`
- `Controllers/PlaninarskaUdrugaController.cs`
- `Controllers/KontrolnaTockaController.cs`

## Kako taj dio radi (jednostavno objasnjenje)
Tijek jednog zahtjeva, npr. `Korisnik/Index`:
1. Routing pogodi `KorisnikController.Index`.
2. ASP.NET prvo napravi controller i ubrizga mu repo objekte iz DI containera.
3. Controller pozove `GetAll()` nad repositoryjem.
4. Podaci se mapiraju u ViewModel.
5. View renderira listu.

Korist:
- cistiji controller kod
- lakse testiranje
- jasna separacija odgovornosti
- laksa zamjena mock sloja pravim persistence slojem

## Provjera ispravnosti
Nakon refaktora je pokrenut build:
- `dotnet build`
- rezultat: `Build succeeded`

To potvrduje da je mock repository + DI sloj tehnicki ispravno povezan i spreman za Lab 2 evaluaciju.

## Sto reci na usmenom
Kratka verzija:
- "Uveli smo repository sloj da controller ne ovisi o izvoru podataka."
- "Mock podaci dolaze iz Lab 1, ali se citaju kroz repozitorije."
- "Repositorye registriramo u Program.cs i ubrizgavamo kroz konstruktor controllera."
- "Zato je kod odrziv i lako prebaciv na pravu bazu kasnije."
