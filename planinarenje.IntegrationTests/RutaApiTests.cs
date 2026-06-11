using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using planinarenje.Data;
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

    // ---- Lab5: PUT / DELETE testovi ----

    [Fact]
    public async Task Put_Returns200_AndUpdatesEntity_WhenExists()
    {
        var id = await SeedRutaAsync();

        var dto = new RutaUpdateDto
        {
            IdKontrolnaTocka = TestData.KontrolnaTockaId,
            Naziv = "Ažurirana ruta",
            Pocetak = "Polazište",
            Kraj = "Cilj",
            VrijemeHodaMin = 90,
            DuljinaKm = 4.2m,
            TezinaRute = TezinaRute.Srednja
        };

        var response = await _adminClient.PutAsJsonAsync($"/api/ruta/{id}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var azuriran = await response.Content.ReadFromJsonAsync<RutaDto>();
        azuriran!.IdRuta.Should().Be(id);
        azuriran.Naziv.Should().Be(dto.Naziv);
    }

    [Fact]
    public async Task Put_Returns404_WhenMissing()
    {
        var dto = new RutaUpdateDto
        {
            IdKontrolnaTocka = TestData.KontrolnaTockaId,
            Naziv = "Ne postoji",
            Pocetak = "P",
            Kraj = "K",
            VrijemeHodaMin = 60,
            DuljinaKm = 1.0m,
            TezinaRute = TezinaRute.Laka
        };

        var response = await _adminClient.PutAsJsonAsync("/api/ruta/99999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Returns204_WhenExists()
    {
        var id = await SeedRutaAsync();

        var response = await _adminClient.DeleteAsync($"/api/ruta/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_Returns404_WhenMissing()
    {
        var response = await _adminClient.DeleteAsync("/api/ruta/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Seeda jednu throwaway rutu (auto-generirani Id) i vrati njezin Id.
    private async Task<int> SeedRutaAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new Ruta
        {
            IdKontrolnaTocka = TestData.KontrolnaTockaId,
            Naziv = "Seed ruta",
            Pocetak = "Početak",
            Kraj = "Kraj",
            VrijemeHodaMin = 60,
            DuljinaKm = 3.5m,
            TezinaRute = TezinaRute.Laka
        };
        db.Rute.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdRuta;
    }

    // ---- Lab5: Autorizacijski testovi (401 / 403) ----

    [Fact]
    public async Task Post_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.PostAsJsonAsync("/api/ruta", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_Returns403_WhenNotAdmin()
    {
        using var planinarClient = AuthHelper.CreatePlaninarClient(_factory);

        var response = await planinarClient.PostAsJsonAsync("/api/ruta", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
