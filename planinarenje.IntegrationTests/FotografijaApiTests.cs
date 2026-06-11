using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using planinarenje.Entiteti;
using planinarenje.IntegrationTests.Helpers;
using planinarenje.Models.Dto.Fotografija;

namespace planinarenje.IntegrationTests;

public class FotografijaApiTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _planinarClient = null!;

    public FotografijaApiTests(CustomWebAppFactory factory) => _factory = factory;

    public async Task InitializeAsync()
    {
        await _factory.SeedDatabaseAsync();
        _anonClient = AuthHelper.CreateAnonymousClient(_factory);
        // Planinar je vlasnik Posjeta 1 (IdKorisnik=1 = PlaninarKorisnikId).
        _planinarClient = AuthHelper.CreatePlaninarClient(_factory);
    }

    public Task DisposeAsync()
    {
        _anonClient.Dispose();
        _planinarClient.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetAll_Returns200_AndNonEmptyList()
    {
        var response = await _anonClient.GetAsync("/api/fotografija");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var lista = await response.Content.ReadFromJsonAsync<List<FotografijaDto>>();
        lista.Should().NotBeNull().And.NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Returns200_WhenExists()
    {
        var response = await _anonClient.GetAsync($"/api/fotografija/{TestData.FotografijaId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<FotografijaDto>();
        dto!.IdFotografija.Should().Be(TestData.FotografijaId);
    }

    [Fact]
    public async Task GetById_Returns404_WhenMissing()
    {
        var response = await _anonClient.GetAsync("/api/fotografija/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_Returns201_AndCreatesEntity()
    {
        // Planinar je vlasnik Posjeta 1 → smije dodati fotografiju.
        var dto = new FotografijaCreateDto
        {
            IdPosjet = TestData.PosjetId,
            NazivDatoteke = "integ_test_foto.jpg",
            PutanjaDatoteke = "/uploads/posjeti/1/integ_test_foto.jpg",
            TipSlike = TipSlike.Krajolik
        };

        var response = await _planinarClient.PostAsJsonAsync("/api/fotografija", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var kreiran = await response.Content.ReadFromJsonAsync<FotografijaDto>();
        kreiran!.NazivDatoteke.Should().Be(dto.NazivDatoteke);
    }

    [Fact]
    public async Task Post_Returns400_WhenModelInvalid()
    {
        var response = await _planinarClient.PostAsJsonAsync("/api/fotografija", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
