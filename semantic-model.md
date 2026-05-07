---

# Semantički model baze podataka — Planinarska aplikacija

## Kratki opis
Digitalna planinarska knjižica — aplikacija za evidenciju posjeta kontrolnim točkama, praćenje ruta, područja i osvajanje medalja.

## Pregled tablica

| Tablica | Opis | PK | Broj atributa |
|---------|------|----|---------------|
| Korisnik | Registrirani korisnik aplikacije | IdKorisnik | 11 |
| Knjizica | Digitalna planinarska knjižica (1:1 s korisnikom) | IdKnjizica | 5 |
| Posjet | Evidencija posjeta kontrolnoj točki | IdPosjet | 12 |
| Fotografija | Fotografija vezana uz posjet | IdFotografija | 7 |
| KontrolnaTocka | Vrh, vidikovac ili KT s GUID oznakom | IdKontrolnaTocka | 9 |
| Ruta | Planinarska ruta do kontrolne točke | IdRuta | 14 |
| Podrucje | Planinarsko područje Hrvatske | IdPodrucje | 5 |
| PlaninarskiObjekt | Dom, kuća ili sklonište | IdPlaninarskiObjekt | 15 |
| PlaninarskaUdruga | Planinarsko društvo/udruga | IdPlaninarskaUdruga | 10 |
| Medalja | Definicija medalje i uvjeta | IdMedalja | 5 |
| KorisnikMedalja | Veza korisnik-medalja (N:N) | IdKorisnikMedalja | 5 |

## Detalji po tablicama

### Korisnik
| Atribut | C# tip | DB ograničenja | Opis |
|---------|--------|----------------|------|
| IdKorisnik | int | PK, NN | Jedinstveni identifikator korisnika |
| Ime | string | NN, MaxLength(100) | Ime korisnika |
| Prezime | string | NN, MaxLength(100) | Prezime korisnika |
| Email | string | NN, MaxLength(150), UNIQUE | E-mail adresa korisnika |
| KorisnickoIme | string | NN, MaxLength(100), UNIQUE | Korisničko ime |
| PasswordHash | string | NN, MaxLength(255) | Hash lozinke |
| DatumRodenja | DateTime? | NULL | Datum rođenja |
| DatumRegistracije | DateTime | NN | Datum registracije |
| BrojMobitela | string? | NULL, MaxLength(30) | Kontakt broj |
| ProfilnaSlika | string? | NULL, MaxLength(255) | Putanja do profilne slike |
| StatusAktivan | bool | NN | Status aktivacije računa |

**Relacije:**
- Korisnik 1──1 Knjizica (FK: Knjizica.IdKorisnik, UNIQUE)
- Korisnik 1──N Posjet (FK: Posjet.IdKorisnik)
- Korisnik 1──N KorisnikMedalja (FK: KorisnikMedalja.IdKorisnik)

---

### Knjizica
| Atribut | C# tip | DB ograničenja | Opis |
|---------|--------|----------------|------|
| IdKnjizica | int | PK, NN | Jedinstveni identifikator knjižice |
| IdKorisnik | int | FK, NN, UNIQUE | Veza na korisnika (1:1) |
| DatumKreiranja | DateTime | NN | Datum kreiranja |
| Napomena | string? | NULL | Napomena |
| StatusAktivna | bool | NN | Status aktivacije |

**Relacije:**
- Knjizica 1──1 Korisnik (FK: Knjizica.IdKorisnik)
- Knjizica 1──N Posjet (FK: Posjet.IdKnjizica)

---

### Posjet
| Atribut | C# tip | DB ograničenja | Opis |
|---------|--------|----------------|------|
| IdPosjet | int | PK, NN | Jedinstveni identifikator posjeta |
| IdKorisnik | int | FK, NN | Veza na korisnika |
| IdKnjizica | int | FK, NN | Veza na knjižicu |
| IdKontrolnaTocka | int | FK, NN | Veza na kontrolnu točku |
| IdRuta | int | FK, NN | Veza na rutu |
| DatumVrijemePosjeta | DateTime | NN | Datum i vrijeme posjeta |
| VrijemeUsponaMin | int? | NULL | Vrijeme uspona u minutama |
| DozivljajPosjeta | DozivljajPosjeta | NN | Doživljaj posjeta |
| OpisIskustva | string? | NULL | Opis iskustva |
| UneseniGUID | string | NN, MaxLength(100) | Uneseni GUID oznake |
| JeLiPotvrdenPosjet | bool | NN | Je li posjet potvrđen |
| DatumKreiranjaZapisa | DateTime | NN | Datum kreiranja zapisa |

