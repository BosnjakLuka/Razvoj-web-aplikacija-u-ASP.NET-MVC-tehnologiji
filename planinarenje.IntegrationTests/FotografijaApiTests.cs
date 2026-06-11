using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using planinarenje.Data;
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

    // ---- Lab5: PUT / DELETE testovi (Planinar je vlasnik Posjeta 1) ----

    [Fact]
    public async Task Put_Returns200_AndUpdatesEntity_WhenExists()
    {
        var id = await SeedFotografijaAsync();

        var dto = new FotografijaUpdateDto
        {
            TipSlike = TipSlike.Selfie,
            Opis = "Ažurirani opis fotografije."
        };

        var response = await _planinarClient.PutAsJsonAsync($"/api/fotografija/{id}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var azuriran = await response.Content.ReadFromJsonAsync<FotografijaDto>();
        azuriran!.IdFotografija.Should().Be(id);
        azuriran.Opis.Should().Be(dto.Opis);
    }

    [Fact]
    public async Task Put_Returns404_WhenMissing()
    {
        var dto = new FotografijaUpdateDto
        {
            TipSlike = TipSlike.Oznaka,
            Opis = "Ne postoji"
        };

        var response = await _planinarClient.PutAsJsonAsync("/api/fotografija/99999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Returns204_WhenExists()
    {
        var id = await SeedFotografijaAsync();

        var response = await _planinarClient.DeleteAsync($"/api/fotografija/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_Returns404_WhenMissing()
    {
        var response = await _planinarClient.DeleteAsync("/api/fotografija/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Seeda jednu throwaway fotografiju vezanu uz seedirani Posjet 1 (vlasnik = Planinar).
    private async Task<int> SeedFotografijaAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new Fotografija
        {
            IdPosjet = TestData.PosjetId,
            NazivDatoteke = "seed_foto.jpg",
            PutanjaDatoteke = "/uploads/posjeti/1/seed_foto.jpg",
            TipSlike = TipSlike.Krajolik,
            DatumUploada = DateTime.UtcNow
        };
        db.Fotografije.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdFotografija;
    }

    // ---- Lab5: Autorizacijski testovi (401 / 403) ----

    [Fact]
    public async Task Post_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.PostAsJsonAsync("/api/fotografija", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Strani planinar (autentificiran, ali nije vlasnik Posjeta 1) → 403.
    [Fact]
    public async Task Post_Returns403_WhenNotOwner()
    {
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);

        var dto = new FotografijaCreateDto
        {
            IdPosjet = TestData.PosjetId,
            NazivDatoteke = "foreign.jpg",
            PutanjaDatoteke = "/uploads/posjeti/1/foreign.jpg",
            TipSlike = TipSlike.Krajolik
        };

        var response = await foreignClient.PostAsJsonAsync("/api/fotografija", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
