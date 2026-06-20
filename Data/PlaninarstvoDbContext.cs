using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using planinarenje.Entiteti;

namespace planinarenje.Data;

public class PlaninarstvoDbContext : IdentityDbContext<AppUser>
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public PlaninarstvoDbContext(DbContextOptions<PlaninarstvoDbContext> options, IHttpContextAccessor? httpContextAccessor = null)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<LogAktivnosti> LogoviAktivnosti { get; set; }
    public DbSet<Korisnik> Korisnici { get; set; }
    public DbSet<Knjizica> Knjizice { get; set; }
    public DbSet<Posjet> Posjeti { get; set; }
    public DbSet<Fotografija> Fotografije { get; set; }
    public DbSet<KontrolnaTocka> KontrolneTocke { get; set; }
    public DbSet<Ruta> Rute { get; set; }
    public DbSet<Podrucje> Podrucja { get; set; }
    public DbSet<PlaninarskiObjekt> PlaninarskiObjekti { get; set; }
    public DbSet<PlaninarskaUdruga> PlaninarskeUdruge { get; set; }
    public DbSet<Medalja> Medalje { get; set; }
    public DbSet<KorisnikMedalja> KorisnikMedalje { get; set; }
    public DbSet<Obavijest> Obavijesti { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ruta>()
            .Property(r => r.DuljinaKm)
            .HasPrecision(5, 2);

        modelBuilder.Entity<Knjizica>()
            .HasIndex(k => k.IdKorisnik)
            .IsUnique();

        modelBuilder.Entity<Korisnik>()
            .HasIndex(k => k.Email)
            .IsUnique();

        modelBuilder.Entity<Korisnik>()
            .HasIndex(k => k.KorisnickoIme)
            .IsUnique();

        modelBuilder.Entity<KontrolnaTocka>()
            .HasIndex(kt => kt.GUIDOznaka)
            .IsUnique();

        modelBuilder.Entity<PlaninarskaUdruga>()
            .HasIndex(pu => pu.OIB)
            .IsUnique();

        modelBuilder.Entity<Podrucje>().HasData(
            new Podrucje
            {
                IdPodrucje = 1,
                Naziv = "Slavonija",
                Opis = "Nizinsko i brežuljkasto područje istočne Hrvatske s Papukom, Psunjem, Krndijom i drugim slavonskim gorjima.",
                Regija = "Istočna Hrvatska",
                MinimalanBrojKTZaObilazak = 2
            },
            new Podrucje
            {
                IdPodrucje = 2,
                Naziv = "Moslavačka gora i Bilogora",
                Opis = "Niža šumovita gorja s kraćim planinarskim usponima i manjim brojem kontrolnih točaka.",
                Regija = "Središnja Hrvatska",
                MinimalanBrojKTZaObilazak = 1
            },
            new Podrucje
            {
                IdPodrucje = 3,
                Naziv = "Hrvatsko zagorje i Međimurje",
                Opis = "Brežuljkasto područje s vidikovcima, utvrdama i poznatim vrhovima kao što su Ivanščica i Ravna gora.",
                Regija = "Sjeverna Hrvatska",
                MinimalanBrojKTZaObilazak = 3
            },
            new Podrucje
            {
                IdPodrucje = 4,
                Naziv = "Medvednica",
                Opis = "Planina iznad Zagreba s gusto razvijenom mrežom putova, domova i kontrolnih točaka.",
                Regija = "Središnja Hrvatska",
                MinimalanBrojKTZaObilazak = 2
            },
            new Podrucje
            {
                IdPodrucje = 5,
                Naziv = "Samoborsko gorje",
                Opis = "Popularno planinarsko područje zapadno od Zagreba, poznato po Okiću, Japetiću i Oštrcu.",
                Regija = "Središnja Hrvatska",
                MinimalanBrojKTZaObilazak = 2
            },
            new Podrucje
            {
                IdPodrucje = 6,
                Naziv = "Žumberačka gora",
                Opis = "Planinsko i granično područje s višim vrhovima i rjeđe naseljenim grebenima.",
                Regija = "Središnja Hrvatska",
                MinimalanBrojKTZaObilazak = 1
            },
            new Podrucje
            {
                IdPodrucje = 7,
                Naziv = "Karlovačko pokuplje, Kordun i Banovina",
                Opis = "Područje nižih gora i šumovitih uzvisina južno od Karlovca i prema Banovini.",
                Regija = "Središnja Hrvatska",
                MinimalanBrojKTZaObilazak = 1
            },
            new Podrucje
            {
                IdPodrucje = 8,
                Naziv = "Gorski kotar - južni dio",
                Opis = "Dio Gorskog kotara s višim vrhovima, stjenovitim skupinama i zahtjevnijim usponima.",
                Regija = "Gorska Hrvatska",
                MinimalanBrojKTZaObilazak = 4
            },
            new Podrucje
            {
                IdPodrucje = 9,
                Naziv = "Gorski kotar - sjeverni dio",
                Opis = "Šumovito i planinsko područje s vrhovima poput Risnjaka, Snježnika i Skradskog vrha.",
                Regija = "Gorska Hrvatska",
                MinimalanBrojKTZaObilazak = 3
            },
            new Podrucje
            {
                IdPodrucje = 10,
                Naziv = "Istra",
                Opis = "Područje Učke i Ćićarije s istaknutim obalnim i planinskim vidikovcima.",
                Regija = "Zapadna Hrvatska",
                MinimalanBrojKTZaObilazak = 2
            },
            new Podrucje
            {
                IdPodrucje = 11,
                Naziv = "Sjeverni Velebit",
                Opis = "Visokoplaninsko područje s izrazito atraktivnim velebitskim vrhovima i oštrim kršem.",
                Regija = "Primorsko-gorska Hrvatska",
                MinimalanBrojKTZaObilazak = 3
            },
            new Podrucje
            {
                IdPodrucje = 12,
                Naziv = "Srednji Velebit",
                Opis = "Središnji dio Velebita sa srednje zahtjevnim i zahtjevnim vrhovima i planinarskim kućama.",
                Regija = "Lika i Primorje",
                MinimalanBrojKTZaObilazak = 2
            },
            new Podrucje
            {
                IdPodrucje = 13,
                Naziv = "Južni Velebit",
                Opis = "Najviši i alpinistički najdojmljiviji dio Velebita s Vaganskim vrhom i Svetim brdom.",
                Regija = "Lika i Dalmacija",
                MinimalanBrojKTZaObilazak = 3
            },
            new Podrucje
            {
                IdPodrucje = 14,
                Naziv = "Lika",
                Opis = "Prostrano područje ličkih planina i osamljenih vrhova izvan glavnog velebitskog lanca.",
                Regija = "Lika",
                MinimalanBrojKTZaObilazak = 1
            },
            new Podrucje
            {
                IdPodrucje = 15,
                Naziv = "Jadranski otoci - sjeverni dio",
                Opis = "Sjeverni jadranski otoci s nižim, ali vrlo atraktivnim otočnim vrhovima.",
                Regija = "Jadranska Hrvatska",
                MinimalanBrojKTZaObilazak = 1
            },
            new Podrucje
            {
                IdPodrucje = 16,
                Naziv = "Jadranski otoci - južni dio",
                Opis = "Južni jadranski otoci s većim brojem otočnih vrhova i raznolikim podlogama.",
                Regija = "Jadranska Hrvatska",
                MinimalanBrojKTZaObilazak = 2
            },
            new Podrucje
            {
                IdPodrucje = 17,
                Naziv = "Dalmatinska zagora",
                Opis = "Područje Dinare, Promine, Svilaje i drugih planina dalmatinskog zaleđa.",
                Regija = "Dalmatinsko zaleđe",
                MinimalanBrojKTZaObilazak = 2
            },
            new Podrucje
            {
                IdPodrucje = 18,
                Naziv = "Dalmacija",
                Opis = "Priobalno i zaleđno područje srednje Dalmacije s planinama uz obalu i u zaleđu.",
                Regija = "Dalmacija",
                MinimalanBrojKTZaObilazak = 2
            },
            new Podrucje
            {
                IdPodrucje = 19,
                Naziv = "Biokovo i Zagora",
                Opis = "Krševito visokoplaninsko područje Biokova i zaleđa s vrlo izraženim visinskim razlikama.",
                Regija = "Južna Dalmacija",
                MinimalanBrojKTZaObilazak = 3
            },
            new Podrucje
            {
                IdPodrucje = 20,
                Naziv = "Dubrovačko područje",
                Opis = "Južnohrvatsko područje s manjim brojem, ali vrlo atraktivnih kontrolnih točaka.",
                Regija = "Krajnji jug Hrvatske",
                MinimalanBrojKTZaObilazak = 1
            }
        );

        modelBuilder.Entity<Medalja>().HasData(
            new Medalja
            {
                IdMedalja = 1,
                Naziv = "Početnik",
                Opis = "Osnovna medalja za prvi ispravno evidentirani obilazak područja.",
                MinimalanBrojKontrolnihTocaka = 1,
                MinimalanBrojPodrucja = 1
            },
            new Medalja
            {
                IdMedalja = 2,
                Naziv = "Brončana značka",
                Opis = "Potrebno je obići zadani broj KT-a iz 10 područja i ukupno 25 KT-a.",
                MinimalanBrojKontrolnihTocaka = 25,
                MinimalanBrojPodrucja = 10
            },
            new Medalja
            {
                IdMedalja = 3,
                Naziv = "Srebrna značka",
                Opis = "Potrebno je obići zadani broj KT-a iz 15 područja i ukupno 50 KT-a, uz obaveznu Dinaru (Sinjal).",
                MinimalanBrojKontrolnihTocaka = 50,
                MinimalanBrojPodrucja = 15
            },
            new Medalja
            {
                IdMedalja = 4,
                Naziv = "Zlatna značka",
                Opis = "Potrebno je obići zadani broj KT-a iz svih 20 područja i ukupno 75 KT-a.",
                MinimalanBrojKontrolnihTocaka = 75,
                MinimalanBrojPodrucja = 20
            },
            new Medalja
            {
                IdMedalja = 5,
                Naziv = "Posebno priznanje",
                Opis = "Potrebno je obići 100 KT-a uz ispunjene uvjete za zlatnu značku.",
                MinimalanBrojKontrolnihTocaka = 100,
                MinimalanBrojPodrucja = 20
            },
            new Medalja
            {
                IdMedalja = 6,
                Naziv = "Visoko priznanje",
                Opis = "Potrebno je obići 125 KT-a uz ispunjene uvjete za posebno priznanje.",
                MinimalanBrojKontrolnihTocaka = 125,
                MinimalanBrojPodrucja = 20
            },
            new Medalja
            {
                IdMedalja = 7,
                Naziv = "Najviše priznanje",
                Opis = "Potrebno je obići 155 KT-a uz ispunjene uvjete za visoko priznanje.",
                MinimalanBrojKontrolnihTocaka = 155,
                MinimalanBrojPodrucja = 20
            }
        );

        modelBuilder.Entity<PlaninarskaUdruga>().HasData(
            new PlaninarskaUdruga
            {
                IdPlaninarskaUdruga = 1,
                OIB = "40461293872",
                Naziv = "HPD Mosor",
                Email = "hpd.mosor@hps.hr",
                BrojTelefona = null,
                Adresa = "p.p. 233",
                PostanskiBroj = "21000",
                Grad = "Split",
                Zupanija = "Splitsko-dalmatinska",
                BrojClanova = 350
            },
            new PlaninarskaUdruga
            {
                IdPlaninarskaUdruga = 2,
                OIB = "48938096579",
                Naziv = "HPD Gora",
                Email = "hpd.gora@hps.hr",
                BrojTelefona = null,
                Adresa = "Dubravica 27a",
                PostanskiBroj = "10000",
                Grad = "Zagreb",
                Zupanija = "Grad Zagreb",
                BrojClanova = 180
            },
            new PlaninarskaUdruga
            {
                IdPlaninarskaUdruga = 3,
                OIB = "95873199484",
                Naziv = "PD Zavižan",
                Email = "pd.zavizan@hps.hr",
                BrojTelefona = null,
                Adresa = "Mala vrata 20",
                PostanskiBroj = "53270",
                Grad = "Senj",
                Zupanija = "Ličko-senjska",
                BrojClanova = 120
            },
            new PlaninarskaUdruga
            {
                IdPlaninarskaUdruga = 4,
                OIB = "92966614510",
                Naziv = "PD Paklenica",
                Email = "pd.paklenica@hps.hr",
                BrojTelefona = null,
                Adresa = "Majke Margarite 6",
                PostanskiBroj = "23000",
                Grad = "Zadar",
                Zupanija = "Zadarska",
                BrojClanova = 220
            },
            new PlaninarskaUdruga
            {
                IdPlaninarskaUdruga = 5,
                OIB = "12345678901",
                Naziv = "PD Dr. Maks Plotnikov",
                Email = "info@pddr-maks-plotnikov.hr",
                BrojTelefona = "0991234567",
                Adresa = "Andrije Hebranga 26",
                PostanskiBroj = "10430",
                Grad = "Samobor",
                Zupanija = "Zagrebačka",
                BrojClanova = 95
            }
        );

        modelBuilder.Entity<PlaninarskiObjekt>().HasData(
            new PlaninarskiObjekt
            {
                IdPlaninarskiObjekt = 1,
                IdPodrucje = 5,
                IdPlaninarskaUdruga = 5,
                Naziv = "Planinarski dom Dr. Maks Plotnikov",
                TipObjekta = TipObjekta.Dom,
                NadmorskaVisina = 411,
                Kapacitet = 14,
                Opis = "Dom podno ruševina Okić-grada; polazišna je točka za Okić i okolne putove.",
                ImeOdgovorneOsobe = "Stjepan Jandrečić",
                Telefon = "0918909624",
                Email = "aplantosar@gmail.com",
                Adresa = "Okić, Samobor",
                ImaNocenje = true,
                ImaHranu = true,
                RadnoVrijemeOpis = "Otvoren vikendom i blagdanima."
            },
            new PlaninarskiObjekt
            {
                IdPlaninarskiObjekt = 2,
                IdPodrucje = 5,
                IdPlaninarskaUdruga = 2,
                Naziv = "Planinarski dom Željezničar",
                TipObjekta = TipObjekta.Dom,
                NadmorskaVisina = 691,
                Kapacitet = 25,
                Opis = "Popularan planinarski dom u Samoborskom i Žumberačkom gorju.",
                ImeOdgovorneOsobe = "Dežurni domar",
                Telefon = null,
                Email = null,
                Adresa = "Samoborsko gorje",
                ImaNocenje = true,
                ImaHranu = true,
                RadnoVrijemeOpis = "Prema rasporedu dežurstva i vikendom."
            },
            new PlaninarskiObjekt
            {
                IdPlaninarskiObjekt = 3,
                IdPodrucje = 11,
                IdPlaninarskaUdruga = 3,
                Naziv = "Planinarska kuća Sijaset",
                TipObjekta = TipObjekta.Kuca,
                NadmorskaVisina = 328,
                Kapacitet = 12,
                Opis = "Niži planinarski objekt na Velebitu, pogodan kao polazište za ture.",
                ImeOdgovorneOsobe = "Dežurni član društva",
                Telefon = null,
                Email = "pd.zavizan@hps.hr",
                Adresa = "Velebit, Senj",
                ImaNocenje = true,
                ImaHranu = false,
                RadnoVrijemeOpis = "Povremeno otvorena ili po dogovoru."
            },
            new PlaninarskiObjekt
            {
                IdPlaninarskiObjekt = 4,
                IdPodrucje = 13,
                IdPlaninarskaUdruga = 4,
                Naziv = "Planinarski dom Paklenica",
                TipObjekta = TipObjekta.Dom,
                NadmorskaVisina = 480,
                Kapacitet = 44,
                Opis = "Dom na početku klanca Velike Paklenice s hranom, pićem i noćenjem.",
                ImeOdgovorneOsobe = "Irena Šaran",
                Telefon = "0977557654",
                Email = "pd.paklenica@hps.hr",
                Adresa = "Velika Paklenica",
                ImaNocenje = true,
                ImaHranu = true,
                RadnoVrijemeOpis = "Otvoren stalno."
            },
            new PlaninarskiObjekt
            {
                IdPlaninarskiObjekt = 5,
                IdPodrucje = 18,
                IdPlaninarskaUdruga = 1,
                Naziv = "Planinarska kuća Lugarnica",
                TipObjekta = TipObjekta.Kuca,
                NadmorskaVisina = 872,
                Kapacitet = 20,
                Opis = "Planinarska kuća na Mosoru pogodna za kraće i srednje duge uspone.",
                ImeOdgovorneOsobe = "Dežurna osoba društva",
                Telefon = null,
                Email = "hpd.mosor@hps.hr",
                Adresa = "Mosor, Split",
                ImaNocenje = true,
                ImaHranu = false,
                RadnoVrijemeOpis = "Otvorenost prema obavijesti društva."
            }
        );

        var kontrolnaTockaSeed = new KontrolnaTocka[]
        {
            new KontrolnaTocka
            {
                IdKontrolnaTocka = 1,
                GUIDOznaka = "KT-HPO-2-1-VIS",
                IdPodrucje = 2,
                Naziv = "Moslavačka gora – vrh Vis",
                TipKontrolneTocke = TipKontrolneTocke.Vrh,
                NadmorskaVisina = 437,
                Opis = "Najviši vrh Moslavačke gore i dobra početna kontrolna točka za početničke obilaznike.",
                Koordinate = "N/A",
                OpisZiga = "Metalni žig na vršnoj oznaci."
            },
            new KontrolnaTocka
            {
                IdKontrolnaTocka = 2,
                GUIDOznaka = "KT-HPO-4-4-SLJEME",
                IdPodrucje = 4,
                Naziv = "Sljeme – vrh",
                TipKontrolneTocke = TipKontrolneTocke.Vrh,
                NadmorskaVisina = 1033,
                Opis = "Najviši vrh Medvednice; vrh je lako dostupan i planinarima i izletnicima.",
                Koordinate = "N 45° 53' 57.4'' E 15° 56' 50.6''",
                OpisZiga = "Metalni žig vrha nalazi se na promidžbenom panou kod televizijskog tornja."
            },
            new KontrolnaTocka
            {
                IdKontrolnaTocka = 3,
                GUIDOznaka = "KT-HPO-5-1-OKIC",
                IdPodrucje = 5,
                Naziv = "Okić – vrh",
                TipKontrolneTocke = TipKontrolneTocke.Vrh,
                NadmorskaVisina = 499,
                Opis = "Stari grad i vršna gradina s vidikom prema Zagrebu i Medvednici.",
                Koordinate = "N 45° 44' 55.4'' E 15° 42' 24.0''",
                OpisZiga = "Metalni žig vrha ugrađen je na zid u najvišem dijelu gradine."
            },
            new KontrolnaTocka
            {
                IdKontrolnaTocka = 4,
                GUIDOznaka = "KT-HPO-5-4-JAPETIC",
                IdPodrucje = 5,
                Naziv = "Japetić – vrh",
                TipKontrolneTocke = TipKontrolneTocke.Vrh,
                NadmorskaVisina = 879,
                Opis = "Najviši vrh Samoborskoga gorja; poznat po piramidi i domu Žitnica.",
                Koordinate = "N 45° 44' 56.3'' E 15° 36' 32.8''",
                OpisZiga = "Metalni žig ugrađen je na konstrukciju piramide."
            },
            new KontrolnaTocka
            {
                IdKontrolnaTocka = 5,
                GUIDOznaka = "KT-HPO-11-2-ZAVIZAN",
                IdPodrucje = 11,
                Naziv = "Veliki Zavižan – vrh",
                TipKontrolneTocke = TipKontrolneTocke.Vrh,
                NadmorskaVisina = 1676,
                Opis = "Jedan od najpoznatijih vrhova Sjevernog Velebita s vrlo atraktivnim pogledima.",
                Koordinate = "N/A",
                OpisZiga = "Žig kontrolne točke nalazi se na vrhu ili u blizini planinarskog doma Zavižan."
            }
        };

        modelBuilder.Entity<KontrolnaTocka>().HasData(
            kontrolnaTockaSeed.Concat(PlaninarstvoSeedData.NoveKontrolneTocke).ToArray()
        );

        var rutaSeed = new Ruta[]
        {
            new Ruta
            {
                IdRuta = 1,
                IdKontrolnaTocka = 1,
                Naziv = "Kutina – Humka – Vis",
                Pocetak = "Kutina / Humka",
                Kraj = "Vrh Vis",
                VrijemeHodaMin = 90,
                DuljinaKm = 4.5m,
                VisinskaRazlikaM = 260,
                Opis = "Primjer kraće rute do najviše točke Moslavačke gore.",
                OznakaNaTerenu = "MG-01",
                GodinaObnove = 2023,
                Napomena = "Pogodna za početnike.",
                TezinaRute = TezinaRute.Laka,
                GPXPath = "C:\\GPX\\ruta_vis.gpx"
            },
            new Ruta
            {
                IdRuta = 2,
                IdKontrolnaTocka = 2,
                Naziv = "Gračani – Puntijarka – Sljeme",
                Pocetak = "Gračani",
                Kraj = "Sljeme",
                VrijemeHodaMin = 150,
                DuljinaKm = 8.2m,
                VisinskaRazlikaM = 780,
                Opis = "Popularan uspon preko Puntijarke prema vrhu Medvednice.",
                OznakaNaTerenu = "M-04",
                GodinaObnove = 2022,
                Napomena = "Jedna od najčešće korištenih ruta na Medvednici.",
                TezinaRute = TezinaRute.Srednja,
                GPXPath = "C:\\GPX\\ruta_sljeme.gpx"
            },
            new Ruta
            {
                IdRuta = 3,
                IdKontrolnaTocka = 3,
                Naziv = "Klake – pl. dom pod Okićem – Okić-grad",
                Pocetak = "Klake",
                Kraj = "Okić – vrh",
                VrijemeHodaMin = 40,
                DuljinaKm = 1.8m,
                VisinskaRazlikaM = 210,
                Opis = "Najkraći klasični prilaz vrhu Okić preko doma pod Okićem.",
                OznakaNaTerenu = "SG-01",
                GodinaObnove = 2021,
                Napomena = "Strmiji završni dio prema gradini.",
                TezinaRute = TezinaRute.Srednja,
                GPXPath = "C:\\GPX\\ruta_okic.gpx"
            },
            new Ruta
            {
                IdRuta = 4,
                IdKontrolnaTocka = 4,
                Naziv = "Šoićeva kuća – Japetić",
                Pocetak = "Šoićeva kuća",
                Kraj = "Japetić – vrh",
                VrijemeHodaMin = 90,
                DuljinaKm = 5.4m,
                VisinskaRazlikaM = 430,
                Opis = "Klasičan prilaz preko livada i Katina krča prema vrhu Japetić.",
                OznakaNaTerenu = "SG-04",
                GodinaObnove = 2020,
                Napomena = "Ruta je pregledna i često korištena.",
                TezinaRute = TezinaRute.Srednja,
                GPXPath = "C:\\GPX\\ruta_japetic.gpx"
            },
            new Ruta
            {
                IdRuta = 5,
                IdKontrolnaTocka = 5,
                Naziv = "Dom Zavižan – Veliki Zavižan – dom Zavižan",
                Pocetak = "Planinarski dom Zavižan",
                Kraj = "Veliki Zavižan",
                VrijemeHodaMin = 150,
                DuljinaKm = 6.7m,
                VisinskaRazlikaM = 320,
                Opis = "Kružna tura s polaskom od doma Zavižan preko Balinovca do Velikog Zavižana.",
                OznakaNaTerenu = "SV-02",
                GodinaObnove = 2024,
                Napomena = "U nepovoljnim uvjetima potreban dodatni oprez.",
                TezinaRute = TezinaRute.Teska,
                GPXPath = "C:\\GPX\\ruta_zavizan.gpx"
            }
        };

        modelBuilder.Entity<Ruta>().HasData(
            rutaSeed.Concat(PlaninarstvoSeedData.NoveRute).ToArray()
        );

        modelBuilder.Entity<Korisnik>().HasData(
            new Korisnik
            {
                IdKorisnik = 1,
                Ime = "Luka",
                Prezime = "Bošnjak",
                Email = "luka.bosnjak92@gmail.com",
                KorisnickoIme = "Boss",
                DatumRodenja = new DateTime(2004, 6, 29),
                DatumRegistracije = new DateTime(2026, 4, 1, 9, 0, 0),
                BrojMobitela = "0979545897",
                ProfilnaSlika = "/Slike/Profil/Boss.jpeg",
                StatusAktivan = true
            },
            new Korisnik
            {
                IdKorisnik = 2,
                Ime = "Test",
                Prezime = "Test",
                Email = "test123@gmail.com",
                KorisnickoIme = "Test",
                DatumRodenja = new DateTime(2005, 1, 1),
                DatumRegistracije = new DateTime(2026, 4, 1, 9, 15, 0),
                BrojMobitela = null,
                ProfilnaSlika = "/Slike/Profil/test.jpg",
                StatusAktivan = true
            }
        );

        modelBuilder.Entity<Obavijest>().HasData(
            new Obavijest
            {
                IdObavijest = 1,
                Naslov = "Dobrodošli u planinarsku aplikaciju",
                Sadrzaj = "Aplikacija je pokrenuta i spremna za korištenje.",
                DatumObjave = new DateTime(2026, 4, 1, 10, 0, 0),
                JeAktivna = true,
                IdKorisnik = 1
            },
            new Obavijest
            {
                IdObavijest = 2,
                Naslov = "Nova ruta dodana: Zavižan",
                Sadrzaj = "Dodana je kružna tura od doma Zavižan preko Balinovca.",
                DatumObjave = new DateTime(2026, 4, 15, 14, 30, 0),
                JeAktivna = true,
                IdKorisnik = 1
            },
            new Obavijest
            {
                IdObavijest = 3,
                Naslov = "Održavanje sustava",
                Sadrzaj = "Planirano održavanje 30. travnja od 22:00 do 23:00.",
                DatumObjave = new DateTime(2026, 4, 28, 9, 0, 0),
                JeAktivna = false,
                IdKorisnik = 2
            }
        );

        modelBuilder.Entity<Knjizica>().HasData(
            new Knjizica
            {
                IdKnjizica = 1,
                IdKorisnik = 1,
                DatumKreiranja = new DateTime(2026, 4, 1, 9, 5, 0),
                Napomena = "Glavna digitalna knjižica korisnika Luka Bošnjak.",
                StatusAktivna = true
            },
            new Knjizica
            {
                IdKnjizica = 2,
                IdKorisnik = 2,
                DatumKreiranja = new DateTime(2026, 4, 1, 9, 20, 0),
                Napomena = "Testna digitalna knjižica za provjeru funkcionalnosti aplikacije.",
                StatusAktivna = true
            }
        );

        modelBuilder.Entity<Posjet>().HasData(
            new Posjet
            {
                IdPosjet = 1,
                IdKorisnik = 1,
                IdKnjizica = 1,
                IdKontrolnaTocka = 1,
                IdRuta = 1,
                DatumVrijemePosjeta = new DateTime(2026, 4, 5, 10, 15, 0),
                VrijemeUsponaMin = 92,
                DozivljajPosjeta = DozivljajPosjeta.VrloLagano,
                OpisIskustva = "Prvi evidentirani uspon u aplikaciji. Lagana i ugodna ruta po suhom vremenu.",
                UneseniGUID = "KT-HPO-2-1-VIS",
                JeLiPotvrdenPosjet = true,
                DatumKreiranjaZapisa = new DateTime(2026, 4, 5, 12, 0, 0)
            },
            new Posjet
            {
                IdPosjet = 2,
                IdKorisnik = 1,
                IdKnjizica = 1,
                IdKontrolnaTocka = 3,
                IdRuta = 3,
                DatumVrijemePosjeta = new DateTime(2026, 4, 12, 8, 40, 0),
                VrijemeUsponaMin = 43,
                DozivljajPosjeta = DozivljajPosjeta.KratkoAliTesko,
                OpisIskustva = "Kratak, ali strm završni dio prema gradini Okić.",
                UneseniGUID = "KT-HPO-5-1-OKIC",
                JeLiPotvrdenPosjet = true,
                DatumKreiranjaZapisa = new DateTime(2026, 4, 12, 10, 0, 0)
            },
            new Posjet
            {
                IdPosjet = 3,
                IdKorisnik = 1,
                IdKnjizica = 1,
                IdKontrolnaTocka = 4,
                IdRuta = 4,
                DatumVrijemePosjeta = new DateTime(2026, 4, 19, 9, 10, 0),
                VrijemeUsponaMin = 96,
                DozivljajPosjeta = DozivljajPosjeta.Srednje,
                OpisIskustva = "Ugodna tura s dobrim vremenom i lijepim pogledima s piramide.",
                UneseniGUID = "KT-HPO-5-4-JAPETIC",
                JeLiPotvrdenPosjet = true,
                DatumKreiranjaZapisa = new DateTime(2026, 4, 19, 11, 10, 0)
            },
            new Posjet
            {
                IdPosjet = 4,
                IdKorisnik = 2,
                IdKnjizica = 2,
                IdKontrolnaTocka = 1,
                IdRuta = 1,
                DatumVrijemePosjeta = new DateTime(2026, 4, 8, 11, 0, 0),
                VrijemeUsponaMin = 95,
                DozivljajPosjeta = DozivljajPosjeta.Lagano,
                OpisIskustva = "Testni korisnik uspješno evidentirao svoj prvi posjet i time zadovoljio uvjet za početničku medalju.",
                UneseniGUID = "KT-HPO-2-1-VIS",
                JeLiPotvrdenPosjet = true,
                DatumKreiranjaZapisa = new DateTime(2026, 4, 8, 12, 45, 0)
            },
            new Posjet
            {
                IdPosjet = 5,
                IdKorisnik = 2,
                IdKnjizica = 2,
                IdKontrolnaTocka = 2,
                IdRuta = 2,
                DatumVrijemePosjeta = new DateTime(2026, 4, 26, 7, 50, 0),
                VrijemeUsponaMin = 155,
                DozivljajPosjeta = DozivljajPosjeta.FizickiNaporno,
                OpisIskustva = "Duži uspon do Sljemena preko Puntijarke, ali bez tehnički teških dijelova.",
                UneseniGUID = "KT-HPO-4-4-SLJEME",
                JeLiPotvrdenPosjet = true,
                DatumKreiranjaZapisa = new DateTime(2026, 4, 26, 10, 40, 0)
            }
        );

        modelBuilder.Entity<Fotografija>().HasData(
            new Fotografija
            {
                IdFotografija = 1,
                IdPosjet = 1,
                NazivDatoteke = "vis_luka_selfie.jpg",
                PutanjaDatoteke = "/slike/posjeti/vis_luka_selfie.jpg",
                DatumUploada = new DateTime(2026, 4, 5, 12, 5, 0),
                TipSlike = TipSlike.Selfie,
                Opis = "Selfie korisnika Luke na vrhu Vis."
            },
            new Fotografija
            {
                IdFotografija = 2,
                IdPosjet = 2,
                NazivDatoteke = "okic_luka_selfie.jpg",
                PutanjaDatoteke = "/slike/posjeti/okic_luka_selfie.jpg",
                DatumUploada = new DateTime(2026, 4, 12, 10, 5, 0),
                TipSlike = TipSlike.Selfie,
                Opis = "Fotografija Luke kod oznake vrha Okić."
            },
            new Fotografija
            {
                IdFotografija = 3,
                IdPosjet = 3,
                NazivDatoteke = "japetic_luka_selfie.jpg",
                PutanjaDatoteke = "/slike/posjeti/japetic_luka_selfie.jpg",
                DatumUploada = new DateTime(2026, 4, 19, 11, 15, 0),
                TipSlike = TipSlike.Selfie,
                Opis = "Selfie na vrhu Japetić uz piramidu."
            },
            new Fotografija
            {
                IdFotografija = 4,
                IdPosjet = 4,
                NazivDatoteke = "vis_test_selfie.jpg",
                PutanjaDatoteke = "/slike/posjeti/vis_test_selfie.jpg",
                DatumUploada = new DateTime(2026, 4, 8, 12, 50, 0),
                TipSlike = TipSlike.Selfie,
                Opis = "Testni korisnik na vrhu Vis."
            },
            new Fotografija
            {
                IdFotografija = 5,
                IdPosjet = 5,
                NazivDatoteke = "sljeme_test_selfie.jpg",
                PutanjaDatoteke = "/slike/posjeti/sljeme_test_selfie.jpg",
                DatumUploada = new DateTime(2026, 4, 26, 10, 45, 0),
                TipSlike = TipSlike.Selfie,
                Opis = "Testni korisnik na vrhu Sljeme kod oznake."
            }
        );

        modelBuilder.Entity<KorisnikMedalja>().HasData(
            new KorisnikMedalja
            {
                IdKorisnikMedalja = 1,
                IdKorisnik = 1,
                IdMedalja = 1,
                DatumDodjele = new DateTime(2026, 4, 19, 12, 0, 0),
                Napomena = "Korisnik je zadovoljio uvjet početničke medalje jer ima evidentiran obilazak područja 2 (Moslavačka gora i Bilogora), gdje je prag 1 KT."
            },
            new KorisnikMedalja
            {
                IdKorisnikMedalja = 2,
                IdKorisnik = 2,
                IdMedalja = 1,
                DatumDodjele = new DateTime(2026, 4, 8, 13, 0, 0),
                Napomena = "Korisnik je zadovoljio uvjet početničke medalje jer ima evidentiran obilazak područja 2 (Moslavačka gora i Bilogora), gdje je prag 1 KT."
            }
        );

        modelBuilder.Entity<Fotografija>().HasQueryFilter(x => x.DeletedAt == null);
        modelBuilder.Entity<KorisnikMedalja>().HasQueryFilter(x => x.DeletedAt == null);
        modelBuilder.Entity<KontrolnaTocka>().HasQueryFilter(x => x.DeletedAt == null);
        modelBuilder.Entity<Medalja>().HasQueryFilter(x => x.DeletedAt == null);
        modelBuilder.Entity<PlaninarskaUdruga>().HasQueryFilter(x => x.DeletedAt == null);
        modelBuilder.Entity<PlaninarskiObjekt>().HasQueryFilter(x => x.DeletedAt == null);
        modelBuilder.Entity<Posjet>().HasQueryFilter(x => x.DeletedAt == null);
        modelBuilder.Entity<Podrucje>().HasQueryFilter(x => x.DeletedAt == null);
        modelBuilder.Entity<Ruta>().HasQueryFilter(x => x.DeletedAt == null);
    }

    // ---------------------------------------------------------------------
    // Audit log: svaka Added/Modified/Deleted promjena nad domenskim entitetima
    // (namespace planinarenje.Entiteti, osim LogAktivnosti samog) automatski se
    // zapisuje u LogoviAktivnosti, uz tko je promjenu napravio i koja su polja
    // promijenjena (za Modified: stara -> nova vrijednost).
    // ---------------------------------------------------------------------

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var pendingAuditEntries = OnBeforeSaveChanges();
        var result = base.SaveChanges(acceptAllChangesOnSuccess);
        OnAfterSaveChanges(pendingAuditEntries);
        return result;
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var pendingAuditEntries = OnBeforeSaveChanges();
        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        await OnAfterSaveChangesAsync(pendingAuditEntries, cancellationToken);
        return result;
    }

    private List<AuditZapisUIzradi> OnBeforeSaveChanges()
    {
        ChangeTracker.DetectChanges();

        var (appUserId, korisnickoIme) = DohvatiTrenutnogKorisnika();
        var pending = new List<AuditZapisUIzradi>();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State is EntityState.Detached or EntityState.Unchanged)
                continue;

            // Audit pratimo samo nad domenskim entitetima ove aplikacije, ne nad samim audit logom.
            if (entry.Entity is LogAktivnosti || entry.Entity.GetType().Namespace != "planinarenje.Entiteti")
                continue;

            var auditEntry = new AuditZapisUIzradi(entry, appUserId, korisnickoIme);
            pending.Add(auditEntry);

            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    auditEntry.SvojstvaSGeneriranomVrijednoscu.Add(property);
                    continue;
                }

                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.IdEntiteta = property.CurrentValue?.ToString() ?? string.Empty;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.NoveVrijednosti[property.Metadata.Name] = property.CurrentValue;
                        break;
                    case EntityState.Deleted:
                        auditEntry.StareVrijednosti[property.Metadata.Name] = property.OriginalValue;
                        break;
                    case EntityState.Modified:
                        if (property.IsModified && !Equals(property.OriginalValue, property.CurrentValue))
                        {
                            auditEntry.StareVrijednosti[property.Metadata.Name] = property.OriginalValue;
                            auditEntry.NoveVrijednosti[property.Metadata.Name] = property.CurrentValue;
                        }
                        break;
                }
            }
        }

        // Modified/Deleted entiteti već imaju poznat PK, pa se mogu odmah pretvoriti u LogAktivnosti.
        // Added entiteti čekaju generirani PK do nakon SaveChanges (vidi OnAfterSaveChanges).
        var spremniOdmah = pending.Where(p => p.SvojstvaSGeneriranomVrijednoscu.Count == 0).ToList();
        foreach (var auditEntry in spremniOdmah)
        {
            if (auditEntry.ImaPromjene)
            {
                LogoviAktivnosti.Add(auditEntry.IzgradiZapis());
            }
        }

        return pending.Where(p => p.SvojstvaSGeneriranomVrijednoscu.Count > 0).ToList();
    }

    private void OnAfterSaveChanges(List<AuditZapisUIzradi> pendingAuditEntries)
    {
        if (pendingAuditEntries.Count == 0) return;

        foreach (var auditEntry in pendingAuditEntries)
        {
            DopuniGenerirunePKVrijednosti(auditEntry);
            if (auditEntry.ImaPromjene)
            {
                LogoviAktivnosti.Add(auditEntry.IzgradiZapis());
            }
        }

        base.SaveChanges();
    }

    private async Task OnAfterSaveChangesAsync(List<AuditZapisUIzradi> pendingAuditEntries, CancellationToken cancellationToken)
    {
        if (pendingAuditEntries.Count == 0) return;

        foreach (var auditEntry in pendingAuditEntries)
        {
            DopuniGenerirunePKVrijednosti(auditEntry);
            if (auditEntry.ImaPromjene)
            {
                LogoviAktivnosti.Add(auditEntry.IzgradiZapis());
            }
        }

        await base.SaveChangesAsync(cancellationToken);
    }

    private static void DopuniGenerirunePKVrijednosti(AuditZapisUIzradi auditEntry)
    {
        foreach (var property in auditEntry.SvojstvaSGeneriranomVrijednoscu)
        {
            if (property.Metadata.IsPrimaryKey())
            {
                auditEntry.IdEntiteta = property.CurrentValue?.ToString() ?? string.Empty;
            }
            else
            {
                auditEntry.NoveVrijednosti[property.Metadata.Name] = property.CurrentValue;
            }
        }
    }

    private (string AppUserId, string? KorisnickoIme) DohvatiTrenutnogKorisnika()
    {
        var user = _httpContextAccessor?.HttpContext?.User;
        var appUserId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
        var korisnickoIme = user?.Identity?.IsAuthenticated == true ? user.Identity!.Name : null;

        return (appUserId ?? "Sustav", korisnickoIme);
    }

    // Privremeni radni objekt dok se ne sazna generirani PK (za Added entitete) - nije persistirana klasa.
    private sealed class AuditZapisUIzradi
    {
        private readonly EntityEntry _entry;
        private readonly string _appUserId;
        private readonly string? _korisnickoIme;

        public AuditZapisUIzradi(EntityEntry entry, string appUserId, string? korisnickoIme)
        {
            _entry = entry;
            _appUserId = appUserId;
            _korisnickoIme = korisnickoIme;
        }

        public string IdEntiteta { get; set; } = string.Empty;
        public List<PropertyEntry> SvojstvaSGeneriranomVrijednoscu { get; } = new();
        public Dictionary<string, object?> StareVrijednosti { get; } = new();
        public Dictionary<string, object?> NoveVrijednosti { get; } = new();

        public bool ImaPromjene => _entry.State switch
        {
            EntityState.Added => true,
            EntityState.Deleted => true,
            EntityState.Modified => StareVrijednosti.Count > 0,
            _ => false
        };

        public LogAktivnosti IzgradiZapis()
        {
            var akcija = _entry.State switch
            {
                EntityState.Added => TipAkcijeLoga.Kreirano,
                EntityState.Deleted => TipAkcijeLoga.Obrisano,
                _ => TipAkcijeLoga.Azurirano
            };

            return new LogAktivnosti
            {
                VrijemeDogadaja = DateTime.UtcNow,
                AppUserId = _appUserId,
                KorisnickoIme = _korisnickoIme,
                NazivEntiteta = _entry.Entity.GetType().Name,
                IdEntiteta = IdEntiteta,
                Akcija = akcija,
                Detalji = IzgradiDetalje(akcija)
            };
        }

        private string IzgradiDetalje(TipAkcijeLoga akcija)
        {
            if (akcija == TipAkcijeLoga.Azurirano)
            {
                var promjene = StareVrijednosti.Keys
                    .Select(naziv => $"{naziv}: '{FormatVrijednost(StareVrijednosti[naziv])}' -> '{FormatVrijednost(NoveVrijednosti.GetValueOrDefault(naziv))}'");
                return string.Join("; ", promjene);
            }

            var vrijednosti = akcija == TipAkcijeLoga.Kreirano ? NoveVrijednosti : StareVrijednosti;
            var polja = vrijednosti
                .Where(kv => kv.Value != null)
                .Select(kv => $"{kv.Key}: '{FormatVrijednost(kv.Value)}'");
            return string.Join("; ", polja);
        }

        private static string FormatVrijednost(object? vrijednost)
        {
            if (vrijednost is null) return "null";
            if (vrijednost is DateTime dt) return dt.ToString("yyyy-MM-dd HH:mm");
            return vrijednost.ToString() ?? "null";
        }
    }
}
