using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace planinarenje.Migrations
{
    /// <inheritdoc />
    public partial class AddExpandedKontrolneTockeIRute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "KontrolneTocke",
                columns: new[] { "IdKontrolnaTocka", "DeletedAt", "GUIDOznaka", "IdPodrucje", "Koordinate", "NadmorskaVisina", "Naziv", "Opis", "OpisZiga", "TipKontrolneTocke" },
                values: new object[,]
                {
                    { 7, null, "KAP8371", 1, "N 45° 28' 12.0'' E 17° 52' 30.0''", 790, "Krndija – vrh Kapovac", "Najviši vrh Krndije u slavonskom gorju; šumoviti vrh s markiranim pristupom.", "Metalni žig na vršnoj oznaci.", 0 },
                    { 8, null, "IVA5629", 1, "N 45° 31' 10.0'' E 17° 40' 15.0''", 913, "Papuk – vrh Ivačka glava", "Najviši vrh Papuka i cijele Slavonije; dostupan s više strana.", "Metalni žig na vrhu kod geodetskog stupa.", 0 },
                    { 9, null, "BRE7412", 1, "N 45° 16' 45.0'' E 17° 18' 20.0''", 984, "Psunj – vrh Brezovo polje", "Najviši vrh Psunja i jedan od najviših slavonskih vrhova; šumovit i miran.", "Metalni žig na oznaci vrha.", 0 },
                    { 10, null, "STA2087", 2, "N 45° 53' 00.0'' E 17° 07' 30.0''", 309, "Bilogora – Stankov vrh", "Najviši vrh Bilogore s vidikovcem i planinarskim putom kroz šumu.", "Metalni žig na drvenom stupu kod vidikovca.", 0 },
                    { 11, null, "MOH6243", 3, "N 46° 24' 50.0'' E 16° 22' 10.0''", 344, "Međimurske gorice – vrh Mohokos", "Najviši vrh Međimurja; lagani pristup i lijep pogled prema Alpama i Zagorju.", "Metalni žig na oznaci vrha.", 0 },
                    { 12, null, "IVN3815", 3, "N 46° 10' 55.0'' E 16° 06' 45.0''", 1060, "Ivanščica – vrh Ivanščica", "Najviši vrh Hrvatskog zagorja i najistaknutiji zagorski vrh s panoramskim vidicima.", "Metalni žig na vršnom stupu.", 0 },
                    { 13, null, "RAV9174", 3, "N 46° 04' 20.0'' E 15° 56' 30.0''", 680, "Ravna gora – vrh (piramida)", "Šumoviti vrh s geodetskom piramidom i markiranim pristupom iz Gornje Stubice.", "Metalni žig na piramidi.", 0 },
                    { 14, null, "SUS4538", 3, "N 46° 11' 40.0'' E 15° 54' 20.0''", 846, "Strahinjščica – vrh Sušec", "Najviši vrh Strahinjščice s pogledom prema Ivanščici i Krapinskoj dolini.", "Metalni žig na kamenoj oznaci vrha.", 0 },
                    { 15, null, "GRH7260", 4, "N 45° 52' 30.0'' E 16° 03' 10.0''", 492, "Grohot – vrh", "Niži vrh Medvednice s vidikovcem i starim hrastovima; pogodan za kraće ture.", "Metalni žig na drvenoj oznaci vrha.", 0 },
                    { 16, null, "LIP3492", 4, "N 45° 54' 10.0'' E 15° 55' 40.0''", 709, "Lipa – vrh", "Šumoviti vrh Medvednice na sjevernom grebenu; miran i manje posjećen.", "Metalni žig na vršnom stupu.", 0 },
                    { 17, null, "MEG8156", 4, "N 45° 51' 45.0'' E 15° 56' 50.0''", 579, "Medvedgrad", "Srednjovjekovna utvrda na južnim padinama Medvednice; kontrolna točka HPO-a.", "Metalni žig na ulaznom zidu utvrde.", 2 },
                    { 18, null, "PLE6703", 5, "N 45° 43' 30.0'' E 15° 39' 20.0''", 779, "Plešivica – vrh", "Vrh Samoborskog gorja s pogledom na vinograde i Žumberak; blizu planinarskog doma.", "Metalni žig na kamenoj oznaci.", 0 },
                    { 19, null, "OST5281", 5, "N 45° 44' 10.0'' E 15° 40' 55.0''", 752, "Oštrc – vrh", "Popularan vrh s kapelom Sv. Ane na vrhu i panoramskim vidicima.", "Metalni žig na kapelici na vrhu.", 0 },
                    { 20, null, "TUS9047", 6, "N 45° 44' 00.0'' E 15° 30' 10.0''", 585, "Tuščak – gradina", "Stara gradina na zapadnom dijelu Žumberačke gore; pogled prema Žumberku.", "Metalni žig na ruševini gradine.", 2 },
                    { 21, null, "SGE2634", 6, "N 45° 42' 45.0'' E 15° 22' 30.0''", 1178, "Sveta Gera – vrh", "Najviši vrh Žumberačke gore i cijele Žumberačko-samoborske regije.", "Metalni žig na vršnom stupu.", 0 },
                    { 22, null, "PLI7819", 6, "N 45° 43' 20.0'' E 15° 25' 50.0''", 977, "Pliješ – vrh", "Šumoviti vrh Žumberačke gore s markiranim putom iz Budinjaka.", "Metalni žig na oznaci vrha.", 0 },
                    { 23, null, "VOD4153", 7, "N 45° 27' 10.0'' E 15° 32' 20.0''", 538, "Vodenica – vrh", "Najviši vrh Pokuplja; miran vrh s pogledom na Kupu i okolne šume.", "Metalni žig na drvenoj oznaci.", 0 },
                    { 24, null, "PET6928", 7, "N 45° 19' 20.0'' E 15° 47' 00.0''", 512, "Petrova gora – vrh Petrovac", "Vrh Petrove gore s poznatim spomenikom i vidikovcem prema Kordunu.", "Metalni žig na spomeniku kod vrha.", 0 },
                    { 25, null, "KLE3047", 8, "N 45° 17' 55.0'' E 15° 10' 40.0''", 1181, "Klek – vrh", "Karakteristična stijena iznad Ogulina; simbol hrvatskog planinarstva od 1874. godine.", "Metalni žig na vršnom stupu.", 0 },
                    { 26, null, "BJE8592", 8, "N 45° 15' 50.0'' E 14° 58' 30.0''", 1534, "Bjelolasica – vrh Kula", "Najviši vrh Gorskog kotara i hrvatski vrh izvan Velebita i Dinare.", "Metalni žig na geodetskom stupu na vrhu.", 0 },
                    { 27, null, "SAM1736", 8, "N 45° 16' 20.0'' E 14° 55' 10.0''", 1302, "Samarske stijene – vrh", "Spektakularne stjenovite formacije u srcu Gorskog kotara; zahtjevan pristup.", "Metalni žig na stijeni kod vrha.", 0 },
                    { 28, null, "RIS4208", 9, "N 45° 25' 35.0'' E 14° 45' 20.0''", 1528, "Risnjak – vrh", "Najviši vrh istoimenog nacionalnog parka; panoramski pogled od Alpa do mora.", "Metalni žig na vršnom stupu kod kapelice.", 0 },
                    { 29, null, "SNJ6371", 9, "N 45° 26' 10.0'' E 14° 35' 40.0''", 1505, "Snježnik – vrh", "Drugi najviši vrh Gorskog kotara; poznat po kasnom snijegu i alpskim livadama.", "Metalni žig na kamenoj oznaci vrha.", 0 },
                    { 30, null, "SKR2845", 9, "N 45° 24' 05.0'' E 15° 02' 15.0''", 1043, "Skradski vrh", "Popularan izletnički vrh u sjevernom Gorskom kotaru s planinarskim domom.", "Metalni žig na vršnom stupu.", 0 },
                    { 31, null, "VOJ7164", 10, "N 45° 17' 10.0'' E 14° 11' 55.0''", 1396, "Učka – vrh Vojak", "Najviši vrh Istre s kamenim tornjem na vrhu i pogledom na Kvarner i Alpe.", "Metalni žig na kamenom tornju na vrhu.", 0 },
                    { 32, null, "VPL3920", 10, "N 45° 27' 20.0'' E 14° 13' 30.0''", 1272, "Ćićarija – vrh Veliki Planik", "Najviši vrh Ćićarije s travnatim vršnim područjem i pogledom prema Učki.", "Metalni žig na kamenoj oznaci.", 0 },
                    { 33, null, "MRA8451", 11, "N 44° 46' 30.0'' E 14° 58' 50.0''", 1699, "Mali Rajinac – vrh", "Jedan od najviših velebitskih vrhova na sjevernom dijelu; krški vrh s divljim pogledom.", "Metalni žig na vrhu stijene.", 0 },
                    { 34, null, "ZEC6237", 12, "N 44° 36' 15.0'' E 15° 03' 40.0''", 1622, "Zečjak – vrh", "Najviši vrh Srednjeg Velebita; stjenovit i zahtjevan teren.", "Metalni žig na kamenoj piramidi.", 0 },
                    { 35, null, "SAT1584", 12, "N 44° 34' 50.0'' E 15° 05' 10.0''", 1622, "Šatorina – vrh", "Karakteristični vrh Srednjeg Velebita s oblikom šatora; divlji krški krajolik.", "Metalni žig na oznaci vrha.", 0 },
                    { 36, null, "VAG7302", 13, "N 44° 21' 50.0'' E 15° 30' 20.0''", 1757, "Vaganski vrh", "Najviši vrh Velebita i treći najviši vrh Hrvatske; zahtjevan pristup iz Paklenice.", "Metalni žig na geodetskom stupu na vrhu.", 0 },
                    { 37, null, "SVB4916", 13, "N 44° 19' 40.0'' E 15° 30' 55.0''", 1751, "Sveto brdo – vrh", "Drugi najviši vrh Velebita s kapelom na vrhu; pogled na more i Liku.", "Metalni žig na kapeli na vrhu.", 0 },
                    { 38, null, "ANI2058", 13, "N 44° 18' 15.0'' E 15° 27' 40.0''", 712, "Anića kuk – vrh", "Impozantna stijena u klancu Velike Paklenice; alpinistički značajan vrh.", "Metalni žig na vršnom stupu.", 0 },
                    { 39, null, "OZE8743", 14, "N 44° 46' 10.0'' E 15° 44' 30.0''", 1657, "Lička Plješivica – vrh Ozeblin", "Najviši vrh Ličke Plješivice i Like; zahtjevan pristup šumskim putovima.", "Metalni žig na vrhu.", 0 },
                    { 40, null, "POT5261", 14, "N 44° 10' 55.0'' E 16° 10' 20.0''", 1425, "Poštak – vrh", "Istaknuti lički vrh na granici prema Dalmaciji s otvorenim pogledom.", "Metalni žig na vršnoj oznaci.", 0 },
                    { 41, null, "OBZ3179", 15, "N 45° 01' 20.0'' E 14° 37' 50.0''", 569, "Krk – vrh Obzova", "Najviši vrh otoka Krka s pogledom na Kvarner i okolne otoke.", "Metalni žig na vršnom stupu.", 0 },
                    { 42, null, "SIS6420", 15, "N 44° 52' 30.0'' E 14° 22' 10.0''", 639, "Cres – vrh Sis", "Najviši vrh otoka Cresa; divlji otočni krajolik s pogledom na Jadran.", "Metalni žig na kamenoj oznaci.", 0 },
                    { 43, null, "VID8537", 16, "N 43° 18' 40.0'' E 16° 37' 20.0''", 780, "Brač – vrh Vidova gora", "Najviši vrh jadranskih otoka; spektakularan pogled na Zlatni rat i Hvar.", "Metalni žig na vršnom stupu.", 0 },
                    { 44, null, "SNK2074", 16, "N 43° 10' 35.0'' E 16° 39' 50.0''", 626, "Hvar – vrh Sv. Nikola", "Najviši vrh otoka Hvara s pogledom na paklinske otoke i pelješku obalu.", "Metalni žig na kapelici Sv. Nikole.", 0 },
                    { 45, null, "KOM9361", 16, "N 42° 57' 30.0'' E 16° 53' 15.0''", 508, "Korčula – vrh Kom", "Najviši vrh otoka Korčule s gustim makijama i pogledom na Pelješac.", "Metalni žig na kamenoj oznaci.", 0 },
                    { 46, null, "DIN4728", 17, "N 43° 59' 25.0'' E 16° 22' 50.0''", 1831, "Dinara – vrh Dinara (Sinjal)", "Najviši vrh Republike Hrvatske; obavezna kontrolna točka za srebrnu značku HPO-a.", "Metalni žig na geodetskom stupu na vrhu.", 0 },
                    { 47, null, "SVL5839", 17, "N 43° 44' 10.0'' E 16° 28' 30.0''", 1508, "Svilaja – vrh Svilaja", "Najviši vrh planine Svilaje u dalmatinskom zaleđu; zahtjevan pristup.", "Metalni žig na vršnom stupu.", 0 },
                    { 48, null, "CAV7162", 17, "N 43° 51' 40.0'' E 16° 05' 20.0''", 1147, "Promina – vrh Čavnovka", "Najviši vrh planine Promine iznad Drniša; pogled na Krku i Zagoru.", "Metalni žig na kamenoj oznaci.", 0 },
                    { 49, null, "LJU3084", 18, "N 43° 31' 20.0'' E 16° 31' 50.0''", 1262, "Mosor – vrh Ljubljan", "Istaknuti vrh Mosora iznad Splita; markiran pristup iz Dugopolja.", "Metalni žig na vrhu.", 0 },
                    { 50, null, "BIR6597", 18, "N 43° 33' 50.0'' E 16° 24' 10.0''", 631, "Kozjak – vrh Biranj", "Vrh planine Kozjak iznad Kaštela s pogledom na Split i otoke.", "Metalni žig na kamenoj oznaci.", 0 },
                    { 51, null, "SJU4213", 19, "N 43° 20' 10.0'' E 17° 03' 00.0''", 1762, "Sv. Jure – vrh", "Najviši vrh Biokova i drugi najviši vrh uz obalu; pristup cestom ili pješice.", "Metalni žig na kapeli Sv. Jure.", 0 },
                    { 52, null, "VOS8746", 19, "N 43° 18' 55.0'' E 17° 04' 20.0''", 1421, "Vošac – vrh", "Popularan biokovački vrh s pogledom na makarsku rivijeru i otoke.", "Metalni žig na vršnom stupu.", 0 },
                    { 53, null, "KIM3509", 19, "N 43° 19' 30.0'' E 17° 04' 50.0''", 1536, "Kimet – vrh", "Zahtjevniji biokovački vrh; stjenovit i izložen vjetru.", "Metalni žig na stijeni.", 0 },
                    { 54, null, "SIL2871", 20, "N 42° 55' 20.0'' E 17° 07' 30.0''", 960, "Pelješac – vrh Sv. Ilija", "Najviši vrh poluotoka Pelješca; zahtjevna staza s pogledom na Korčulu i Mljet.", "Metalni žig na vršnom stupu.", 0 },
                    { 55, null, "ILJ6034", 20, "N 42° 38' 40.0'' E 18° 15' 10.0''", 1234, "Sniježnica – Ilijin vrh", "Najviši vrh dubrovačkog zaleđa; panoramski pogled od Dubrovnika do crnogorskih planina.", "Metalni žig na vrhu.", 0 },
                    { 56, null, "VSV7283", 8, "N 45° 18' 40.0'' E 14° 39' 50.0''", 1428, "Viševica – vrh", "Istaknuti vrh južnog Gorskog kotara s pogledom na Kvarner i otoke.", "Metalni žig na vršnom stupu.", 0 },
                    { 57, null, "CAR5190", 1, "N 45° 14' 30.0'' E 18° 07' 20.0''", 421, "Dilj gora – vrh Čardak", "Najviši vrh Dilj gore kod Slavonskog Broda; blag i pristupačan vrh.", "Metalni žig na vršnoj oznaci.", 0 },
                    { 58, null, "ZBE8416", 10, "N 45° 29' 10.0'' E 14° 08' 40.0''", 1014, "Ćićarija – vrh Žbevnica", "Vrh Ćićarije s travnatim vršnim područjem i pogledom prema slovenskoj granici.", "Metalni žig na kamenoj oznaci.", 0 },
                    { 59, null, "VRA2758", 3, "N 46° 09' 00.0'' E 16° 27' 30.0''", 643, "Kalnik – vrh Vranilac", "Najviši vrh Kalnika sa stijenama i pogledom na Podravinu; zahtjevniji pristup.", "Metalni žig na stijeni kod vrha.", 0 },
                    { 60, null, "HOR6391", 4, "N 45° 52' 10.0'' E 15° 57' 20.0''", 450, "Horvatovih 500 stuba", "Poznate stube na Medvednici; jedna od dvije kontrolne točke HPO-a koje nisu vrhovi.", "Metalni žig na oznaci kod stuba.", 2 }
                });

            migrationBuilder.InsertData(
                table: "Rute",
                columns: new[] { "IdRuta", "DeletedAt", "DuljinaKm", "GPXPath", "GodinaObnove", "IdKontrolnaTocka", "Kraj", "Napomena", "Naziv", "Opis", "OznakaNaTerenu", "Pocetak", "TezinaRute", "VisinskaRazlikaM", "VrijemeHodaMin" },
                values: new object[,]
                {
                    { 6, null, 5.5m, "C:\\GPX\\ruta_ivacka.gpx", 2023, 8, "Ivačka glava", "Dobro markiran put kroz park prirode Papuk.", "Jankovac – Ivačka glava", "Pristup Papuku od planinarskog doma Jankovac kroz bukovu šumu do najvišeg slavonskog vrha.", "PP-01", "Jankovac", 1, 530, 120 },
                    { 7, null, 7.2m, "C:\\GPX\\ruta_psunj.gpx", 2022, 9, "Brezovo polje", "Slabije markiran u gornjem dijelu.", "Brestovac – Brezovo polje", "Duži pristup Psunju iz sela Brestovac kroz šumu; pogodan za iskusnije planinare.", "PS-01", "Brestovac", 1, 620, 150 },
                    { 8, null, 9.0m, "C:\\GPX\\ruta_ivanscica.gpx", 2023, 12, "Ivanščica", "Dug, ali dobro markiran put.", "Ivanec – Ivanščica vrh", "Klasičan pristup najvišem vrhu Zagorja iz Ivanca preko planinarske kuće.", "IZ-02", "Ivanec", 1, 780, 180 },
                    { 9, null, 4.0m, "C:\\GPX\\ruta_susec.gpx", 2021, 14, "Sušec", "Pogodan za poluizlete.", "Radoboj – Sušec", "Kraći pristup vrhu Strahinjščice iz Radoboja kroz šumu.", "SH-01", "Radoboj", 0, 450, 90 },
                    { 10, null, 3.2m, "C:\\GPX\\ruta_grohot.gpx", 2024, 15, "Grohot", "Idealan za kratke popodnevne ture.", "Šestine – Grohot", "Kratak uspon od Šestina do vrha Grohot na Medvednici.", "MED-07", "Šestine", 0, 280, 60 },
                    { 11, null, 2.5m, "C:\\GPX\\ruta_medvedgrad.gpx", 2024, 17, "Medvedgrad", "Popularna obiteljska ruta.", "Šestinski dol – Medvedgrad", "Kratak ali strm pristup utvrdi Medvedgrad s južne strane.", "MED-02", "Šestinski dol", 0, 300, 45 },
                    { 12, null, 4.0m, "C:\\GPX\\ruta_plesivica.gpx", 2021, 18, "Plešivica – vrh", "Lijep pogled na vinograde tijekom uspona.", "Poljanica – Plešivica", "Pristup Plešivici s južne strane iz Poljanice kroz vinograde i šumu.", "SG-02", "Poljanica Samoborska", 0, 420, 75 },
                    { 13, null, 3.0m, "C:\\GPX\\ruta_ostrc.gpx", 2022, 19, "Oštrc – vrh", "Atraktivan grebenski put s pogledima.", "Japetić dom – Oštrc", "Grebenski prijelaz od doma Žitnica kod Japetića do vrha Oštrc preko kapele Sv. Ane.", "SG-03", "Planinarski dom Žitnica", 1, 280, 60 },
                    { 14, null, 8.5m, "C:\\GPX\\ruta_svetagera.gpx", 2021, 21, "Sveta Gera", "Potrebna dobra kondicija za dulji uspon.", "Budinjak – Sveta Gera", "Dugačak pristup najvišem vrhu Žumberačke gore iz Budinjaka.", "ZG-01", "Budinjak", 2, 650, 180 },
                    { 15, null, 4.5m, "C:\\GPX\\ruta_klek.gpx", 2023, 25, "Klek – vrh", "Završni dio zahtijeva osnovnu opremu i iskustvo.", "Bjelsko – Klek", "Klasičan pristup Kleku iz sela Bjelsko; strm završni dio uz pomoć sajli.", "GK-01", "Bjelsko", 2, 780, 120 },
                    { 16, null, 5.0m, "C:\\GPX\\ruta_bjelolasica.gpx", 2024, 26, "Bjelolasica – Kula", "Relativno lagodan pristup s makadama.", "Begovo Razdolje – Bjelolasica", "Pristup najvišem vrhu Gorskog kotara iz Begovog Razdolja.", "GK-05", "Begovo Razdolje", 1, 430, 90 },
                    { 17, null, 7.0m, "C:\\GPX\\ruta_risnjak.gpx", 2024, 28, "Risnjak – vrh", "Prolaz kroz NP Risnjak; plaćanje ulaznice.", "Crni Lug – Risnjak", "Klasičan pristup Risnjaku iz Crnog Luga kroz nacionalni park.", "GK-08", "Crni Lug", 1, 680, 150 },
                    { 18, null, 5.5m, "C:\\GPX\\ruta_snjeznik.gpx", 2023, 29, "Snježnik – vrh", "Može imati snijega do kasnog proljeća.", "Platak – Snježnik", "Pristup Snježniku s Platka preko planinskog doma.", "GK-10", "Platak", 1, 510, 120 },
                    { 19, null, 4.2m, "C:\\GPX\\ruta_vojak.gpx", 2024, 31, "Učka – Vojak", "Najpopularnija ruta na Učki.", "Poklon – Vojak", "Najpopularniji pristup Vojaku s prijevoja Poklon; dobro markiran.", "IS-01", "Poklon", 0, 520, 90 },
                    { 20, null, 8.0m, "C:\\GPX\\ruta_mrajinac.gpx", 2023, 33, "Mali Rajinac", "Ozbiljan krški teren; potrebna dobra oprema.", "Alan – Mali Rajinac", "Zahtjevan pristup jednom od najviših velebitskih vrhova iz doma Alan.", "SV-03", "Planinarski dom Alan", 2, 650, 180 },
                    { 21, null, 14.0m, "C:\\GPX\\ruta_vaganski.gpx", 2024, 36, "Vaganski vrh", "Cijeli dan hoda; potrebna odlična kondicija.", "Starigrad Paklenica – Vaganski vrh", "Dugi i zahtjevni uspon na najviši vrh Velebita kroz NP Paklenica.", "JV-01", "Starigrad-Paklenica", 2, 1550, 360 },
                    { 22, null, 3.5m, "C:\\GPX\\ruta_anicakuk.gpx", 2022, 38, "Anića kuk – vrh", "Završni dio tehnički zahtjevan.", "Velika Paklenica – Anića kuk", "Pristup Anića kuku iz klanca Velike Paklenice; alpinistički značajan vrh.", "JV-04", "Velika Paklenica ulaz", 2, 500, 120 },
                    { 23, null, 10.0m, "C:\\GPX\\ruta_ozeblin.gpx", 2021, 39, "Ozeblin", "Slabije markiran gornji dio; potrebna navigacija.", "Glogovac – Ozeblin", "Dugačak pristup najvišem vrhu Like iz sela Glogovac.", "LI-01", "Glogovac", 2, 900, 240 },
                    { 24, null, 5.5m, "C:\\GPX\\ruta_obzova.gpx", 2023, 41, "Obzova – vrh", "Ljeti ponijeti dovoljno vode.", "Baška – Obzova", "Pristup najvišem vrhu Krka iz Baške; otočni krški teren.", "OT-01", "Baška", 1, 500, 120 },
                    { 25, null, 4.5m, "C:\\GPX\\ruta_vidovagora.gpx", 2024, 43, "Vidova gora", "Popularna turistička ruta s izvrsnim vidikom.", "Nerežišća – Vidova gora", "Pristup najvišem otočnom vrhu iz mjesta Nerežišća; pogled na Zlatni rat.", "OT-05", "Nerežišća", 0, 480, 90 },
                    { 26, null, 9.0m, "C:\\GPX\\ruta_dinara.gpx", 2024, 46, "Dinara (Sinjal)", "Obavezna točka za srebrnu značku HPO-a. Zahtjevan pristup.", "Glavaš – Dinara (Sinjal)", "Klasičan pristup najvišem vrhu Hrvatske iz zaseoka Glavaš iznad Vrlike.", "DZ-01", "Glavaš", 2, 950, 240 },
                    { 27, null, 9.5m, "C:\\GPX\\ruta_svilaja.gpx", 2022, 47, "Svilaja – vrh", "Zahtjevan uspon po toplom vremenu.", "Muć – Svilaja", "Dugi pristup vrhu Svilaje iz Muća kroz dalmatinsko zaleđe.", "DZ-03", "Muć", 2, 1050, 210 },
                    { 28, null, 6.5m, "C:\\GPX\\ruta_mosor.gpx", 2023, 49, "Mosor – Ljubljan", "Popularna splitska planinarska ruta.", "Dugopolje – Ljubljan", "Pristup Mosoru iz Dugopolja s markiranim putom prema vrhu Ljubljan.", "DA-02", "Dugopolje", 1, 860, 150 },
                    { 29, null, 11.0m, "C:\\GPX\\ruta_svjure_biokovo.gpx", 2024, 51, "Sv. Jure", "Iznimno zahtjevna ruta; cijeli dan hoda.", "Bast – Sv. Jure Biokovo", "Najzahtjevniji pristup Biokovu iz Basta na obali; ogromna visinska razlika.", "BI-01", "Bast", 2, 1600, 300 },
                    { 30, null, 6.0m, "C:\\GPX\\ruta_vosac.gpx", 2023, 52, "Vošac – vrh", "Strm, ali dobro markiran pristup.", "Makarska – Vošac", "Popularan uspon na Biokovo iz Makarske s pogledom na rivijeru.", "BI-03", "Makarska", 2, 1300, 180 },
                    { 31, null, 6.5m, "C:\\GPX\\ruta_svilija_peljesac.gpx", 2022, 54, "Sv. Ilija Pelješac", "Zahtjevan uspon, posebno ljeti.", "Orebić – Sv. Ilija Pelješac", "Pristup najvišem pelješkom vrhu iz Orebića; pogled na Korčulu.", "DU-01", "Orebić", 2, 900, 180 },
                    { 32, null, 6.0m, "C:\\GPX\\ruta_snijeznica.gpx", 2021, 55, "Sniježnica – Ilijin vrh", "Ljeti ponijeti dovoljno vode; manje markacija.", "Pridvorje – Sniježnica", "Pristup najvišem vrhu dubrovačkog zaleđa iz Pridvorja.", "DU-02", "Pridvorje", 1, 750, 150 },
                    { 33, null, 7.0m, "C:\\GPX\\ruta_kapovac.gpx", 2022, 7, "Kapovac", "Dulji pristup kroz slavonsku šumu.", "Našice – Kapovac", "Pristup vrhu Krndije iz Našica preko šumskih putova.", "SL-01", "Našice", 1, 540, 150 },
                    { 34, null, 5.5m, "C:\\GPX\\ruta_plijes.gpx", 2023, 22, "Pliješ – vrh", "Umjeren pristup šumskim putevima.", "Budinjak – Pliješ", "Pristup Pliješu iz Budinjaka kroz Žumberačku goru.", "ZG-02", "Budinjak", 1, 500, 120 },
                    { 35, null, 3.5m, "C:\\GPX\\ruta_vranilac.gpx", 2022, 59, "Vranilac – vrh", "Završni dio zahtijeva pažnju.", "Kalnik selo – Vranilac", "Pristup Kalniku iz istoimenog sela; strm završni dio uz stijene.", "ZA-03", "Kalnik (selo)", 1, 340, 90 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 59);
        }
    }
}
