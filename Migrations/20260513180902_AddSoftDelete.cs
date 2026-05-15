using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace planinarenje.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Rute",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Posjeti",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Podrucja",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "PlaninarskiObjekti",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "PlaninarskeUdruge",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Medalje",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "KorisnikMedalje",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "KontrolneTocke",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Fotografije",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Fotografije",
                keyColumn: "IdFotografija",
                keyValue: 1,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Fotografije",
                keyColumn: "IdFotografija",
                keyValue: 2,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Fotografije",
                keyColumn: "IdFotografija",
                keyValue: 3,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Fotografije",
                keyColumn: "IdFotografija",
                keyValue: 4,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Fotografije",
                keyColumn: "IdFotografija",
                keyValue: 5,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 1,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 2,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 3,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 4,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 5,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "KorisnikMedalje",
                keyColumn: "IdKorisnikMedalja",
                keyValue: 1,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "KorisnikMedalje",
                keyColumn: "IdKorisnikMedalja",
                keyValue: 2,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Medalje",
                keyColumn: "IdMedalja",
                keyValue: 1,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Medalje",
                keyColumn: "IdMedalja",
                keyValue: 2,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Medalje",
                keyColumn: "IdMedalja",
                keyValue: 3,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Medalje",
                keyColumn: "IdMedalja",
                keyValue: 4,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Medalje",
                keyColumn: "IdMedalja",
                keyValue: 5,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Medalje",
                keyColumn: "IdMedalja",
                keyValue: 6,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Medalje",
                keyColumn: "IdMedalja",
                keyValue: 7,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "PlaninarskeUdruge",
                keyColumn: "IdPlaninarskaUdruga",
                keyValue: 1,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "PlaninarskeUdruge",
                keyColumn: "IdPlaninarskaUdruga",
                keyValue: 2,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "PlaninarskeUdruge",
                keyColumn: "IdPlaninarskaUdruga",
                keyValue: 3,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "PlaninarskeUdruge",
                keyColumn: "IdPlaninarskaUdruga",
                keyValue: 4,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "PlaninarskeUdruge",
                keyColumn: "IdPlaninarskaUdruga",
                keyValue: 5,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "PlaninarskiObjekti",
                keyColumn: "IdPlaninarskiObjekt",
                keyValue: 1,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "PlaninarskiObjekti",
                keyColumn: "IdPlaninarskiObjekt",
                keyValue: 2,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "PlaninarskiObjekti",
                keyColumn: "IdPlaninarskiObjekt",
                keyValue: 3,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "PlaninarskiObjekti",
                keyColumn: "IdPlaninarskiObjekt",
                keyValue: 4,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "PlaninarskiObjekti",
                keyColumn: "IdPlaninarskiObjekt",
                keyValue: 5,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 1,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 2,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 3,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 4,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 5,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 6,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 7,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 8,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 9,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 10,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 11,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 12,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 13,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 14,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 15,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 16,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 17,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 18,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 19,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 20,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Posjeti",
                keyColumn: "IdPosjet",
                keyValue: 1,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Posjeti",
                keyColumn: "IdPosjet",
                keyValue: 2,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Posjeti",
                keyColumn: "IdPosjet",
                keyValue: 3,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Posjeti",
                keyColumn: "IdPosjet",
                keyValue: 4,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Posjeti",
                keyColumn: "IdPosjet",
                keyValue: 5,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 1,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 2,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 3,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 4,
                column: "DeletedAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 5,
                column: "DeletedAt",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Rute");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Posjeti");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Podrucja");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "PlaninarskiObjekti");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "PlaninarskeUdruge");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Medalje");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "KorisnikMedalje");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "KontrolneTocke");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Fotografije");
        }
    }
}
