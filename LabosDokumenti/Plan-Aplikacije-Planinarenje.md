# Planinarska aplikacija — finalni model baze podataka

## Kratki opis teme

Planinarska aplikacija zamišljena je kao digitalna zamjena za klasičnu planinarsku knjižicu u koju planinari skupljaju žigove kao dokaz da su obišli određene vrhove, vidikovce ili druge kontrolne točke.  
Umjesto fizičkog žiga i papirnate evidencije, korisnik u aplikaciji evidentira svoj posjet tako da unese posjet kontrolnoj točki, odabere rutu, upiše doživljaj posjeta te priloži fotografije kao dokaz obilaska.  
Aplikacija također omogućuje organizaciju podataka po područjima, pregled planinarskih objekata i udruga te sustav medalja koje korisnik može osvojiti ispunjavanjem određenih uvjeta.

---

# Finalni popis tablica, atributa i relacija

## 1. Korisnik

| Atribut | Tip | Oznaka | Objašnjenje |
|---|---|---|---|
| IdKorisnik | INT | PK, AI, NN | Jedinstveni identifikator korisnika |
| Ime | VARCHAR(100) | NN | Ime korisnika |
| Prezime | VARCHAR(100) | NN | Prezime korisnika |
| Email | VARCHAR(150) | NN, UNIQUE | E-mail adresa korisnika |
| KorisnickoIme | VARCHAR(100) | NN, UNIQUE | Korisničko ime za prijavu |
| PasswordHash | VARCHAR(255) | NN | Hash lozinke korisnika |
| DatumRodenja | DATE | NULL | Datum rođenja korisnika |
| DatumRegistracije | DATETIME | NN | Datum i vrijeme registracije |
| BrojMobitela | VARCHAR(30) | NULL | Kontakt broj mobitela |
| ProfilnaSlika | VARCHAR(255) | NULL | Putanja do profilne slike |
| StatusAktivan | BOOLEAN | NN | Označava je li korisnički račun aktivan |

---

## 2. Knjizica

| Atribut | Tip | Oznaka | Objašnjenje |
|---|---|---|---|
| IdKnjizica | INT | PK, AI, NN | Jedinstveni identifikator knjižice |
| IdKorisnik | INT | FK, NN, UNIQUE | Veza na korisnika; UNIQUE osigurava 1:1 vezu |
| DatumKreiranja | DATETIME | NN | Datum i vrijeme kreiranja knjižice |
| Napomena | TEXT | NULL | Dodatna napomena uz knjižicu |
| StatusAktivna | BOOLEAN | NN | Označava je li knjižica aktivna |

---

## 3. Posjet

| Atribut | Tip | Oznaka | Objašnjenje |
|---|---|---|---|
| IdPosjet | INT | PK, AI, NN | Jedinstveni identifikator posjeta |
| IdKorisnik | INT | FK, NN | Veza na korisnika koji je obavio posjet |
| IdKnjizica | INT | FK, NN | Veza na knjižicu u kojoj je zapis posjeta |
| IdKontrolnaTocka | INT | FK, NN | Veza na kontrolnu točku koja je obiđena |
| IdRuta | INT | FK, NN | Veza na rutu kojom je kontrolna točka obiđena |
| DatumVrijemePosjeta | DATETIME | NN | Datum i vrijeme obavljenog posjeta |
| VrijemeUsponaMin | INT | NULL | Vrijeme uspona izraženo u minutama |
| DozivljajPosjeta | ENUM | NN | Enum: VrloLagano, Lagano, Srednje, Zahtjevno, VrloZahtjevno, KratkoAliTesko, DugoAliLagano, FizickiNaporno, TehnickiZahtjevno |
| OpisIskustva | TEXT | NULL | Korisnikov opis i dojam s rute / uspona |
| UneseniGUID | VARCHAR(100) | NN | GUID oznake koji korisnik unosi kao potvrdu |
| JeLiPotvrdenPosjet | BOOLEAN | NN | Označava je li posjet uspješno potvrđen |
| DatumKreiranjaZapisa | DATETIME | NN | Datum i vrijeme kreiranja zapisa u sustavu |

---

## 4. Fotografija

| Atribut | Tip | Oznaka | Objašnjenje |
|---|---|---|---|
| IdFotografija | INT | PK, AI, NN | Jedinstveni identifikator fotografije |
| IdPosjet | INT | FK, NN | Veza na posjet kojem fotografija pripada |
| NazivDatoteke | VARCHAR(255) | NN | Naziv datoteke fotografije |
| PutanjaDatoteke | VARCHAR(255) | NN | Putanja do spremljene fotografije |
| DatumUploada | DATETIME | NN | Datum i vrijeme uploada |
| TipSlike | ENUM | NN | Enum: Selfie, Oznaka, Krajolik, Mapa, Drugo |
| Opis | TEXT | NULL | Dodatni opis fotografije |

