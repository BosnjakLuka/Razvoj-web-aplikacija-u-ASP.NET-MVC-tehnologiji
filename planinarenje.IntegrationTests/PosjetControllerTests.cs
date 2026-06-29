using System.Net;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.IntegrationTests.Helpers;

namespace planinarenje.IntegrationTests;

/// <summary>
/// Integracijski testovi za MVC PosjetController (ne-API) — CRUD kroz HTML forme,
/// po roli: guest (anoniman), Planinar (vlasnik), strani Planinar (nije vlasnik), Admin.
/// Posjet/Podrucje/Ruta imaju globalni query filter (DeletedAt == null) u PlaninarstvoDbContext,
/// pa se soft-obrisani zapisi provjeravaju s IgnoreQueryFilters().
/// </summary>
public class PosjetControllerTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _planinarClient = null!;
    private HttpClient _adminClient = null!;

    public PosjetControllerTests(CustomWebAppFactory factory)
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

    // ---- Index / Details — javno dostupni ----

    [Fact]
    public async Task Index_Returns200_Anonymous()
    {
        var response = await _anonClient.GetAsync("/Posjet/Index");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns200_WhenExists_Anonymous()
    {
        var response = await _anonClient.GetAsync($"/Posjet/Details/{TestData.PosjetId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns404_WhenMissing()
    {
        var response = await _anonClient.GetAsync("/Posjet/Details/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---- Create ----

    [Fact]
    public async Task Create_Get_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.GetAsync("/Posjet/Create");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_Get_Returns200_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync("/Posjet/Create");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_Post_Returns401_WhenAnonymous()
    {
        var form = new Dictionary<string, string>
        {
            ["IdKontrolnaTocka"] = TestData.KontrolnaTockaId.ToString(),
            ["IdRuta"] = TestData.RutaId.ToString(),
            ["UneseniGUID"] = "KT-HPO-2-1-VIS"
        };

        var response = await _anonClient.PostAsync("/Posjet/Create", new FormUrlEncodedContent(form));
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_Post_RedirectsToIndex_AndCreatesEntity_WhenPlaninarValid()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_planinarClient, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["IdKorisnik"] = TestData.PlaninarKorisnikId.ToString(),
            ["IdKnjizica"] = TestData.KnjizicaId.ToString(),
            ["IdKontrolnaTocka"] = TestData.KontrolnaTockaId.ToString(),
            ["IdRuta"] = TestData.RutaId.ToString(),
            ["DatumVrijemePosjeta"] = "2026-06-01T10:00",
            ["DozivljajPosjeta"] = nameof(DozivljajPosjeta.Srednje),
            ["UneseniGUID"] = "KT-HPO-2-1-VIS",
            ["OpisIskustva"] = "Integracijski MVC test posjet."
        };

        var response = await _planinarClient.PostAsync("/Posjet/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.OriginalString.Should().Contain("/Posjet");

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var created = db.Posjeti.SingleOrDefault(p => p.OpisIskustva == "Integracijski MVC test posjet.");
        created.Should().NotBeNull();
        created!.IdKorisnik.Should().Be(TestData.PlaninarKorisnikId);
    }

    [Fact]
    public async Task Create_Post_ReturnsViewWithErrors_WhenGuidNeMatchesKontrolnuTocku()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_planinarClient, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["IdKorisnik"] = TestData.PlaninarKorisnikId.ToString(),
            ["IdKnjizica"] = TestData.KnjizicaId.ToString(),
            ["IdKontrolnaTocka"] = TestData.KontrolnaTockaId.ToString(),
            ["IdRuta"] = TestData.RutaId.ToString(),
            ["DatumVrijemePosjeta"] = "2026-06-01T10:00",
            ["DozivljajPosjeta"] = nameof(DozivljajPosjeta.Srednje),
            ["UneseniGUID"] = "POGRESAN-GUID"
        };

        var response = await _planinarClient.PostAsync("/Posjet/Create", new FormUrlEncodedContent(form));

        // Validacija ne uspije -> vraca se ista View (200), bez redirecta.
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ---- Edit ----

    [Fact]
    public async Task Edit_Get_Returns200_WhenOwnerPlaninar()
    {
        var id = await SeedPosjetAsync();

        var response = await _planinarClient.GetAsync($"/Posjet/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Get_Returns403_WhenForeignPlaninar()
    {
        var id = await SeedPosjetAsync();
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);

        var response = await foreignClient.GetAsync($"/Posjet/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Edit_Get_Returns200_WhenAdmin_NotOwner()
    {
        var id = await SeedPosjetAsync();

        var response = await _adminClient.GetAsync($"/Posjet/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Post_UpdatesEntity_WhenOwnerPlaninar()
    {
        var id = await SeedPosjetAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_planinarClient, $"/Posjet/Edit/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["IdKorisnik"] = TestData.PlaninarKorisnikId.ToString(),
            ["IdKnjizica"] = TestData.KnjizicaId.ToString(),
            ["IdKontrolnaTocka"] = TestData.KontrolnaTockaId.ToString(),
            ["IdRuta"] = TestData.RutaId.ToString(),
            ["DatumVrijemePosjeta"] = "2026-02-01T10:00",
            ["DozivljajPosjeta"] = nameof(DozivljajPosjeta.Lagano),
            ["UneseniGUID"] = "KT-HPO-2-1-VIS",
            ["OpisIskustva"] = "Azurirani opis MVC testa."
        };

        var response = await _planinarClient.PostAsync($"/Posjet/Edit/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var updated = db.Posjeti.Single(p => p.IdPosjet == id);
        updated.OpisIskustva.Should().Be("Azurirani opis MVC testa.");
    }

    [Fact]
    public async Task Edit_Post_Returns403_WhenForeignPlaninar()
    {
        var id = await SeedPosjetAsync();
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);
        // Token mora dolaziti iz ISTOG klijenta koji šalje POST (antiforgery kolačić je
        // vezan na taj HttpClient-ov cookie container); GET /Posjet/Create samo zahtijeva
        // [Authorize] (bez provjere vlasništva), pa je dovoljan za dobivanje tokena.
        var token = await AntiForgeryHelper.GetTokenAsync(foreignClient, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["IdKorisnik"] = TestData.PlaninarKorisnikId.ToString(),
            ["IdKnjizica"] = TestData.KnjizicaId.ToString(),
            ["IdKontrolnaTocka"] = TestData.KontrolnaTockaId.ToString(),
            ["IdRuta"] = TestData.RutaId.ToString(),
            ["DatumVrijemePosjeta"] = "2026-02-01T10:00",
            ["DozivljajPosjeta"] = nameof(DozivljajPosjeta.Lagano),
            ["UneseniGUID"] = "KT-HPO-2-1-VIS"
        };

        var response = await foreignClient.PostAsync($"/Posjet/Edit/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ---- Delete (soft delete) ----

    [Fact]
    public async Task Delete_Get_Returns200_WhenOwnerPlaninar()
    {
        var id = await SeedPosjetAsync();

        var response = await _planinarClient.GetAsync($"/Posjet/Delete/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_Post_SoftDeletesEntity_WhenOwnerPlaninar()
    {
        var id = await SeedPosjetAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_planinarClient, $"/Posjet/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _planinarClient.PostAsync($"/Posjet/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var deleted = db.Posjeti.IgnoreQueryFilters().Single(p => p.IdPosjet == id);
        deleted.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_Post_Returns403_WhenForeignPlaninar()
    {
        var id = await SeedPosjetAsync();
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);
        var token = await AntiForgeryHelper.GetTokenAsync(foreignClient, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await foreignClient.PostAsync($"/Posjet/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_Post_SoftDeletes_WhenAdmin_NotOwner()
    {
        var id = await SeedPosjetAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/Posjet/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _adminClient.PostAsync($"/Posjet/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
    }

    // Seeda jedan throwaway posjet vlasnistvo Planinara, za Edit/Delete testove.
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
}
