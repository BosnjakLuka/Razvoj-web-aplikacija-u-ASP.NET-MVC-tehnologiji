using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
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
}
