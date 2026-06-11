using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.IntegrationTests.Helpers;
using planinarenje.Models.Dto.Medalja;

namespace planinarenje.IntegrationTests;

public class MedaljaApiTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _adminClient = null!;

    public MedaljaApiTests(CustomWebAppFactory factory) => _factory = factory;

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
        var response = await _anonClient.GetAsync("/api/medalja");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var lista = await response.Content.ReadFromJsonAsync<List<MedaljaDto>>();
        lista.Should().NotBeNull().And.NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Returns200_WhenExists()
    {
        var response = await _anonClient.GetAsync($"/api/medalja/{TestData.MedaljaId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<MedaljaDto>();
        dto!.IdMedalja.Should().Be(TestData.MedaljaId);
    }

    [Fact]
    public async Task GetById_Returns404_WhenMissing()
    {
        var response = await _anonClient.GetAsync("/api/medalja/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_Returns201_AndCreatesEntity()
    {
        var dto = new MedaljaCreateDto
        {
            Naziv = "Testna integracijska medalja",
            MinimalanBrojKontrolnihTocaka = 5,
            MinimalanBrojPodrucja = 2
        };

        var response = await _adminClient.PostAsJsonAsync("/api/medalja", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var kreiran = await response.Content.ReadFromJsonAsync<MedaljaDto>();
        kreiran!.Naziv.Should().Be(dto.Naziv);
    }

    [Fact]
    public async Task Post_Returns400_WhenModelInvalid()
    {
        var response = await _adminClient.PostAsJsonAsync("/api/medalja", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ---- Lab5: PUT / DELETE testovi ----

    [Fact]
    public async Task Put_Returns200_AndUpdatesEntity_WhenExists()
    {
        var id = await SeedMedaljaAsync();

        var dto = new MedaljaUpdateDto
        {
            Naziv = "Ažurirana medalja",
            MinimalanBrojKontrolnihTocaka = 7,
            MinimalanBrojPodrucja = 3
        };

        var response = await _adminClient.PutAsJsonAsync($"/api/medalja/{id}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var azuriran = await response.Content.ReadFromJsonAsync<MedaljaDto>();
        azuriran!.IdMedalja.Should().Be(id);
        azuriran.Naziv.Should().Be(dto.Naziv);
    }

    [Fact]
    public async Task Put_Returns404_WhenMissing()
    {
        var dto = new MedaljaUpdateDto
        {
            Naziv = "Ne postoji",
            MinimalanBrojKontrolnihTocaka = 1,
            MinimalanBrojPodrucja = 1
        };

        var response = await _adminClient.PutAsJsonAsync("/api/medalja/99999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Returns204_WhenExists()
    {
        var id = await SeedMedaljaAsync();

        var response = await _adminClient.DeleteAsync($"/api/medalja/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_Returns404_WhenMissing()
    {
        var response = await _adminClient.DeleteAsync("/api/medalja/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Seeda jednu throwaway medalju (auto-generirani Id) i vrati njezin Id.
    private async Task<int> SeedMedaljaAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new Medalja
        {
            Naziv = "Seed medalja",
            MinimalanBrojKontrolnihTocaka = 5,
            MinimalanBrojPodrucja = 2
        };
        db.Medalje.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdMedalja;
    }

    // ---- Lab5: Autorizacijski testovi (401 / 403) ----

    [Fact]
    public async Task Post_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.PostAsJsonAsync("/api/medalja", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_Returns403_WhenNotAdmin()
    {
        using var planinarClient = AuthHelper.CreatePlaninarClient(_factory);

        var response = await planinarClient.PostAsJsonAsync("/api/medalja", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
