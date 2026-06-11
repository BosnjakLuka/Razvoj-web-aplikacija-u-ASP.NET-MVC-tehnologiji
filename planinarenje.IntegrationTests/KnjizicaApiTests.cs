using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.IntegrationTests.Helpers;
using planinarenje.Models.Dto.Knjizica;

namespace planinarenje.IntegrationTests;

/// <summary>
/// KnjizicaApiController GET zahtijeva autentikaciju ([Authorize]), ne AllowAnonymous.
/// Koristimo Admin klijenta koji zaobilazi provjeru vlasništva.
/// </summary>
public class KnjizicaApiTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _adminClient = null!;

    public KnjizicaApiTests(CustomWebAppFactory factory) => _factory = factory;

    public async Task InitializeAsync()
    {
        await _factory.SeedDatabaseAsync();
        _adminClient = AuthHelper.CreateAdminClient(_factory);
    }

    public Task DisposeAsync()
    {
        _adminClient.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetAll_Returns200_AndNonEmptyList()
    {
        var response = await _adminClient.GetAsync("/api/knjizica");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var lista = await response.Content.ReadFromJsonAsync<List<KnjizicaDto>>();
        lista.Should().NotBeNull().And.NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Returns200_WhenExists()
    {
        var response = await _adminClient.GetAsync($"/api/knjizica/{TestData.KnjizicaId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<KnjizicaDto>();
        dto!.IdKnjizica.Should().Be(TestData.KnjizicaId);
    }

    [Fact]
    public async Task GetById_Returns404_WhenMissing()
    {
        var response = await _adminClient.GetAsync("/api/knjizica/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_Returns201_AndCreatesEntity()
    {
        // Korisnik 100 (dodaje TestDataSeeder) nema Knjižicu → POST smije proći.
        var dto = new KnjizicaCreateDto
        {
            IdKorisnik = TestData.KorisnikBezKnjiziceId,
            StatusAktivna = true
        };

        var response = await _adminClient.PostAsJsonAsync("/api/knjizica", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var kreirana = await response.Content.ReadFromJsonAsync<KnjizicaDto>();
        kreirana!.IdKorisnik.Should().Be(TestData.KorisnikBezKnjiziceId);
    }

    [Fact]
    public async Task Post_Returns400_WhenModelInvalid()
    {
        // IdKorisnik = 0 → [Required] validacija puca (int default 0 ne prođe jer nije nullable)
        // Ali zapravo KnjizicaCreateDto.IdKorisnik nema [Required] na int (uvijek ima vrijednost 0).
        // Šaljemo JSON bez ijednog polja — model state provjera validira da IdKorisnik = 0,
        // a controller tada vraća BadRequest jer Korisnik 0 ne postoji.
        var response = await _adminClient.PostAsJsonAsync("/api/knjizica",
            new KnjizicaCreateDto { IdKorisnik = 0, StatusAktivna = true });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ---- Lab5: PUT / DELETE testovi ----

    [Fact]
    public async Task Put_Returns200_AndUpdatesEntity_WhenExists()
    {
        var id = await SeedKnjizicaAsync();

        var dto = new KnjizicaUpdateDto
        {
            Napomena = "Ažurirana napomena",
            StatusAktivna = true
        };

        var response = await _adminClient.PutAsJsonAsync($"/api/knjizica/{id}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var azurirana = await response.Content.ReadFromJsonAsync<KnjizicaDto>();
        azurirana!.IdKnjizica.Should().Be(id);
        azurirana.Napomena.Should().Be(dto.Napomena);
    }

    [Fact]
    public async Task Put_Returns404_WhenMissing()
    {
        var dto = new KnjizicaUpdateDto
        {
            Napomena = "Ne postoji",
            StatusAktivna = true
        };

        var response = await _adminClient.PutAsJsonAsync("/api/knjizica/99999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Returns204_WhenExists()
    {
        var id = await SeedKnjizicaAsync();

        var response = await _adminClient.DeleteAsync($"/api/knjizica/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_Returns404_WhenMissing()
    {
        var response = await _adminClient.DeleteAsync("/api/knjizica/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Seeda svježeg Korisnika i njegovu Knjižicu (1:1) te vrati Id knjižice.
    private async Task<int> SeedKnjizicaAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var korisnik = new Korisnik
        {
            Ime = "Seed",
            Prezime = "Knjizica",
            Email = $"seed-knjizica-{Guid.NewGuid():N}@test.hr",
            KorisnickoIme = $"seed_knjizica_{Guid.NewGuid():N}",
            DatumRegistracije = DateTime.UtcNow,
            StatusAktivan = true
        };
        db.Korisnici.Add(korisnik);
        await db.SaveChangesAsync();

        var knjizica = new Knjizica
        {
            IdKorisnik = korisnik.IdKorisnik,
            DatumKreiranja = DateTime.UtcNow,
            StatusAktivna = true,
            Napomena = "Seed knjižica"
        };
        db.Knjizice.Add(knjizica);
        await db.SaveChangesAsync();
        return knjizica.IdKnjizica;
    }

    // ---- Lab5: Autorizacijski testovi (401 / 403) ----

    [Fact]
    public async Task Post_Returns401_WhenAnonymous()
    {
        using var anonClient = AuthHelper.CreateAnonymousClient(_factory);

        var response = await anonClient.PostAsJsonAsync("/api/knjizica", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_Returns403_WhenNotAdmin()
    {
        using var planinarClient = AuthHelper.CreatePlaninarClient(_factory);

        var response = await planinarClient.PostAsJsonAsync("/api/knjizica", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
