using System.Globalization;

namespace planinarenje.Entiteti;

public class Lab1Podaci
{
    public List<Podrucje> Podrucja { get; init; } = new();
    public List<Korisnik> Korisnici { get; init; } = new();
    public List<Knjizica> Knjizice { get; init; } = new();
    public List<Medalja> Medalje { get; init; } = new();
    public List<PlaninarskaUdruga> PlaninarskeUdruge { get; init; } = new();
    public List<PlaninarskiObjekt> PlaninarskiObjekti { get; init; } = new();
    public List<KontrolnaTocka> KontrolneTocke { get; init; } = new();
    public List<Ruta> Rute { get; init; } = new();
    public List<Posjet> Posjeti { get; init; } = new();
    public List<Fotografija> Fotografije { get; init; } = new();
    public List<KorisnikMedalja> KorisnikMedalje { get; init; } = new();
}

public static class Lab1PodaciFactory
{
    public static Lab1Podaci Kreiraj()
    {
        var podrucja = new List<Podrucje>
        {
            new() { IdPodrucje = 1, Naziv = "Slavonija", Opis = "Nizinsko i brezuljkasto podrucje istocne Hrvatske s Papukom, Psunjem, Krndijom i drugim slavonskim gorjima.", Regija = "Istocna Hrvatska", MinimalanBrojKTZaObilazak = 2, UkupanBrojKT = 9 },
            new() { IdPodrucje = 2, Naziv = "Moslavacka gora i Bilogora", Opis = "Niza sumovita gorja s kracim planinarskim usponima i manjim brojem kontrolnih tocaka.", Regija = "Sredisnja Hrvatska", MinimalanBrojKTZaObilazak = 1, UkupanBrojKT = 2 },
            new() { IdPodrucje = 3, Naziv = "Hrvatsko zagorje i Medimurje", Opis = "Brezuljkasto podrucje s vidikovcima, utvrdama i poznatim vrhovima kao sto su Ivanscica i Ravna gora.", Regija = "Sjeverna Hrvatska", MinimalanBrojKTZaObilazak = 3, UkupanBrojKT = 9 },
            new() { IdPodrucje = 4, Naziv = "Medvednica", Opis = "Planina iznad Zagreba s gusto razvijenom mrezom putova, domova i kontrolnih tocaka.", Regija = "Sredisnja Hrvatska", MinimalanBrojKTZaObilazak = 2, UkupanBrojKT = 6 },
            new() { IdPodrucje = 5, Naziv = "Samoborsko gorje", Opis = "Popularno planinarsko podrucje zapadno od Zagreba, poznato po Okicu, Japeticu i Ostrcu.", Regija = "Sredisnja Hrvatska", MinimalanBrojKTZaObilazak = 2, UkupanBrojKT = 5 },
            new() { IdPodrucje = 6, Naziv = "Zumberacka gora", Opis = "Planinsko i granicno podrucje s visim vrhovima i rjede naseljenim grebenima.", Regija = "Sredisnja Hrvatska", MinimalanBrojKTZaObilazak = 1, UkupanBrojKT = 5 },
            new() { IdPodrucje = 7, Naziv = "Karlovacko pokuplje, Kordun i Banovina", Opis = "Podrucje nizih gora i sumovitih uzvisina juzno od Karlovca i prema Banovini.", Regija = "Sredisnja Hrvatska", MinimalanBrojKTZaObilazak = 1, UkupanBrojKT = 5 },
            new() { IdPodrucje = 8, Naziv = "Gorski kotar - juzni dio", Opis = "Dio Gorskog kotara s visim vrhovima, stjenovitim skupinama i zahtjevnijim usponima.", Regija = "Gorska Hrvatska", MinimalanBrojKTZaObilazak = 4, UkupanBrojKT = 14 },
            new() { IdPodrucje = 9, Naziv = "Gorski kotar - sjeverni dio", Opis = "Sumovito i planinsko podrucje s vrhovima poput Risnjaka, Snjeznika i Skradskog vrha.", Regija = "Gorska Hrvatska", MinimalanBrojKTZaObilazak = 3, UkupanBrojKT = 12 },
            new() { IdPodrucje = 10, Naziv = "Istra", Opis = "Podrucje Ucke i Cicarije s istaknutim obalnim i planinskim vidikovcima.", Regija = "Zapadna Hrvatska", MinimalanBrojKTZaObilazak = 2, UkupanBrojKT = 7 },
            new() { IdPodrucje = 11, Naziv = "Sjeverni Velebit", Opis = "Visokoplaninsko podrucje s izrazito atraktivnim velebitskim vrhovima i ostrim krsem.", Regija = "Primorsko-gorska Hrvatska", MinimalanBrojKTZaObilazak = 3, UkupanBrojKT = 9 },
            new() { IdPodrucje = 12, Naziv = "Srednji Velebit", Opis = "Sredisnji dio Velebita sa srednje zahtjevnim i zahtjevnim vrhovima i planinarskim kucama.", Regija = "Lika i Primorje", MinimalanBrojKTZaObilazak = 2, UkupanBrojKT = 9 },
            new() { IdPodrucje = 13, Naziv = "Juzni Velebit", Opis = "Najvisi i alpinisticki najdojmljiviji dio Velebita s Vaganskim vrhom i Svetim brdom.", Regija = "Lika i Dalmacija", MinimalanBrojKTZaObilazak = 3, UkupanBrojKT = 10 },
            new() { IdPodrucje = 14, Naziv = "Lika", Opis = "Prostrano podrucje lickih planina i osamljenih vrhova izvan glavnog velebitskog lanca.", Regija = "Lika", MinimalanBrojKTZaObilazak = 1, UkupanBrojKT = 5 },
            new() { IdPodrucje = 15, Naziv = "Jadranski otoci - sjeverni dio", Opis = "Sjeverni jadranski otoci s nizim, ali vrlo atraktivnim otocnim vrhovima.", Regija = "Jadranska Hrvatska", MinimalanBrojKTZaObilazak = 1, UkupanBrojKT = 5 },
            new() { IdPodrucje = 16, Naziv = "Jadranski otoci - juzni dio", Opis = "Juzni jadranski otoci s vecim brojem otocnih vrhova i raznolikim podlogama.", Regija = "Jadranska Hrvatska", MinimalanBrojKTZaObilazak = 2, UkupanBrojKT = 10 },
            new() { IdPodrucje = 17, Naziv = "Dalmatinska zagora", Opis = "Podrucje Dinare, Promine, Svilaje i drugih planina dalmatinskog zaleda.", Regija = "Dalmatinsko zaledje", MinimalanBrojKTZaObilazak = 2, UkupanBrojKT = 8 },
            new() { IdPodrucje = 18, Naziv = "Dalmacija", Opis = "Priobalno i zaledno podrucje srednje Dalmacije s planinama uz obalu i u zaleđu.", Regija = "Dalmacija", MinimalanBrojKTZaObilazak = 2, UkupanBrojKT = 9 },
            new() { IdPodrucje = 19, Naziv = "Biokovo i Zagora", Opis = "Krsevito visokoplaninsko podrucje Biokova i zaleda s vrlo izrazenim visinskim razlikama.", Regija = "Juzna Dalmacija", MinimalanBrojKTZaObilazak = 3, UkupanBrojKT = 10 },
            new() { IdPodrucje = 20, Naziv = "Dubrovacko podrucje", Opis = "Juznohrvatsko podrucje s manjim brojem, ali vrlo atraktivnih kontrolnih tocaka.", Regija = "Krajnji jug Hrvatske", MinimalanBrojKTZaObilazak = 1, UkupanBrojKT = 3 }
        };

        var korisnici = new List<Korisnik>
        {
            new()
            {
                IdKorisnik = 1,
                Ime = "Luka",
                Prezime = "Bosnjak",
                Email = "luka.bosnjak92@gmail.com",
                KorisnickoIme = "Boss",
                PasswordHash = "123456789",
                DatumRodenja = D("2004-06-29"),
                DatumRegistracije = DT("2026-04-01 09:00:00"),
                BrojMobitela = "0979545897",
                ProfilnaSlika = @"C:\Users\lukab\Documents\Projekt\Razvoj-web-aplikacija-u-ASP.NET-MVC-tehnologiji\Slike\Profil\Boss.jpeg",
                StatusAktivan = true
            },
            new()
            {
                IdKorisnik = 2,
                Ime = "Test",
                Prezime = "Test",
                Email = "test123@gmail.com",
                KorisnickoIme = "Test",
                PasswordHash = "123456789",
                DatumRodenja = D("2005-01-01"),
                DatumRegistracije = DT("2026-04-01 09:15:00"),
                BrojMobitela = null,
                ProfilnaSlika = @"C:\Users\lukab\Documents\Projekt\Razvoj-web-aplikacija-u-ASP.NET-MVC-tehnologiji\Slike\Profil\test.jpg",
                StatusAktivan = true
            }
        };

        var knjizice = new List<Knjizica>
        {
            new() { IdKnjizica = 1, IdKorisnik = 1, DatumKreiranja = DT("2026-04-01 09:05:00"), Napomena = "Glavna digitalna knjizica korisnika Luka Bosnjak.", StatusAktivna = true },
            new() { IdKnjizica = 2, IdKorisnik = 2, DatumKreiranja = DT("2026-04-01 09:20:00"), Napomena = "Testna digitalna knjizica za provjeru funkcionalnosti aplikacije.", StatusAktivna = true }
        };

        var medalje = new List<Medalja>
        {
            new() { IdMedalja = 1, Naziv = "Pocetnik", Opis = "Osnovna medalja za prvi ispravno evidentirani obilazak podrucja.", MinimalanBrojKontrolnihTocaka = 1, MinimalanBrojPodrucja = 1 },
            new() { IdMedalja = 2, Naziv = "Broncana znacka", Opis = "Potrebno je obici zadani broj KT-a iz 10 podrucja i ukupno 25 KT-a.", MinimalanBrojKontrolnihTocaka = 25, MinimalanBrojPodrucja = 10 },
            new() { IdMedalja = 3, Naziv = "Srebrna znacka", Opis = "Potrebno je obici zadani broj KT-a iz 15 podrucja i ukupno 50 KT-a, uz obaveznu Dinaru (Sinjal).", MinimalanBrojKontrolnihTocaka = 50, MinimalanBrojPodrucja = 15 },
            new() { IdMedalja = 4, Naziv = "Zlatna znacka", Opis = "Potrebno je obici zadani broj KT-a iz svih 20 podrucja i ukupno 75 KT-a.", MinimalanBrojKontrolnihTocaka = 75, MinimalanBrojPodrucja = 20 },
            new() { IdMedalja = 5, Naziv = "Posebno priznanje", Opis = "Potrebno je obici 100 KT-a uz ispunjene uvjete za zlatnu znacku.", MinimalanBrojKontrolnihTocaka = 100, MinimalanBrojPodrucja = 20 },
            new() { IdMedalja = 6, Naziv = "Visoko priznanje", Opis = "Potrebno je obici 125 KT-a uz ispunjene uvjete za posebno priznanje.", MinimalanBrojKontrolnihTocaka = 125, MinimalanBrojPodrucja = 20 },
            new() { IdMedalja = 7, Naziv = "Najvise priznanje", Opis = "Potrebno je obici 155 KT-a uz ispunjene uvjete za visoko priznanje.", MinimalanBrojKontrolnihTocaka = 155, MinimalanBrojPodrucja = 20 }
        };

        var udruge = new List<PlaninarskaUdruga>
        {
            new() { IdPlaninarskaUdruga = 1, OIB = "40461293872", Naziv = "HPD Mosor", Email = "hpd.mosor@hps.hr", BrojTelefona = null, Adresa = "p.p. 233", PostanskiBroj = "21000", Grad = "Split", Zupanija = "Splitsko-dalmatinska", BrojClanova = 350 },
            new() { IdPlaninarskaUdruga = 2, OIB = "48938096579", Naziv = "HPD Gora", Email = "hpd.gora@hps.hr", BrojTelefona = null, Adresa = "Dubravica 27a", PostanskiBroj = "10000", Grad = "Zagreb", Zupanija = "Grad Zagreb", BrojClanova = 180 },
            new() { IdPlaninarskaUdruga = 3, OIB = "95873199484", Naziv = "PD Zavizan", Email = "pd.zavizan@hps.hr", BrojTelefona = null, Adresa = "Mala vrata 20", PostanskiBroj = "53270", Grad = "Senj", Zupanija = "Licko-senjska", BrojClanova = 120 },
            new() { IdPlaninarskaUdruga = 4, OIB = "92966614510", Naziv = "PD Paklenica", Email = "pd.paklenica@hps.hr", BrojTelefona = null, Adresa = "Majke Margarite 6", PostanskiBroj = "23000", Grad = "Zadar", Zupanija = "Zadarska", BrojClanova = 220 },
            new() { IdPlaninarskaUdruga = 5, OIB = "12345678901", Naziv = "PD Dr. Maks Plotnikov", Email = "info@pddr-maks-plotnikov.hr", BrojTelefona = "0991234567", Adresa = "Andrije Hebranga 26", PostanskiBroj = "10430", Grad = "Samobor", Zupanija = "Zagrebacka", BrojClanova = 95 }
        };

        var objekti = new List<PlaninarskiObjekt>
        {
            new() { IdPlaninarskiObjekt = 1, IdPodrucje = 5, IdPlaninarskaUdruga = 5, Naziv = "Planinarski dom Dr. Maks Plotnikov", TipObjekta = TipObjekta.Dom, NadmorskaVisina = 411, Kapacitet = 14, Opis = "Dom podno rusevina Okic-grada; polazisna je tocka za Okic i okolne putove.", ImeOdgovorneOsobe = "Stjepan Jandrecic", Telefon = "0918909624", Email = "aplantosar@gmail.com", Adresa = "Okic, Samobor", ImaNocenje = true, ImaHranu = true, RadnoVrijemeOpis = "Otvoren vikendom i blagdanima." },
            new() { IdPlaninarskiObjekt = 2, IdPodrucje = 5, IdPlaninarskaUdruga = 2, Naziv = "Planinarski dom Zeljeznicar", TipObjekta = TipObjekta.Dom, NadmorskaVisina = 691, Kapacitet = 25, Opis = "Popularan planinarski dom u Samoborskom i Zumberackom gorju.", ImeOdgovorneOsobe = "Dezurni domar", Telefon = null, Email = null, Adresa = "Samoborsko gorje", ImaNocenje = true, ImaHranu = true, RadnoVrijemeOpis = "Prema rasporedu dezurstva i vikendom." },
            new() { IdPlaninarskiObjekt = 3, IdPodrucje = 11, IdPlaninarskaUdruga = 3, Naziv = "Planinarska kuca Sijaset", TipObjekta = TipObjekta.Kuca, NadmorskaVisina = 328, Kapacitet = 12, Opis = "Nizi planinarski objekt na Velebitu, pogodan kao polaziste za ture.", ImeOdgovorneOsobe = "Dezurni clan drustva", Telefon = null, Email = "pd.zavizan@hps.hr", Adresa = "Velebit, Senj", ImaNocenje = true, ImaHranu = false, RadnoVrijemeOpis = "Povremeno otvorena ili po dogovoru." },
            new() { IdPlaninarskiObjekt = 4, IdPodrucje = 13, IdPlaninarskaUdruga = 4, Naziv = "Planinarski dom Paklenica", TipObjekta = TipObjekta.Dom, NadmorskaVisina = 480, Kapacitet = 44, Opis = "Dom na pocetku klanca Velike Paklenice s hranom, picem i nocenjem.", ImeOdgovorneOsobe = "Irena Saran", Telefon = "0977557654", Email = "pd.paklenica@hps.hr", Adresa = "Velika Paklenica", ImaNocenje = true, ImaHranu = true, RadnoVrijemeOpis = "Otvoren stalno." },
            new() { IdPlaninarskiObjekt = 5, IdPodrucje = 18, IdPlaninarskaUdruga = 1, Naziv = "Planinarska kuca Lugarnica", TipObjekta = TipObjekta.Kuca, NadmorskaVisina = 872, Kapacitet = 20, Opis = "Planinarska kuca na Mosoru pogodna za krace i srednje duge uspone.", ImeOdgovorneOsobe = "Dezurna osoba drustva", Telefon = null, Email = "hpd.mosor@hps.hr", Adresa = "Mosor, Split", ImaNocenje = true, ImaHranu = false, RadnoVrijemeOpis = "Otvorenost prema obavijesti drustva." }
        };

        var kontrolneTocke = new List<KontrolnaTocka>
        {
            new() { IdKontrolnaTocka = 1, GUIDOznaka = "KT-HPO-2-1-VIS", IdPodrucje = 2, Naziv = "Moslavacka gora - vrh Vis", TipKontrolneTocke = TipKontrolneTocke.Vrh, NadmorskaVisina = 437, Opis = "Najvisi vrh Moslavacke gore i dobra pocetna kontrolna tocka za pocetnicke obilaznike.", Koordinate = "N/A", OpisZiga = "Metalni zig na vrsnoj oznaci." },
            new() { IdKontrolnaTocka = 2, GUIDOznaka = "KT-HPO-4-4-SLJEME", IdPodrucje = 4, Naziv = "Sljeme - vrh", TipKontrolneTocke = TipKontrolneTocke.Vrh, NadmorskaVisina = 1033, Opis = "Najvisi vrh Medvednice; vrh je lako dostupan i planinarima i izletnicima.", Koordinate = "N 45 53' 57.4'' E 15 56' 50.6''", OpisZiga = "Metalni zig vrha nalazi se na promidzbenom panou kod televizijskog tornja." },
            new() { IdKontrolnaTocka = 3, GUIDOznaka = "KT-HPO-5-1-OKIC", IdPodrucje = 5, Naziv = "Okic - vrh", TipKontrolneTocke = TipKontrolneTocke.Vrh, NadmorskaVisina = 499, Opis = "Stari grad i vrsna gradina s vidikom prema Zagrebu i Medvednici.", Koordinate = "N 45 44' 55.4'' E 15 42' 24.0''", OpisZiga = "Metalni zig vrha ugraden je na zid u najvisem dijelu gradine." },
            new() { IdKontrolnaTocka = 4, GUIDOznaka = "KT-HPO-5-4-JAPETIC", IdPodrucje = 5, Naziv = "Japetic - vrh", TipKontrolneTocke = TipKontrolneTocke.Vrh, NadmorskaVisina = 879, Opis = "Najvisi vrh Samoborskoga gorja; poznat po piramidi i domu Zitnica.", Koordinate = "N 45 44' 56.3'' E 15 36' 32.8''", OpisZiga = "Metalni zig ugraden je na konstrukciju piramide." },
            new() { IdKontrolnaTocka = 5, GUIDOznaka = "KT-HPO-11-2-ZAVIZAN", IdPodrucje = 11, Naziv = "Veliki Zavizan - vrh", TipKontrolneTocke = TipKontrolneTocke.Vrh, NadmorskaVisina = 1676, Opis = "Jedan od najpoznatijih vrhova Sjevernog Velebita s vrlo atraktivnim pogledima.", Koordinate = "N/A", OpisZiga = "Zig kontrolne tocke nalazi se na vrhu ili u blizini planinarskog doma Zavizan." }
        };

        var rute = new List<Ruta>
        {
            new() { IdRuta = 1, IdKontrolnaTocka = 1, Naziv = "Kutina - Humka - Vis", Pocetak = "Kutina / Humka", Kraj = "Vrh Vis", VrijemeHodaMin = 90, DuljinaKm = 4.5m, VisinskaRazlikaM = 260, Opis = "Primjer krace rute do najvise tocke Moslavacke gore.", OznakaNaTerenu = "MG-01", GodinaObnove = 2023, Napomena = "Pogodna za pocetnike.", TezinaRute = TezinaRute.Laka, GPXPath = @"C:\GPX\ruta_vis.gpx" },
            new() { IdRuta = 2, IdKontrolnaTocka = 2, Naziv = "Gracani - Puntijarka - Sljeme", Pocetak = "Gracani", Kraj = "Sljeme", VrijemeHodaMin = 150, DuljinaKm = 8.2m, VisinskaRazlikaM = 780, Opis = "Popularan uspon preko Puntijarke prema vrhu Medvednice.", OznakaNaTerenu = "M-04", GodinaObnove = 2022, Napomena = "Jedna od najcesce koristenih ruta na Medvednici.", TezinaRute = TezinaRute.Srednja, GPXPath = @"C:\GPX\ruta_sljeme.gpx" },
            new() { IdRuta = 3, IdKontrolnaTocka = 3, Naziv = "Klake - pl. dom pod Okicem - Okic-grad", Pocetak = "Klake", Kraj = "Okic - vrh", VrijemeHodaMin = 40, DuljinaKm = 1.8m, VisinskaRazlikaM = 210, Opis = "Najkraci klasicni prilaz vrhu Okic preko doma pod Okicem.", OznakaNaTerenu = "SG-01", GodinaObnove = 2021, Napomena = "Strmiji zavrsni dio prema gradini.", TezinaRute = TezinaRute.Srednja, GPXPath = @"C:\GPX\ruta_okic.gpx" },
            new() { IdRuta = 4, IdKontrolnaTocka = 4, Naziv = "Soiceva kuca - Japetic", Pocetak = "Soiceva kuca", Kraj = "Japetic - vrh", VrijemeHodaMin = 90, DuljinaKm = 5.4m, VisinskaRazlikaM = 430, Opis = "Klasican prilaz preko livada i Katina krca prema vrhu Japetic.", OznakaNaTerenu = "SG-04", GodinaObnove = 2020, Napomena = "Ruta je pregledna i cesto koristena.", TezinaRute = TezinaRute.Srednja, GPXPath = @"C:\GPX\ruta_japetic.gpx" },
            new() { IdRuta = 5, IdKontrolnaTocka = 5, Naziv = "Dom Zavizan - Veliki Zavizan - dom Zavizan", Pocetak = "Planinarski dom Zavizan", Kraj = "Veliki Zavizan", VrijemeHodaMin = 150, DuljinaKm = 6.7m, VisinskaRazlikaM = 320, Opis = "Kruzna tura s polaskom od doma Zavizan preko Balinovca do Velikog Zavižana.", OznakaNaTerenu = "SV-02", GodinaObnove = 2024, Napomena = "U nepovoljnim uvjetima potreban dodatni oprez.", TezinaRute = TezinaRute.Teska, GPXPath = @"C:\GPX\ruta_zavizan.gpx" }
        };

        var posjeti = new List<Posjet>
        {
            new() { IdPosjet = 1, IdKorisnik = 1, IdKnjizica = 1, IdKontrolnaTocka = 1, IdRuta = 1, DatumVrijemePosjeta = DT("2026-04-05 10:15:00"), VrijemeUsponaMin = 92, DozivljajPosjeta = DozivljajPosjeta.VrloLagano, OpisIskustva = "Prvi evidentirani uspon u aplikaciji. Lagana i ugodna ruta po suhom vremenu.", UneseniGUID = "KT-HPO-2-1-VIS", JeLiPotvrdenPosjet = true, DatumKreiranjaZapisa = DT("2026-04-05 12:00:00") },
            new() { IdPosjet = 2, IdKorisnik = 1, IdKnjizica = 1, IdKontrolnaTocka = 3, IdRuta = 3, DatumVrijemePosjeta = DT("2026-04-12 08:40:00"), VrijemeUsponaMin = 43, DozivljajPosjeta = DozivljajPosjeta.KratkoAliTesko, OpisIskustva = "Kratak, ali strm zavrsni dio prema gradini Okic.", UneseniGUID = "KT-HPO-5-1-OKIC", JeLiPotvrdenPosjet = true, DatumKreiranjaZapisa = DT("2026-04-12 10:00:00") },
            new() { IdPosjet = 3, IdKorisnik = 1, IdKnjizica = 1, IdKontrolnaTocka = 4, IdRuta = 4, DatumVrijemePosjeta = DT("2026-04-19 09:10:00"), VrijemeUsponaMin = 96, DozivljajPosjeta = DozivljajPosjeta.Srednje, OpisIskustva = "Ugodna tura s dobrim vremenom i lijepim pogledima s piramide.", UneseniGUID = "KT-HPO-5-4-JAPETIC", JeLiPotvrdenPosjet = true, DatumKreiranjaZapisa = DT("2026-04-19 11:10:00") },
            new() { IdPosjet = 4, IdKorisnik = 2, IdKnjizica = 2, IdKontrolnaTocka = 1, IdRuta = 1, DatumVrijemePosjeta = DT("2026-04-08 11:00:00"), VrijemeUsponaMin = 95, DozivljajPosjeta = DozivljajPosjeta.Lagano, OpisIskustva = "Testni korisnik uspjesno evidentirao svoj prvi posjet i time zadovoljio uvjet za pocetnicku medalju.", UneseniGUID = "KT-HPO-2-1-VIS", JeLiPotvrdenPosjet = true, DatumKreiranjaZapisa = DT("2026-04-08 12:45:00") },
            new() { IdPosjet = 5, IdKorisnik = 2, IdKnjizica = 2, IdKontrolnaTocka = 2, IdRuta = 2, DatumVrijemePosjeta = DT("2026-04-26 07:50:00"), VrijemeUsponaMin = 155, DozivljajPosjeta = DozivljajPosjeta.FizickiNaporno, OpisIskustva = "Duzi uspon do Sljemena preko Puntijarke, ali bez tehnicki teskih dijelova.", UneseniGUID = "KT-HPO-4-4-SLJEME", JeLiPotvrdenPosjet = true, DatumKreiranjaZapisa = DT("2026-04-26 10:40:00") }
        };

        var fotografije = new List<Fotografija>
        {
            new() { IdFotografija = 1, IdPosjet = 1, NazivDatoteke = "vis_luka_selfie.jpg", PutanjaDatoteke = @"C:\Slike\Posjeti\vis_luka_selfie.jpg", DatumUploada = DT("2026-04-05 12:05:00"), TipSlike = TipSlike.Selfie, Opis = "Selfie korisnika Luke na vrhu Vis." },
            new() { IdFotografija = 2, IdPosjet = 2, NazivDatoteke = "okic_luka_selfie.jpg", PutanjaDatoteke = @"C:\Slike\Posjeti\okic_luka_selfie.jpg", DatumUploada = DT("2026-04-12 10:05:00"), TipSlike = TipSlike.Selfie, Opis = "Fotografija Luke kod oznake vrha Okic." },
            new() { IdFotografija = 3, IdPosjet = 3, NazivDatoteke = "japetic_luka_selfie.jpg", PutanjaDatoteke = @"C:\Slike\Posjeti\japetic_luka_selfie.jpg", DatumUploada = DT("2026-04-19 11:15:00"), TipSlike = TipSlike.Selfie, Opis = "Selfie na vrhu Japetic uz piramidu." },
            new() { IdFotografija = 4, IdPosjet = 4, NazivDatoteke = "vis_test_selfie.jpg", PutanjaDatoteke = @"C:\Slike\Posjeti\vis_test_selfie.jpg", DatumUploada = DT("2026-04-08 12:50:00"), TipSlike = TipSlike.Selfie, Opis = "Testni korisnik na vrhu Vis." },
            new() { IdFotografija = 5, IdPosjet = 5, NazivDatoteke = "sljeme_test_selfie.jpg", PutanjaDatoteke = @"C:\Slike\Posjeti\sljeme_test_selfie.jpg", DatumUploada = DT("2026-04-26 10:45:00"), TipSlike = TipSlike.Selfie, Opis = "Testni korisnik na vrhu Sljeme kod oznake." }
        };

        var korisnikMedalje = new List<KorisnikMedalja>
        {
            new() { IdKorisnikMedalja = 1, IdKorisnik = 1, IdMedalja = 1, DatumDodjele = DT("2026-04-19 12:00:00"), Napomena = "Korisnik je zadovoljio uvjet pocetnicke medalje jer ima evidentiran obilazak podrucja 2 (Moslavacka gora i Bilogora), gdje je prag 1 KT." },
            new() { IdKorisnikMedalja = 2, IdKorisnik = 2, IdMedalja = 1, DatumDodjele = DT("2026-04-08 13:00:00"), Napomena = "Korisnik je zadovoljio uvjet pocetnicke medalje jer ima evidentiran obilazak podrucja 2 (Moslavacka gora i Bilogora), gdje je prag 1 KT." }
        };

        PoveziRelacije(podrucja, korisnici, knjizice, medalje, udruge, objekti, kontrolneTocke, rute, posjeti, fotografije, korisnikMedalje);

        return new Lab1Podaci
        {
            Podrucja = podrucja,
            Korisnici = korisnici,
            Knjizice = knjizice,
            Medalje = medalje,
            PlaninarskeUdruge = udruge,
            PlaninarskiObjekti = objekti,
            KontrolneTocke = kontrolneTocke,
            Rute = rute,
            Posjeti = posjeti,
            Fotografije = fotografije,
            KorisnikMedalje = korisnikMedalje
        };
    }

