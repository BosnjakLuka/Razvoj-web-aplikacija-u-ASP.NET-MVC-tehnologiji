using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.IntegrationTests.Helpers;
using planinarenje.Models.Dto.KontrolnaTocka;

namespace planinarenje.IntegrationTests;

public class KontrolnaTockaApiTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _adminClient = null!;

    public KontrolnaTockaApiTests(CustomWebAppFactory factory) => _factory = factory;

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
        var response = await _anonClient.GetAsync("/api/kontrolnatocka");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var lista = await response.Content.ReadFromJsonAsync<List<KontrolnaTockaDto>>();
        lista.Should().NotBeNull().And.NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Returns200_WhenExists()
    {
        var response = await _anonClient.GetAsync($"/api/kontrolnatocka/{TestData.KontrolnaTockaId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<KontrolnaTockaDto>();
        dto!.IdKontrolnaTocka.Should().Be(TestData.KontrolnaTockaId);
    }

    [Fact]
    public async Task GetById_Returns404_WhenMissing()
    {
        var response = await _anonClient.GetAsync("/api/kontrolnatocka/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_Returns201_AndCreatesEntity()
    {
        var dto = new KontrolnaTockaCreateDto
        {
            GUIDOznaka = "TEST-NEW-KT-GUID-999",
            IdPodrucje = TestData.PodrucjeId,
            Naziv = "Testni vrh integracijski",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 500
        };

        var response = await _adminClient.PostAsJsonAsync("/api/kontrolnatocka", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var kreiran = await response.Content.ReadFromJsonAsync<KontrolnaTockaDto>();
        kreiran!.GUIDOznaka.Should().Be(dto.GUIDOznaka);
    }

    [Fact]
    public async Task Post_Returns400_WhenModelInvalid()
    {
        var response = await _adminClient.PostAsJsonAsync("/api/kontrolnatocka", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ---- Lab5: PUT / DELETE testovi ----

    [Fact]
    public async Task Put_Returns200_AndUpdatesEntity_WhenExists()
    {
        var id = await SeedKontrolnaTockaAsync();

        var dto = new KontrolnaTockaUpdateDto
        {
            GUIDOznaka = Guid.NewGuid().ToString(),
            IdPodrucje = TestData.PodrucjeId,
            Naziv = "Ažurirana kontrolna točka",
            TipKontrolneTocke = TipKontrolneTocke.Vidikovac,
            NadmorskaVisina = 600
        };

        var response = await _adminClient.PutAsJsonAsync($"/api/kontrolnatocka/{id}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var azuriran = await response.Content.ReadFromJsonAsync<KontrolnaTockaDto>();
        azuriran!.IdKontrolnaTocka.Should().Be(id);
        azuriran.Naziv.Should().Be(dto.Naziv);
    }

    [Fact]
    public async Task Put_Returns404_WhenMissing()
    {
        var dto = new KontrolnaTockaUpdateDto
        {
            GUIDOznaka = Guid.NewGuid().ToString(),
            IdPodrucje = TestData.PodrucjeId,
            Naziv = "Ne postoji",
            TipKontrolneTocke = TipKontrolneTocke.Vrh
        };

        var response = await _adminClient.PutAsJsonAsync("/api/kontrolnatocka/99999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Returns204_WhenExists()
    {
        var id = await SeedKontrolnaTockaAsync();

        var response = await _adminClient.DeleteAsync($"/api/kontrolnatocka/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_Returns404_WhenMissing()
    {
        var response = await _adminClient.DeleteAsync("/api/kontrolnatocka/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Seeda jednu throwaway kontrolnu točku (auto-generirani Id) i vrati njezin Id.
    private async Task<int> SeedKontrolnaTockaAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new KontrolnaTocka
        {
            GUIDOznaka = Guid.NewGuid().ToString(),
            IdPodrucje = TestData.PodrucjeId,
            Naziv = "Seed kontrolna točka",
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            NadmorskaVisina = 500
        };
        db.KontrolneTocke.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdKontrolnaTocka;
    }

    // ---- Lab5: Autorizacijski testovi (401 / 403) ----

    [Fact]
    public async Task Post_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.PostAsJsonAsync("/api/kontrolnatocka", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_Returns403_WhenNotAdmin()
    {
        using var planinarClient = AuthHelper.CreatePlaninarClient(_factory);

        var response = await planinarClient.PostAsJsonAsync("/api/kontrolnatocka", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
