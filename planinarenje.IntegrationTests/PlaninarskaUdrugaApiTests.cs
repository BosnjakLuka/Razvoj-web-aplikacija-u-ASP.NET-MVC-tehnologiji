using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.IntegrationTests.Helpers;
using planinarenje.Models.Dto.PlaninarskaUdruga;

namespace planinarenje.IntegrationTests;

public class PlaninarskaUdrugaApiTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _adminClient = null!;

    public PlaninarskaUdrugaApiTests(CustomWebAppFactory factory) => _factory = factory;

    public async Task InitializeAsync()
    {
        await _factory.SeedDatabaseAsync();
        _anonClient = AuthHelper.CreateAnonymousClient(_factory);
        _adminClient = AuthHelper.CreateAdminClient(_factory);
    }

    public Task DisposeAsync()
    {
        _anonClient.Dispose();
        _adminClient.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetAll_Returns200_AndNonEmptyList()
    {
        var response = await _anonClient.GetAsync("/api/planinarskaudruga");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var lista = await response.Content.ReadFromJsonAsync<List<PlaninarskaUdrugaDto>>();
        lista.Should().NotBeNull().And.NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Returns200_WhenExists()
    {
        var response = await _anonClient.GetAsync($"/api/planinarskaudruga/{TestData.PlaninarskaUdrugaId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<PlaninarskaUdrugaDto>();
        dto!.IdPlaninarskaUdruga.Should().Be(TestData.PlaninarskaUdrugaId);
    }

    [Fact]
    public async Task GetById_Returns404_WhenMissing()
    {
        var response = await _anonClient.GetAsync("/api/planinarskaudruga/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_Returns201_AndCreatesEntity()
    {
        // OIB mora biti jedinstven i 11 znamenki — seedirani OIBovi su već zauzeti,
        // koristimo novi koji ne postoji u HasData seedu.
        var dto = new PlaninarskaUdrugaCreateDto
        {
            OIB = "99999999999",
            Naziv = "PD Testno integracijsko",
            Grad = "Zagreb"
        };

        var response = await _adminClient.PostAsJsonAsync("/api/planinarskaudruga", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var kreiran = await response.Content.ReadFromJsonAsync<PlaninarskaUdrugaDto>();
        kreiran!.OIB.Should().Be(dto.OIB);
    }

    [Fact]
    public async Task Post_Returns400_WhenModelInvalid()
    {
        var response = await _adminClient.PostAsJsonAsync("/api/planinarskaudruga", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ---- Lab5: PUT / DELETE testovi ----

    [Fact]
    public async Task Put_Returns200_AndUpdatesEntity_WhenExists()
    {
        var id = await SeedUdrugaAsync();

        var dto = new PlaninarskaUdrugaUpdateDto
        {
            OIB = Random.Shared.NextInt64(10000000000L, 99999999999L).ToString(),
            Naziv = "PD Ažurirano",
            Grad = "Split"
        };

        var response = await _adminClient.PutAsJsonAsync($"/api/planinarskaudruga/{id}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var azuriran = await response.Content.ReadFromJsonAsync<PlaninarskaUdrugaDto>();
        azuriran!.IdPlaninarskaUdruga.Should().Be(id);
        azuriran.Naziv.Should().Be(dto.Naziv);
    }

    [Fact]
    public async Task Put_Returns404_WhenMissing()
    {
        var dto = new PlaninarskaUdrugaUpdateDto
        {
            OIB = Random.Shared.NextInt64(10000000000L, 99999999999L).ToString(),
            Naziv = "Ne postoji"
        };

        var response = await _adminClient.PutAsJsonAsync("/api/planinarskaudruga/99999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Returns204_WhenExists()
    {
        var id = await SeedUdrugaAsync();

        var response = await _adminClient.DeleteAsync($"/api/planinarskaudruga/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_Returns404_WhenMissing()
    {
        var response = await _adminClient.DeleteAsync("/api/planinarskaudruga/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Seeda jednu throwaway udrugu (jedinstveni OIB) i vrati njezin Id.
    private async Task<int> SeedUdrugaAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new PlaninarskaUdruga
        {
            OIB = Random.Shared.NextInt64(10000000000L, 99999999999L).ToString(),
            Naziv = "PD Seed",
            Grad = "Zagreb"
        };
        db.PlaninarskeUdruge.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdPlaninarskaUdruga;
    }

    // ---- Lab5: Autorizacijski testovi (401 / 403) ----

    [Fact]
    public async Task Post_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.PostAsJsonAsync("/api/planinarskaudruga", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_Returns403_WhenNotAdmin()
    {
        using var planinarClient = AuthHelper.CreatePlaninarClient(_factory);

        var response = await planinarClient.PostAsJsonAsync("/api/planinarskaudruga", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