---

## 5. KontrolnaTocka

| Atribut | Tip | Oznaka | Objašnjenje |
|---|---|---|---|
| IdKontrolnaTocka | INT | PK, AI, NN | Jedinstveni identifikator kontrolne točke |
| GUIDOznaka | VARCHAR(100) | NN, UNIQUE | Jedinstveni GUID oznake na vrhu / vidikovcu / točki |
| IdPodrucje | INT | FK, NN | Veza na područje kojem točka pripada |
| Naziv | VARCHAR(150) | NN | Naziv kontrolne točke |
| TipKontrolneTocke | ENUM | NN | Enum: Vrh, Vidikovac, KontrolnaTocka |
| NadmorskaVisina | INT | NULL | Nadmorska visina kontrolne točke |
| Opis | TEXT | NULL | Opis točke, uključujući vidik i dodatne informacije |
| Koordinate | VARCHAR(100) | NULL | Geografske koordinate |
| OpisZiga | TEXT | NULL | Opis žiga ili oznake na kontrolnoj točki |

---

## 6. Ruta

| Atribut | Tip | Oznaka | Objašnjenje |
|---|---|---|---|
| IdRuta | INT | PK, AI, NN | Jedinstveni identifikator rute |
| IdKontrolnaTocka | INT | FK, NN | Veza na kontrolnu točku do koje ruta vodi |
| Naziv | VARCHAR(200) | NN | Naziv rute |
| Pocetak | VARCHAR(150) | NN | Početna točka rute |
| Kraj | VARCHAR(150) | NN | Završna točka rute |
| VrijemeHodaMin | INT | NN | Procijenjeno vrijeme hoda u minutama |
| DuljinaKm | DECIMAL(5,2) | NN | Duljina rute u kilometrima |
| VisinskaRazlikaM | INT | NULL | Visinska razlika u metrima |
| Opis | TEXT | NULL | Opis rute |
| OznakaNaTerenu | VARCHAR(50) | NULL | Oznaka / broj markacije na terenu |
| GodinaObnove | INT | NULL | Godina obnove rute |
| Napomena | TEXT | NULL | Posebne napomene uz rutu |
| TezinaRute | ENUM | NN | Enum: Laka, Srednja, Teska |
| GPXPath | VARCHAR(255) | NULL | Putanja do GPX datoteke rute |

---

## 7. Podrucje

| Atribut | Tip | Oznaka | Objašnjenje |
|---|---|---|---|
| IdPodrucje | INT | PK, AI, NN | Jedinstveni identifikator područja |
| Naziv | VARCHAR(150) | NN | Naziv područja |
| Opis | TEXT | NULL | Opis područja |
| Regija | VARCHAR(150) | NULL | Šira regija / geografska pripadnost |
| MinimalanBrojKTZaObilazak | INT | NN | Minimalan broj kontrolnih točaka da bi se područje smatralo obiđenim |

---

## 8. PlaninarskiObjekt

| Atribut | Tip | Oznaka | Objašnjenje |
|---|---|---|---|
| IdPlaninarskiObjekt | INT | PK, AI, NN | Jedinstveni identifikator planinarskog objekta |
| IdPodrucje | INT | FK, NN | Veza na područje u kojem se objekt nalazi |
| IdPlaninarskaUdruga | INT | FK, NN | Veza na planinarsku udrugu koja upravlja objektom |
| Naziv | VARCHAR(150) | NN | Naziv objekta |
| TipObjekta | ENUM | NN | Enum: Dom, Kuca, Skloniste |
| NadmorskaVisina | INT | NULL | Nadmorska visina objekta |
| Kapacitet | INT | NULL | Kapacitet objekta |
| Opis | TEXT | NULL | Opis objekta |
| ImeOdgovorneOsobe | VARCHAR(150) | NULL | Ime odgovorne osobe |
| Telefon | VARCHAR(30) | NULL | Kontakt telefon |
| Email | VARCHAR(150) | NULL | Kontakt e-mail |
| Adresa | VARCHAR(255) | NULL | Adresa objekta |
| ImaNocenje | BOOLEAN | NN | Ima li objekt mogućnost noćenja |
| ImaHranu | BOOLEAN | NN | Ima li objekt mogućnost prehrane |
| RadnoVrijemeOpis | TEXT | NULL | Opis radnog vremena ili otvorenosti objekta |

---

## 9. PlaninarskaUdruga

| Atribut | Tip | Oznaka | Objašnjenje |
|---|---|---|---|
| IdPlaninarskaUdruga | INT | PK, AI, NN | Jedinstveni identifikator planinarske udruge |
| OIB | VARCHAR(11) | NN, UNIQUE | OIB udruge |
| Naziv | VARCHAR(150) | NN | Naziv udruge |
| Email | VARCHAR(150) | NULL | E-mail udruge |
| BrojTelefona | VARCHAR(30) | NULL | Kontakt broj |
| Adresa | VARCHAR(255) | NULL | Adresa udruge |
| PostanskiBroj | VARCHAR(20) | NULL | Poštanski broj |
| Grad | VARCHAR(100) | NULL | Grad |
| Zupanija | VARCHAR(100) | NULL | Županija |
| BrojClanova | INT | NULL | Broj članova udruge |

