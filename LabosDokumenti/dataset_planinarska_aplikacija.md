# Planinarska aplikacija — full dataset za punjenje tablica

## Kratko objašnjenje teme

Ovaj dokument sadrži **konkretan primjer popunjenih tablica** za projekt *Planinarska aplikacija*.  
Aplikacija služi kao digitalna zamjena za klasičnu planinarsku knjižicu: korisnik evidentira obilazak kontrolnih točaka, veže posjet uz odabranu rutu, sprema fotografije kao dokaz obilaska i prati osvajanje medalja.

## Napomena o podacima

- **Podaci za područja** temelje se na pravilima i raspodjeli područja koje si ti zadao za projekt.
- **Podaci za kontrolne točke, planinarske udruge i planinarske objekte** u velikoj su mjeri usklađeni s javno dostupnim HPS izvorima.
- **Podaci za posjete, fotografije, knjižice i korisnik-medalja** su **primjeri testnih podataka** napravljeni za punjenje modela i demonstraciju rada aplikacije.

---

# 1. Tablica `Podrucje`

## Čemu služi tablica
Tablica `Podrucje` opisuje planinarska područja Hrvatske.  
Svako područje ima:
- svoj identifikator
- naziv
- kratak opis
- regiju
- minimalan broj kontrolnih točaka potreban da bi se područje smatralo obiđenim
- ukupan broj kontrolnih točaka u području

## Atributi
- `IdPodrucje` — primarni ključ područja
- `Naziv` — naziv područja
- `Opis` — kratki opis područja
- `Regija` — šira geografska regija
- `MinimalanBrojKTZaObilazak` — minimalan broj KT da bi se područje smatralo obiđenim
- `UkupanBrojKT` — ukupan broj KT u području

## Podaci

| IdPodrucje | Naziv | Opis | Regija | MinimalanBrojKTZaObilazak | UkupanBrojKT |
|---:|---|---|---|---:|---:|
| 1 | Slavonija | Nizinsko i brežuljkasto područje istočne Hrvatske s Papukom, Psunjem, Krndijom i drugim slavonskim gorjima. | Istočna Hrvatska | 2 | 9 |
| 2 | Moslavačka gora i Bilogora | Niža šumovita gorja s kraćim planinarskim usponima i manjim brojem kontrolnih točaka. | Središnja Hrvatska | 1 | 2 |
| 3 | Hrvatsko zagorje i Međimurje | Brežuljkasto područje s vidikovcima, utvrdama i poznatim vrhovima kao što su Ivanščica i Ravna gora. | Sjeverna Hrvatska | 3 | 9 |
| 4 | Medvednica | Planina iznad Zagreba s gusto razvijenom mrežom putova, domova i kontrolnih točaka. | Središnja Hrvatska | 2 | 6 |
| 5 | Samoborsko gorje | Popularno planinarsko područje zapadno od Zagreba, poznato po Okiću, Japetiću i Oštrcu. | Središnja Hrvatska | 2 | 5 |
| 6 | Žumberačka gora | Planinsko i granično područje s višim vrhovima i rjeđe naseljenim grebenima. | Središnja Hrvatska | 1 | 5 |
| 7 | Karlovačko pokuplje, Kordun i Banovina | Područje nižih gora i šumovitih uzvisina južno od Karlovca i prema Banovini. | Središnja Hrvatska | 1 | 5 |
| 8 | Gorski kotar - južni dio | Dio Gorskog kotara s višim vrhovima, stjenovitim skupinama i zahtjevnijim usponima. | Gorska Hrvatska | 4 | 14 |
| 9 | Gorski kotar - sjeverni dio | Šumovito i planinsko područje s vrhovima poput Risnjaka, Snježnika i Skradskog vrha. | Gorska Hrvatska | 3 | 12 |
| 10 | Istra | Područje Učke i Ćićarije s istaknutim obalnim i planinskim vidikovcima. | Zapadna Hrvatska | 2 | 7 |
| 11 | Sjeverni Velebit | Visokoplaninsko područje s izrazito atraktivnim velebitskim vrhovima i oštrim kršem. | Primorsko-gorska Hrvatska | 3 | 9 |
| 12 | Srednji Velebit | Središnji dio Velebita sa srednje zahtjevnim i zahtjevnim vrhovima i planinarskim kućama. | Lika i Primorje | 2 | 9 |
| 13 | Južni Velebit | Najviši i alpinistički najdojmljiviji dio Velebita s Vaganskim vrhom i Svetim brdom. | Lika i Dalmacija | 3 | 10 |
| 14 | Lika | Prostrano područje ličkih planina i osamljenih vrhova izvan glavnog velebitskog lanca. | Lika | 1 | 5 |
| 15 | Jadranski otoci - sjeverni dio | Sjeverni jadranski otoci s nižim, ali vrlo atraktivnim otočnim vrhovima. | Jadranska Hrvatska | 1 | 5 |
| 16 | Jadranski otoci - južni dio | Južni jadranski otoci s većim brojem otočnih vrhova i raznolikim podlogama. | Jadranska Hrvatska | 2 | 10 |
| 17 | Dalmatinska zagora | Područje Dinare, Promine, Svilaje i drugih planina dalmatinskog zaleđa. | Dalmatinsko zaleđe | 2 | 8 |
| 18 | Dalmacija | Priobalno i zaleđno područje srednje Dalmacije s planinama uz obalu i u zaleđu. | Dalmacija | 2 | 9 |
| 19 | Biokovo i Zagora | Krševito visokoplaninsko područje Biokova i zaleđa s vrlo izraženim visinskim razlikama. | Južna Dalmacija | 3 | 10 |
| 20 | Dubrovačko područje | Južnohrvatsko područje s manjim brojem, ali vrlo atraktivnih kontrolnih točaka. | Krajnji jug Hrvatske | 1 | 3 |

