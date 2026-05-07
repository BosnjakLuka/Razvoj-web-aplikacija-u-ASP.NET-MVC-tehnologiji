using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace planinarenje.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Korisnici",
                columns: table => new
                {
                    IdKorisnik = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Ime = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prezime = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    KorisnickoIme = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumRodenja = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DatumRegistracije = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    BrojMobitela = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProfilnaSlika = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StatusAktivan = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnici", x => x.IdKorisnik);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Medalje",
                columns: table => new
                {
                    IdMedalja = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Naziv = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Opis = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MinimalanBrojKontrolnihTocaka = table.Column<int>(type: "int", nullable: false),
                    MinimalanBrojPodrucja = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medalje", x => x.IdMedalja);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlaninarskeUdruge",
                columns: table => new
                {
                    IdPlaninarskaUdruga = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OIB = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Naziv = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BrojTelefona = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Adresa = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PostanskiBroj = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Grad = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Zupanija = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BrojClanova = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaninarskeUdruge", x => x.IdPlaninarskaUdruga);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Podrucja",
                columns: table => new
                {
                    IdPodrucje = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Naziv = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Opis = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Regija = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MinimalanBrojKTZaObilazak = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Podrucja", x => x.IdPodrucje);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Knjizice",
                columns: table => new
                {
                    IdKnjizica = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdKorisnik = table.Column<int>(type: "int", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Napomena = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StatusAktivna = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Knjizice", x => x.IdKnjizica);
                    table.ForeignKey(
                        name: "FK_Knjizice_Korisnici_IdKorisnik",
                        column: x => x.IdKorisnik,
                        principalTable: "Korisnici",
                        principalColumn: "IdKorisnik",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "KorisnikMedalje",
                columns: table => new
                {
                    IdKorisnikMedalja = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdKorisnik = table.Column<int>(type: "int", nullable: false),
                    IdMedalja = table.Column<int>(type: "int", nullable: false),
                    DatumDodjele = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Napomena = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KorisnikMedalje", x => x.IdKorisnikMedalja);
                    table.ForeignKey(
                        name: "FK_KorisnikMedalje_Korisnici_IdKorisnik",
                        column: x => x.IdKorisnik,
                        principalTable: "Korisnici",
                        principalColumn: "IdKorisnik",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KorisnikMedalje_Medalje_IdMedalja",
                        column: x => x.IdMedalja,
                        principalTable: "Medalje",
                        principalColumn: "IdMedalja",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "KontrolneTocke",
                columns: table => new
                {
                    IdKontrolnaTocka = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GUIDOznaka = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdPodrucje = table.Column<int>(type: "int", nullable: false),
                    Naziv = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipKontrolneTocke = table.Column<int>(type: "int", nullable: false),
                    NadmorskaVisina = table.Column<int>(type: "int", nullable: true),
                    Opis = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Koordinate = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OpisZiga = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KontrolneTocke", x => x.IdKontrolnaTocka);
                    table.ForeignKey(
                        name: "FK_KontrolneTocke_Podrucja_IdPodrucje",
                        column: x => x.IdPodrucje,
                        principalTable: "Podrucja",
                        principalColumn: "IdPodrucje",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlaninarskiObjekti",
                columns: table => new
                {
                    IdPlaninarskiObjekt = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdPodrucje = table.Column<int>(type: "int", nullable: false),
                    IdPlaninarskaUdruga = table.Column<int>(type: "int", nullable: false),
                    Naziv = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipObjekta = table.Column<int>(type: "int", nullable: false),
                    NadmorskaVisina = table.Column<int>(type: "int", nullable: true),
                    Kapacitet = table.Column<int>(type: "int", nullable: true),
                    Opis = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImeOdgovorneOsobe = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefon = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Adresa = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImaNocenje = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ImaHranu = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RadnoVrijemeOpis = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaninarskiObjekti", x => x.IdPlaninarskiObjekt);
                    table.ForeignKey(
                        name: "FK_PlaninarskiObjekti_PlaninarskeUdruge_IdPlaninarskaUdruga",
                        column: x => x.IdPlaninarskaUdruga,
                        principalTable: "PlaninarskeUdruge",
                        principalColumn: "IdPlaninarskaUdruga",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaninarskiObjekti_Podrucja_IdPodrucje",
                        column: x => x.IdPodrucje,
                        principalTable: "Podrucja",
                        principalColumn: "IdPodrucje",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Rute",
                columns: table => new
                {
                    IdRuta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdKontrolnaTocka = table.Column<int>(type: "int", nullable: false),
                    Naziv = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Pocetak = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Kraj = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VrijemeHodaMin = table.Column<int>(type: "int", nullable: false),
                    DuljinaKm = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    VisinskaRazlikaM = table.Column<int>(type: "int", nullable: true),
                    Opis = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OznakaNaTerenu = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GodinaObnove = table.Column<int>(type: "int", nullable: true),
                    Napomena = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TezinaRute = table.Column<int>(type: "int", nullable: false),
                    GPXPath = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rute", x => x.IdRuta);
                    table.ForeignKey(
                        name: "FK_Rute_KontrolneTocke_IdKontrolnaTocka",
                        column: x => x.IdKontrolnaTocka,
                        principalTable: "KontrolneTocke",
                        principalColumn: "IdKontrolnaTocka",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Posjeti",
                columns: table => new
                {
                    IdPosjet = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdKorisnik = table.Column<int>(type: "int", nullable: false),
                    IdKnjizica = table.Column<int>(type: "int", nullable: false),
                    IdKontrolnaTocka = table.Column<int>(type: "int", nullable: false),
                    IdRuta = table.Column<int>(type: "int", nullable: false),
                    DatumVrijemePosjeta = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    VrijemeUsponaMin = table.Column<int>(type: "int", nullable: true),
                    DozivljajPosjeta = table.Column<int>(type: "int", nullable: false),
                    OpisIskustva = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UneseniGUID = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JeLiPotvrdenPosjet = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DatumKreiranjaZapisa = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posjeti", x => x.IdPosjet);
                    table.ForeignKey(
                        name: "FK_Posjeti_Knjizice_IdKnjizica",
                        column: x => x.IdKnjizica,
                        principalTable: "Knjizice",
                        principalColumn: "IdKnjizica",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posjeti_KontrolneTocke_IdKontrolnaTocka",
                        column: x => x.IdKontrolnaTocka,
                        principalTable: "KontrolneTocke",
                        principalColumn: "IdKontrolnaTocka",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posjeti_Korisnici_IdKorisnik",
                        column: x => x.IdKorisnik,
                        principalTable: "Korisnici",
                        principalColumn: "IdKorisnik",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posjeti_Rute_IdRuta",
                        column: x => x.IdRuta,
                        principalTable: "Rute",
                        principalColumn: "IdRuta",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Fotografije",
                columns: table => new
                {
                    IdFotografija = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdPosjet = table.Column<int>(type: "int", nullable: false),
                    NazivDatoteke = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PutanjaDatoteke = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumUploada = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TipSlike = table.Column<int>(type: "int", nullable: false),
                    Opis = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fotografije", x => x.IdFotografija);
                    table.ForeignKey(
                        name: "FK_Fotografije_Posjeti_IdPosjet",
                        column: x => x.IdPosjet,
                        principalTable: "Posjeti",
                        principalColumn: "IdPosjet",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Korisnici",
                columns: new[] { "IdKorisnik", "BrojMobitela", "DatumRegistracije", "DatumRodenja", "Email", "Ime", "KorisnickoIme", "PasswordHash", "Prezime", "ProfilnaSlika", "StatusAktivan" },
                values: new object[,]
                {
                    { 1, "0979545897", new DateTime(2026, 4, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2004, 6, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "luka.bosnjak92@gmail.com", "Luka", "Boss", "123456789", "Bošnjak", "/Slike/Profil/Boss.jpeg", true },
                    { 2, null, new DateTime(2026, 4, 1, 9, 15, 0, 0, DateTimeKind.Unspecified), new DateTime(2005, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "test123@gmail.com", "Test", "Test", "123456789", "Test", "/Slike/Profil/test.jpg", true }
                });

            migrationBuilder.InsertData(
                table: "Medalje",
                columns: new[] { "IdMedalja", "MinimalanBrojKontrolnihTocaka", "MinimalanBrojPodrucja", "Naziv", "Opis" },
                values: new object[,]
                {
                    { 1, 1, 1, "Početnik", "Osnovna medalja za prvi ispravno evidentirani obilazak područja." },
                    { 2, 25, 10, "Brončana značka", "Potrebno je obići zadani broj KT-a iz 10 područja i ukupno 25 KT-a." },
                    { 3, 50, 15, "Srebrna značka", "Potrebno je obići zadani broj KT-a iz 15 područja i ukupno 50 KT-a, uz obaveznu Dinaru (Sinjal)." },
                    { 4, 75, 20, "Zlatna značka", "Potrebno je obići zadani broj KT-a iz svih 20 područja i ukupno 75 KT-a." },
                    { 5, 100, 20, "Posebno priznanje", "Potrebno je obići 100 KT-a uz ispunjene uvjete za zlatnu značku." },
                    { 6, 125, 20, "Visoko priznanje", "Potrebno je obići 125 KT-a uz ispunjene uvjete za posebno priznanje." },
                    { 7, 155, 20, "Najviše priznanje", "Potrebno je obići 155 KT-a uz ispunjene uvjete za visoko priznanje." }
                });

            migrationBuilder.InsertData(
                table: "PlaninarskeUdruge",
                columns: new[] { "IdPlaninarskaUdruga", "Adresa", "BrojClanova", "BrojTelefona", "Email", "Grad", "Naziv", "OIB", "PostanskiBroj", "Zupanija" },
                values: new object[,]
                {
                    { 1, "p.p. 233", 350, null, "hpd.mosor@hps.hr", "Split", "HPD Mosor", "40461293872", "21000", "Splitsko-dalmatinska" },
                    { 2, "Dubravica 27a", 180, null, "hpd.gora@hps.hr", "Zagreb", "HPD Gora", "48938096579", "10000", "Grad Zagreb" },
                    { 3, "Mala vrata 20", 120, null, "pd.zavizan@hps.hr", "Senj", "PD Zavižan", "95873199484", "53270", "Ličko-senjska" },
                    { 4, "Majke Margarite 6", 220, null, "pd.paklenica@hps.hr", "Zadar", "PD Paklenica", "92966614510", "23000", "Zadarska" },
                    { 5, "Andrije Hebranga 26", 95, "0991234567", "info@pddr-maks-plotnikov.hr", "Samobor", "PD Dr. Maks Plotnikov", "12345678901", "10430", "Zagrebačka" }
                });

            migrationBuilder.InsertData(
                table: "Podrucja",
                columns: new[] { "IdPodrucje", "MinimalanBrojKTZaObilazak", "Naziv", "Opis", "Regija" },
                values: new object[,]
                {
                    { 1, 2, "Slavonija", "Nizinsko i brežuljkasto područje istočne Hrvatske s Papukom, Psunjem, Krndijom i drugim slavonskim gorjima.", "Istočna Hrvatska" },
                    { 2, 1, "Moslavačka gora i Bilogora", "Niža šumovita gorja s kraćim planinarskim usponima i manjim brojem kontrolnih točaka.", "Središnja Hrvatska" },
                    { 3, 3, "Hrvatsko zagorje i Međimurje", "Brežuljkasto područje s vidikovcima, utvrdama i poznatim vrhovima kao što su Ivanščica i Ravna gora.", "Sjeverna Hrvatska" },
                    { 4, 2, "Medvednica", "Planina iznad Zagreba s gusto razvijenom mrežom putova, domova i kontrolnih točaka.", "Središnja Hrvatska" },
                    { 5, 2, "Samoborsko gorje", "Popularno planinarsko područje zapadno od Zagreba, poznato po Okiću, Japetiću i Oštrcu.", "Središnja Hrvatska" },
                    { 6, 1, "Žumberačka gora", "Planinsko i granično područje s višim vrhovima i rjeđe naseljenim grebenima.", "Središnja Hrvatska" },
                    { 7, 1, "Karlovačko pokuplje, Kordun i Banovina", "Područje nižih gora i šumovitih uzvisina južno od Karlovca i prema Banovini.", "Središnja Hrvatska" },
                    { 8, 4, "Gorski kotar - južni dio", "Dio Gorskog kotara s višim vrhovima, stjenovitim skupinama i zahtjevnijim usponima.", "Gorska Hrvatska" },
                    { 9, 3, "Gorski kotar - sjeverni dio", "Šumovito i planinsko područje s vrhovima poput Risnjaka, Snježnika i Skradskog vrha.", "Gorska Hrvatska" },
                    { 10, 2, "Istra", "Područje Učke i Ćićarije s istaknutim obalnim i planinskim vidikovcima.", "Zapadna Hrvatska" },
                    { 11, 3, "Sjeverni Velebit", "Visokoplaninsko područje s izrazito atraktivnim velebitskim vrhovima i oštrim kršem.", "Primorsko-gorska Hrvatska" },
                    { 12, 2, "Srednji Velebit", "Središnji dio Velebita sa srednje zahtjevnim i zahtjevnim vrhovima i planinarskim kućama.", "Lika i Primorje" },
                    { 13, 3, "Južni Velebit", "Najviši i alpinistički najdojmljiviji dio Velebita s Vaganskim vrhom i Svetim brdom.", "Lika i Dalmacija" },
                    { 14, 1, "Lika", "Prostrano područje ličkih planina i osamljenih vrhova izvan glavnog velebitskog lanca.", "Lika" },
                    { 15, 1, "Jadranski otoci - sjeverni dio", "Sjeverni jadranski otoci s nižim, ali vrlo atraktivnim otočnim vrhovima.", "Jadranska Hrvatska" },
                    { 16, 2, "Jadranski otoci - južni dio", "Južni jadranski otoci s većim brojem otočnih vrhova i raznolikim podlogama.", "Jadranska Hrvatska" },
                    { 17, 2, "Dalmatinska zagora", "Područje Dinare, Promine, Svilaje i drugih planina dalmatinskog zaleđa.", "Dalmatinsko zaleđe" },
                    { 18, 2, "Dalmacija", "Priobalno i zaleđno područje srednje Dalmacije s planinama uz obalu i u zaleđu.", "Dalmacija" },
                    { 19, 3, "Biokovo i Zagora", "Krševito visokoplaninsko područje Biokova i zaleđa s vrlo izraženim visinskim razlikama.", "Južna Dalmacija" },
                    { 20, 1, "Dubrovačko područje", "Južnohrvatsko područje s manjim brojem, ali vrlo atraktivnih kontrolnih točaka.", "Krajnji jug Hrvatske" }
                });

            migrationBuilder.InsertData(
                table: "Knjizice",
                columns: new[] { "IdKnjizica", "DatumKreiranja", "IdKorisnik", "Napomena", "StatusAktivna" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 1, 9, 5, 0, 0, DateTimeKind.Unspecified), 1, "Glavna digitalna knjižica korisnika Luka Bošnjak.", true },
                    { 2, new DateTime(2026, 4, 1, 9, 20, 0, 0, DateTimeKind.Unspecified), 2, "Testna digitalna knjižica za provjeru funkcionalnosti aplikacije.", true }
                });

            migrationBuilder.InsertData(
                table: "KontrolneTocke",
                columns: new[] { "IdKontrolnaTocka", "GUIDOznaka", "IdPodrucje", "Koordinate", "NadmorskaVisina", "Naziv", "Opis", "OpisZiga", "TipKontrolneTocke" },
                values: new object[,]
                {
                    { 1, "KT-HPO-2-1-VIS", 2, "N/A", 437, "Moslavačka gora – vrh Vis", "Najviši vrh Moslavačke gore i dobra početna kontrolna točka za početničke obilaznike.", "Metalni žig na vršnoj oznaci.", 0 },
                    { 2, "KT-HPO-4-4-SLJEME", 4, "N 45° 53' 57.4'' E 15° 56' 50.6''", 1033, "Sljeme – vrh", "Najviši vrh Medvednice; vrh je lako dostupan i planinarima i izletnicima.", "Metalni žig vrha nalazi se na promidžbenom panou kod televizijskog tornja.", 0 },
                    { 3, "KT-HPO-5-1-OKIC", 5, "N 45° 44' 55.4'' E 15° 42' 24.0''", 499, "Okić – vrh", "Stari grad i vršna gradina s vidikom prema Zagrebu i Medvednici.", "Metalni žig vrha ugrađen je na zid u najvišem dijelu gradine.", 0 },
                    { 4, "KT-HPO-5-4-JAPETIC", 5, "N 45° 44' 56.3'' E 15° 36' 32.8''", 879, "Japetić – vrh", "Najviši vrh Samoborskoga gorja; poznat po piramidi i domu Žitnica.", "Metalni žig ugrađen je na konstrukciju piramide.", 0 },
                    { 5, "KT-HPO-11-2-ZAVIZAN", 11, "N/A", 1676, "Veliki Zavižan – vrh", "Jedan od najpoznatijih vrhova Sjevernog Velebita s vrlo atraktivnim pogledima.", "Žig kontrolne točke nalazi se na vrhu ili u blizini planinarskog doma Zavižan.", 0 }
                });

            migrationBuilder.InsertData(
                table: "KorisnikMedalje",
                columns: new[] { "IdKorisnikMedalja", "DatumDodjele", "IdKorisnik", "IdMedalja", "Napomena" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 19, 12, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, "Korisnik je zadovoljio uvjet početničke medalje jer ima evidentiran obilazak područja 2 (Moslavačka gora i Bilogora), gdje je prag 1 KT." },
                    { 2, new DateTime(2026, 4, 8, 13, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, "Korisnik je zadovoljio uvjet početničke medalje jer ima evidentiran obilazak područja 2 (Moslavačka gora i Bilogora), gdje je prag 1 KT." }
                });

            migrationBuilder.InsertData(
                table: "PlaninarskiObjekti",
                columns: new[] { "IdPlaninarskiObjekt", "Adresa", "Email", "IdPlaninarskaUdruga", "IdPodrucje", "ImaHranu", "ImaNocenje", "ImeOdgovorneOsobe", "Kapacitet", "NadmorskaVisina", "Naziv", "Opis", "RadnoVrijemeOpis", "Telefon", "TipObjekta" },
                values: new object[,]
                {
                    { 1, "Okić, Samobor", "aplantosar@gmail.com", 5, 5, true, true, "Stjepan Jandrečić", 14, 411, "Planinarski dom Dr. Maks Plotnikov", "Dom podno ruševina Okić-grada; polazišna je točka za Okić i okolne putove.", "Otvoren vikendom i blagdanima.", "0918909624", 0 },
                    { 2, "Samoborsko gorje", null, 2, 5, true, true, "Dežurni domar", 25, 691, "Planinarski dom Željezničar", "Popularan planinarski dom u Samoborskom i Žumberačkom gorju.", "Prema rasporedu dežurstva i vikendom.", null, 0 },
                    { 3, "Velebit, Senj", "pd.zavizan@hps.hr", 3, 11, false, true, "Dežurni član društva", 12, 328, "Planinarska kuća Sijaset", "Niži planinarski objekt na Velebitu, pogodan kao polazište za ture.", "Povremeno otvorena ili po dogovoru.", null, 1 },
                    { 4, "Velika Paklenica", "pd.paklenica@hps.hr", 4, 13, true, true, "Irena Šaran", 44, 480, "Planinarski dom Paklenica", "Dom na početku klanca Velike Paklenice s hranom, pićem i noćenjem.", "Otvoren stalno.", "0977557654", 0 },
                    { 5, "Mosor, Split", "hpd.mosor@hps.hr", 1, 18, false, true, "Dežurna osoba društva", 20, 872, "Planinarska kuća Lugarnica", "Planinarska kuća na Mosoru pogodna za kraće i srednje duge uspone.", "Otvorenost prema obavijesti društva.", null, 1 }
                });

            migrationBuilder.InsertData(
                table: "Rute",
                columns: new[] { "IdRuta", "DuljinaKm", "GPXPath", "GodinaObnove", "IdKontrolnaTocka", "Kraj", "Napomena", "Naziv", "Opis", "OznakaNaTerenu", "Pocetak", "TezinaRute", "VisinskaRazlikaM", "VrijemeHodaMin" },
                values: new object[,]
                {
                    { 1, 4.5m, "C:\\GPX\\ruta_vis.gpx", 2023, 1, "Vrh Vis", "Pogodna za početnike.", "Kutina – Humka – Vis", "Primjer kraće rute do najviše točke Moslavačke gore.", "MG-01", "Kutina / Humka", 0, 260, 90 },
                    { 2, 8.2m, "C:\\GPX\\ruta_sljeme.gpx", 2022, 2, "Sljeme", "Jedna od najčešće korištenih ruta na Medvednici.", "Gračani – Puntijarka – Sljeme", "Popularan uspon preko Puntijarke prema vrhu Medvednice.", "M-04", "Gračani", 1, 780, 150 },
                    { 3, 1.8m, "C:\\GPX\\ruta_okic.gpx", 2021, 3, "Okić – vrh", "Strmiji završni dio prema gradini.", "Klake – pl. dom pod Okićem – Okić-grad", "Najkraći klasični prilaz vrhu Okić preko doma pod Okićem.", "SG-01", "Klake", 1, 210, 40 },
                    { 4, 5.4m, "C:\\GPX\\ruta_japetic.gpx", 2020, 4, "Japetić – vrh", "Ruta je pregledna i često korištena.", "Šoićeva kuća – Japetić", "Klasičan prilaz preko livada i Katina krča prema vrhu Japetić.", "SG-04", "Šoićeva kuća", 1, 430, 90 },
                    { 5, 6.7m, "C:\\GPX\\ruta_zavizan.gpx", 2024, 5, "Veliki Zavižan", "U nepovoljnim uvjetima potreban dodatni oprez.", "Dom Zavižan – Veliki Zavižan – dom Zavižan", "Kružna tura s polaskom od doma Zavižan preko Balinovca do Velikog Zavižana.", "SV-02", "Planinarski dom Zavižan", 2, 320, 150 }
                });

            migrationBuilder.InsertData(
                table: "Posjeti",
                columns: new[] { "IdPosjet", "DatumKreiranjaZapisa", "DatumVrijemePosjeta", "DozivljajPosjeta", "IdKnjizica", "IdKontrolnaTocka", "IdKorisnik", "IdRuta", "JeLiPotvrdenPosjet", "OpisIskustva", "UneseniGUID", "VrijemeUsponaMin" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 5, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 5, 10, 15, 0, 0, DateTimeKind.Unspecified), 0, 1, 1, 1, 1, true, "Prvi evidentirani uspon u aplikaciji. Lagana i ugodna ruta po suhom vremenu.", "KT-HPO-2-1-VIS", 92 },
                    { 2, new DateTime(2026, 4, 12, 10, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 12, 8, 40, 0, 0, DateTimeKind.Unspecified), 5, 1, 3, 1, 3, true, "Kratak, ali strm završni dio prema gradini Okić.", "KT-HPO-5-1-OKIC", 43 },
                    { 3, new DateTime(2026, 4, 19, 11, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 19, 9, 10, 0, 0, DateTimeKind.Unspecified), 2, 1, 4, 1, 4, true, "Ugodna tura s dobrim vremenom i lijepim pogledima s piramide.", "KT-HPO-5-4-JAPETIC", 96 },
                    { 4, new DateTime(2026, 4, 8, 12, 45, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 8, 11, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, 1, 2, 1, true, "Testni korisnik uspješno evidentirao svoj prvi posjet i time zadovoljio uvjet za početničku medalju.", "KT-HPO-2-1-VIS", 95 },
                    { 5, new DateTime(2026, 4, 26, 10, 40, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 26, 7, 50, 0, 0, DateTimeKind.Unspecified), 7, 2, 2, 2, 2, true, "Duži uspon do Sljemena preko Puntijarke, ali bez tehnički teških dijelova.", "KT-HPO-4-4-SLJEME", 155 }
                });

            migrationBuilder.InsertData(
                table: "Fotografije",
                columns: new[] { "IdFotografija", "DatumUploada", "IdPosjet", "NazivDatoteke", "Opis", "PutanjaDatoteke", "TipSlike" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 5, 12, 5, 0, 0, DateTimeKind.Unspecified), 1, "vis_luka_selfie.jpg", "Selfie korisnika Luke na vrhu Vis.", "/slike/posjeti/vis_luka_selfie.jpg", 0 },
                    { 2, new DateTime(2026, 4, 12, 10, 5, 0, 0, DateTimeKind.Unspecified), 2, "okic_luka_selfie.jpg", "Fotografija Luke kod oznake vrha Okić.", "/slike/posjeti/okic_luka_selfie.jpg", 0 },
                    { 3, new DateTime(2026, 4, 19, 11, 15, 0, 0, DateTimeKind.Unspecified), 3, "japetic_luka_selfie.jpg", "Selfie na vrhu Japetić uz piramidu.", "/slike/posjeti/japetic_luka_selfie.jpg", 0 },
                    { 4, new DateTime(2026, 4, 8, 12, 50, 0, 0, DateTimeKind.Unspecified), 4, "vis_test_selfie.jpg", "Testni korisnik na vrhu Vis.", "/slike/posjeti/vis_test_selfie.jpg", 0 },
                    { 5, new DateTime(2026, 4, 26, 10, 45, 0, 0, DateTimeKind.Unspecified), 5, "sljeme_test_selfie.jpg", "Testni korisnik na vrhu Sljeme kod oznake.", "/slike/posjeti/sljeme_test_selfie.jpg", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fotografije_IdPosjet",
                table: "Fotografije",
                column: "IdPosjet");

            migrationBuilder.CreateIndex(
                name: "IX_Knjizice_IdKorisnik",
                table: "Knjizice",
                column: "IdKorisnik",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KontrolneTocke_GUIDOznaka",
                table: "KontrolneTocke",
                column: "GUIDOznaka",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KontrolneTocke_IdPodrucje",
                table: "KontrolneTocke",
                column: "IdPodrucje");

            migrationBuilder.CreateIndex(
                name: "IX_Korisnici_Email",
                table: "Korisnici",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Korisnici_KorisnickoIme",
                table: "Korisnici",
                column: "KorisnickoIme",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KorisnikMedalje_IdKorisnik",
                table: "KorisnikMedalje",
                column: "IdKorisnik");

            migrationBuilder.CreateIndex(
                name: "IX_KorisnikMedalje_IdMedalja",
                table: "KorisnikMedalje",
                column: "IdMedalja");

            migrationBuilder.CreateIndex(
                name: "IX_PlaninarskeUdruge_OIB",
                table: "PlaninarskeUdruge",
                column: "OIB",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaninarskiObjekti_IdPlaninarskaUdruga",
                table: "PlaninarskiObjekti",
                column: "IdPlaninarskaUdruga");

            migrationBuilder.CreateIndex(
                name: "IX_PlaninarskiObjekti_IdPodrucje",
                table: "PlaninarskiObjekti",
                column: "IdPodrucje");

            migrationBuilder.CreateIndex(
                name: "IX_Posjeti_IdKnjizica",
                table: "Posjeti",
                column: "IdKnjizica");

            migrationBuilder.CreateIndex(
                name: "IX_Posjeti_IdKontrolnaTocka",
                table: "Posjeti",
                column: "IdKontrolnaTocka");

            migrationBuilder.CreateIndex(
                name: "IX_Posjeti_IdKorisnik",
                table: "Posjeti",
                column: "IdKorisnik");

            migrationBuilder.CreateIndex(
                name: "IX_Posjeti_IdRuta",
                table: "Posjeti",
                column: "IdRuta");

            migrationBuilder.CreateIndex(
                name: "IX_Rute_IdKontrolnaTocka",
                table: "Rute",
                column: "IdKontrolnaTocka");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fotografije");

            migrationBuilder.DropTable(
                name: "KorisnikMedalje");

            migrationBuilder.DropTable(
                name: "PlaninarskiObjekti");

            migrationBuilder.DropTable(
                name: "Posjeti");

            migrationBuilder.DropTable(
                name: "Medalje");

            migrationBuilder.DropTable(
                name: "PlaninarskeUdruge");

            migrationBuilder.DropTable(
                name: "Knjizice");

            migrationBuilder.DropTable(
                name: "Rute");

            migrationBuilder.DropTable(
                name: "Korisnici");

            migrationBuilder.DropTable(
                name: "KontrolneTocke");

            migrationBuilder.DropTable(
                name: "Podrucja");
        }
    }
}
