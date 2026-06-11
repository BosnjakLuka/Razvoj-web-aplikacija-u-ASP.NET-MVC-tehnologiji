using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
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
}
