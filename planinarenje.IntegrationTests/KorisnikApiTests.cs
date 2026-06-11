using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
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
}
