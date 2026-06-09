# Checklist — Faza 3 (Web API + DTO)

> Prolazi ovu listu da potvrdiš da je Faza 3 stvarno gotova.
> Aplikaciju pokreni s `dotnet run` (profil `https` ili `http`), Development okruženje.
> Swagger UI: **http://localhost:5041/swagger** (ili `https://localhost:7187/swagger`).

---

## A) Što je implementirano (pregled)

- [x] Swagger / OpenAPI uključen **samo u Development** (`/swagger`), filtriran na `api/` rute
- [x] `Controllers/Api/ApiBaseController.cs` — zajednički helperi za vlasništvo (`GetCurrentKorisnikAsync`, `IsOwnerOrAdminAsync`)
- [x] **11 API kontrolera** u `Controllers/Api/` — svaki s GET (lista) / GET{id} / POST / PUT / DELETE
- [x] **DTO klase** u `Models/Dto/<Entitet>/` za svih 11 entiteta (Read / Create / Update)
- [x] Privatne `ToDto(...)` metode za mapiranje (bez AutoMappera)
- [x] `app.MapControllers()` dodan u `Program.cs` (atributno rutiranje `api/...`)

---

## B) Build i pokretanje

- [ ] `dotnet build` prolazi bez **grešaka** (postojećih 8 warninga je iz starijih labova, ne iz API koda)
- [ ] Aplikacija se pokreće (`dotnet run`) bez rušenja na startu
- [ ] `/swagger` se otvara i prikazuje svih 11 API grupa
- [ ] `/swagger/v1/swagger.json` vraća **HTTP 200** (ne 500)
- [ ] Postojeći Lab1–4 MVC ekrani (Posjet, KontrolnaTocka, …) **i dalje rade** kao prije

> ℹ️ Tijekom izrade provjereno automatski: build 0 grešaka, swagger.json = 200, svih 11 `/api/*` ruta prisutno.

---

## C) Endpointi po entitetu

Za svaki kontroler postoji 5 osnovnih metoda + query parametri:

- [ ] `api/posjet` — query: `korisnikId, kontrolnaTockaId, datumOd, datumDo, dozivljaj`
- [ ] `api/kontrolnatocka` — query: `podrucjeId, tip, naziv`
- [ ] `api/ruta` — query: `tezina, kontrolnaTockaId, naziv`
- [ ] `api/podrucje` — query: `naziv`
- [ ] `api/planinarskiobjekt` — query: `podrucjeId, udrugaId, tip, naziv`
- [ ] `api/planinarskaudruga` — query: `naziv, grad`
- [ ] `api/medalja` — query: `naziv`
- [ ] `api/korisnik` — query: `query` (ime/prezime/korisničko ime)
- [ ] `api/knjizica` — (vlasnik vidi svoje, Admin sve)
- [ ] `api/korisnikmedalja` — query: `korisnikId, medaljaId`
- [ ] `api/fotografija` — query: `posjetId, tip`

---

## D) Status kodovi (testirati u Swaggeru)

- [ ] `GET /api/posjet` → **200** + lista nije prazna (ima seed podataka)
- [ ] `GET /api/posjet/1` → **200**
- [ ] `GET /api/posjet/99999` → **404**
- [ ] `POST /api/podrucje` **bez prijave** → **401** *(automatski provjereno: vraća 401)*
- [ ] `POST /api/podrucje` prijavljen kao **Planinar** (ne Admin) → **403**
- [ ] `POST /api/podrucje` prijavljen kao **Admin** s validnim tijelom → **201** (+ `Location` header)
- [ ] `POST /api/podrucje` s nevaljanim tijelom (npr. prazan `naziv`) → **400** + poruke na hrvatskom
- [ ] `PUT /api/podrucje/{id}` Admin → **200**; nepostojeći id → **404**
- [ ] `DELETE /api/podrucje/{id}` Admin → **204**

---

## E) Autorizacija i vlasništvo

- [ ] Javni GET-ovi rade **anonimno** (Posjet, KontrolnaTocka, Ruta, Podrucje, Objekt, Udruga, Medalja, KorisnikMedalja, Fotografija, Korisnik)
- [ ] `GET /api/knjizica` **bez prijave** → **401** *(automatski provjereno)*
- [ ] `POST /api/posjet` kao Planinar kreira posjet, a `IdKorisnik` se uzima iz **prijavljenog korisnika** (ne iz tijela zahtjeva)
- [ ] `PUT /api/posjet/{tuđi}` kao Planinar koji **nije vlasnik** → **403**
- [ ] `DELETE /api/posjet/{vlastiti}` kao vlasnik → **204** (soft delete — `DeletedAt`, zapis ostaje u bazi)
- [ ] Admin može uređivati/brisati tuđe posjete

---

## F) DTO i sigurnost podataka (R3)

- [ ] Nijedan endpoint **ne izlaže EF entitet direktno** — uvijek ide kroz DTO
- [ ] `GET /api/korisnik` **anoniman** vraća samo: `idKorisnik, ime, prezime, korisnickoIme, profilnaSlika, datumRegistracije` *(automatski provjereno — nema email/OIB/JMBG)*
- [ ] `GET /api/korisnik` kao **Admin** dodatno vraća `email, brojMobitela, datumRodenja, statusAktivan`
- [ ] **Nigdje** se ne izlaže `OIB` korisnika, `JMBG`, `AppUserId` ni `PasswordHash`
      *(napomena: `OIB` planinarske **udruge** je javni poslovni podatak i namjerno je izložen)*
- [ ] Create/Update DTO sadrže samo primitivne tipove + FK ID-eve (bez ugniježđenih objekata)
- [ ] Read DTO denormalizirano prikazuje nazive vezanih entiteta (npr. `nazivKontrolneTocke`, `imePrezimeKorisnika`)

---

## G) Soft delete u API-ju (R8)

- [ ] Soft-deletani zapisi (`DeletedAt != null`) **ne pojavljuju se** ni u jednoj GET listi (globalni query filter aktivan)
- [ ] DELETE na entitetima s `DeletedAt` postavlja `DeletedAt` (ne briše fizički)
- [ ] DELETE na `Korisnik` / `Knjizica` (nemaju `DeletedAt`) postavlja `StatusAktivan/StatusAktivna = false`

---

## H) Konvencije (CLAUDE.md)

- [ ] API kontroleri su u `Controllers/Api/`, naziv `XyzApiController`, `[ApiController]`, `[Route("api/xyz")]`
- [ ] Hrvatski nazivi entiteta/polja zadržani; `ErrorMessage` validacija na hrvatskom
- [ ] Standardni status kodovi 200/201/204/400/401/403/404

---

## Brzi ručni test kroz Swagger (preporučeni redoslijed)

1. Otvori `/swagger` → vidi 11 grupa.
2. `GET /api/podrucje` → 200, lista 20 područja.
3. Bez prijave probaj `POST /api/podrucje` → 401.
4. Prijavi se u aplikaciji kao **admin@planinarenje.hr** (lozinka iz `IdentitySeed`), pa ponovo `POST /api/podrucje` s `{ "naziv": "Test", "minimalanBrojKTZaObilazak": 1 }` → 201.
5. `GET /api/korisnik` kao anoniman → bez `email`; kao admin → s `email`.
6. `GET /api/posjet/99999` → 404.

> Kad su svi okviri u sekcijama B–H označeni — **Faza 3 je gotova.**
