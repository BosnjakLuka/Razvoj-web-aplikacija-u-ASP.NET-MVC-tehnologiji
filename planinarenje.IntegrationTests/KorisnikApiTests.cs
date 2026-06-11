using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.IntegrationTests.Helpers;
using planinarenje.Models.Dto.Korisnik;

namespace planinarenje.IntegrationTests;

public class KorisnikApiTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _adminClient = null!;

    public KorisnikApiTests(CustomWebAppFactory factory) => _factory = factory;

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
        var response = await _anonClient.GetAsync("/api/korisnik");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var lista = await response.Content.ReadFromJsonAsync<List<KorisnikPublicDto>>();
        lista.Should().NotBeNull().And.NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Returns200_WhenExists()
    {
        var response = await _anonClient.GetAsync($"/api/korisnik/{TestData.PlaninarKorisnikId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<KorisnikPublicDto>();
        dto!.IdKorisnik.Should().Be(TestData.PlaninarKorisnikId);
    }

    [Fact]
    public async Task GetById_Returns404_WhenMissing()
    {
        var response = await _anonClient.GetAsync("/api/korisnik/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_Returns201_AndCreatesEntity()
    {
        // Email i korisnickoIme moraju biti jedinstveni u bazi.
        var dto = new KorisnikCreateDto
        {
            Ime = "Novi",
            Prezime = "KorisnikTest",
            Email = "novi.korisnik.integ@test.hr",
            KorisnickoIme = "novi_korisnik_integ"
        };

        var response = await _adminClient.PostAsJsonAsync("/api/korisnik", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var kreiran = await response.Content.ReadFromJsonAsync<KorisnikAdminDto>();
        kreiran!.Email.Should().Be(dto.Email);
    }

    [Fact]
    public async Task Post_Returns400_WhenModelInvalid()
    {
        var response = await _adminClient.PostAsJsonAsync("/api/korisnik", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ---- Lab5: PUT / DELETE testovi ----

    [Fact]
    public async Task Put_Returns200_AndUpdatesEntity_WhenExists()
    {
        var id = await SeedKorisnikAsync();

        var dto = new KorisnikUpdateDto
        {
            Ime = "Ažuriran",
            Prezime = "Korisnik",
            Email = $"azuriran-{Guid.NewGuid():N}@test.hr",
            KorisnickoIme = $"azuriran_{Guid.NewGuid():N}",
            StatusAktivan = true
        };

        var response = await _adminClient.PutAsJsonAsync($"/api/korisnik/{id}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var azuriran = await response.Content.ReadFromJsonAsync<KorisnikAdminDto>();
        azuriran!.IdKorisnik.Should().Be(id);
        azuriran.Ime.Should().Be(dto.Ime);
    }

    [Fact]
    public async Task Put_Returns404_WhenMissing()
    {
        var dto = new KorisnikUpdateDto
        {
            Ime = "Ne",
            Prezime = "Postoji",
            Email = $"nepostoji-{Guid.NewGuid():N}@test.hr",
            KorisnickoIme = $"nepostoji_{Guid.NewGuid():N}",
            StatusAktivan = true
        };

        var response = await _adminClient.PutAsJsonAsync("/api/korisnik/99999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Returns204_WhenExists()
    {
        var id = await SeedKorisnikAsync();

        var response = await _adminClient.DeleteAsync($"/api/korisnik/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_Returns404_WhenMissing()
    {
        var response = await _adminClient.DeleteAsync("/api/korisnik/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Seeda svježeg throwaway Korisnika (jedinstveni email/korisničko ime) i vrati njegov Id.
    private async Task<int> SeedKorisnikAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new Korisnik
        {
            Ime = "Seed",
            Prezime = "Korisnik",
            Email = $"seed-{Guid.NewGuid():N}@test.hr",
            KorisnickoIme = $"seed_{Guid.NewGuid():N}",
            DatumRegistracije = DateTime.UtcNow,
            StatusAktivan = true
        };
        db.Korisnici.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdKorisnik;
    }

    // ---- Lab5: Autorizacijski testovi (401 / 403) ----

    [Fact]
    public async Task Post_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.PostAsJsonAsync("/api/korisnik", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_Returns403_WhenNotAdmin()
    {
        using var planinarClient = AuthHelper.CreatePlaninarClient(_factory);

        var response = await planinarClient.PostAsJsonAsync("/api/korisnik", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