---

# 2. Tablica `Korisnik`

## Čemu služi tablica
Tablica `Korisnik` sadrži podatke o registriranim korisnicima aplikacije.

## Atributi
- `IdKorisnik`
- `Ime`
- `Prezime`
- `Email`
- `KorisnickoIme`
- `PasswordHash`
- `DatumRodenja`
- `DatumRegistracije`
- `BrojMobitela`
- `ProfilnaSlika`
- `StatusAktivan`

## Podaci

| IdKorisnik | Ime | Prezime | Email | KorisnickoIme | PasswordHash | DatumRodenja | DatumRegistracije | BrojMobitela | ProfilnaSlika | StatusAktivan |
|---:|---|---|---|---|---|---|---|---|---|---|
| 1 | Luka | Bošnjak | luka.bosnjak92@gmail.com | Boss | 123456789 | 2004-06-29 | 2026-04-01 09:00:00 | 0979545897 | C:\Users\lukab\Documents\Projekt\Razvoj-web-aplikacija-u-ASP.NET-MVC-tehnologiji\Slike\Profil\Boss.jpeg | true |
| 2 | Test | Test | test123@gmail.com | Test | 123456789 | 2005-01-01 | 2026-04-01 09:15:00 | NULL | C:\Users\lukab\Documents\Projekt\Razvoj-web-aplikacija-u-ASP.NET-MVC-tehnologiji\Slike\Profil\test.jpg | true |

---

# 3. Tablica `Knjizica`

## Čemu služi tablica
Svaki korisnik ima jednu digitalnu knjižicu u koju se upisuju njegovi posjeti.

## Atributi
- `IdKnjizica`
- `IdKorisnik`
- `DatumKreiranja`
- `Napomena`
- `StatusAktivna`

## Podaci

| IdKnjizica | IdKorisnik | DatumKreiranja | Napomena | StatusAktivna |
|---:|---:|---|---|---|
| 1 | 1 | 2026-04-01 09:05:00 | Glavna digitalna knjižica korisnika Luka Bošnjak. | true |
| 2 | 2 | 2026-04-01 09:20:00 | Testna digitalna knjižica za provjeru funkcionalnosti aplikacije. | true |

---

# 4. Tablica `Medalja`

## Čemu služi tablica
Tablica `Medalja` definira vrste medalja i njihove opće uvjete.

## Atributi
- `IdMedalja`
- `Naziv`
- `Opis`
- `MinimalanBrojKontrolnihTocaka`
- `MinimalanBrojPodrucja`

## Podaci

