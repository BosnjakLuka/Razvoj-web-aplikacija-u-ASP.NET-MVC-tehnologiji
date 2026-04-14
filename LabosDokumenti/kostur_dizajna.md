# Kostur dizajna

## Osnovna ideja dizajna

Cilj dizajna je napraviti **modernu digitalnu verziju planinarske knjižice**, inspiriranu postojećom HPS logikom i strukturom sadržaja, ali vizualno čišću, suvremeniju i pregledniju.

Aplikacija ne bi trebala izgledati kao generična poslovna CRUD aplikacija niti kao zastarjeli portal, nego kao:

**osobna digitalna planinarska evidencija i karta napretka**.

Korisnik pri otvaranju aplikacije treba imati osjećaj:

> Ovo je moja osobna digitalna planinarska knjižica, pregled vrhova, ruta, područja i osvojenih medalja.

---

## Inspiracija

Dizajn je inspiriran HPS stranicom i općom logikom planinarskih portala, ali ne želi kopirati zastarjeli izgled.  
Preuzima se:

- informacijska struktura
- osjećaj planinarskog identiteta
- centralna važnost vrhova, ruta, objekata i područja
- portal pristup informacijama

Ne preuzima se:

- zastarjeli raspored elemenata
- prenatrpanost sadržajem
- vizualni šum
- stari institucionalni izgled

Zato je cilj napraviti:

**HPS inspiraciju u modernijem, čišćem i korisnički orijentiranom obliku**.

---

## Glavni motiv stranice

Glavni motiv dizajna je:

**digitalna planinarska knjižica + topografska karta + planinarski portal**

To znači da vizualni identitet aplikacije treba spajati:

- osobnu evidenciju korisnika
- planinarske vrhove i rute
- kartu Hrvatske i planinarskih područja
- osjećaj outdoor / hiking aplikacije
- pregled napretka kroz medalje i obilazak područja

---

## Vizualni stil

Preporučeni stil je:

**moderni outdoor / hiking portal**

Karakteristike tog stila:

- čisti i pregledni layout
- dovoljno praznog prostora između elemenata
- kartice za prikaz važnih informacija
- moderne sekcije umjesto zastarjelih blokova
- kombinacija institucionalne ozbiljnosti i planinarskog identiteta
- čitljiv i jednostavan prikaz podataka

Dizajn mora biti:

- unique / non-standard
- moderan
- tematski vezan uz planinarenje
- vizualno dosljedan kroz sve stranice

---

## Osjećaj koji dizajn treba prenijeti

Dizajn treba ostaviti dojam:

- prirodno
- pregledno
- informativno
- outdoor
- osobno
- organizirano
- moderno

Ne treba izgledati:

- kao zadani Bootstrap template
- kao klasični admin panel
- kao zastarjela tablična aplikacija

---

## Boje

Predložena paleta boja:

- **tamno plava** kao institucionalna i navigacijska boja
- **tamno zelena** kao glavna outdoor / planinarska akcent boja
- **maslinasta / kadulja zelena** za sekundarne elemente
- **svijetlo bež / krem** za toplije pozadine kartica i sekcija
- **bijela / svijetlo siva** za osnovne površine
- **narančasta ili planinarska crvena** za CTA elemente, naglaske i statuse

Ova kombinacija daje dojam:

- HPS inspiracije
- prirode i planina
- ozbiljnosti
- modernosti

---

## Pozadine i teksture

Vizualni detalji trebaju biti suptilni i nenametljivi.

Preporučeno:

- hero/banner sekcija s planinskom fotografijom
- vrlo lagane topografske linije u pozadini pojedinih dijelova
- diskretna tekstura papira ili knjižice na nekim karticama
- eventualno ilustracija karte Hrvatske ili planinskog reljefa

Važno:

- ne pretjerivati s teksturama
- ne stvarati vizualni kaos
- pozadina mora podržavati sadržaj, a ne odvlačiti pažnju

---

## Početna stranica

Početna stranica treba biti **custom stranica**, a ne obični generički Home.

### Struktura početne stranice

#### 1. Hero sekcija
Na vrhu stranice treba biti veliki banner / hero blok koji sadrži:

- veliki naslov
- kratki opis aplikacije
- planinarsku fotografiju ili motiv karte
- 1–2 istaknuta gumba

Primjer sadržaja:

- naslov: **Digitalna planinarska knjižica**
- podnaslov: **Osobna evidencija vrhova, ruta, područja i medalja**
- gumbi:
  - Pregled kontrolnih točaka
  - Pregled područja

#### 2. Kratki info blok
Ispod hero sekcije treba biti pregled osnovnih statistika:

- broj kontrolnih točaka
- broj ruta
- broj područja
- broj planinarskih objekata
- broj medalja

To se prikazuje kroz male stat kartice.

#### 3. O aplikaciji
Posebna sekcija s kratkim tekstom koji objašnjava:

- da aplikacija zamjenjuje papirnatu planinarsku knjižicu
- da prati vrhove, rute i obilazak područja
- da je inspirirana planinarskom obilaznicom i HPS logikom
- da omogućuje digitalni pregled napretka

