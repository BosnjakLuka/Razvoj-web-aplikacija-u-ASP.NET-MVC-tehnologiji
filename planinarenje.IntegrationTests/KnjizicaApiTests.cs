using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
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
}
