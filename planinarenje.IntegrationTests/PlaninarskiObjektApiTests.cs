using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using planinarenje.Entiteti;
using planinarenje.IntegrationTests.Helpers;
using planinarenje.Models.Dto.PlaninarskiObjekt;

namespace planinarenje.IntegrationTests;

public class PlaninarskiObjektApiTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _adminClient = null!;

    public PlaninarskiObjektApiTests(CustomWebAppFactory factory) => _factory = factory;

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
        var response = await _anonClient.GetAsync("/api/planinarskiobjekt");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var lista = await response.Content.ReadFromJsonAsync<List<PlaninarskiObjektDto>>();
        lista.Should().NotBeNull().And.NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Returns200_WhenExists()
    {
        var response = await _anonClient.GetAsync($"/api/planinarskiobjekt/{TestData.PlaninarskiObjektId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<PlaninarskiObjektDto>();
        dto!.IdPlaninarskiObjekt.Should().Be(TestData.PlaninarskiObjektId);
    }

    [Fact]
    public async Task GetById_Returns404_WhenMissing()
    {
        var response = await _anonClient.GetAsync("/api/planinarskiobjekt/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_Returns201_AndCreatesEntity()
    {
        var dto = new PlaninarskiObjektCreateDto
        {
            IdPodrucje = TestData.PodrucjeId,
            IdPlaninarskaUdruga = TestData.PlaninarskaUdrugaId,
            Naziv = "Testni integracijski dom",
            TipObjekta = TipObjekta.Dom
        };

        var response = await _adminClient.PostAsJsonAsync("/api/planinarskiobjekt", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var kreiran = await response.Content.ReadFromJsonAsync<PlaninarskiObjektDto>();
        kreiran!.Naziv.Should().Be(dto.Naziv);
    }

    [Fact]
    public async Task Post_Returns400_WhenModelInvalid()
    {
        var response = await _adminClient.PostAsJsonAsync("/api/planinarskiobjekt", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