---

## 10. Medalja

| Atribut | Tip | Oznaka | Objašnjenje |
|---|---|---|---|
| IdMedalja | INT | PK, AI, NN | Jedinstveni identifikator medalje |
| Naziv | VARCHAR(100) | NN | Naziv medalje |
| Opis | TEXT | NULL | Opis medalje i kriterija |
| MinimalanBrojKontrolnihTocaka | INT | NN | Ukupan minimalan broj kontrolnih točaka za medalju |
| MinimalanBrojPodrucja | INT | NN | Minimalan broj područja koja moraju biti obiđena |

---

## 11. KorisnikMedalja

| Atribut | Tip | Oznaka | Objašnjenje |
|---|---|---|---|
| IdKorisnikMedalja | INT | PK, AI, NN | Jedinstveni identifikator dodjele medalje |
| IdKorisnik | INT | FK, NN | Veza na korisnika |
| IdMedalja | INT | FK, NN | Veza na medalju |
| DatumDodjele | DATETIME | NN | Datum i vrijeme dodjele medalje |
| Napomena | TEXT | NULL | Dodatna napomena uz dodjelu |

---

# Relacije između tablica

## 1:1 relacije

### Korisnik (1) — (1) Knjizica
Jedan korisnik ima jednu knjižicu, a jedna knjižica pripada jednom korisniku.

---

## 1:N relacije

### Korisnik (1) — (N) Posjet
Jedan korisnik može imati više posjeta, a svaki posjet pripada jednom korisniku.

### Knjizica (1) — (N) Posjet
Jedna knjižica sadrži više zapisa o posjetima, a svaki posjet pripada jednoj knjižici.

### KontrolnaTocka (1) — (N) Posjet
Jedna kontrolna točka može imati više posjeta različitih korisnika, a svaki posjet se odnosi na jednu kontrolnu točku.

### Ruta (1) — (N) Posjet
Jedna ruta može biti korištena u više posjeta, a svaki posjet koristi jednu rutu.

### Posjet (1) — (N) Fotografija
Jedan posjet može imati više fotografija, a svaka fotografija pripada jednom posjetu.

### Podrucje (1) — (N) KontrolnaTocka
Jedno područje ima više kontrolnih točaka, a svaka kontrolna točka pripada jednom području.

### KontrolnaTocka (1) — (N) Ruta
Jedna kontrolna točka može imati više ruta koje vode do nje, a svaka ruta vodi do jedne kontrolne točke.

### Podrucje (1) — (N) PlaninarskiObjekt
Jedno područje može imati više planinarskih objekata, a svaki planinarski objekt pripada jednom području.

### PlaninarskaUdruga (1) — (N) PlaninarskiObjekt
Jedna planinarska udruga može upravljati s više planinarskih objekata, a svaki objekt pripada jednoj udruzi.

### Korisnik (1) — (N) KorisnikMedalja
Jedan korisnik može imati više dodijeljenih medalja.

### Medalja (1) — (N) KorisnikMedalja
Jedna medalja može biti dodijeljena većem broju korisnika.

---

## N:N relacije

### Korisnik (N) — (N) KontrolnaTocka, riješeno preko tablice Posjet
Jedan korisnik može obići više kontrolnih točaka, a jednu kontrolnu točku može obići više korisnika.  
Ta veza riješena je pomoću tablice `Posjet`, koja dodatno sprema detalje obilaska.

### Korisnik (N) — (N) Medalja, riješeno preko tablice KorisnikMedalja
Jedan korisnik može imati više medalja, a jednu medalju može imati više korisnika.  
Ta veza riješena je pomoću tablice `KorisnikMedalja`.

---

# Dodatna logika sustava medalja

Tablica `Medalja` definira opće uvjete za osvajanje medalje:
- ukupan broj kontrolnih točaka
- broj područja koja moraju biti obiđena

Tablica `Podrucje` definira koliko je minimalno kontrolnih točaka potrebno unutar tog područja da bi se ono smatralo obiđenim, preko atributa:
- `MinimalanBrojKTZaObilazak`

To znači da korisnik može osvojiti određenu medalju samo ako:
1. ima dovoljan ukupan broj kontrolnih točaka
2. ima dovoljan broj obiđenih područja
3. svako od tih područja zadovoljava svoj minimalni broj KT za obilazak

Ova logika omogućuje da različita područja imaju različite pragove obilaska, što je preciznije i realnije od jednog zajedničkog pravila za sva područja.
