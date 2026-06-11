using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
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
}