    private static void PoveziRelacije(
        List<Podrucje> podrucja,
        List<Korisnik> korisnici,
        List<Knjizica> knjizice,
        List<Medalja> medalje,
        List<PlaninarskaUdruga> udruge,
        List<PlaninarskiObjekt> objekti,
        List<KontrolnaTocka> kontrolneTocke,
        List<Ruta> rute,
        List<Posjet> posjeti,
        List<Fotografija> fotografije,
        List<KorisnikMedalja> korisnikMedalje)
    {
        foreach (var knjizica in knjizice)
        {
            var korisnik = korisnici.Single(k => k.IdKorisnik == knjizica.IdKorisnik);
            knjizica.Korisnik = korisnik;
            korisnik.Knjizica = knjizica;
        }

        foreach (var objekt in objekti)
        {
            objekt.Podrucje = podrucja.Single(p => p.IdPodrucje == objekt.IdPodrucje);
            objekt.PlaninarskaUdruga = udruge.Single(u => u.IdPlaninarskaUdruga == objekt.IdPlaninarskaUdruga);

            objekt.Podrucje.PlaninarskiObjekti.Add(objekt);
            objekt.PlaninarskaUdruga.PlaninarskiObjekti.Add(objekt);
        }

        foreach (var kontrolnaTocka in kontrolneTocke)
        {
            kontrolnaTocka.Podrucje = podrucja.Single(p => p.IdPodrucje == kontrolnaTocka.IdPodrucje);
            kontrolnaTocka.Podrucje.KontrolneTocke.Add(kontrolnaTocka);
        }

        foreach (var ruta in rute)
        {
            ruta.KontrolnaTocka = kontrolneTocke.Single(kt => kt.IdKontrolnaTocka == ruta.IdKontrolnaTocka);
            ruta.KontrolnaTocka.Rute.Add(ruta);
        }

        foreach (var posjet in posjeti)
        {
            posjet.Korisnik = korisnici.Single(k => k.IdKorisnik == posjet.IdKorisnik);
            posjet.Knjizica = knjizice.Single(k => k.IdKnjizica == posjet.IdKnjizica);
            posjet.KontrolnaTocka = kontrolneTocke.Single(kt => kt.IdKontrolnaTocka == posjet.IdKontrolnaTocka);
            posjet.Ruta = rute.Single(r => r.IdRuta == posjet.IdRuta);

            posjet.Korisnik.Posjeti.Add(posjet);
            posjet.Knjizica.Posjeti.Add(posjet);
            posjet.KontrolnaTocka.Posjeti.Add(posjet);
            posjet.Ruta.Posjeti.Add(posjet);
        }

        foreach (var fotografija in fotografije)
        {
            fotografija.Posjet = posjeti.Single(p => p.IdPosjet == fotografija.IdPosjet);
            fotografija.Posjet.Fotografije.Add(fotografija);
        }

        foreach (var korisnikMedalja in korisnikMedalje)
        {
            korisnikMedalja.Korisnik = korisnici.Single(k => k.IdKorisnik == korisnikMedalja.IdKorisnik);
            korisnikMedalja.Medalja = medalje.Single(m => m.IdMedalja == korisnikMedalja.IdMedalja);

            korisnikMedalja.Korisnik.KorisnikMedalje.Add(korisnikMedalja);
            korisnikMedalja.Medalja.KorisnikMedalje.Add(korisnikMedalja);
        }
    }

    private static DateTime DT(string value)
    {
        return DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
    }

    private static DateTime D(string value)
    {
        return DateTime.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}