| IdMedalja | Naziv | Opis | MinimalanBrojKontrolnihTocaka | MinimalanBrojPodrucja |
|---:|---|---|---:|---:|
| 1 | Početnik | Osnovna medalja za prvi ispravno evidentirani obilazak područja. | 1 | 1 |
| 2 | Brončana značka | Potrebno je obići zadani broj KT-a iz 10 područja i ukupno 25 KT-a. | 25 | 10 |
| 3 | Srebrna značka | Potrebno je obići zadani broj KT-a iz 15 područja i ukupno 50 KT-a, uz obaveznu Dinaru (Sinjal). | 50 | 15 |
| 4 | Zlatna značka | Potrebno je obići zadani broj KT-a iz svih 20 područja i ukupno 75 KT-a. | 75 | 20 |
| 5 | Posebno priznanje | Potrebno je obići 100 KT-a uz ispunjene uvjete za zlatnu značku. | 100 | 20 |
| 6 | Visoko priznanje | Potrebno je obići 125 KT-a uz ispunjene uvjete za posebno priznanje. | 125 | 20 |
| 7 | Najviše priznanje | Potrebno je obići 155 KT-a uz ispunjene uvjete za visoko priznanje. | 155 | 20 |

---

# 5. Tablica `PlaninarskaUdruga`

## Čemu služi tablica
Tablica `PlaninarskaUdruga` sadrži osnovne podatke o planinarskim društvima i udrugama.

## Atributi
- `IdPlaninarskaUdruga`
- `OIB`
- `Naziv`
- `Email`
- `BrojTelefona`
- `Adresa`
- `PostanskiBroj`
- `Grad`
- `Zupanija`
- `BrojClanova`

## Podaci

| IdPlaninarskaUdruga | OIB | Naziv | Email | BrojTelefona | Adresa | PostanskiBroj | Grad | Zupanija | BrojClanova |
|---:|---|---|---|---|---|---|---|---|---:|
| 1 | 40461293872 | HPD Mosor | hpd.mosor@hps.hr | NULL | p.p. 233 | 21000 | Split | Splitsko-dalmatinska | 350 |
| 2 | 48938096579 | HPD Gora | hpd.gora@hps.hr | NULL | Dubravica 27a | 10000 | Zagreb | Grad Zagreb | 180 |
| 3 | 95873199484 | PD Zavižan | pd.zavizan@hps.hr | NULL | Mala vrata 20 | 53270 | Senj | Ličko-senjska | 120 |
| 4 | 92966614510 | PD Paklenica | pd.paklenica@hps.hr | NULL | Majke Margarite 6 | 23000 | Zadar | Zadarska | 220 |
| 5 | 12345678901 | PD Dr. Maks Plotnikov | info@pddr-maks-plotnikov.hr | 0991234567 | Andrije Hebranga 26 | 10430 | Samobor | Zagrebačka | 95 |

---

# 6. Tablica `PlaninarskiObjekt`

## Čemu služi tablica
Tablica `PlaninarskiObjekt` sadrži planinarske domove, kuće i skloništa.

## Atributi
- `IdPlaninarskiObjekt`
- `IdPodrucje`
- `IdPlaninarskaUdruga`
- `Naziv`
- `TipObjekta`
- `NadmorskaVisina`
- `Kapacitet`
- `Opis`
- `ImeOdgovorneOsobe`
- `Telefon`
- `Email`
- `Adresa`
- `ImaNocenje`
- `ImaHranu`
- `RadnoVrijemeOpis`

## Podaci

| IdPlaninarskiObjekt | IdPodrucje | IdPlaninarskaUdruga | Naziv | TipObjekta | NadmorskaVisina | Kapacitet | Opis | ImeOdgovorneOsobe | Telefon | Email | Adresa | ImaNocenje | ImaHranu | RadnoVrijemeOpis |
|---:|---:|---:|---|---|---:|---:|---|---|---|---|---|---|---|---|
| 1 | 5 | 5 | Planinarski dom Dr. Maks Plotnikov | Dom | 411 | 14 | Dom podno ruševina Okić-grada; polazišna je točka za Okić i okolne putove. | Stjepan Jandrečić | 0918909624 | aplantosar@gmail.com | Okić, Samobor | true | true | Otvoren vikendom i blagdanima. |
| 2 | 5 | 2 | Planinarski dom Željezničar | Dom | 691 | 25 | Popularan planinarski dom u Samoborskom i Žumberačkom gorju. | Dežurni domar | NULL | NULL | Samoborsko gorje | true | true | Prema rasporedu dežurstva i vikendom. |
| 3 | 11 | 3 | Planinarska kuća Sijaset | Kuca | 328 | 12 | Niži planinarski objekt na Velebitu, pogodan kao polazište za ture. | Dežurni član društva | NULL | pd.zavizan@hps.hr | Velebit, Senj | true | false | Povremeno otvorena ili po dogovoru. |
| 4 | 13 | 4 | Planinarski dom Paklenica | Dom | 480 | 44 | Dom na početku klanca Velike Paklenice s hranom, pićem i noćenjem. | Irena Šaran | 0977557654 | pd.paklenica@hps.hr | Velika Paklenica | true | true | Otvoren stalno. |
| 5 | 18 | 1 | Planinarska kuća Lugarnica | Kuca | 872 | 20 | Planinarska kuća na Mosoru pogodna za kraće i srednje duge uspone. | Dežurna osoba društva | NULL | hpd.mosor@hps.hr | Mosor, Split | true | false | Otvorenost prema obavijesti društva. |