#### 4. Karta / pregled Hrvatske
Na početnoj stranici je poželjna sekcija s:

- ilustracijom planinarske karte Hrvatske
- ili grafičkim pregledom planinarskih područja

To dodatno pojačava identitet aplikacije.

#### 5. Quick links / istaknute sekcije
Na dnu ili u sredini početne mogu biti kartice koje vode na glavne cjeline:

- Kontrolne točke
- Rute
- Područja
- Planinarski objekti
- Udruge
- Korisnici
- Medalje

---

## Navigacija

Navigacija mora biti pregledna, potpuna i dosljedna.

### Glavna navigacija
Glavni navbar na vrhu stranice treba sadržavati:

- Naslovnica
- Kontrolne točke
- Rute
- Područja
- Objekti
- Udruge
- Korisnici
- Posjeti
- Medalje

### Sekundarna navigacija
Sekundarna navigacija može se koristiti lokalno na pojedinim ekranima, ali ne treba preopteretiti cijeli layout.

### Breadcrumbs
Na svim detail stranicama treba koristiti breadcrumbs, npr.:

- Naslovnica / Područja / Samoborsko gorje
- Naslovnica / Kontrolne točke / Okić
- Naslovnica / Korisnici / Luka Bošnjak

Breadcrumbs su važni za preglednost i profesionalni MVC dojam.

---

## Pristup korisniku i admin logika

U ovoj fazi aplikacije pretpostavlja se da je:

**administrator već prijavljen**

Zbog toga:

- nema implementacije Sign In / Sign Up
- nema Create/Edit/Delete funkcionalnosti
- aplikacija služi za pregled podataka i navigaciju
- sve stranice su read-only

To je u skladu s laboratorijskom vježbom, gdje je fokus na prikazu podataka, MVC strukturi i UX-u.

---

## Prikaz listi

Liste ne trebaju sve izgledati isto.

### Kartični prikaz je preporučen za:
- Kontrolne točke
- Rute
- Područja
- Planinarske objekte
- Medalje

### Tablični prikaz je preporučen za:
- Korisnike
- Posjete
- Fotografije
- KorisnikMedalja
- Udruge

Razlog kombiniranja kartica i tablica:

- aplikacija izgleda raznovrsnije
- nije monotona
- lakše je postići unique UX
- podaci se prikazuju na način koji odgovara njihovoj prirodi

---

## Prikaz detalja

Details stranice ne smiju biti samo sirovi ispis atributa.

Svaki details ekran treba izgledati kao **profil ili kartica entiteta**.

### Kontrolna točka — Details
Treba prikazivati:

- naziv
- tip kontrolne točke
- nadmorsku visinu
- GUID
- opis
- koordinate
- područje
- povezane rute

### Ruta — Details
Treba prikazivati:

- naziv
- početak i kraj
- trajanje
- duljinu
- visinsku razliku
- težinu kao badge
- opis
- napomenu
- poveznicu na kontrolnu točku

### Područje — Details
Treba prikazivati:

- naziv
- opis
- regiju
- minimalni broj KT za obilazak
- sve KT u području
- planinarske objekte iz tog područja

### Korisnik — Details
Treba prikazivati:

- osnovne podatke korisnika
- njegovu knjižicu
- posjete
- osvojene medalje
- eventualno osnovnu statistiku

### Posjet — Details
Treba prikazivati:

- korisnika
- kontrolnu točku
- rutu
- datum i vrijeme
- doživljaj posjeta
- opis iskustva
- fotografije

---

## Komponente koje treba koristiti

Za postizanje modernog i unique UX-a preporučuju se sljedeće komponente:

### Kartice
Za pregled vrhova, ruta, područja, objekata i medalja.

### Badgevi
Za:

- tip kontrolne točke
- težinu rute
- tip objekta
- status posjeta
- osvojene medalje

### Stat kartice
Za početnu stranicu i dashboard prikaz.

### Progress elementi
Za prikaz napretka po medaljama ili područjima.

### Hero sekcija
Za početnu stranicu.

### Breadcrumbs
Za navigaciju kroz detalje.

### Link kartice / CTA sekcije
Za quick navigation na glavne dijelove aplikacije.

---

## Dizajnerska logika aplikacije

Cijela aplikacija treba izgledati kao spoj:

- planinarskog portala
- digitalne knjižice
- osobnog dashboarda napretka

Ključna ideja nije “baza podataka prikazana na webu”, nego:

> moderan sustav za pregled planinarskih vrhova, ruta, područja i osobnog napretka korisnika

---

## Zaključak

Konačni cilj dizajna je postići aplikaciju koja:

- izgleda modernije od HPS stranice
- zadržava planinarski identitet
- jasno pokazuje da se radi o digitalnoj planinarskoj knjižici
- ima dobru navigaciju i preglednost
- koristi kombinaciju kartica i tablica
- ne izgleda kao default Bootstrap template
- ostavlja dojam outdoor / hiking web aplikacije

Sažeto:

**HPS inspiracija + moderna digitalna planinarska knjižica + dashboard napretka**
