using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace planinarenje.Migrations
{
    /// <inheritdoc />
    public partial class DodanaObavijest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DuljinaKm",
                table: "Rute",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.CreateTable(
                name: "Obavijesti",
                columns: table => new
                {
                    IdObavijest = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Naslov = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sadrzaj = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumObjave = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    JeAktivna = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdKorisnik = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obavijesti", x => x.IdObavijest);
                    table.ForeignKey(
                        name: "FK_Obavijesti_Korisnici_IdKorisnik",
                        column: x => x.IdKorisnik,
                        principalTable: "Korisnici",
                        principalColumn: "IdKorisnik",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Obavijesti",
                columns: new[] { "IdObavijest", "DatumObjave", "IdKorisnik", "JeAktivna", "Naslov", "Sadrzaj" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), 1, true, "Dobrodošli u planinarsku aplikaciju", "Aplikacija je pokrenuta i spremna za korištenje." },
                    { 2, new DateTime(2026, 4, 15, 14, 30, 0, 0, DateTimeKind.Unspecified), 1, true, "Nova ruta dodana: Zavižan", "Dodana je kružna tura od doma Zavižan preko Balinovca." },
                    { 3, new DateTime(2026, 4, 28, 9, 0, 0, 0, DateTimeKind.Unspecified), 2, false, "Održavanje sustava", "Planirano održavanje 30. travnja od 22:00 do 23:00." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Obavijesti_IdKorisnik",
                table: "Obavijesti",
                column: "IdKorisnik");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Obavijesti");

            migrationBuilder.AlterColumn<decimal>(
                name: "DuljinaKm",
                table: "Rute",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);
        }
    }
}