---

# 7. Tablica `KontrolnaTocka`

## Čemu služi tablica
Tablica `KontrolnaTocka` sadrži vrhove, vidikovce i druge službene kontrolne točke.

## Atributi
- `IdKontrolnaTocka`
- `GUIDOznaka`
- `IdPodrucje`
- `Naziv`
- `TipKontrolneTocke`
- `NadmorskaVisina`
- `Opis`
- `Koordinate`
- `OpisZiga`

## Podaci

| IdKontrolnaTocka | GUIDOznaka | IdPodrucje | Naziv | TipKontrolneTocke | NadmorskaVisina | Opis | Koordinate | OpisZiga |
|---:|---|---:|---|---|---:|---|---|---|
| 1 | KT-HPO-2-1-VIS | 2 | Moslavačka gora – vrh Vis | Vrh | 437 | Najviši vrh Moslavačke gore i dobra početna kontrolna točka za početničke obilaznike. | N/A | Metalni žig na vršnoj oznaci. |
| 2 | KT-HPO-4-4-SLJEME | 4 | Sljeme – vrh | Vrh | 1033 | Najviši vrh Medvednice; vrh je lako dostupan i planinarima i izletnicima. | N 45° 53' 57.4'' E 15° 56' 50.6'' | Metalni žig vrha nalazi se na promidžbenom panou kod televizijskog tornja. |
| 3 | KT-HPO-5-1-OKIC | 5 | Okić – vrh | Vrh | 499 | Stari grad i vršna gradina s vidikom prema Zagrebu i Medvednici. | N 45° 44' 55.4'' E 15° 42' 24.0'' | Metalni žig vrha ugrađen je na zid u najvišem dijelu gradine. |
| 4 | KT-HPO-5-4-JAPETIC | 5 | Japetić – vrh | Vrh | 879 | Najviši vrh Samoborskoga gorja; poznat po piramidi i domu Žitnica. | N 45° 44' 56.3'' E 15° 36' 32.8'' | Metalni žig ugrađen je na konstrukciju piramide. |
| 5 | KT-HPO-11-2-ZAVIZAN | 11 | Veliki Zavižan – vrh | Vrh | 1676 | Jedan od najpoznatijih vrhova Sjevernog Velebita s vrlo atraktivnim pogledima. | N/A | Žig kontrolne točke nalazi se na vrhu ili u blizini planinarskog doma Zavižan. |

---

# 8. Tablica `Ruta`

## Čemu služi tablica
Tablica `Ruta` opisuje konkretne prilaze do kontrolnih točaka.

## Atributi
- `IdRuta`
- `IdKontrolnaTocka`
- `Naziv`
- `Pocetak`
- `Kraj`
- `VrijemeHodaMin`
- `DuljinaKm`
- `VisinskaRazlikaM`
- `Opis`
- `OznakaNaTerenu`
- `GodinaObnove`
- `Napomena`
- `TezinaRute`
- `GPXPath`

## Podaci

