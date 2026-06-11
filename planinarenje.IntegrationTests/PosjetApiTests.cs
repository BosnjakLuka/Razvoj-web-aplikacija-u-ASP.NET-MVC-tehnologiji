using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.IntegrationTests.Helpers;
using planinarenje.Models.Dto.Posjet;

namespace planinarenje.IntegrationTests;

/// <summary>
/// Integracijski testovi za PosjetApiController (/api/posjet).
/// Svaka test klasa dobiva svoju izoliranu InMemory bazu (uniqueDbName u Factory).
/// </summary>
public class PosjetApiTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _planinarClient = null!;
    private HttpClient _adminClient = null!;

    public PosjetApiTests(CustomWebAppFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.SeedDatabaseAsync();
        _anonClient = AuthHelper.CreateAnonymousClient(_factory);
        _planinarClient = AuthHelper.CreatePlaninarClient(_factory);
        _adminClient = AuthHelper.CreateAdminClient(_factory);
    }

    public Task DisposeAsync()
    {
        _anonClient.Dispose();
        _planinarClient.Dispose();
        _adminClient.Dispose();
        return Task.CompletedTask;
    }

    // Test 1: GET /api/posjet → 200, lista nije prazna (HasData seedovi su učitani)
    [Fact]
    public async Task GetAll_Returns200_AndNonEmptyList()
    {
        var response = await _anonClient.GetAsync("/api/posjet");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var lista = await response.Content.ReadFromJsonAsync<List<PosjetDto>>();
        lista.Should().NotBeNull().And.NotBeEmpty();
    }

    // Test 2: GET /api/posjet/{id} → 200 kad postoji
    [Fact]
    public async Task GetById_Returns200_WhenExists()
    {
        var response = await _anonClient.GetAsync($"/api/posjet/{TestData.PosjetId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<PosjetDto>();
        dto.Should().NotBeNull();
        dto!.IdPosjet.Should().Be(TestData.PosjetId);
    }

    // Test 3: GET /api/posjet/{id} → 404 kad ne postoji
    [Fact]
    public async Task GetById_Returns404_WhenMissing()
    {
        var response = await _anonClient.GetAsync("/api/posjet/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Test 4: POST /api/posjet → 201, entitet u bazi
    // Planinar (AppUserId=PlaninarAppUserId) je vlasnik Knjizice 1 i Korisnika 1.
    [Fact]
    public async Task Post_Returns201_AndCreatesEntity()
    {
        var dto = new PosjetCreateDto
        {
            IdKnjizica = TestData.KnjizicaId,
            IdKontrolnaTocka = TestData.KontrolnaTockaId,
            IdRuta = TestData.RutaId,
            DatumVrijemePosjeta = new DateTime(2026, 6, 1, 10, 0, 0),
            DozivljajPosjeta = DozivljajPosjeta.Srednje,
            UneseniGUID = "NOVI-TEST-GUID",
            OpisIskustva = "Integracijski test posjet."
        };

        var response = await _planinarClient.PostAsJsonAsync("/api/posjet", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var kreiran = await response.Content.ReadFromJsonAsync<PosjetDto>();
        kreiran.Should().NotBeNull();
        kreiran!.IdKorisnik.Should().Be(TestData.PlaninarKorisnikId);
    }

    // Test 5: POST /api/posjet → 400 za nevažeće tijelo (nema obaveznih polja)
    [Fact]
    public async Task Post_Returns400_WhenModelInvalid()
    {
        var response = await _planinarClient.PostAsJsonAsync("/api/posjet", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ---- Lab5: PUT / DELETE testovi (Admin zaobilazi provjeru vlasništva) ----

    [Fact]
    public async Task Put_Returns200_AndUpdatesEntity_WhenExists()
    {
        var id = await SeedPosjetAsync();

        var dto = new PosjetUpdateDto
        {
            IdKnjizica = TestData.KnjizicaId,
            IdKontrolnaTocka = TestData.KontrolnaTockaId,
            IdRuta = TestData.RutaId,
            DatumVrijemePosjeta = new DateTime(2026, 2, 1, 10, 0, 0),
            DozivljajPosjeta = DozivljajPosjeta.Lagano,
            UneseniGUID = "AZURIRANO-POSJET-GUID",
            OpisIskustva = "Ažurirani opis."
        };

        var response = await _adminClient.PutAsJsonAsync($"/api/posjet/{id}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var azuriran = await response.Content.ReadFromJsonAsync<PosjetDto>();
        azuriran!.IdPosjet.Should().Be(id);
        azuriran.OpisIskustva.Should().Be(dto.OpisIskustva);
    }

    [Fact]
    public async Task Put_Returns404_WhenMissing()
    {
        var dto = new PosjetUpdateDto
        {
            IdKnjizica = TestData.KnjizicaId,
            IdKontrolnaTocka = TestData.KontrolnaTockaId,
            IdRuta = TestData.RutaId,
            DatumVrijemePosjeta = new DateTime(2026, 2, 1, 10, 0, 0),
            DozivljajPosjeta = DozivljajPosjeta.Lagano,
            UneseniGUID = "NE-POSTOJI-GUID"
        };

        var response = await _adminClient.PutAsJsonAsync("/api/posjet/99999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Returns204_WhenExists()
    {
        var id = await SeedPosjetAsync();

        var response = await _adminClient.DeleteAsync($"/api/posjet/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_Returns404_WhenMissing()
    {
        var response = await _adminClient.DeleteAsync("/api/posjet/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Seeda jedan throwaway posjet (auto-generirani Id) vezan uz seedirane korisnika/knjižicu/KT/rutu.
    private async Task<int> SeedPosjetAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new Posjet
        {
            IdKorisnik = TestData.PlaninarKorisnikId,
            IdKnjizica = TestData.KnjizicaId,
            IdKontrolnaTocka = TestData.KontrolnaTockaId,
            IdRuta = TestData.RutaId,
            DatumVrijemePosjeta = new DateTime(2026, 1, 1, 9, 0, 0),
            DozivljajPosjeta = DozivljajPosjeta.Srednje,
            UneseniGUID = "SEED-POSJET-GUID",
            DatumKreiranjaZapisa = DateTime.UtcNow
        };
        db.Posjeti.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdPosjet;
    }

    // ---- Lab5: Autorizacijski testovi (401 / 403) ----

    [Fact]
    public async Task Post_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.PostAsJsonAsync("/api/posjet", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Strani planinar (autentificiran, ali nije vlasnik knjižice/posjeta) → 403.
    [Fact]
    public async Task Post_Returns403_WhenNotOwner()
    {
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);

        var dto = new PosjetCreateDto
        {
            IdKnjizica = TestData.KnjizicaId,
            IdKontrolnaTocka = TestData.KontrolnaTockaId,
            IdRuta = TestData.RutaId,
            DatumVrijemePosjeta = new DateTime(2026, 6, 1, 10, 0, 0),
            DozivljajPosjeta = DozivljajPosjeta.Srednje,
            UneseniGUID = "FOREIGN-TEST-GUID"
        };

        var response = await foreignClient.PostAsJsonAsync("/api/posjet", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
