using System.Net;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.IntegrationTests.Helpers;

namespace planinarenje.IntegrationTests;

/// <summary>
/// Integracijski testovi za MVC FotografijaController — po roli: guest, Planinar (vlasnik/strani), Admin.
/// Index je Admin-only; Create/Edit/Delete/Details provjeravaju vlasništvo preko Posjet.IdKorisnik.
/// </summary>
public class FotografijaControllerTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _planinarClient = null!;
    private HttpClient _adminClient = null!;

    public FotografijaControllerTests(CustomWebAppFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.SeedDatabaseAsync();
        _anonClient = AuthHelper.CreateAnonymousClient(_factory);
        _planinarClient = AuthHelper.CreatePlaninarClient(_factory);
        _adminClient = AuthHelper.CreateAdminClient(_factory);
    }

    public Task DisposeAsync()
    {
        _anonClient.Dispose();
        _planinarClient.Dispose();
        _adminClient.Dispose();
        return Task.CompletedTask;
    }

    // ---- Index — Admin samo ----

    [Fact]
    public async Task Index_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.GetAsync("/Fotografija/Index");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Index_Returns403_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync("/Fotografija/Index");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Index_Returns200_WhenAdmin()
    {
        var response = await _adminClient.GetAsync("/Fotografija/Index");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ---- Create ----

    [Fact]
    public async Task Create_Get_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.GetAsync("/Fotografija/Create");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_Get_Returns200_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync("/Fotografija/Create");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_Post_RedirectsToIndex_AndCreatesEntity_WhenOwnerPlaninar()
    {
        var posjetId = await SeedPosjetAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_planinarClient, "/Fotografija/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["IdPosjet"] = posjetId.ToString(),
            ["NazivDatoteke"] = "test-slika.jpg",
            ["PutanjaDatoteke"] = "/Slike/Fotografije/test-slika.jpg",
            ["TipSlike"] = nameof(TipSlike.Krajolik)
        };

        var response = await _planinarClient.PostAsync("/Fotografija/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var created = db.Fotografije.SingleOrDefault(f => f.NazivDatoteke == "test-slika.jpg");
        created.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_Post_Returns403_WhenForeignPlaninar()
    {
        var posjetId = await SeedPosjetAsync();
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);
        var token = await AntiForgeryHelper.GetTokenAsync(foreignClient, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["IdPosjet"] = posjetId.ToString(),
            ["NazivDatoteke"] = "hakirano.jpg",
            ["PutanjaDatoteke"] = "/Slike/Fotografije/hakirano.jpg",
            ["TipSlike"] = nameof(TipSlike.Krajolik)
        };

        var response = await foreignClient.PostAsync("/Fotografija/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ---- Edit ----

    [Fact]
    public async Task Edit_Get_Returns200_WhenOwnerPlaninar()
    {
        var id = await SeedFotografijaAsync();

        var response = await _planinarClient.GetAsync($"/Fotografija/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Get_Returns403_WhenForeignPlaninar()
    {
        var id = await SeedFotografijaAsync();
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);

        var response = await foreignClient.GetAsync($"/Fotografija/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Edit_Get_Returns200_WhenAdmin_NotOwner()
    {
        var id = await SeedFotografijaAsync();

        var response = await _adminClient.GetAsync($"/Fotografija/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Post_UpdatesEntity_WhenOwnerPlaninar()
    {
        var id = await SeedFotografijaAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_planinarClient, $"/Fotografija/Edit/{id}");

        using var scope0 = _factory.Services.CreateScope();
        var db0 = scope0.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var idPosjet = db0.Fotografije.Single(f => f.IdFotografija == id).IdPosjet;

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["IdPosjet"] = idPosjet.ToString(),
            ["NazivDatoteke"] = "azurirano.jpg",
            ["PutanjaDatoteke"] = "/Slike/Fotografije/azurirano.jpg",
            ["TipSlike"] = nameof(TipSlike.Selfie)
        };

        var response = await _planinarClient.PostAsync($"/Fotografija/Edit/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var updated = db.Fotografije.Single(f => f.IdFotografija == id);
        updated.NazivDatoteke.Should().Be("azurirano.jpg");
    }

    [Fact]
    public async Task Edit_Post_Returns403_WhenForeignPlaninar()
    {
        var id = await SeedFotografijaAsync();
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);
        var token = await AntiForgeryHelper.GetTokenAsync(foreignClient, "/Posjet/Create");

        using var scope0 = _factory.Services.CreateScope();
        var db0 = scope0.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var idPosjet = db0.Fotografije.Single(f => f.IdFotografija == id).IdPosjet;

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["IdPosjet"] = idPosjet.ToString(),
            ["NazivDatoteke"] = "hakirano.jpg",
            ["PutanjaDatoteke"] = "/Slike/Fotografije/hakirano.jpg",
            ["TipSlike"] = nameof(TipSlike.Selfie)
        };

        var response = await foreignClient.PostAsync($"/Fotografija/Edit/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ---- Delete (soft delete) ----

    [Fact]
    public async Task Delete_Get_Returns200_WhenOwnerPlaninar()
    {
        var id = await SeedFotografijaAsync();

        var response = await _planinarClient.GetAsync($"/Fotografija/Delete/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_Post_SoftDeletesEntity_WhenOwnerPlaninar()
    {
        var id = await SeedFotografijaAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_planinarClient, $"/Fotografija/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _planinarClient.PostAsync($"/Fotografija/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var deleted = db.Fotografije.IgnoreQueryFilters().Single(f => f.IdFotografija == id);
        deleted.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_Post_Returns403_WhenForeignPlaninar()
    {
        var id = await SeedFotografijaAsync();
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);
        var token = await AntiForgeryHelper.GetTokenAsync(foreignClient, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await foreignClient.PostAsync($"/Fotografija/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ---- Details ----

    [Fact]
    public async Task Details_Returns200_WhenOwnerPlaninar()
    {
        var id = await SeedFotografijaAsync();

        var response = await _planinarClient.GetAsync($"/Fotografija/Details/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns403_WhenForeignPlaninar()
    {
        var id = await SeedFotografijaAsync();
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);

        var response = await foreignClient.GetAsync($"/Fotografija/Details/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Details_Returns404_WhenMissing()
    {
        var response = await _adminClient.GetAsync("/Fotografija/Details/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Seeda throwaway Posjet vlasnistvo Planinara (potreban za vlasništvo fotografije).
    private async Task<int> SeedPosjetAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new Posjet
        {
            IdKorisnik = TestData.PlaninarKorisnikId,
            IdKnjizica = TestData.KnjizicaId,
            IdKontrolnaTocka = TestData.KontrolnaTockaId,
            IdRuta = TestData.RutaId,
            DatumVrijemePosjeta = new DateTime(2026, 1, 1, 9, 0, 0),
            DozivljajPosjeta = DozivljajPosjeta.Srednje,
            UneseniGUID = "KT-HPO-2-1-VIS",
            DatumKreiranjaZapisa = DateTime.UtcNow
        };
        db.Posjeti.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdPosjet;
    }

    private async Task<int> SeedFotografijaAsync()
    {
        var posjetId = await SeedPosjetAsync();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new Fotografija
        {
            IdPosjet = posjetId,
            NazivDatoteke = "throwaway.jpg",
            PutanjaDatoteke = "/Slike/Fotografije/throwaway.jpg",
            TipSlike = TipSlike.Krajolik,
            DatumUploada = DateTime.UtcNow
        };
        db.Fotografije.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdFotografija;
    }
}