| IdRuta | IdKontrolnaTocka | Naziv | Pocetak | Kraj | VrijemeHodaMin | DuljinaKm | VisinskaRazlikaM | Opis | OznakaNaTerenu | GodinaObnove | Napomena | TezinaRute | GPXPath |
|---:|---:|---|---|---|---:|---:|---:|---|---|---:|---|---|---|
| 1 | 1 | Kutina – Humka – Vis | Kutina / Humka | Vrh Vis | 90 | 4.5 | 260 | Primjer kraće rute do najviše točke Moslavačke gore. | MG-01 | 2023 | Pogodna za početnike. | Laka | C:\GPX\ruta_vis.gpx |
| 2 | 2 | Gračani – Puntijarka – Sljeme | Gračani | Sljeme | 150 | 8.2 | 780 | Popularan uspon preko Puntijarke prema vrhu Medvednice. | M-04 | 2022 | Jedna od najčešće korištenih ruta na Medvednici. | Srednja | C:\GPX\ruta_sljeme.gpx |
| 3 | 3 | Klake – pl. dom pod Okićem – Okić-grad | Klake | Okić – vrh | 40 | 1.8 | 210 | Najkraći klasični prilaz vrhu Okić preko doma pod Okićem. | SG-01 | 2021 | Strmiji završni dio prema gradini. | Srednja | C:\GPX\ruta_okic.gpx |
| 4 | 4 | Šoićeva kuća – Japetić | Šoićeva kuća | Japetić – vrh | 90 | 5.4 | 430 | Klasičan prilaz preko livada i Katina krča prema vrhu Japetić. | SG-04 | 2020 | Ruta je pregledna i često korištena. | Srednja | C:\GPX\ruta_japetic.gpx |
| 5 | 5 | Dom Zavižan – Veliki Zavižan – dom Zavižan | Planinarski dom Zavižan | Veliki Zavižan | 150 | 6.7 | 320 | Kružna tura s polaskom od doma Zavižan preko Balinovca do Velikog Zavižana. | SV-02 | 2024 | U nepovoljnim uvjetima potreban dodatni oprez. | Zahtjevna | C:\GPX\ruta_zavizan.gpx |

---

# 9. Tablica `Posjet`

## Čemu služi tablica
Tablica `Posjet` je najvažnija radna tablica aplikacije.  
Ona povezuje korisnika, knjižicu, kontrolnu točku i rutu, te opisuje stvarni obilazak.

## Atributi
- `IdPosjet`
- `IdKorisnik`
- `IdKnjizica`
- `IdKontrolnaTocka`
- `IdRuta`
- `DatumVrijemePosjeta`
- `VrijemeUsponaMin`
- `DozivljajPosjeta`
- `OpisIskustva`
- `UneseniGUID`
- `JeLiPotvrdenPosjet`
- `DatumKreiranjaZapisa`

## Podaci

| IdPosjet | IdKorisnik | IdKnjizica | IdKontrolnaTocka | IdRuta | DatumVrijemePosjeta | VrijemeUsponaMin | DozivljajPosjeta | OpisIskustva | UneseniGUID | JeLiPotvrdenPosjet | DatumKreiranjaZapisa |
|---:|---:|---:|---:|---:|---|---:|---|---|---|---|---|
| 1 | 1 | 1 | 1 | 1 | 2026-04-05 10:15:00 | 92 | VrloLagano | Prvi evidentirani uspon u aplikaciji. Lagana i ugodna ruta po suhom vremenu. | KT-HPO-2-1-VIS | true | 2026-04-05 12:00:00 |
| 2 | 1 | 1 | 3 | 3 | 2026-04-12 08:40:00 | 43 | KratkoAliTesko | Kratak, ali strm završni dio prema gradini Okić. | KT-HPO-5-1-OKIC | true | 2026-04-12 10:00:00 |
| 3 | 1 | 1 | 4 | 4 | 2026-04-19 09:10:00 | 96 | Srednje | Ugodna tura s dobrim vremenom i lijepim pogledima s piramide. | KT-HPO-5-4-JAPETIC | true | 2026-04-19 11:10:00 |
| 4 | 2 | 2 | 1 | 1 | 2026-04-08 11:00:00 | 95 | Lagano | Testni korisnik uspješno evidentirao svoj prvi posjet i time zadovoljio uvjet za početničku medalju. | KT-HPO-2-1-VIS | true | 2026-04-08 12:45:00 |
| 5 | 2 | 2 | 2 | 2 | 2026-04-26 07:50:00 | 155 | FizickiNaporno | Duži uspon do Sljemena preko Puntijarke, ali bez tehnički teških dijelova. | KT-HPO-4-4-SLJEME | true | 2026-04-26 10:40:00 |

---

