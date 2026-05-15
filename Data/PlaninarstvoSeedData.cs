using planinarenje.Entiteti;

namespace planinarenje.Data;

public static class PlaninarstvoSeedData
{
    public static readonly KontrolnaTocka[] NoveKontrolneTocke = new[]
    {
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 7,
            GUIDOznaka = "KAP8371",
            IdPodrucje = 1,
            Naziv = "Krndija – vrh Kapovac",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 790,
            Opis = "Najviši vrh Krndije u slavonskom gorju; šumoviti vrh s markiranim pristupom.",
            Koordinate = "N 45° 28' 12.0'' E 17° 52' 30.0''",
            OpisZiga = "Metalni žig na vršnoj oznaci."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 8,
            GUIDOznaka = "IVA5629",
            IdPodrucje = 1,
            Naziv = "Papuk – vrh Ivačka glava",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 913,
            Opis = "Najviši vrh Papuka i cijele Slavonije; dostupan s više strana.",
            Koordinate = "N 45° 31' 10.0'' E 17° 40' 15.0''",
            OpisZiga = "Metalni žig na vrhu kod geodetskog stupa."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 9,
            GUIDOznaka = "BRE7412",
            IdPodrucje = 1,
            Naziv = "Psunj – vrh Brezovo polje",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 984,
            Opis = "Najviši vrh Psunja i jedan od najviših slavonskih vrhova; šumovit i miran.",
            Koordinate = "N 45° 16' 45.0'' E 17° 18' 20.0''",
            OpisZiga = "Metalni žig na oznaci vrha."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 10,
            GUIDOznaka = "STA2087",
            IdPodrucje = 2,
            Naziv = "Bilogora – Stankov vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 309,
            Opis = "Najviši vrh Bilogore s vidikovcem i planinarskim putom kroz šumu.",
            Koordinate = "N 45° 53' 00.0'' E 17° 07' 30.0''",
            OpisZiga = "Metalni žig na drvenom stupu kod vidikovca."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 11,
            GUIDOznaka = "MOH6243",
            IdPodrucje = 3,
            Naziv = "Međimurske gorice – vrh Mohokos",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 344,
            Opis = "Najviši vrh Međimurja; lagani pristup i lijep pogled prema Alpama i Zagorju.",
            Koordinate = "N 46° 24' 50.0'' E 16° 22' 10.0''",
            OpisZiga = "Metalni žig na oznaci vrha."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 12,
            GUIDOznaka = "IVN3815",
            IdPodrucje = 3,
            Naziv = "Ivanščica – vrh Ivanščica",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1060,
            Opis = "Najviši vrh Hrvatskog zagorja i najistaknutiji zagorski vrh s panoramskim vidicima.",
            Koordinate = "N 46° 10' 55.0'' E 16° 06' 45.0''",
            OpisZiga = "Metalni žig na vršnom stupu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 13,
            GUIDOznaka = "RAV9174",
            IdPodrucje = 3,
            Naziv = "Ravna gora – vrh (piramida)",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 680,
            Opis = "Šumoviti vrh s geodetskom piramidom i markiranim pristupom iz Gornje Stubice.",
            Koordinate = "N 46° 04' 20.0'' E 15° 56' 30.0''",
            OpisZiga = "Metalni žig na piramidi."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 14,
            GUIDOznaka = "SUS4538",
            IdPodrucje = 3,
            Naziv = "Strahinjščica – vrh Sušec",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 846,
            Opis = "Najviši vrh Strahinjščice s pogledom prema Ivanščici i Krapinskoj dolini.",
            Koordinate = "N 46° 11' 40.0'' E 15° 54' 20.0''",
            OpisZiga = "Metalni žig na kamenoj oznaci vrha."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 15,
            GUIDOznaka = "GRH7260",
            IdPodrucje = 4,
            Naziv = "Grohot – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 492,
            Opis = "Niži vrh Medvednice s vidikovcem i starim hrastovima; pogodan za kraće ture.",
            Koordinate = "N 45° 52' 30.0'' E 16° 03' 10.0''",
            OpisZiga = "Metalni žig na drvenoj oznaci vrha."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 16,
            GUIDOznaka = "LIP3492",
            IdPodrucje = 4,
            Naziv = "Lipa – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 709,
            Opis = "Šumoviti vrh Medvednice na sjevernom grebenu; miran i manje posjećen.",
            Koordinate = "N 45° 54' 10.0'' E 15° 55' 40.0''",
            OpisZiga = "Metalni žig na vršnom stupu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 17,
            GUIDOznaka = "MEG8156",
            IdPodrucje = 4,
            Naziv = "Medvedgrad",
            TipKontrolneTocke = TipKontrolneTocke.KontrolnaTocka,
            NadmorskaVisina = 579,
            Opis = "Srednjovjekovna utvrda na južnim padinama Medvednice; kontrolna točka HPO-a.",
            Koordinate = "N 45° 51' 45.0'' E 15° 56' 50.0''",
            OpisZiga = "Metalni žig na ulaznom zidu utvrde."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 18,
            GUIDOznaka = "PLE6703",
            IdPodrucje = 5,
            Naziv = "Plešivica – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 779,
            Opis = "Vrh Samoborskog gorja s pogledom na vinograde i Žumberak; blizu planinarskog doma.",
            Koordinate = "N 45° 43' 30.0'' E 15° 39' 20.0''",
            OpisZiga = "Metalni žig na kamenoj oznaci."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 19,
            GUIDOznaka = "OST5281",
            IdPodrucje = 5,
            Naziv = "Oštrc – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 752,
            Opis = "Popularan vrh s kapelom Sv. Ane na vrhu i panoramskim vidicima.",
            Koordinate = "N 45° 44' 10.0'' E 15° 40' 55.0''",
            OpisZiga = "Metalni žig na kapelici na vrhu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 20,
            GUIDOznaka = "TUS9047",
            IdPodrucje = 6,
            Naziv = "Tuščak – gradina",
            TipKontrolneTocke = TipKontrolneTocke.KontrolnaTocka,
            NadmorskaVisina = 585,
            Opis = "Stara gradina na zapadnom dijelu Žumberačke gore; pogled prema Žumberku.",
            Koordinate = "N 45° 44' 00.0'' E 15° 30' 10.0''",
            OpisZiga = "Metalni žig na ruševini gradine."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 21,
            GUIDOznaka = "SGE2634",
            IdPodrucje = 6,
            Naziv = "Sveta Gera – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1178,
            Opis = "Najviši vrh Žumberačke gore i cijele Žumberačko-samoborske regije.",
            Koordinate = "N 45° 42' 45.0'' E 15° 22' 30.0''",
            OpisZiga = "Metalni žig na vršnom stupu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 22,
            GUIDOznaka = "PLI7819",
            IdPodrucje = 6,
            Naziv = "Pliješ – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 977,
            Opis = "Šumoviti vrh Žumberačke gore s markiranim putom iz Budinjaka.",
            Koordinate = "N 45° 43' 20.0'' E 15° 25' 50.0''",
            OpisZiga = "Metalni žig na oznaci vrha."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 23,
            GUIDOznaka = "VOD4153",
            IdPodrucje = 7,
            Naziv = "Vodenica – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 538,
            Opis = "Najviši vrh Pokuplja; miran vrh s pogledom na Kupu i okolne šume.",
            Koordinate = "N 45° 27' 10.0'' E 15° 32' 20.0''",
            OpisZiga = "Metalni žig na drvenoj oznaci."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 24,
            GUIDOznaka = "PET6928",
            IdPodrucje = 7,
            Naziv = "Petrova gora – vrh Petrovac",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 512,
            Opis = "Vrh Petrove gore s poznatim spomenikom i vidikovcem prema Kordunu.",
            Koordinate = "N 45° 19' 20.0'' E 15° 47' 00.0''",
            OpisZiga = "Metalni žig na spomeniku kod vrha."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 25,
            GUIDOznaka = "KLE3047",
            IdPodrucje = 8,
            Naziv = "Klek – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1181,
            Opis = "Karakteristična stijena iznad Ogulina; simbol hrvatskog planinarstva od 1874. godine.",
            Koordinate = "N 45° 17' 55.0'' E 15° 10' 40.0''",
            OpisZiga = "Metalni žig na vršnom stupu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 26,
            GUIDOznaka = "BJE8592",
            IdPodrucje = 8,
            Naziv = "Bjelolasica – vrh Kula",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1534,
            Opis = "Najviši vrh Gorskog kotara i hrvatski vrh izvan Velebita i Dinare.",
            Koordinate = "N 45° 15' 50.0'' E 14° 58' 30.0''",
            OpisZiga = "Metalni žig na geodetskom stupu na vrhu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 27,
            GUIDOznaka = "SAM1736",
            IdPodrucje = 8,
            Naziv = "Samarske stijene – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1302,
            Opis = "Spektakularne stjenovite formacije u srcu Gorskog kotara; zahtjevan pristup.",
            Koordinate = "N 45° 16' 20.0'' E 14° 55' 10.0''",
            OpisZiga = "Metalni žig na stijeni kod vrha."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 28,
            GUIDOznaka = "RIS4208",
            IdPodrucje = 9,
            Naziv = "Risnjak – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1528,
            Opis = "Najviši vrh istoimenog nacionalnog parka; panoramski pogled od Alpa do mora.",
            Koordinate = "N 45° 25' 35.0'' E 14° 45' 20.0''",
            OpisZiga = "Metalni žig na vršnom stupu kod kapelice."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 29,
            GUIDOznaka = "SNJ6371",
            IdPodrucje = 9,
            Naziv = "Snježnik – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1505,
            Opis = "Drugi najviši vrh Gorskog kotara; poznat po kasnom snijegu i alpskim livadama.",
            Koordinate = "N 45° 26' 10.0'' E 14° 35' 40.0''",
            OpisZiga = "Metalni žig na kamenoj oznaci vrha."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 30,
            GUIDOznaka = "SKR2845",
            IdPodrucje = 9,
            Naziv = "Skradski vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1043,
            Opis = "Popularan izletnički vrh u sjevernom Gorskom kotaru s planinarskim domom.",
            Koordinate = "N 45° 24' 05.0'' E 15° 02' 15.0''",
            OpisZiga = "Metalni žig na vršnom stupu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 31,
            GUIDOznaka = "VOJ7164",
            IdPodrucje = 10,
            Naziv = "Učka – vrh Vojak",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1396,
            Opis = "Najviši vrh Istre s kamenim tornjem na vrhu i pogledom na Kvarner i Alpe.",
            Koordinate = "N 45° 17' 10.0'' E 14° 11' 55.0''",
            OpisZiga = "Metalni žig na kamenom tornju na vrhu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 32,
            GUIDOznaka = "VPL3920",
            IdPodrucje = 10,
            Naziv = "Ćićarija – vrh Veliki Planik",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1272,
            Opis = "Najviši vrh Ćićarije s travnatim vršnim područjem i pogledom prema Učki.",
            Koordinate = "N 45° 27' 20.0'' E 14° 13' 30.0''",
            OpisZiga = "Metalni žig na kamenoj oznaci."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 33,
            GUIDOznaka = "MRA8451",
            IdPodrucje = 11,
            Naziv = "Mali Rajinac – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1699,
            Opis = "Jedan od najviših velebitskih vrhova na sjevernom dijelu; krški vrh s divljim pogledom.",
            Koordinate = "N 44° 46' 30.0'' E 14° 58' 50.0''",
            OpisZiga = "Metalni žig na vrhu stijene."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 34,
            GUIDOznaka = "ZEC6237",
            IdPodrucje = 12,
            Naziv = "Zečjak – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1622,
            Opis = "Najviši vrh Srednjeg Velebita; stjenovit i zahtjevan teren.",
            Koordinate = "N 44° 36' 15.0'' E 15° 03' 40.0''",
            OpisZiga = "Metalni žig na kamenoj piramidi."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 35,
            GUIDOznaka = "SAT1584",
            IdPodrucje = 12,
            Naziv = "Šatorina – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1622,
            Opis = "Karakteristični vrh Srednjeg Velebita s oblikom šatora; divlji krški krajolik.",
            Koordinate = "N 44° 34' 50.0'' E 15° 05' 10.0''",
            OpisZiga = "Metalni žig na oznaci vrha."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 36,
            GUIDOznaka = "VAG7302",
            IdPodrucje = 13,
            Naziv = "Vaganski vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1757,
            Opis = "Najviši vrh Velebita i treći najviši vrh Hrvatske; zahtjevan pristup iz Paklenice.",
            Koordinate = "N 44° 21' 50.0'' E 15° 30' 20.0''",
            OpisZiga = "Metalni žig na geodetskom stupu na vrhu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 37,
            GUIDOznaka = "SVB4916",
            IdPodrucje = 13,
            Naziv = "Sveto brdo – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1751,
            Opis = "Drugi najviši vrh Velebita s kapelom na vrhu; pogled na more i Liku.",
            Koordinate = "N 44° 19' 40.0'' E 15° 30' 55.0''",
            OpisZiga = "Metalni žig na kapeli na vrhu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 38,
            GUIDOznaka = "ANI2058",
            IdPodrucje = 13,
            Naziv = "Anića kuk – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 712,
            Opis = "Impozantna stijena u klancu Velike Paklenice; alpinistički značajan vrh.",
            Koordinate = "N 44° 18' 15.0'' E 15° 27' 40.0''",
            OpisZiga = "Metalni žig na vršnom stupu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 39,
            GUIDOznaka = "OZE8743",
            IdPodrucje = 14,
            Naziv = "Lička Plješivica – vrh Ozeblin",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1657,
            Opis = "Najviši vrh Ličke Plješivice i Like; zahtjevan pristup šumskim putovima.",
            Koordinate = "N 44° 46' 10.0'' E 15° 44' 30.0''",
            OpisZiga = "Metalni žig na vrhu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 40,
            GUIDOznaka = "POT5261",
            IdPodrucje = 14,
            Naziv = "Poštak – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1425,
            Opis = "Istaknuti lički vrh na granici prema Dalmaciji s otvorenim pogledom.",
            Koordinate = "N 44° 10' 55.0'' E 16° 10' 20.0''",
            OpisZiga = "Metalni žig na vršnoj oznaci."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 41,
            GUIDOznaka = "OBZ3179",
            IdPodrucje = 15,
            Naziv = "Krk – vrh Obzova",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 569,
            Opis = "Najviši vrh otoka Krka s pogledom na Kvarner i okolne otoke.",
            Koordinate = "N 45° 01' 20.0'' E 14° 37' 50.0''",
            OpisZiga = "Metalni žig na vršnom stupu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 42,
            GUIDOznaka = "SIS6420",
            IdPodrucje = 15,
            Naziv = "Cres – vrh Sis",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 639,
            Opis = "Najviši vrh otoka Cresa; divlji otočni krajolik s pogledom na Jadran.",
            Koordinate = "N 44° 52' 30.0'' E 14° 22' 10.0''",
            OpisZiga = "Metalni žig na kamenoj oznaci."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 43,
            GUIDOznaka = "VID8537",
            IdPodrucje = 16,
            Naziv = "Brač – vrh Vidova gora",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 780,
            Opis = "Najviši vrh jadranskih otoka; spektakularan pogled na Zlatni rat i Hvar.",
            Koordinate = "N 43° 18' 40.0'' E 16° 37' 20.0''",
            OpisZiga = "Metalni žig na vršnom stupu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 44,
            GUIDOznaka = "SNK2074",
            IdPodrucje = 16,
            Naziv = "Hvar – vrh Sv. Nikola",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 626,
            Opis = "Najviši vrh otoka Hvara s pogledom na paklinske otoke i pelješku obalu.",
            Koordinate = "N 43° 10' 35.0'' E 16° 39' 50.0''",
            OpisZiga = "Metalni žig na kapelici Sv. Nikole."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 45,
            GUIDOznaka = "KOM9361",
            IdPodrucje = 16,
            Naziv = "Korčula – vrh Kom",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 508,
            Opis = "Najviši vrh otoka Korčule s gustim makijama i pogledom na Pelješac.",
            Koordinate = "N 42° 57' 30.0'' E 16° 53' 15.0''",
            OpisZiga = "Metalni žig na kamenoj oznaci."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 46,
            GUIDOznaka = "DIN4728",
            IdPodrucje = 17,
            Naziv = "Dinara – vrh Dinara (Sinjal)",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1831,
            Opis = "Najviši vrh Republike Hrvatske; obavezna kontrolna točka za srebrnu značku HPO-a.",
            Koordinate = "N 43° 59' 25.0'' E 16° 22' 50.0''",
            OpisZiga = "Metalni žig na geodetskom stupu na vrhu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 47,
            GUIDOznaka = "SVL5839",
            IdPodrucje = 17,
            Naziv = "Svilaja – vrh Svilaja",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1508,
            Opis = "Najviši vrh planine Svilaje u dalmatinskom zaleđu; zahtjevan pristup.",
            Koordinate = "N 43° 44' 10.0'' E 16° 28' 30.0''",
            OpisZiga = "Metalni žig na vršnom stupu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 48,
            GUIDOznaka = "CAV7162",
            IdPodrucje = 17,
            Naziv = "Promina – vrh Čavnovka",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1147,
            Opis = "Najviši vrh planine Promine iznad Drniša; pogled na Krku i Zagoru.",
            Koordinate = "N 43° 51' 40.0'' E 16° 05' 20.0''",
            OpisZiga = "Metalni žig na kamenoj oznaci."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 49,
            GUIDOznaka = "LJU3084",
            IdPodrucje = 18,
            Naziv = "Mosor – vrh Ljubljan",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1262,
            Opis = "Istaknuti vrh Mosora iznad Splita; markiran pristup iz Dugopolja.",
            Koordinate = "N 43° 31' 20.0'' E 16° 31' 50.0''",
            OpisZiga = "Metalni žig na vrhu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 50,
            GUIDOznaka = "BIR6597",
            IdPodrucje = 18,
            Naziv = "Kozjak – vrh Biranj",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 631,
            Opis = "Vrh planine Kozjak iznad Kaštela s pogledom na Split i otoke.",
            Koordinate = "N 43° 33' 50.0'' E 16° 24' 10.0''",
            OpisZiga = "Metalni žig na kamenoj oznaci."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 51,
            GUIDOznaka = "SJU4213",
            IdPodrucje = 19,
            Naziv = "Sv. Jure – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1762,
            Opis = "Najviši vrh Biokova i drugi najviši vrh uz obalu; pristup cestom ili pješice.",
            Koordinate = "N 43° 20' 10.0'' E 17° 03' 00.0''",
            OpisZiga = "Metalni žig na kapeli Sv. Jure."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 52,
            GUIDOznaka = "VOS8746",
            IdPodrucje = 19,
            Naziv = "Vošac – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1421,
            Opis = "Popularan biokovački vrh s pogledom na makarsku rivijeru i otoke.",
            Koordinate = "N 43° 18' 55.0'' E 17° 04' 20.0''",
            OpisZiga = "Metalni žig na vršnom stupu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 53,
            GUIDOznaka = "KIM3509",
            IdPodrucje = 19,
            Naziv = "Kimet – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1536,
            Opis = "Zahtjevniji biokovački vrh; stjenovit i izložen vjetru.",
            Koordinate = "N 43° 19' 30.0'' E 17° 04' 50.0''",
            OpisZiga = "Metalni žig na stijeni."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 54,
            GUIDOznaka = "SIL2871",
            IdPodrucje = 20,
            Naziv = "Pelješac – vrh Sv. Ilija",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 960,
            Opis = "Najviši vrh poluotoka Pelješca; zahtjevna staza s pogledom na Korčulu i Mljet.",
            Koordinate = "N 42° 55' 20.0'' E 17° 07' 30.0''",
            OpisZiga = "Metalni žig na vršnom stupu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 55,
            GUIDOznaka = "ILJ6034",
            IdPodrucje = 20,
            Naziv = "Sniježnica – Ilijin vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1234,
            Opis = "Najviši vrh dubrovačkog zaleđa; panoramski pogled od Dubrovnika do crnogorskih planina.",
            Koordinate = "N 42° 38' 40.0'' E 18° 15' 10.0''",
            OpisZiga = "Metalni žig na vrhu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 56,
            GUIDOznaka = "VSV7283",
            IdPodrucje = 8,
            Naziv = "Viševica – vrh",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1428,
            Opis = "Istaknuti vrh južnog Gorskog kotara s pogledom na Kvarner i otoke.",
            Koordinate = "N 45° 18' 40.0'' E 14° 39' 50.0''",
            OpisZiga = "Metalni žig na vršnom stupu."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 57,
            GUIDOznaka = "CAR5190",
            IdPodrucje = 1,
            Naziv = "Dilj gora – vrh Čardak",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 421,
            Opis = "Najviši vrh Dilj gore kod Slavonskog Broda; blag i pristupačan vrh.",
            Koordinate = "N 45° 14' 30.0'' E 18° 07' 20.0''",
            OpisZiga = "Metalni žig na vršnoj oznaci."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 58,
            GUIDOznaka = "ZBE8416",
            IdPodrucje = 10,
            Naziv = "Ćićarija – vrh Žbevnica",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 1014,
            Opis = "Vrh Ćićarije s travnatim vršnim područjem i pogledom prema slovenskoj granici.",
            Koordinate = "N 45° 29' 10.0'' E 14° 08' 40.0''",
            OpisZiga = "Metalni žig na kamenoj oznaci."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 59,
            GUIDOznaka = "VRA2758",
            IdPodrucje = 3,
            Naziv = "Kalnik – vrh Vranilac",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 643,
            Opis = "Najviši vrh Kalnika sa stijenama i pogledom na Podravinu; zahtjevniji pristup.",
            Koordinate = "N 46° 09' 00.0'' E 16° 27' 30.0''",
            OpisZiga = "Metalni žig na stijeni kod vrha."
        },
        new KontrolnaTocka
        {
            IdKontrolnaTocka = 60,
            GUIDOznaka = "HOR6391",
            IdPodrucje = 4,
            Naziv = "Horvatovih 500 stuba",
            TipKontrolneTocke = TipKontrolneTocke.KontrolnaTocka,
            NadmorskaVisina = 450,
            Opis = "Poznate stube na Medvednici; jedna od dvije kontrolne točke HPO-a koje nisu vrhovi.",
            Koordinate = "N 45° 52' 10.0'' E 15° 57' 20.0''",
            OpisZiga = "Metalni žig na oznaci kod stuba."
        }
    };

    public static readonly Ruta[] NoveRute = new[]
    {
        new Ruta
        {
            IdRuta = 6,
            IdKontrolnaTocka = 8,
            Naziv = "Jankovac – Ivačka glava",
            Pocetak = "Jankovac",
            Kraj = "Ivačka glava",
            VrijemeHodaMin = 120,
            DuljinaKm = 5.5m,
            VisinskaRazlikaM = 530,
            Opis = "Pristup Papuku od planinarskog doma Jankovac kroz bukovu šumu do najvišeg slavonskog vrha.",
            OznakaNaTerenu = "PP-01",
            GodinaObnove = 2023,
            Napomena = "Dobro markiran put kroz park prirode Papuk.",
            TezinaRute = TezinaRute.Srednja,
            GPXPath = "C:\\GPX\\ruta_ivacka.gpx"
        },
        new Ruta
        {
            IdRuta = 7,
            IdKontrolnaTocka = 9,
            Naziv = "Brestovac – Brezovo polje",
            Pocetak = "Brestovac",
            Kraj = "Brezovo polje",
            VrijemeHodaMin = 150,
            DuljinaKm = 7.2m,
            VisinskaRazlikaM = 620,
            Opis = "Duži pristup Psunju iz sela Brestovac kroz šumu; pogodan za iskusnije planinare.",
            OznakaNaTerenu = "PS-01",
            GodinaObnove = 2022,
            Napomena = "Slabije markiran u gornjem dijelu.",
            TezinaRute = TezinaRute.Srednja,
            GPXPath = "C:\\GPX\\ruta_psunj.gpx"
        },
        new Ruta
        {
            IdRuta = 8,
            IdKontrolnaTocka = 12,
            Naziv = "Ivanec – Ivanščica vrh",
            Pocetak = "Ivanec",
            Kraj = "Ivanščica",
            VrijemeHodaMin = 180,
            DuljinaKm = 9.0m,
            VisinskaRazlikaM = 780,
            Opis = "Klasičan pristup najvišem vrhu Zagorja iz Ivanca preko planinarske kuće.",
            OznakaNaTerenu = "IZ-02",
            GodinaObnove = 2023,
            Napomena = "Dug, ali dobro markiran put.",
            TezinaRute = TezinaRute.Srednja,
            GPXPath = "C:\\GPX\\ruta_ivanscica.gpx"
        },
        new Ruta
        {
            IdRuta = 9,
            IdKontrolnaTocka = 14,
            Naziv = "Radoboj – Sušec",
            Pocetak = "Radoboj",
            Kraj = "Sušec",
            VrijemeHodaMin = 90,
            DuljinaKm = 4.0m,
            VisinskaRazlikaM = 450,
            Opis = "Kraći pristup vrhu Strahinjščice iz Radoboja kroz šumu.",
            OznakaNaTerenu = "SH-01",
            GodinaObnove = 2021,
            Napomena = "Pogodan za poluizlete.",
            TezinaRute = TezinaRute.Laka,
            GPXPath = "C:\\GPX\\ruta_susec.gpx"
        },
        new Ruta
        {
            IdRuta = 10,
            IdKontrolnaTocka = 15,
            Naziv = "Šestine – Grohot",
            Pocetak = "Šestine",
            Kraj = "Grohot",
            VrijemeHodaMin = 60,
            DuljinaKm = 3.2m,
            VisinskaRazlikaM = 280,
            Opis = "Kratak uspon od Šestina do vrha Grohot na Medvednici.",
            OznakaNaTerenu = "MED-07",
            GodinaObnove = 2024,
            Napomena = "Idealan za kratke popodnevne ture.",
            TezinaRute = TezinaRute.Laka,
            GPXPath = "C:\\GPX\\ruta_grohot.gpx"
        },
        new Ruta
        {
            IdRuta = 11,
            IdKontrolnaTocka = 17,
            Naziv = "Šestinski dol – Medvedgrad",
            Pocetak = "Šestinski dol",
            Kraj = "Medvedgrad",
            VrijemeHodaMin = 45,
            DuljinaKm = 2.5m,
            VisinskaRazlikaM = 300,
            Opis = "Kratak ali strm pristup utvrdi Medvedgrad s južne strane.",
            OznakaNaTerenu = "MED-02",
            GodinaObnove = 2024,
            Napomena = "Popularna obiteljska ruta.",
            TezinaRute = TezinaRute.Laka,
            GPXPath = "C:\\GPX\\ruta_medvedgrad.gpx"
        },
        new Ruta
        {
            IdRuta = 12,
            IdKontrolnaTocka = 18,
            Naziv = "Poljanica – Plešivica",
            Pocetak = "Poljanica Samoborska",
            Kraj = "Plešivica – vrh",
            VrijemeHodaMin = 75,
            DuljinaKm = 4.0m,
            VisinskaRazlikaM = 420,
            Opis = "Pristup Plešivici s južne strane iz Poljanice kroz vinograde i šumu.",
            OznakaNaTerenu = "SG-02",
            GodinaObnove = 2021,
            Napomena = "Lijep pogled na vinograde tijekom uspona.",
            TezinaRute = TezinaRute.Laka,
            GPXPath = "C:\\GPX\\ruta_plesivica.gpx"
        },
        new Ruta
        {
            IdRuta = 13,
            IdKontrolnaTocka = 19,
            Naziv = "Japetić dom – Oštrc",
            Pocetak = "Planinarski dom Žitnica",
            Kraj = "Oštrc – vrh",
            VrijemeHodaMin = 60,
            DuljinaKm = 3.0m,
            VisinskaRazlikaM = 280,
            Opis = "Grebenski prijelaz od doma Žitnica kod Japetića do vrha Oštrc preko kapele Sv. Ane.",
            OznakaNaTerenu = "SG-03",
            GodinaObnove = 2022,
            Napomena = "Atraktivan grebenski put s pogledima.",
            TezinaRute = TezinaRute.Srednja,
            GPXPath = "C:\\GPX\\ruta_ostrc.gpx"
        },
        new Ruta
        {
            IdRuta = 14,
            IdKontrolnaTocka = 21,
            Naziv = "Budinjak – Sveta Gera",
            Pocetak = "Budinjak",
            Kraj = "Sveta Gera",
            VrijemeHodaMin = 180,
            DuljinaKm = 8.5m,
            VisinskaRazlikaM = 650,
            Opis = "Dugačak pristup najvišem vrhu Žumberačke gore iz Budinjaka.",
            OznakaNaTerenu = "ZG-01",
            GodinaObnove = 2021,
            Napomena = "Potrebna dobra kondicija za dulji uspon.",
            TezinaRute = TezinaRute.Teska,
            GPXPath = "C:\\GPX\\ruta_svetagera.gpx"
        },
        new Ruta
        {
            IdRuta = 15,
            IdKontrolnaTocka = 25,
            Naziv = "Bjelsko – Klek",
            Pocetak = "Bjelsko",
            Kraj = "Klek – vrh",
            VrijemeHodaMin = 120,
            DuljinaKm = 4.5m,
            VisinskaRazlikaM = 780,
            Opis = "Klasičan pristup Kleku iz sela Bjelsko; strm završni dio uz pomoć sajli.",
            OznakaNaTerenu = "GK-01",
            GodinaObnove = 2023,
            Napomena = "Završni dio zahtijeva osnovnu opremu i iskustvo.",
            TezinaRute = TezinaRute.Teska,
            GPXPath = "C:\\GPX\\ruta_klek.gpx"
        },
        new Ruta
        {
            IdRuta = 16,
            IdKontrolnaTocka = 26,
            Naziv = "Begovo Razdolje – Bjelolasica",
            Pocetak = "Begovo Razdolje",
            Kraj = "Bjelolasica – Kula",
            VrijemeHodaMin = 90,
            DuljinaKm = 5.0m,
            VisinskaRazlikaM = 430,
            Opis = "Pristup najvišem vrhu Gorskog kotara iz Begovog Razdolja.",
            OznakaNaTerenu = "GK-05",
            GodinaObnove = 2024,
            Napomena = "Relativno lagodan pristup s makadama.",
            TezinaRute = TezinaRute.Srednja,
            GPXPath = "C:\\GPX\\ruta_bjelolasica.gpx"
        },
        new Ruta
        {
            IdRuta = 17,
            IdKontrolnaTocka = 28,
            Naziv = "Crni Lug – Risnjak",
            Pocetak = "Crni Lug",
            Kraj = "Risnjak – vrh",
            VrijemeHodaMin = 150,
            DuljinaKm = 7.0m,
            VisinskaRazlikaM = 680,
            Opis = "Klasičan pristup Risnjaku iz Crnog Luga kroz nacionalni park.",
            OznakaNaTerenu = "GK-08",
            GodinaObnove = 2024,
            Napomena = "Prolaz kroz NP Risnjak; plaćanje ulaznice.",
            TezinaRute = TezinaRute.Srednja,
            GPXPath = "C:\\GPX\\ruta_risnjak.gpx"
        },
        new Ruta
        {
            IdRuta = 18,
            IdKontrolnaTocka = 29,
            Naziv = "Platak – Snježnik",
            Pocetak = "Platak",
            Kraj = "Snježnik – vrh",
            VrijemeHodaMin = 120,
            DuljinaKm = 5.5m,
            VisinskaRazlikaM = 510,
            Opis = "Pristup Snježniku s Platka preko planinskog doma.",
            OznakaNaTerenu = "GK-10",
            GodinaObnove = 2023,
            Napomena = "Može imati snijega do kasnog proljeća.",
            TezinaRute = TezinaRute.Srednja,
            GPXPath = "C:\\GPX\\ruta_snjeznik.gpx"
        },
        new Ruta
        {
            IdRuta = 19,
            IdKontrolnaTocka = 31,
            Naziv = "Poklon – Vojak",
            Pocetak = "Poklon",
            Kraj = "Učka – Vojak",
            VrijemeHodaMin = 90,
            DuljinaKm = 4.2m,
            VisinskaRazlikaM = 520,
            Opis = "Najpopularniji pristup Vojaku s prijevoja Poklon; dobro markiran.",
            OznakaNaTerenu = "IS-01",
            GodinaObnove = 2024,
            Napomena = "Najpopularnija ruta na Učki.",
            TezinaRute = TezinaRute.Laka,
            GPXPath = "C:\\GPX\\ruta_vojak.gpx"
        },
        new Ruta
        {
            IdRuta = 20,
            IdKontrolnaTocka = 33,
            Naziv = "Alan – Mali Rajinac",
            Pocetak = "Planinarski dom Alan",
            Kraj = "Mali Rajinac",
            VrijemeHodaMin = 180,
            DuljinaKm = 8.0m,
            VisinskaRazlikaM = 650,
            Opis = "Zahtjevan pristup jednom od najviših velebitskih vrhova iz doma Alan.",
            OznakaNaTerenu = "SV-03",
            GodinaObnove = 2023,
            Napomena = "Ozbiljan krški teren; potrebna dobra oprema.",
            TezinaRute = TezinaRute.Teska,
            GPXPath = "C:\\GPX\\ruta_mrajinac.gpx"
        },
        new Ruta
        {
            IdRuta = 21,
            IdKontrolnaTocka = 36,
            Naziv = "Starigrad Paklenica – Vaganski vrh",
            Pocetak = "Starigrad-Paklenica",
            Kraj = "Vaganski vrh",
            VrijemeHodaMin = 360,
            DuljinaKm = 14.0m,
            VisinskaRazlikaM = 1550,
            Opis = "Dugi i zahtjevni uspon na najviši vrh Velebita kroz NP Paklenica.",
            OznakaNaTerenu = "JV-01",
            GodinaObnove = 2024,
            Napomena = "Cijeli dan hoda; potrebna odlična kondicija.",
            TezinaRute = TezinaRute.Teska,
            GPXPath = "C:\\GPX\\ruta_vaganski.gpx"
        },
        new Ruta
        {
            IdRuta = 22,
            IdKontrolnaTocka = 38,
            Naziv = "Velika Paklenica – Anića kuk",
            Pocetak = "Velika Paklenica ulaz",
            Kraj = "Anića kuk – vrh",
            VrijemeHodaMin = 120,
            DuljinaKm = 3.5m,
            VisinskaRazlikaM = 500,
            Opis = "Pristup Anića kuku iz klanca Velike Paklenice; alpinistički značajan vrh.",
            OznakaNaTerenu = "JV-04",
            GodinaObnove = 2022,
            Napomena = "Završni dio tehnički zahtjevan.",
            TezinaRute = TezinaRute.Teska,
            GPXPath = "C:\\GPX\\ruta_anicakuk.gpx"
        },
        new Ruta
        {
            IdRuta = 23,
            IdKontrolnaTocka = 39,
            Naziv = "Glogovac – Ozeblin",
            Pocetak = "Glogovac",
            Kraj = "Ozeblin",
            VrijemeHodaMin = 240,
            DuljinaKm = 10.0m,
            VisinskaRazlikaM = 900,
            Opis = "Dugačak pristup najvišem vrhu Like iz sela Glogovac.",
            OznakaNaTerenu = "LI-01",
            GodinaObnove = 2021,
            Napomena = "Slabije markiran gornji dio; potrebna navigacija.",
            TezinaRute = TezinaRute.Teska,
            GPXPath = "C:\\GPX\\ruta_ozeblin.gpx"
        },
        new Ruta
        {
            IdRuta = 24,
            IdKontrolnaTocka = 41,
            Naziv = "Baška – Obzova",
            Pocetak = "Baška",
            Kraj = "Obzova – vrh",
            VrijemeHodaMin = 120,
            DuljinaKm = 5.5m,
            VisinskaRazlikaM = 500,
            Opis = "Pristup najvišem vrhu Krka iz Baške; otočni krški teren.",
            OznakaNaTerenu = "OT-01",
            GodinaObnove = 2023,
            Napomena = "Ljeti ponijeti dovoljno vode.",
            TezinaRute = TezinaRute.Srednja,
            GPXPath = "C:\\GPX\\ruta_obzova.gpx"
        },
        new Ruta
        {
            IdRuta = 25,
            IdKontrolnaTocka = 43,
            Naziv = "Nerežišća – Vidova gora",
            Pocetak = "Nerežišća",
            Kraj = "Vidova gora",
            VrijemeHodaMin = 90,
            DuljinaKm = 4.5m,
            VisinskaRazlikaM = 480,
            Opis = "Pristup najvišem otočnom vrhu iz mjesta Nerežišća; pogled na Zlatni rat.",
            OznakaNaTerenu = "OT-05",
            GodinaObnove = 2024,
            Napomena = "Popularna turistička ruta s izvrsnim vidikom.",
            TezinaRute = TezinaRute.Laka,
            GPXPath = "C:\\GPX\\ruta_vidovagora.gpx"
        },
        new Ruta
        {
            IdRuta = 26,
            IdKontrolnaTocka = 46,
            Naziv = "Glavaš – Dinara (Sinjal)",
            Pocetak = "Glavaš",
            Kraj = "Dinara (Sinjal)",
            VrijemeHodaMin = 240,
            DuljinaKm = 9.0m,
            VisinskaRazlikaM = 950,
            Opis = "Klasičan pristup najvišem vrhu Hrvatske iz zaseoka Glavaš iznad Vrlike.",
            OznakaNaTerenu = "DZ-01",
            GodinaObnove = 2024,
            Napomena = "Obavezna točka za srebrnu značku HPO-a. Zahtjevan pristup.",
            TezinaRute = TezinaRute.Teska,
            GPXPath = "C:\\GPX\\ruta_dinara.gpx"
        },
        new Ruta
        {
            IdRuta = 27,
            IdKontrolnaTocka = 47,
            Naziv = "Muć – Svilaja",
            Pocetak = "Muć",
            Kraj = "Svilaja – vrh",
            VrijemeHodaMin = 210,
            DuljinaKm = 9.5m,
            VisinskaRazlikaM = 1050,
            Opis = "Dugi pristup vrhu Svilaje iz Muća kroz dalmatinsko zaleđe.",
            OznakaNaTerenu = "DZ-03",
            GodinaObnove = 2022,
            Napomena = "Zahtjevan uspon po toplom vremenu.",
            TezinaRute = TezinaRute.Teska,
            GPXPath = "C:\\GPX\\ruta_svilaja.gpx"
        },
        new Ruta
        {
            IdRuta = 28,
            IdKontrolnaTocka = 49,
            Naziv = "Dugopolje – Ljubljan",
            Pocetak = "Dugopolje",
            Kraj = "Mosor – Ljubljan",
            VrijemeHodaMin = 150,
            DuljinaKm = 6.5m,
            VisinskaRazlikaM = 860,
            Opis = "Pristup Mosoru iz Dugopolja s markiranim putom prema vrhu Ljubljan.",
            OznakaNaTerenu = "DA-02",
            GodinaObnove = 2023,
            Napomena = "Popularna splitska planinarska ruta.",
            TezinaRute = TezinaRute.Srednja,
            GPXPath = "C:\\GPX\\ruta_mosor.gpx"
        },
        new Ruta
        {
            IdRuta = 29,
            IdKontrolnaTocka = 51,
            Naziv = "Bast – Sv. Jure Biokovo",
            Pocetak = "Bast",
            Kraj = "Sv. Jure",
            VrijemeHodaMin = 300,
            DuljinaKm = 11.0m,
            VisinskaRazlikaM = 1600,
            Opis = "Najzahtjevniji pristup Biokovu iz Basta na obali; ogromna visinska razlika.",
            OznakaNaTerenu = "BI-01",
            GodinaObnove = 2024,
            Napomena = "Iznimno zahtjevna ruta; cijeli dan hoda.",
            TezinaRute = TezinaRute.Teska,
            GPXPath = "C:\\GPX\\ruta_svjure_biokovo.gpx"
        },
        new Ruta
        {
            IdRuta = 30,
            IdKontrolnaTocka = 52,
            Naziv = "Makarska – Vošac",
            Pocetak = "Makarska",
            Kraj = "Vošac – vrh",
            VrijemeHodaMin = 180,
            DuljinaKm = 6.0m,
            VisinskaRazlikaM = 1300,
            Opis = "Popularan uspon na Biokovo iz Makarske s pogledom na rivijeru.",
            OznakaNaTerenu = "BI-03",
            GodinaObnove = 2023,
            Napomena = "Strm, ali dobro markiran pristup.",
            TezinaRute = TezinaRute.Teska,
            GPXPath = "C:\\GPX\\ruta_vosac.gpx"
        },
        new Ruta
        {
            IdRuta = 31,
            IdKontrolnaTocka = 54,
            Naziv = "Orebić – Sv. Ilija Pelješac",
            Pocetak = "Orebić",
            Kraj = "Sv. Ilija Pelješac",
            VrijemeHodaMin = 180,
            DuljinaKm = 6.5m,
            VisinskaRazlikaM = 900,
            Opis = "Pristup najvišem pelješkom vrhu iz Orebića; pogled na Korčulu.",
            OznakaNaTerenu = "DU-01",
            GodinaObnove = 2022,
            Napomena = "Zahtjevan uspon, posebno ljeti.",
            TezinaRute = TezinaRute.Teska,
            GPXPath = "C:\\GPX\\ruta_svilija_peljesac.gpx"
        },
        new Ruta
        {
            IdRuta = 32,
            IdKontrolnaTocka = 55,
            Naziv = "Pridvorje – Sniježnica",
            Pocetak = "Pridvorje",
            Kraj = "Sniježnica – Ilijin vrh",
            VrijemeHodaMin = 150,
            DuljinaKm = 6.0m,
            VisinskaRazlikaM = 750,
            Opis = "Pristup najvišem vrhu dubrovačkog zaleđa iz Pridvorja.",
            OznakaNaTerenu = "DU-02",
            GodinaObnove = 2021,
            Napomena = "Ljeti ponijeti dovoljno vode; manje markacija.",
            TezinaRute = TezinaRute.Srednja,
            GPXPath = "C:\\GPX\\ruta_snijeznica.gpx"
        },
        new Ruta
        {
            IdRuta = 33,
            IdKontrolnaTocka = 7,
            Naziv = "Našice – Kapovac",
            Pocetak = "Našice",
            Kraj = "Kapovac",
            VrijemeHodaMin = 150,
            DuljinaKm = 7.0m,
            VisinskaRazlikaM = 540,
            Opis = "Pristup vrhu Krndije iz Našica preko šumskih putova.",
            OznakaNaTerenu = "SL-01",
            GodinaObnove = 2022,
            Napomena = "Dulji pristup kroz slavonsku šumu.",
            TezinaRute = TezinaRute.Srednja,
            GPXPath = "C:\\GPX\\ruta_kapovac.gpx"
        },
        new Ruta
        {
            IdRuta = 34,
            IdKontrolnaTocka = 22,
            Naziv = "Budinjak – Pliješ",
            Pocetak = "Budinjak",
            Kraj = "Pliješ – vrh",
            VrijemeHodaMin = 120,
            DuljinaKm = 5.5m,
            VisinskaRazlikaM = 500,
            Opis = "Pristup Pliješu iz Budinjaka kroz Žumberačku goru.",
            OznakaNaTerenu = "ZG-02",
            GodinaObnove = 2023,
            Napomena = "Umjeren pristup šumskim putevima.",
            TezinaRute = TezinaRute.Srednja,
            GPXPath = "C:\\GPX\\ruta_plijes.gpx"
        },
        new Ruta
        {
            IdRuta = 35,
            IdKontrolnaTocka = 59,
            Naziv = "Kalnik selo – Vranilac",
            Pocetak = "Kalnik (selo)",
            Kraj = "Vranilac – vrh",
            VrijemeHodaMin = 90,
            DuljinaKm = 3.5m,
            VisinskaRazlikaM = 340,
            Opis = "Pristup Kalniku iz istoimenog sela; strm završni dio uz stijene.",
            OznakaNaTerenu = "ZA-03",
            GodinaObnove = 2022,
            Napomena = "Završni dio zahtijeva pažnju.",
            TezinaRute = TezinaRute.Srednja,
            GPXPath = "C:\\GPX\\ruta_vranilac.gpx"
        }
    };
}
