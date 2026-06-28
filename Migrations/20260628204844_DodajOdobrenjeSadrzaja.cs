using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace planinarenje.Migrations
{
    /// <inheritdoc />
    public partial class DodajOdobrenjeSadrzaja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DatumPrijave",
                table: "Rute",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdKreator",
                table: "Rute",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "JeOdobreno",
                table: "Rute",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DatumPrijave",
                table: "Podrucja",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdKreator",
                table: "Podrucja",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "JeOdobreno",
                table: "Podrucja",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DatumPrijave",
                table: "PlaninarskiObjekti",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdKreator",
                table: "PlaninarskiObjekti",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "JeOdobreno",
                table: "PlaninarskiObjekti",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DatumPrijave",
                table: "PlaninarskeUdruge",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdKreator",
                table: "PlaninarskeUdruge",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "JeOdobreno",
                table: "PlaninarskeUdruge",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DatumPrijave",
                table: "KontrolneTocke",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdKreator",
                table: "KontrolneTocke",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "JeOdobreno",
                table: "KontrolneTocke",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 1,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 2,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 3,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 4,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 5,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 7,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 8,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 9,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 10,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 11,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 12,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 13,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 14,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 15,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 16,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 17,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 18,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 19,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 20,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 21,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 22,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 23,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 24,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 25,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 26,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 27,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 28,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 29,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 30,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 31,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 32,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 33,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 34,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 35,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 36,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 37,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 38,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 39,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 40,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 41,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 42,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 43,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 44,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 45,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 46,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 47,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 48,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 49,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 50,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 51,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 52,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 53,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 54,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 55,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 56,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 57,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 58,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 59,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "KontrolneTocke",
                keyColumn: "IdKontrolnaTocka",
                keyValue: 60,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "PlaninarskeUdruge",
                keyColumn: "IdPlaninarskaUdruga",
                keyValue: 1,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "PlaninarskeUdruge",
                keyColumn: "IdPlaninarskaUdruga",
                keyValue: 2,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "PlaninarskeUdruge",
                keyColumn: "IdPlaninarskaUdruga",
                keyValue: 3,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "PlaninarskeUdruge",
                keyColumn: "IdPlaninarskaUdruga",
                keyValue: 4,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "PlaninarskeUdruge",
                keyColumn: "IdPlaninarskaUdruga",
                keyValue: 5,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "PlaninarskiObjekti",
                keyColumn: "IdPlaninarskiObjekt",
                keyValue: 1,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "PlaninarskiObjekti",
                keyColumn: "IdPlaninarskiObjekt",
                keyValue: 2,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "PlaninarskiObjekti",
                keyColumn: "IdPlaninarskiObjekt",
                keyValue: 3,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "PlaninarskiObjekti",
                keyColumn: "IdPlaninarskiObjekt",
                keyValue: 4,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "PlaninarskiObjekti",
                keyColumn: "IdPlaninarskiObjekt",
                keyValue: 5,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 1,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 2,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 3,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 4,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 5,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 6,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 7,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 8,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 9,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 10,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 11,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 12,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 13,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 14,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 15,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 16,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 17,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 18,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 19,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Podrucja",
                keyColumn: "IdPodrucje",
                keyValue: 20,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 1,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 2,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 3,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 4,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 5,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 6,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 7,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 8,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 9,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 10,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 11,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 12,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 13,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 14,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 15,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 16,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 17,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 18,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 19,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 20,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 21,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 22,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 23,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 24,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 25,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 26,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 27,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 28,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 29,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 30,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 31,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 32,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 33,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 34,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.UpdateData(
                table: "Rute",
                keyColumn: "IdRuta",
                keyValue: 35,
                columns: new[] { "DatumPrijave", "IdKreator", "JeOdobreno" },
                values: new object[] { null, null, true });

            migrationBuilder.CreateIndex(
                name: "IX_Rute_IdKreator",
                table: "Rute",
                column: "IdKreator");

            migrationBuilder.CreateIndex(
                name: "IX_Podrucja_IdKreator",
                table: "Podrucja",
                column: "IdKreator");

            migrationBuilder.CreateIndex(
                name: "IX_PlaninarskiObjekti_IdKreator",
                table: "PlaninarskiObjekti",
                column: "IdKreator");

            migrationBuilder.CreateIndex(
                name: "IX_PlaninarskeUdruge_IdKreator",
                table: "PlaninarskeUdruge",
                column: "IdKreator");

            migrationBuilder.CreateIndex(
                name: "IX_KontrolneTocke_IdKreator",
                table: "KontrolneTocke",
                column: "IdKreator");

            migrationBuilder.AddForeignKey(
                name: "FK_KontrolneTocke_Korisnici_IdKreator",
                table: "KontrolneTocke",
                column: "IdKreator",
                principalTable: "Korisnici",
                principalColumn: "IdKorisnik");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaninarskeUdruge_Korisnici_IdKreator",
                table: "PlaninarskeUdruge",
                column: "IdKreator",
                principalTable: "Korisnici",
                principalColumn: "IdKorisnik");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaninarskiObjekti_Korisnici_IdKreator",
                table: "PlaninarskiObjekti",
                column: "IdKreator",
                principalTable: "Korisnici",
                principalColumn: "IdKorisnik");

            migrationBuilder.AddForeignKey(
                name: "FK_Podrucja_Korisnici_IdKreator",
                table: "Podrucja",
                column: "IdKreator",
                principalTable: "Korisnici",
                principalColumn: "IdKorisnik");

            migrationBuilder.AddForeignKey(
                name: "FK_Rute_Korisnici_IdKreator",
                table: "Rute",
                column: "IdKreator",
                principalTable: "Korisnici",
                principalColumn: "IdKorisnik");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KontrolneTocke_Korisnici_IdKreator",
                table: "KontrolneTocke");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaninarskeUdruge_Korisnici_IdKreator",
                table: "PlaninarskeUdruge");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaninarskiObjekti_Korisnici_IdKreator",
                table: "PlaninarskiObjekti");

            migrationBuilder.DropForeignKey(
                name: "FK_Podrucja_Korisnici_IdKreator",
                table: "Podrucja");

            migrationBuilder.DropForeignKey(
                name: "FK_Rute_Korisnici_IdKreator",
                table: "Rute");

            migrationBuilder.DropIndex(
                name: "IX_Rute_IdKreator",
                table: "Rute");

            migrationBuilder.DropIndex(
                name: "IX_Podrucja_IdKreator",
                table: "Podrucja");

            migrationBuilder.DropIndex(
                name: "IX_PlaninarskiObjekti_IdKreator",
                table: "PlaninarskiObjekti");

            migrationBuilder.DropIndex(
                name: "IX_PlaninarskeUdruge_IdKreator",
                table: "PlaninarskeUdruge");

            migrationBuilder.DropIndex(
                name: "IX_KontrolneTocke_IdKreator",
                table: "KontrolneTocke");

            migrationBuilder.DropColumn(
                name: "DatumPrijave",
                table: "Rute");

            migrationBuilder.DropColumn(
                name: "IdKreator",
                table: "Rute");

            migrationBuilder.DropColumn(
                name: "JeOdobreno",
                table: "Rute");

            migrationBuilder.DropColumn(
                name: "DatumPrijave",
                table: "Podrucja");

            migrationBuilder.DropColumn(
                name: "IdKreator",
                table: "Podrucja");

            migrationBuilder.DropColumn(
                name: "JeOdobreno",
                table: "Podrucja");

            migrationBuilder.DropColumn(
                name: "DatumPrijave",
                table: "PlaninarskiObjekti");

            migrationBuilder.DropColumn(
                name: "IdKreator",
                table: "PlaninarskiObjekti");

            migrationBuilder.DropColumn(
                name: "JeOdobreno",
                table: "PlaninarskiObjekti");

            migrationBuilder.DropColumn(
                name: "DatumPrijave",
                table: "PlaninarskeUdruge");

            migrationBuilder.DropColumn(
                name: "IdKreator",
                table: "PlaninarskeUdruge");

            migrationBuilder.DropColumn(
                name: "JeOdobreno",
                table: "PlaninarskeUdruge");

            migrationBuilder.DropColumn(
                name: "DatumPrijave",
                table: "KontrolneTocke");

            migrationBuilder.DropColumn(
                name: "IdKreator",
                table: "KontrolneTocke");

            migrationBuilder.DropColumn(
                name: "JeOdobreno",
                table: "KontrolneTocke");
        }
    }
}
