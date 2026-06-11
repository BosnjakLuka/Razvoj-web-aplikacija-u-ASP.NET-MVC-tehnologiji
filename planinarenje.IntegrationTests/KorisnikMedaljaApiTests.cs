using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.IntegrationTests.Helpers;
using planinarenje.Models.Dto.KorisnikMedalja;

namespace planinarenje.IntegrationTests;

public class KorisnikMedaljaApiTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _adminClient = null!;

    public KorisnikMedaljaApiTests(CustomWebAppFactory factory) => _factory = factory;

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
        var response = await _anonClient.GetAsync("/api/korisnikmedalja");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var lista = await response.Content.ReadFromJsonAsync<List<KorisnikMedaljaDto>>();
        lista.Should().NotBeNull().And.NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Returns200_WhenExists()
    {
        var response = await _anonClient.GetAsync($"/api/korisnikmedalja/{TestData.KorisnikMedaljaId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<KorisnikMedaljaDto>();
        dto!.IdKorisnikMedalja.Should().Be(TestData.KorisnikMedaljaId);
    }

    [Fact]
    public async Task GetById_Returns404_WhenMissing()
    {
        var response = await _anonClient.GetAsync("/api/korisnikmedalja/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_Returns201_AndCreatesEntity()
    {
        // Medalja 2 (Brončana značka) nije dodijeljena Korisniku 1 u HasData seedu.
        var dto = new KorisnikMedaljaCreateDto
        {
            IdKorisnik = TestData.PlaninarKorisnikId,
            IdMedalja = 2,
            DatumDodjele = new DateTime(2026, 6, 1)
        };

        var response = await _adminClient.PostAsJsonAsync("/api/korisnikmedalja", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var kreiran = await response.Content.ReadFromJsonAsync<KorisnikMedaljaDto>();
        kreiran!.IdKorisnik.Should().Be(dto.IdKorisnik);
        kreiran.IdMedalja.Should().Be(dto.IdMedalja);
    }

    [Fact]
    public async Task Post_Returns400_WhenModelInvalid()
    {
        var response = await _adminClient.PostAsJsonAsync("/api/korisnikmedalja", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ---- Lab5: PUT / DELETE testovi ----

    [Fact]
    public async Task Put_Returns200_AndUpdatesEntity_WhenExists()
    {
        var id = await SeedKorisnikMedaljaAsync();

        var dto = new KorisnikMedaljaUpdateDto
        {
            DatumDodjele = new DateTime(2026, 5, 1),
            Napomena = "Ažurirana napomena"
        };

        var response = await _adminClient.PutAsJsonAsync($"/api/korisnikmedalja/{id}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var azuriran = await response.Content.ReadFromJsonAsync<KorisnikMedaljaDto>();
        azuriran!.IdKorisnikMedalja.Should().Be(id);
        azuriran.Napomena.Should().Be(dto.Napomena);
    }

    [Fact]
    public async Task Put_Returns404_WhenMissing()
    {
        var dto = new KorisnikMedaljaUpdateDto
        {
            DatumDodjele = new DateTime(2026, 5, 1),
            Napomena = "Ne postoji"
        };

        var response = await _adminClient.PutAsJsonAsync("/api/korisnikmedalja/99999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Returns204_WhenExists()
    {
        var id = await SeedKorisnikMedaljaAsync();

        var response = await _adminClient.DeleteAsync($"/api/korisnikmedalja/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_Returns404_WhenMissing()
    {
        var response = await _adminClient.DeleteAsync("/api/korisnikmedalja/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Seeda jednu throwaway dodjelu medalje (Korisnik 1 + Medalja 1) i vrati njezin Id.
    private async Task<int> SeedKorisnikMedaljaAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new KorisnikMedalja
        {
            IdKorisnik = TestData.PlaninarKorisnikId,
            IdMedalja = TestData.MedaljaId,
            DatumDodjele = new DateTime(2026, 1, 1)
        };
        db.KorisnikMedalje.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdKorisnikMedalja;
    }

    // ---- Lab5: Autorizacijski testovi (401 / 403) ----

    [Fact]
    public async Task Post_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.PostAsJsonAsync("/api/korisnikmedalja", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_Returns403_WhenNotAdmin()
    {
        using var planinarClient = AuthHelper.CreatePlaninarClient(_factory);

        var response = await planinarClient.PostAsJsonAsync("/api/korisnikmedalja", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
