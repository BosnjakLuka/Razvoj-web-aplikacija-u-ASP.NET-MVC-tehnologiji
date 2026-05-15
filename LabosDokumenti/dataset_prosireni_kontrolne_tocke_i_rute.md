# Planinarska aplikacija — prošireni dataset kontrolnih točaka i ruta

## Kratko objašnjenje

Ovaj dokument sadrži **dodatne podatke za tablice `KontrolnaTocka` i `Ruta`** koji se nadovezuju na postojeći `dataset_planinarska_aplikacija.md`.  
Podaci su usklađeni s javno dostupnim popisom kontrolnih točaka [Hrvatske planinarske obilaznice (HPO)](https://www.hps.hr/info/hrvatski-vrhovi/) i pokrivaju svih 20 područja.

## Napomena

- Postojeći dataset ima **6 kontrolnih točaka** (ID 1–6) i **5 ruta** (ID 1–5).
- Ovaj dokument dodaje **kontrolne točke od ID 7 nadalje** i **rute od ID 6 nadalje**.
- `IdPodrucje` vrijednosti odgovaraju tablici `Podrucje` iz postojećeg dataseta (1–20).
- **GUID format**: 3 slova + 4 nasumične znamenke. Prvo slovo je uvijek prvo slovo naziva vrha, a preostala 2 slova služe za razlikovanje vrhova sličnih imena. Primjeri: `MOS1234` (Moslavačka gora), `SJE1234` (Sljeme), `GRO4920` (Gromovača).
- Koordinate su procjene temeljene na poznatim lokacijama vrhova.
- Nadmorske visine preuzete su s HPS popisa.
- Rute su smisleni planinarski pristupi kontrolnim točkama.

---

# 1. Tablica `KontrolnaTocka` — prošireni podaci

## Čemu služi tablica
Tablica `KontrolnaTocka` sadrži vrhove, vidikovce i druge službene kontrolne točke HPO-a.

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

## GUID oznake — pravila

| Pravilo | Opis |
|---|---|
| Duljina | Uvijek 7 znakova: 3 slova + 4 znamenke |
| Prvo slovo | Uvijek prvo slovo naziva vrha/kontrolne točke |
| Slova 2–3 | Dva dodatna slova za jedinstvenu identifikaciju (ne moraju biti doslovno iz naziva) |
| Znamenke | 4 nasumične znamenke (0000–9999) |

### Primjeri iz postojećeg dataseta

| GUIDOznaka | Vrh | Objašnjenje |
|---|---|---|
| MOS1234 | Moslavačka gora – vrh Vis | M od Moslavačka, OS za razlikovanje |
| SJE1234 | Sljeme – vrh | S od Sljeme, JE za razlikovanje |
| OKI1234 | Okić – vrh | O od Okić, KI za razlikovanje |
| JAP1234 | Japetić – vrh | J od Japetić, AP za razlikovanje |
| VZA1234 | Veliki Zavižan – vrh | V od Veliki, ZA za razlikovanje |
| GRO4920 | Gromovača – vrh | G od Gromovača, RO za razlikovanje |

## Podaci

| IdKontrolnaTocka | GUIDOznaka | IdPodrucje | Naziv | TipKontrolneTocke | NadmorskaVisina | Opis | Koordinate | OpisZiga |
|---:|---|---:|---|---|---:|---|---|---|
| 7 | KAP8371 | 1 | Krndija – vrh Kapovac | Vrh | 790 | Najviši vrh Krndije u slavonskom gorju; šumoviti vrh s markiranim pristupom. | N 45° 28' 12.0'' E 17° 52' 30.0'' | Metalni žig na vršnoj oznaci. |
| 8 | IVA5629 | 1 | Papuk – vrh Ivačka glava | Vrh | 913 | Najviši vrh Papuka i cijele Slavonije; dostupan s više strana. | N 45° 31' 10.0'' E 17° 40' 15.0'' | Metalni žig na vrhu kod geodetskog stupa. |
| 9 | BRE7412 | 1 | Psunj – vrh Brezovo polje | Vrh | 984 | Najviši vrh Psunja i jedan od najviših slavonskih vrhova; šumovit i miran. | N 45° 16' 45.0'' E 17° 18' 20.0'' | Metalni žig na oznaci vrha. |
| 10 | STA2087 | 2 | Bilogora – Stankov vrh | Vrh | 309 | Najviši vrh Bilogore s vidikovcem i planinarskim putom kroz šumu. | N 45° 53' 00.0'' E 17° 07' 30.0'' | Metalni žig na drvenom stupu kod vidikovca. |
| 11 | MOH6243 | 3 | Međimurske gorice – vrh Mohokos | Vrh | 344 | Najviši vrh Međimurja; lagani pristup i lijep pogled prema Alpama i Zagorju. | N 46° 24' 50.0'' E 16° 22' 10.0'' | Metalni žig na oznaci vrha. |
| 12 | IVN3815 | 3 | Ivanščica – vrh Ivanščica | Vrh | 1060 | Najviši vrh Hrvatskog zagorja i najistaknutiji zagorski vrh s panoramskim vidicima. | N 46° 10' 55.0'' E 16° 06' 45.0'' | Metalni žig na vršnom stupu. |
| 13 | RAV9174 | 3 | Ravna gora – vrh (piramida) | Vrh | 680 | Šumoviti vrh s geodetskom piramidom i markiranim pristupom iz Gornje Stubice. | N 46° 04' 20.0'' E 15° 56' 30.0'' | Metalni žig na piramidi. |
| 14 | SUS4538 | 3 | Strahinjščica – vrh Sušec | Vrh | 846 | Najviši vrh Strahinjščice s pogledom prema Ivanščici i Krapinskoj dolini. | N 46° 11' 40.0'' E 15° 54' 20.0'' | Metalni žig na kamenoj oznaci vrha. |
| 15 | GRH7260 | 4 | Grohot – vrh | Vrh | 492 | Niži vrh Medvednice s vidikovcem i starim hrastovima; pogodan za kraće ture. | N 45° 52' 30.0'' E 16° 03' 10.0'' | Metalni žig na drvenoj oznaci vrha. |
| 16 | LIP3492 | 4 | Lipa – vrh | Vrh | 709 | Šumoviti vrh Medvednice na sjevernom grebenu; miran i manje posjećen. | N 45° 54' 10.0'' E 15° 55' 40.0'' | Metalni žig na vršnom stupu. |
| 17 | MEG8156 | 4 | Medvedgrad | KontrolnaTocka | 579 | Srednjovjekovna utvrda na južnim padinama Medvednice; kontrolna točka HPO-a. | N 45° 51' 45.0'' E 15° 56' 50.0'' | Metalni žig na ulaznom zidu utvrde. |
| 18 | PLE6703 | 5 | Plešivica – vrh | Vrh | 779 | Vrh Samoborskog gorja s pogledom na vinograde i Žumberak; blizu planinarskog doma. | N 45° 43' 30.0'' E 15° 39' 20.0'' | Metalni žig na kamenoj oznaci. |
| 19 | OST5281 | 5 | Oštrc – vrh | Vrh | 752 | Popularan vrh s kapelom Sv. Ane na vrhu i panoramskim vidicima. | N 45° 44' 10.0'' E 15° 40' 55.0'' | Metalni žig na kapelici na vrhu. |
| 20 | TUS9047 | 6 | Tuščak – gradina | KontrolnaTocka | 585 | Stara gradina na zapadnom dijelu Žumberačke gore; pogled prema Žumberku. | N 45° 44' 00.0'' E 15° 30' 10.0'' | Metalni žig na ruševini gradine. |
| 21 | SGE2634 | 6 | Sveta Gera – vrh | Vrh | 1178 | Najviši vrh Žumberačke gore i cijele Žumberačko-samoborske regije. | N 45° 42' 45.0'' E 15° 22' 30.0'' | Metalni žig na vršnom stupu. |
| 22 | PLI7819 | 6 | Pliješ – vrh | Vrh | 977 | Šumoviti vrh Žumberačke gore s markiranim putom iz Budinjaka. | N 45° 43' 20.0'' E 15° 25' 50.0'' | Metalni žig na oznaci vrha. |
| 23 | VOD4153 | 7 | Vodenica – vrh | Vrh | 538 | Najviši vrh Pokuplja; miran vrh s pogledom na Kupu i okolne šume. | N 45° 27' 10.0'' E 15° 32' 20.0'' | Metalni žig na drvenoj oznaci. |
| 24 | PET6928 | 7 | Petrova gora – vrh Petrovac | Vrh | 512 | Vrh Petrove gore s poznatim spomenikom i vidikovcem prema Kordunu. | N 45° 19' 20.0'' E 15° 47' 00.0'' | Metalni žig na spomeniku kod vrha. |
| 25 | KLE3047 | 8 | Klek – vrh | Vrh | 1181 | Karakteristična stijena iznad Ogulina; simbol hrvatskog planinarstva od 1874. godine. | N 45° 17' 55.0'' E 15° 10' 40.0'' | Metalni žig na vršnom stupu. |
| 26 | BJE8592 | 8 | Bjelolasica – vrh Kula | Vrh | 1534 | Najviši vrh Gorskog kotara i hrvatski vrh izvan Velebita i Dinare. | N 45° 15' 50.0'' E 14° 58' 30.0'' | Metalni žig na geodetskom stupu na vrhu. |
| 27 | SAM1736 | 8 | Samarske stijene – vrh | Vrh | 1302 | Spektakularne stjenovite formacije u srcu Gorskog kotara; zahtjevan pristup. | N 45° 16' 20.0'' E 14° 55' 10.0'' | Metalni žig na stijeni kod vrha. |
| 28 | RIS4208 | 9 | Risnjak – vrh | Vrh | 1528 | Najviši vrh istoimenog nacionalnog parka; panoramski pogled od Alpa do mora. | N 45° 25' 35.0'' E 14° 45' 20.0'' | Metalni žig na vršnom stupu kod kapelice. |
| 29 | SNJ6371 | 9 | Snježnik – vrh | Vrh | 1505 | Drugi najviši vrh Gorskog kotara; poznat po kasnom snijegu i alpskim livadama. | N 45° 26' 10.0'' E 14° 35' 40.0'' | Metalni žig na kamenoj oznaci vrha. |
| 30 | SKR2845 | 9 | Skradski vrh | Vrh | 1043 | Popularan izletnički vrh u sjevernom Gorskom kotaru s planinarskim domom. | N 45° 24' 05.0'' E 15° 02' 15.0'' | Metalni žig na vršnom stupu. |
| 31 | VOJ7164 | 10 | Učka – vrh Vojak | Vrh | 1396 | Najviši vrh Istre s kamenim tornjem na vrhu i pogledom na Kvarner i Alpe. | N 45° 17' 10.0'' E 14° 11' 55.0'' | Metalni žig na kamenom tornju na vrhu. |
| 32 | VPL3920 | 10 | Ćićarija – vrh Veliki Planik | Vrh | 1272 | Najviši vrh Ćićarije s travnatim vršnim područjem i pogledom prema Učki. | N 45° 27' 20.0'' E 14° 13' 30.0'' | Metalni žig na kamenoj oznaci. |
| 33 | MRA8451 | 11 | Mali Rajinac – vrh | Vrh | 1699 | Jedan od najviših velebitskih vrhova na sjevernom dijelu; krški vrh s divljim pogledom. | N 44° 46' 30.0'' E 14° 58' 50.0'' | Metalni žig na vrhu stijene. |
| 34 | ZEC6237 | 12 | Zečjak – vrh | Vrh | 1622 | Najviši vrh Srednjeg Velebita; stjenovit i zahtjevan teren. | N 44° 36' 15.0'' E 15° 03' 40.0'' | Metalni žig na kamenoj piramidi. |
| 35 | SAT1584 | 12 | Šatorina – vrh | Vrh | 1622 | Karakteristični vrh Srednjeg Velebita s oblikom šatora; divlji krški krajolik. | N 44° 34' 50.0'' E 15° 05' 10.0'' | Metalni žig na oznaci vrha. |
| 36 | VAG7302 | 13 | Vaganski vrh | Vrh | 1757 | Najviši vrh Velebita i treći najviši vrh Hrvatske; zahtjevan pristup iz Paklenice. | N 44° 21' 50.0'' E 15° 30' 20.0'' | Metalni žig na geodetskom stupu na vrhu. |
| 37 | SVB4916 | 13 | Sveto brdo – vrh | Vrh | 1751 | Drugi najviši vrh Velebita s kapelom na vrhu; pogled na more i Liku. | N 44° 19' 40.0'' E 15° 30' 55.0'' | Metalni žig na kapeli na vrhu. |
| 38 | ANI2058 | 13 | Anića kuk – vrh | Vrh | 712 | Impozantna stijena u klancu Velike Paklenice; alpinistički značajan vrh. | N 44° 18' 15.0'' E 15° 27' 40.0'' | Metalni žig na vršnom stupu. |
| 39 | OZE8743 | 14 | Lička Plješivica – vrh Ozeblin | Vrh | 1657 | Najviši vrh Ličke Plješivice i Like; zahtjevan pristup šumskim putovima. | N 44° 46' 10.0'' E 15° 44' 30.0'' | Metalni žig na vrhu. |
| 40 | POT5261 | 14 | Poštak – vrh | Vrh | 1425 | Istaknuti lički vrh na granici prema Dalmaciji s otvorenim pogledom. | N 44° 10' 55.0'' E 16° 10' 20.0'' | Metalni žig na vršnoj oznaci. |
| 41 | OBZ3179 | 15 | Krk – vrh Obzova | Vrh | 569 | Najviši vrh otoka Krka s pogledom na Kvarner i okolne otoke. | N 45° 01' 20.0'' E 14° 37' 50.0'' | Metalni žig na vršnom stupu. |
| 42 | SIS6420 | 15 | Cres – vrh Sis | Vrh | 639 | Najviši vrh otoka Cresa; divlji otočni krajolik s pogledom na Jadran. | N 44° 52' 30.0'' E 14° 22' 10.0'' | Metalni žig na kamenoj oznaci. |
| 43 | VID8537 | 16 | Brač – vrh Vidova gora | Vrh | 780 | Najviši vrh jadranskih otoka; spektakularan pogled na Zlatni rat i Hvar. | N 43° 18' 40.0'' E 16° 37' 20.0'' | Metalni žig na vršnom stupu. |
| 44 | SNK2074 | 16 | Hvar – vrh Sv. Nikola | Vrh | 626 | Najviši vrh otoka Hvara s pogledom na paklinske otoke i pelješku obalu. | N 43° 10' 35.0'' E 16° 39' 50.0'' | Metalni žig na kapelici Sv. Nikole. |
| 45 | KOM9361 | 16 | Korčula – vrh Kom | Vrh | 508 | Najviši vrh otoka Korčule s gustim makijama i pogledom na Pelješac. | N 42° 57' 30.0'' E 16° 53' 15.0'' | Metalni žig na kamenoj oznaci. |
| 46 | DIN4728 | 17 | Dinara – vrh Dinara (Sinjal) | Vrh | 1831 | Najviši vrh Republike Hrvatske; obavezna kontrolna točka za srebrnu značku HPO-a. | N 43° 59' 25.0'' E 16° 22' 50.0'' | Metalni žig na geodetskom stupu na vrhu. |
| 47 | SVL5839 | 17 | Svilaja – vrh Svilaja | Vrh | 1508 | Najviši vrh planine Svilaje u dalmatinskom zaleđu; zahtjevan pristup. | N 43° 44' 10.0'' E 16° 28' 30.0'' | Metalni žig na vršnom stupu. |
| 48 | CAV7162 | 17 | Promina – vrh Čavnovka | Vrh | 1147 | Najviši vrh planine Promine iznad Drniša; pogled na Krku i Zagoru. | N 43° 51' 40.0'' E 16° 05' 20.0'' | Metalni žig na kamenoj oznaci. |
| 49 | LJU3084 | 18 | Mosor – vrh Ljubljan | Vrh | 1262 | Istaknuti vrh Mosora iznad Splita; markiran pristup iz Dugopolja. | N 43° 31' 20.0'' E 16° 31' 50.0'' | Metalni žig na vrhu. |
| 50 | BIR6597 | 18 | Kozjak – vrh Biranj | Vrh | 631 | Vrh planine Kozjak iznad Kaštela s pogledom na Split i otoke. | N 43° 33' 50.0'' E 16° 24' 10.0'' | Metalni žig na kamenoj oznaci. |
| 51 | SJU4213 | 19 | Sv. Jure – vrh | Vrh | 1762 | Najviši vrh Biokova i drugi najviši vrh uz obalu; pristup cestom ili pješice. | N 43° 20' 10.0'' E 17° 03' 00.0'' | Metalni žig na kapeli Sv. Jure. |
| 52 | VOS8746 | 19 | Vošac – vrh | Vrh | 1421 | Popularan biokovački vrh s pogledom na makarsku rivijeru i otoke. | N 43° 18' 55.0'' E 17° 04' 20.0'' | Metalni žig na vršnom stupu. |
| 53 | KIM3509 | 19 | Kimet – vrh | Vrh | 1536 | Zahtjevniji biokovački vrh; stjenovit i izložen vjetru. | N 43° 19' 30.0'' E 17° 04' 50.0'' | Metalni žig na stijeni. |
| 54 | SIL2871 | 20 | Pelješac – vrh Sv. Ilija | Vrh | 960 | Najviši vrh poluotoka Pelješca; zahtjevna staza s pogledom na Korčulu i Mljet. | N 42° 55' 20.0'' E 17° 07' 30.0'' | Metalni žig na vršnom stupu. |
| 55 | ILJ6034 | 20 | Sniježnica – Ilijin vrh | Vrh | 1234 | Najviši vrh dubrovačkog zaleđa; panoramski pogled od Dubrovnika do crnogorskih planina. | N 42° 38' 40.0'' E 18° 15' 10.0'' | Metalni žig na vrhu. |
| 56 | VSV7283 | 8 | Viševica – vrh | Vrh | 1428 | Istaknuti vrh južnog Gorskog kotara s pogledom na Kvarner i otoke. | N 45° 18' 40.0'' E 14° 39' 50.0'' | Metalni žig na vršnom stupu. |
| 57 | CAR5190 | 1 | Dilj gora – vrh Čardak | Vrh | 421 | Najviši vrh Dilj gore kod Slavonskog Broda; blag i pristupačan vrh. | N 45° 14' 30.0'' E 18° 07' 20.0'' | Metalni žig na vršnoj oznaci. |
| 58 | ZBE8416 | 10 | Ćićarija – vrh Žbevnica | Vrh | 1014 | Vrh Ćićarije s travnatim vršnim područjem i pogledom prema slovenskoj granici. | N 45° 29' 10.0'' E 14° 08' 40.0'' | Metalni žig na kamenoj oznaci. |
| 59 | VRA2758 | 3 | Kalnik – vrh Vranilac | Vrh | 643 | Najviši vrh Kalnika sa stijenama i pogledom na Podravinu; zahtjevniji pristup. | N 46° 09' 00.0'' E 16° 27' 30.0'' | Metalni žig na stijeni kod vrha. |
| 60 | HOR6391 | 4 | Horvatovih 500 stuba | KontrolnaTocka | 450 | Poznate stube na Medvednici; jedna od dvije kontrolne točke HPO-a koje nisu vrhovi. | N 45° 52' 10.0'' E 15° 57' 20.0'' | Metalni žig na oznaci kod stuba. |

---

# 2. Tablica `Ruta` — prošireni podaci

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
| 6 | 8 | Jankovac – Ivačka glava | Jankovac | Ivačka glava | 120 | 5.5 | 530 | Pristup Papuku od planinarskog doma Jankovac kroz bukovu šumu do najvišeg slavonskog vrha. | PP-01 | 2023 | Dobro markiran put kroz park prirode Papuk. | Srednja | C:\GPX\ruta_ivacka.gpx |
| 7 | 9 | Brestovac – Brezovo polje | Brestovac | Brezovo polje | 150 | 7.2 | 620 | Duži pristup Psunju iz sela Brestovac kroz šumu; pogodan za iskusnije planinare. | PS-01 | 2022 | Slabije markiran u gornjem dijelu. | Srednja | C:\GPX\ruta_psunj.gpx |
| 8 | 12 | Ivanec – Ivanščica vrh | Ivanec | Ivanščica | 180 | 9.0 | 780 | Klasičan pristup najvišem vrhu Zagorja iz Ivanca preko planinarske kuće. | IZ-02 | 2023 | Dug, ali dobro markiran put. | Srednja | C:\GPX\ruta_ivanscica.gpx |
| 9 | 14 | Radoboj – Sušec | Radoboj | Sušec | 90 | 4.0 | 450 | Kraći pristup vrhu Strahinjščice iz Radoboja kroz šumu. | SH-01 | 2021 | Pogodan za poluizlete. | Laka | C:\GPX\ruta_susec.gpx |
| 10 | 15 | Šestine – Grohot | Šestine | Grohot | 60 | 3.2 | 280 | Kratak uspon od Šestina do vrha Grohot na Medvednici. | MED-07 | 2024 | Idealan za kratke popodnevne ture. | Laka | C:\GPX\ruta_grohot.gpx |
| 11 | 17 | Šestinski dol – Medvedgrad | Šestinski dol | Medvedgrad | 45 | 2.5 | 300 | Kratak ali strm pristup utvrdi Medvedgrad s južne strane. | MED-02 | 2024 | Popularna obiteljska ruta. | Laka | C:\GPX\ruta_medvedgrad.gpx |
| 12 | 18 | Poljanica – Plešivica | Poljanica Samoborska | Plešivica – vrh | 75 | 4.0 | 420 | Pristup Plešivici s južne strane iz Poljanice kroz vinograde i šumu. | SG-02 | 2021 | Lijep pogled na vinograde tijekom uspona. | Laka | C:\GPX\ruta_plesivica.gpx |
| 13 | 19 | Japetić dom – Oštrc | Planinarski dom Žitnica | Oštrc – vrh | 60 | 3.0 | 280 | Grebenski prijelaz od doma Žitnica kod Japetića do vrha Oštrc preko kapele Sv. Ane. | SG-03 | 2022 | Atraktivan grebenski put s pogledima. | Srednja | C:\GPX\ruta_ostrc.gpx |
| 14 | 21 | Budinjak – Sveta Gera | Budinjak | Sveta Gera | 180 | 8.5 | 650 | Dugačak pristup najvišem vrhu Žumberačke gore iz Budinjaka. | ZG-01 | 2021 | Potrebna dobra kondicija za dulji uspon. | Zahtjevna | C:\GPX\ruta_svetagera.gpx |
| 15 | 25 | Bjelsko – Klek | Bjelsko | Klek – vrh | 120 | 4.5 | 780 | Klasičan pristup Kleku iz sela Bjelsko; strm završni dio uz pomoć sajli. | GK-01 | 2023 | Završni dio zahtijeva osnovnu opremu i iskustvo. | Zahtjevna | C:\GPX\ruta_klek.gpx |
| 16 | 26 | Begovo Razdolje – Bjelolasica | Begovo Razdolje | Bjelolasica – Kula | 90 | 5.0 | 430 | Pristup najvišem vrhu Gorskog kotara iz Begovog Razdolja. | GK-05 | 2024 | Relativno lagodan pristup s makadama. | Srednja | C:\GPX\ruta_bjelolasica.gpx |
| 17 | 28 | Crni Lug – Risnjak | Crni Lug | Risnjak – vrh | 150 | 7.0 | 680 | Klasičan pristup Risnjaku iz Crnog Luga kroz nacionalni park. | GK-08 | 2024 | Prolaz kroz NP Risnjak; plaćanje ulaznice. | Srednja | C:\GPX\ruta_risnjak.gpx |
| 18 | 29 | Platak – Snježnik | Platak | Snježnik – vrh | 120 | 5.5 | 510 | Pristup Snježniku s Platka preko planinskog doma. | GK-10 | 2023 | Može imati snijega do kasnog proljeća. | Srednja | C:\GPX\ruta_snjeznik.gpx |
| 19 | 31 | Poklon – Vojak | Poklon | Učka – Vojak | 90 | 4.2 | 520 | Najpopularniji pristup Vojaku s prijevoja Poklon; dobro markiran. | IS-01 | 2024 | Najpopularnija ruta na Učki. | Laka | C:\GPX\ruta_vojak.gpx |
| 20 | 33 | Alan – Mali Rajinac | Planinarski dom Alan | Mali Rajinac | 180 | 8.0 | 650 | Zahtjevan pristup jednom od najviših velebitskih vrhova iz doma Alan. | SV-03 | 2023 | Ozbiljan krški teren; potrebna dobra oprema. | Zahtjevna | C:\GPX\ruta_mrajinac.gpx |
| 21 | 36 | Starigrad Paklenica – Vaganski vrh | Starigrad-Paklenica | Vaganski vrh | 360 | 14.0 | 1550 | Dugi i zahtjevni uspon na najviši vrh Velebita kroz NP Paklenica. | JV-01 | 2024 | Cijeli dan hoda; potrebna odlična kondicija. | Zahtjevna | C:\GPX\ruta_vaganski.gpx |
| 22 | 38 | Velika Paklenica – Anića kuk | Velika Paklenica ulaz | Anića kuk – vrh | 120 | 3.5 | 500 | Pristup Anića kuku iz klanca Velike Paklenice; alpinistički značajan vrh. | JV-04 | 2022 | Završni dio tehnički zahtjevan. | Zahtjevna | C:\GPX\ruta_anicakuk.gpx |
| 23 | 39 | Glogovac – Ozeblin | Glogovac | Ozeblin | 240 | 10.0 | 900 | Dugačak pristup najvišem vrhu Like iz sela Glogovac. | LI-01 | 2021 | Slabije markiran gornji dio; potrebna navigacija. | Zahtjevna | C:\GPX\ruta_ozeblin.gpx |
| 24 | 41 | Baška – Obzova | Baška | Obzova – vrh | 120 | 5.5 | 500 | Pristup najvišem vrhu Krka iz Baške; otočni krški teren. | OT-01 | 2023 | Ljeti ponijeti dovoljno vode. | Srednja | C:\GPX\ruta_obzova.gpx |
| 25 | 43 | Nerežišća – Vidova gora | Nerežišća | Vidova gora | 90 | 4.5 | 480 | Pristup najvišem otočnom vrhu iz mjesta Nerežišća; pogled na Zlatni rat. | OT-05 | 2024 | Popularna turistička ruta s izvrsnim vidikom. | Laka | C:\GPX\ruta_vidovagora.gpx |
| 26 | 46 | Glavaš – Dinara (Sinjal) | Glavaš | Dinara (Sinjal) | 240 | 9.0 | 950 | Klasičan pristup najvišem vrhu Hrvatske iz zaseoka Glavaš iznad Vrlike. | DZ-01 | 2024 | Obavezna točka za srebrnu značku HPO-a. Zahtjevan pristup. | Zahtjevna | C:\GPX\ruta_dinara.gpx |
| 27 | 47 | Muć – Svilaja | Muć | Svilaja – vrh | 210 | 9.5 | 1050 | Dugi pristup vrhu Svilaje iz Muća kroz dalmatinsko zaleđe. | DZ-03 | 2022 | Zahtjevan uspon po toplom vremenu. | Zahtjevna | C:\GPX\ruta_svilaja.gpx |
| 28 | 49 | Dugopolje – Ljubljan | Dugopolje | Mosor – Ljubljan | 150 | 6.5 | 860 | Pristup Mosoru iz Dugopolja s markiranim putom prema vrhu Ljubljan. | DA-02 | 2023 | Popularna splitska planinarska ruta. | Srednja | C:\GPX\ruta_mosor.gpx |
| 29 | 51 | Bast – Sv. Jure Biokovo | Bast | Sv. Jure | 300 | 11.0 | 1600 | Najzahtjevniji pristup Biokovu iz Basta na obali; ogromna visinska razlika. | BI-01 | 2024 | Iznimno zahtjevna ruta; cijeli dan hoda. | Zahtjevna | C:\GPX\ruta_svjure_biokovo.gpx |
| 30 | 52 | Makarska – Vošac | Makarska | Vošac – vrh | 180 | 6.0 | 1300 | Popularan uspon na Biokovo iz Makarske s pogledom na rivijeru. | BI-03 | 2023 | Strm, ali dobro markiran pristup. | Zahtjevna | C:\GPX\ruta_vosac.gpx |
| 31 | 54 | Orebić – Sv. Ilija Pelješac | Orebić | Sv. Ilija Pelješac | 180 | 6.5 | 900 | Pristup najvišem pelješkom vrhu iz Orebića; pogled na Korčulu. | DU-01 | 2022 | Zahtjevan uspon, posebno ljeti. | Zahtjevna | C:\GPX\ruta_svilija_peljesac.gpx |
| 32 | 55 | Pridvorje – Sniježnica | Pridvorje | Sniježnica – Ilijin vrh | 150 | 6.0 | 750 | Pristup najvišem vrhu dubrovačkog zaleđa iz Pridvorja. | DU-02 | 2021 | Ljeti ponijeti dovoljno vode; manje markacija. | Srednja | C:\GPX\ruta_snijeznica.gpx |
| 33 | 7 | Našice – Kapovac | Našice | Kapovac | 150 | 7.0 | 540 | Pristup vrhu Krndije iz Našica preko šumskih putova. | SL-01 | 2022 | Dulji pristup kroz slavonsku šumu. | Srednja | C:\GPX\ruta_kapovac.gpx |
| 34 | 22 | Budinjak – Pliješ | Budinjak | Pliješ – vrh | 120 | 5.5 | 500 | Pristup Pliješu iz Budinjaka kroz Žumberačku goru. | ZG-02 | 2023 | Umjeren pristup šumskim putevima. | Srednja | C:\GPX\ruta_plijes.gpx |
| 35 | 59 | Kalnik selo – Vranilac | Kalnik (selo) | Vranilac – vrh | 90 | 3.5 | 340 | Pristup Kalniku iz istoimenog sela; strm završni dio uz stijene. | ZA-03 | 2022 | Završni dio zahtijeva pažnju. | Srednja | C:\GPX\ruta_vranilac.gpx |

---

# 3. Pregled svih GUID oznaka (postojeći + novi)

| ID | GUIDOznaka | Vrh | Područje |
|---:|---|---|---|
| 1 | MOS1234 | Moslavačka gora – vrh Vis | 2 |
| 2 | SJE1234 | Sljeme – vrh | 4 |
| 3 | OKI1234 | Okić – vrh | 5 |
| 4 | JAP1234 | Japetić – vrh | 5 |
| 5 | VZA1234 | Veliki Zavižan – vrh | 11 |
| 6 | GRO4920 | Gromovača – vrh | 11 |
| 7 | KAP8371 | Krndija – vrh Kapovac | 1 |
| 8 | IVA5629 | Papuk – vrh Ivačka glava | 1 |
| 9 | BRE7412 | Psunj – vrh Brezovo polje | 1 |
| 10 | STA2087 | Bilogora – Stankov vrh | 2 |
| 11 | MOH6243 | Međimurske gorice – vrh Mohokos | 3 |
| 12 | IVN3815 | Ivanščica – vrh Ivanščica | 3 |
| 13 | RAV9174 | Ravna gora – vrh (piramida) | 3 |
| 14 | SUS4538 | Strahinjščica – vrh Sušec | 3 |
| 15 | GRH7260 | Grohot – vrh | 4 |
| 16 | LIP3492 | Lipa – vrh | 4 |
| 17 | MEG8156 | Medvedgrad | 4 |
| 18 | PLE6703 | Plešivica – vrh | 5 |
| 19 | OST5281 | Oštrc – vrh | 5 |
| 20 | TUS9047 | Tuščak – gradina | 6 |
| 21 | SGE2634 | Sveta Gera – vrh | 6 |
| 22 | PLI7819 | Pliješ – vrh | 6 |
| 23 | VOD4153 | Vodenica – vrh | 7 |
| 24 | PET6928 | Petrova gora – vrh Petrovac | 7 |
| 25 | KLE3047 | Klek – vrh | 8 |
| 26 | BJE8592 | Bjelolasica – vrh Kula | 8 |
| 27 | SAM1736 | Samarske stijene – vrh | 8 |
| 28 | RIS4208 | Risnjak – vrh | 9 |
| 29 | SNJ6371 | Snježnik – vrh | 9 |
| 30 | SKR2845 | Skradski vrh | 9 |
| 31 | VOJ7164 | Učka – vrh Vojak | 10 |
| 32 | VPL3920 | Ćićarija – vrh Veliki Planik | 10 |
| 33 | MRA8451 | Mali Rajinac – vrh | 11 |
| 34 | ZEC6237 | Zečjak – vrh | 12 |
| 35 | SAT1584 | Šatorina – vrh | 12 |
| 36 | VAG7302 | Vaganski vrh | 13 |
| 37 | SVB4916 | Sveto brdo – vrh | 13 |
| 38 | ANI2058 | Anića kuk – vrh | 13 |
| 39 | OZE8743 | Lička Plješivica – vrh Ozeblin | 14 |
| 40 | POT5261 | Poštak – vrh | 14 |
| 41 | OBZ3179 | Krk – vrh Obzova | 15 |
| 42 | SIS6420 | Cres – vrh Sis | 15 |
| 43 | VID8537 | Brač – vrh Vidova gora | 16 |
| 44 | SNK2074 | Hvar – vrh Sv. Nikola | 16 |
| 45 | KOM9361 | Korčula – vrh Kom | 16 |
| 46 | DIN4728 | Dinara – vrh Dinara (Sinjal) | 17 |
| 47 | SVL5839 | Svilaja – vrh Svilaja | 17 |
| 48 | CAV7162 | Promina – vrh Čavnovka | 17 |
| 49 | LJU3084 | Mosor – vrh Ljubljan | 18 |
| 50 | BIR6597 | Kozjak – vrh Biranj | 18 |
| 51 | SJU4213 | Sv. Jure – vrh | 19 |
| 52 | VOS8746 | Vošac – vrh | 19 |
| 53 | KIM3509 | Kimet – vrh | 19 |
| 54 | SIL2871 | Pelješac – vrh Sv. Ilija | 20 |
| 55 | ILJ6034 | Sniježnica – Ilijin vrh | 20 |
| 56 | VSV7283 | Viševica – vrh | 8 |
| 57 | CAR5190 | Dilj gora – vrh Čardak | 1 |
| 58 | ZBE8416 | Ćićarija – vrh Žbevnica | 10 |
| 59 | VRA2758 | Kalnik – vrh Vranilac | 3 |
| 60 | HOR6391 | Horvatovih 500 stuba | 4 |

---

# 4. Sažetak dodanih podataka

## Kontrolne točke po područjima

| IdPodrucje | Naziv područja | Postojeće KT (ID) | Nove KT (ID) |
|---:|---|---|---|
| 1 | Slavonija | – | 7, 8, 9, 57 |
| 2 | Moslavačka gora i Bilogora | 1 | 10 |
| 3 | Hrvatsko zagorje i Međimurje | – | 11, 12, 13, 14, 59 |
| 4 | Medvednica | 2 | 15, 16, 17, 60 |
| 5 | Samoborsko gorje | 3, 4 | 18, 19 |
| 6 | Žumberačka gora | – | 20, 21, 22 |
| 7 | Karlovačko pokuplje, Kordun i Banovina | – | 23, 24 |
| 8 | Gorski kotar - južni dio | – | 25, 26, 27, 56 |
| 9 | Gorski kotar - sjeverni dio | – | 28, 29, 30 |
| 10 | Istra | – | 31, 32, 58 |
| 11 | Sjeverni Velebit | 5, 6 | 33 |
| 12 | Srednji Velebit | – | 34, 35 |
| 13 | Južni Velebit | – | 36, 37, 38 |
| 14 | Lika | – | 39, 40 |
| 15 | Jadranski otoci - sjeverni dio | – | 41, 42 |
| 16 | Jadranski otoci - južni dio | – | 43, 44, 45 |
| 17 | Dalmatinska zagora | – | 46, 47, 48 |
| 18 | Dalmacija | – | 49, 50 |
| 19 | Biokovo i Zagora | – | 51, 52, 53 |
| 20 | Dubrovačko područje | – | 54, 55 |

## Ukupno

- **Postojeći dataset**: 6 KT + 5 ruta
- **Ovaj prošireni dataset**: 54 nove KT (ID 7–60) + 30 novih ruta (ID 6–35)
- **Ukupno nakon spajanja**: 60 KT + 35 ruta

## Napomena za implementaciju

- Svi `IdPodrucje` vrijednosti referenciraju postojeću tablicu `Podrucje` (ID 1–20).
- Svaka ruta referencira jednu kontrolnu točku preko `IdKontrolnaTocka`.
- `GPXPath` putanje su placeholder vrijednosti — u produkcijskoj verziji zamjenjuju se stvarnim GPX datotekama.
- Koordinate su procjene i treba ih verificirati za točnu implementaciju.
- GUID oznake prate format: **3 slova + 4 znamenke** (npr. `KAP8371`, `DIN4728`, `VOJ7164`).
- Ovi podaci mogu se koristiti za `HasData()` seed metodu u Entity Framework-u ili za ručno punjenje baze.