**Relacije:**
- Posjet N──1 Korisnik (FK: Posjet.IdKorisnik)
- Posjet N──1 Knjizica (FK: Posjet.IdKnjizica)
- Posjet N──1 KontrolnaTocka (FK: Posjet.IdKontrolnaTocka)
- Posjet N──1 Ruta (FK: Posjet.IdRuta)
- Posjet 1──N Fotografija (FK: Fotografija.IdPosjet)

---

### Fotografija
| Atribut | C# tip | DB ograničenja | Opis |
|---------|--------|----------------|------|
| IdFotografija | int | PK, NN | Jedinstveni identifikator fotografije |
| IdPosjet | int | FK, NN | Veza na posjet |
| NazivDatoteke | string | NN, MaxLength(255) | Naziv datoteke |
| PutanjaDatoteke | string | NN, MaxLength(255) | Putanja datoteke |
| DatumUploada | DateTime | NN | Datum uploada |
| TipSlike | TipSlike | NN | Tip slike |
| Opis | string? | NULL | Opis fotografije |

**Relacije:**
- Fotografija N──1 Posjet (FK: Fotografija.IdPosjet)

---

### KontrolnaTocka
| Atribut | C# tip | DB ograničenja | Opis |
|---------|--------|----------------|------|
| IdKontrolnaTocka | int | PK, NN | Jedinstveni identifikator kontrolne točke |
| GUIDOznaka | string | NN, MaxLength(100), UNIQUE | GUID oznaka |
| IdPodrucje | int | FK, NN | Veza na područje |
| Naziv | string | NN, MaxLength(150) | Naziv kontrolne točke |
| TipKontrolneTocke | TipKontrolneTocke | NN | Tip kontrolne točke |
| NadmorskaVisina | int? | NULL | Nadmorska visina |
| Opis | string? | NULL | Opis |
| Koordinate | string? | NULL, MaxLength(100) | Koordinate |
| OpisZiga | string? | NULL | Opis žiga |

**Relacije:**
- KontrolnaTocka N──1 Podrucje (FK: KontrolnaTocka.IdPodrucje)
- KontrolnaTocka 1──N Posjet (FK: Posjet.IdKontrolnaTocka)
- KontrolnaTocka 1──N Ruta (FK: Ruta.IdKontrolnaTocka)

---

### Ruta
| Atribut | C# tip | DB ograničenja | Opis |
|---------|--------|----------------|------|
| IdRuta | int | PK, NN | Jedinstveni identifikator rute |
| IdKontrolnaTocka | int | FK, NN | Veza na kontrolnu točku |
| Naziv | string | NN, MaxLength(200) | Naziv rute |
| Pocetak | string | NN, MaxLength(150) | Početak rute |
| Kraj | string | NN, MaxLength(150) | Kraj rute |
| VrijemeHodaMin | int | NN | Vrijeme hoda u minutama |
| DuljinaKm | decimal | NN | Duljina rute u km |
| VisinskaRazlikaM | int? | NULL | Visinska razlika |
| Opis | string? | NULL | Opis rute |
| OznakaNaTerenu | string? | NULL, MaxLength(50) | Oznaka na terenu |
| GodinaObnove | int? | NULL | Godina obnove |
| Napomena | string? | NULL | Napomena |
| TezinaRute | TezinaRute | NN | Težina rute |
| GPXPath | string? | NULL, MaxLength(255) | Putanja do GPX datoteke |

**Relacije:**
- Ruta N──1 KontrolnaTocka (FK: Ruta.IdKontrolnaTocka)
- Ruta 1──N Posjet (FK: Posjet.IdRuta)

---

### Podrucje
| Atribut | C# tip | DB ograničenja | Opis |
|---------|--------|----------------|------|
| IdPodrucje | int | PK, NN | Jedinstveni identifikator područja |
| Naziv | string | NN, MaxLength(150) | Naziv područja |
| Opis | string? | NULL | Opis područja |
| Regija | string? | NULL, MaxLength(150) | Regija |
| MinimalanBrojKTZaObilazak | int | NN | Minimalan broj KT za obilazak |

**Relacije:**
- Podrucje 1──N KontrolnaTocka (FK: KontrolnaTocka.IdPodrucje)
- Podrucje 1──N PlaninarskiObjekt (FK: PlaninarskiObjekt.IdPodrucje)

---