# 10. Tablica `Fotografija`

## Čemu služi tablica
Tablica `Fotografija` sprema slike vezane uz pojedini posjet.

## Atributi
- `IdFotografija`
- `IdPosjet`
- `NazivDatoteke`
- `PutanjaDatoteke`
- `DatumUploada`
- `TipSlike`
- `Opis`

## Podaci

| IdFotografija | IdPosjet | NazivDatoteke | PutanjaDatoteke | DatumUploada | TipSlike | Opis |
|---:|---:|---|---|---|---|---|
| 1 | 1 | vis_luka_selfie.jpg | C:\Slike\Posjeti\vis_luka_selfie.jpg | 2026-04-05 12:05:00 | Selfie | Selfie korisnika Luke na vrhu Vis. |
| 2 | 2 | okic_luka_selfie.jpg | C:\Slike\Posjeti\okic_luka_selfie.jpg | 2026-04-12 10:05:00 | Selfie | Fotografija Luke kod oznake vrha Okić. |
| 3 | 3 | japetic_luka_selfie.jpg | C:\Slike\Posjeti\japetic_luka_selfie.jpg | 2026-04-19 11:15:00 | Selfie | Selfie na vrhu Japetić uz piramidu. |
| 4 | 4 | vis_test_selfie.jpg | C:\Slike\Posjeti\vis_test_selfie.jpg | 2026-04-08 12:50:00 | Selfie | Testni korisnik na vrhu Vis. |
| 5 | 5 | sljeme_test_selfie.jpg | C:\Slike\Posjeti\sljeme_test_selfie.jpg | 2026-04-26 10:45:00 | Selfie | Testni korisnik na vrhu Sljeme kod oznake. |

---

# 11. Tablica `KorisnikMedalja`

## Čemu služi tablica
Tablica `KorisnikMedalja` povezuje korisnika i medalju koju je osvojio.

## Atributi
- `IdKorisnikMedalja`
- `IdKorisnik`
- `IdMedalja`
- `DatumDodjele`
- `Napomena`

## Podaci

| IdKorisnikMedalja | IdKorisnik | IdMedalja | DatumDodjele | Napomena |
|---:|---:|---:|---|---|
| 1 | 1 | 1 | 2026-04-19 12:00:00 | Korisnik je zadovoljio uvjet početničke medalje jer ima evidentiran obilazak područja 2 (Moslavačka gora i Bilogora), gdje je prag 1 KT. |
| 2 | 2 | 1 | 2026-04-08 13:00:00 | Korisnik je zadovoljio uvjet početničke medalje jer ima evidentiran obilazak područja 2 (Moslavačka gora i Bilogora), gdje je prag 1 KT. |

---

# Sažetak logike osvajanja medalja u ovom datasetu

- **Područje 2 (Moslavačka gora i Bilogora)** ima uvjet `MinimalanBrojKTZaObilazak = 1`.
- Oba korisnika imaju evidentiran posjet kontrolnoj točki **Vis** u tom području.
- Zato se području 2 računa kao **obiđeno** za oba korisnika.
- Budući da medalja **Početnik** traži:
  - najmanje **1 KT**
  - najmanje **1 područje**

  oba korisnika zadovoljavaju uvjet za tu medalju.

- Nitko od korisnika u ovom testnom datasetu **ne zadovoljava uvjete** za brončanu i više medalje.

---

# Kratka povezanost tablica

- `Korisnik` 1:1 `Knjizica`
- `Korisnik` 1:N `Posjet`
- `Knjizica` 1:N `Posjet`
- `KontrolnaTocka` 1:N `Posjet`
- `Ruta` 1:N `Posjet`
- `Posjet` 1:N `Fotografija`
- `Podrucje` 1:N `KontrolnaTocka`
- `Podrucje` 1:N `PlaninarskiObjekt`
- `PlaninarskaUdruga` 1:N `PlaninarskiObjekt`
- `Korisnik` N:N `Medalja` preko `KorisnikMedalja`
- `Korisnik` N:N `KontrolnaTocka` preko `Posjet`

---

# Napomena za implementaciju u kodu

Ovaj dataset je napravljen tako da ti bude koristan za:
- ručno punjenje objekata u `Main` programu
- testiranje LINQ upita
- kasniju migraciju prema pravoj bazi podataka
- demonstraciju pravila za medalje
