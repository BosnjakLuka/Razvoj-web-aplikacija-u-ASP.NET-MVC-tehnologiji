## Plan: UX/UI Hiking Logbook MVC

Uskladiti aplikaciju s Lab 2 zahtjevima kroz jedinstven, tematski hiking UI: novi globalni layout, custom naslovnica, konzistentan sustav komponenti (kartice, badgevi, stat kartice, details sekcije) i potpuna navigacija za Index/Details po svim entitetima, bez CRUD-a.

**Steps**
1. Uskladiti UX smjer i navigacijsku informacijsku arhitekturu prema dokumentima i entitetima: definirati glavne sekcije menija i mapu breadcrumbs tokova (Home, Kontrolne tocke, Rute, Podrucja, Objekti, Udruge, Korisnici, Posjeti, Medalje, Fotografije, KorisnikMedalja). Ovo je temelj za sve ostale korake.
2. Faza A: Globalni vizualni sustav i layout (*blokira sve stranice*).
3. Redizajnirati glavni layout s jakim top-navbarom, sekundarnim meta-redom (naslov stranice + contextual actions), i podnozjem koje ne izgleda kao default template; uvesti globalne CSS varijable i tipografske skale za tamno-plavu, tamno-zelenu/maslinastu, toplo-bez i narancasti akcent.
4. Definirati reusable klase komponenti: app-card, stat-card, detail-panel, badge varijante po semantici (tezina, tip, status), breadcrumb izgled, sekcije za prazna stanja i CTA kartice.
5. Faza B: Custom Home stranica (*depends on 2*).
6. Implementirati hero sekciju koja komunicira digitalnu planinarsku knjizicu i kartu napretka (naslov, opis, 2 primarna gumba).
7. Implementirati stat kartice (broj KT, ruta, podrucja, objekata, medalja) iz pripremljenih podataka i quick-link grid prema glavnim modulima.
8. Dodati tematsku sekciju (karta/reljef feel) i kratki blok O aplikaciji; paziti na citljivost i mobilnu prilagodbu.
9. Faza C: Predlosci za Index i Details po tipovima prikaza (*depends on 2, parallelizable by module groups*).
10. Card-first Index predlozak za: KontrolnaTocka, Ruta, Podrucje, PlaninarskiObjekt, Medalja.
11. Table-first Index predlozak za: Korisnik, Posjet, Fotografija, KorisnikMedalja, PlaninarskaUdruga (po potrebi cards/list toggle za udrugu).
12. Details predlozak kao profil/kartica entiteta s breadcrumbovima i semantickim sekcijama; pripremiti agregirane ViewModel-e gdje su potrebni povezani podaci (npr. Korisnik+Knjizica+Posjeti+Medalje, Podrucje+KT+Objekti, Posjet+Fotografije).
13. Faza D: Potpuna navigacija i linking pravila (*depends on 9-12*).
14. U svim listama osigurati link prema Details; na Details stranicama osigurati povratne i lateralne linkove prema povezanim entitetima.
15. U layout ugraditi globalni aktivni-state menija i konzistentne breadcrumbs obrasce.
16. Faza E: Poliranje i validacija (*depends on all prior phases*).
17. Proci sve stranice za vizualnu dosljednost, kontrast, responsivnost i non-default Bootstrap dojam; ukloniti genericke stilove koji dominiraju.
18. Potvrditi da su view datoteke presentation-focused (bez teske logike) i da je priprema agregata prebacena u controller/viewmodel sloj.

**Relevant files**
- c:/Users/lukab/Documents/Projekt/Razvoj-web-aplikacija-u-ASP.NET-MVC-tehnologiji/LabosDokumenti/kostur_dizajna.md — primarni UX smjer (identitet, boje, homepage struktura, card/table strategija).
- c:/Users/lukab/Documents/Projekt/Razvoj-web-aplikacija-u-ASP.NET-MVC-tehnologiji/LabosDokumenti/Lab 2 - HTML Binding.md — MVC konvencije, Index/Details zahtjevi, mock-repository i DI smjernice.
- c:/Users/lukab/Documents/Projekt/Razvoj-web-aplikacija-u-ASP.NET-MVC-tehnologiji/Views/Shared/_Layout.cshtml — glavni layout i navigacija za redizajn.
- c:/Users/lukab/Documents/Projekt/Razvoj-web-aplikacija-u-ASP.NET-MVC-tehnologiji/wwwroot/css/site.css — centralni stilovi i komponentne klase.
- c:/Users/lukab/Documents/Projekt/Razvoj-web-aplikacija-u-ASP.NET-MVC-tehnologiji/Views/Home/Index.cshtml — custom naslovnica (hero, stats, quick links, thematic blocks).
- c:/Users/lukab/Documents/Projekt/Razvoj-web-aplikacija-u-ASP.NET-MVC-tehnologiji/Controllers/HomeController.cs — priprema podataka za custom naslovnicu.
- c:/Users/lukab/Documents/Projekt/Razvoj-web-aplikacija-u-ASP.NET-MVC-tehnologiji/Entiteti/*.cs — domenski izvor za prikaze, badge semantiku i details sadržaj.

**Verification**
1. Manual smoke: svaka stavka glavne navigacije vodi na valjanu Index stranicu, a svaka stavka liste na valjanu Details stranicu.
2. Manual breadcrumb check: details stranice imaju ispravan hijerarhijski trag (npr. Naslovnica > Podrucja > Naziv podrucja).
3. Responsiveness check: desktop + mobile viewport (navbar collapse, kartice, tablice i hero ostaju citljivi).
4. Visual QA: potvrda da nijedna kljucna stranica ne izgleda kao default Bootstrap template.
5. MVC QA: view bez kompleksne poslovne logike; agregacije pripremljene u controller/viewmodel sloju.

**Decisions**
- U opsegu su iskljucivo read-only Index/Details i navigacija; nema Create/Edit/Delete i nema auth ekrana.
- Navigacijski prioritet ide na entitete koje korisnik dozivljava kao planinarsku evidenciju (KT, rute, podrucja, posjeti, medalje, korisnik).
- Reusable komponentni sustav u CSS-u je obvezan da bi UX bio dosljedan i odrziv kroz sve module.

**Further Considerations**
1. Za Podrucja i Kontrolne tocke preporuka je dodati vizualne indikatore napretka (npr. visited ratio) cim agregati budu dostupni.
2. Za Posjete preporuka je timeline varijanta uz tablicu, ali tek nakon bazne tablice i navigacije.
3. Za Udrugu preporuka je karticni listing ako postoji logo/slika, inace tablicni pregled radi citljivosti podataka.