### PlaninarskiObjekt
| Atribut | C# tip | DB ograničenja | Opis |
|---------|--------|----------------|------|
| IdPlaninarskiObjekt | int | PK, NN | Jedinstveni identifikator objekta |
| IdPodrucje | int | FK, NN | Veza na područje |
| IdPlaninarskaUdruga | int | FK, NN | Veza na udrugu |
| Naziv | string | NN, MaxLength(150) | Naziv objekta |
| TipObjekta | TipObjekta | NN | Tip objekta |
| NadmorskaVisina | int? | NULL | Nadmorska visina |
| Kapacitet | int? | NULL | Kapacitet |
| Opis | string? | NULL | Opis |
| ImeOdgovorneOsobe | string? | NULL, MaxLength(150) | Odgovorna osoba |
| Telefon | string? | NULL, MaxLength(30) | Telefon |
| Email | string? | NULL, MaxLength(150) | Email |
| Adresa | string? | NULL, MaxLength(255) | Adresa |
| ImaNocenje | bool | NN | Mogućnost noćenja |
| ImaHranu | bool | NN | Mogućnost prehrane |
| RadnoVrijemeOpis | string? | NULL | Opis radnog vremena |

**Relacije:**
- PlaninarskiObjekt N──1 Podrucje (FK: PlaninarskiObjekt.IdPodrucje)
- PlaninarskiObjekt N──1 PlaninarskaUdruga (FK: PlaninarskiObjekt.IdPlaninarskaUdruga)

---

### PlaninarskaUdruga
| Atribut | C# tip | DB ograničenja | Opis |
|---------|--------|----------------|------|
| IdPlaninarskaUdruga | int | PK, NN | Jedinstveni identifikator udruge |
| OIB | string | NN, MaxLength(11), UNIQUE | OIB udruge |
| Naziv | string | NN, MaxLength(150) | Naziv udruge |
| Email | string? | NULL, MaxLength(150) | Email |
| BrojTelefona | string? | NULL, MaxLength(30) | Broj telefona |
| Adresa | string? | NULL, MaxLength(255) | Adresa |
| PostanskiBroj | string? | NULL, MaxLength(20) | Poštanski broj |
| Grad | string? | NULL, MaxLength(100) | Grad |
| Zupanija | string? | NULL, MaxLength(100) | Županija |
| BrojClanova | int? | NULL | Broj članova |

**Relacije:**
- PlaninarskaUdruga 1──N PlaninarskiObjekt (FK: PlaninarskiObjekt.IdPlaninarskaUdruga)

---

### Medalja
| Atribut | C# tip | DB ograničenja | Opis |
|---------|--------|----------------|------|
| IdMedalja | int | PK, NN | Jedinstveni identifikator medalje |
| Naziv | string | NN, MaxLength(100) | Naziv medalje |
| Opis | string? | NULL | Opis |
| MinimalanBrojKontrolnihTocaka | int | NN | Minimalan broj KT |
| MinimalanBrojPodrucja | int | NN | Minimalan broj područja |

**Relacije:**
- Medalja 1──N KorisnikMedalja (FK: KorisnikMedalja.IdMedalja)

---

### KorisnikMedalja
| Atribut | C# tip | DB ograničenja | Opis |
|---------|--------|----------------|------|
| IdKorisnikMedalja | int | PK, NN | Jedinstveni identifikator zapisa |
| IdKorisnik | int | FK, NN | Veza na korisnika |
| IdMedalja | int | FK, NN | Veza na medalju |
| DatumDodjele | DateTime | NN | Datum dodjele |
| Napomena | string? | NULL | Napomena |

**Relacije:**
- KorisnikMedalja N──1 Korisnik (FK: KorisnikMedalja.IdKorisnik)
- KorisnikMedalja N──1 Medalja (FK: KorisnikMedalja.IdMedalja)

---

## Enum tipovi

- DozivljajPosjeta: VrloLagano, Lagano, Srednje, Zahtjevno, VrloZahtjevno, KratkoAliTesko, DugoAliLagano, FizickiNaporno, TehnickiZahtjevno
- TipKontrolneTocke: Vrh, Vidikovac, KontrolnaTocka
- TipObjekta: Dom, Kuca, Skloniste
- TezinaRute: Laka, Srednja, Teska
- TipSlike: Selfie, Oznaka, Krajolik, Mapa, Drugo

## Dijagram relacija

Korisnik 1──1 Knjizica
Korisnik 1──N Posjet
Knjizica 1──N Posjet
KontrolnaTocka 1──N Posjet
Ruta 1──N Posjet
Posjet 1──N Fotografija
Podrucje 1──N KontrolnaTocka
KontrolnaTocka 1──N Ruta
Podrucje 1──N PlaninarskiObjekt
PlaninarskaUdruga 1──N PlaninarskiObjekt
Korisnik N──N Medalja (preko KorisnikMedalja)
Korisnik N──N KontrolnaTocka (preko Posjet)

---
