using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
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
}
