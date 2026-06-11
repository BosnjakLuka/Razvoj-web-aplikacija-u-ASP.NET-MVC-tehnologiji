using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using planinarenje.Entiteti;
using planinarenje.IntegrationTests.Helpers;
using planinarenje.Models.Dto.Ruta;

namespace planinarenje.IntegrationTests;

public class RutaApiTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _adminClient = null!;

    public RutaApiTests(CustomWebAppFactory factory) => _factory = factory;

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
        var response = await _anonClient.GetAsync("/api/ruta");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var lista = await response.Content.ReadFromJsonAsync<List<RutaDto>>();
        lista.Should().NotBeNull().And.NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Returns200_WhenExists()
    {
        var response = await _anonClient.GetAsync($"/api/ruta/{TestData.RutaId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<RutaDto>();
        dto!.IdRuta.Should().Be(TestData.RutaId);
    }

    [Fact]
    public async Task GetById_Returns404_WhenMissing()
    {
        var response = await _anonClient.GetAsync("/api/ruta/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_Returns201_AndCreatesEntity()
    {
        var dto = new RutaCreateDto
        {
            IdKontrolnaTocka = TestData.KontrolnaTockaId,
            Naziv = "Testna integracijska ruta",
            Pocetak = "Polazište Test",
            Kraj = "Cilj Test",
            VrijemeHodaMin = 60,
            DuljinaKm = 3.5m,
            TezinaRute = TezinaRute.Laka
        };

        var response = await _adminClient.PostAsJsonAsync("/api/ruta", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var kreiran = await response.Content.ReadFromJsonAsync<RutaDto>();
        kreiran!.Naziv.Should().Be(dto.Naziv);
    }

    [Fact]
    public async Task Post_Returns400_WhenModelInvalid()
    {
        var response = await _adminClient.PostAsJsonAsync("/api/ruta", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
