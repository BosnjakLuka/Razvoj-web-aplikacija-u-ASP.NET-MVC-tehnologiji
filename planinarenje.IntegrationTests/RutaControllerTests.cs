using System.Net;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.IntegrationTests.Helpers;

namespace planinarenje.IntegrationTests;

/// <summary>
/// Integracijski testovi za MVC RutaController — po roli: guest, Planinar (vlasnik/strani), Admin.
/// Index/Details javni; Create/Edit dopusteni Adminu i Planinaru (Planinar-create ide na odobravanje);
/// Delete Admin samo. Ruta ima globalni query filter (DeletedAt == null) -> IgnoreQueryFilters() za soft delete.
/// </summary>
public class RutaControllerTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _planinarClient = null!;
    private HttpClient _adminClient = null!;

    public RutaControllerTests(CustomWebAppFactory factory)
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
        var response = await _anonClient.GetAsync("/Ruta/Index");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns200_WhenExists_Anonymous()
    {
        var response = await _anonClient.GetAsync($"/Ruta/Details/{TestData.RutaId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns404_WhenMissing()
    {
        var response = await _anonClient.GetAsync("/Ruta/Details/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---- Create — Admin i Planinar ----

    [Fact]
    public async Task Create_Get_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.GetAsync("/Ruta/Create");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_Get_Returns200_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync("/Ruta/Create");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_Post_RedirectsToIndex_AndCreatesUnodobrenuEntity_WhenPlaninar()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_planinarClient, "/Ruta/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Testna ruta MVC",
            ["Pocetak"] = "Polaziste MVC",
            ["Kraj"] = "Odrediste MVC",
            ["IdKontrolnaTocka"] = TestData.KontrolnaTockaId.ToString(),
            ["VrijemeHodaMin"] = "120",
            ["DuljinaKm"] = "5.5",
            ["TezinaRute"] = nameof(TezinaRute.Srednja)
        };

        var response = await _planinarClient.PostAsync("/Ruta/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var created = db.Rute.SingleOrDefault(r => r.Naziv == "Testna ruta MVC");
        created.Should().NotBeNull();
        created!.JeOdobreno.Should().BeFalse();
    }

    [Fact]
    public async Task Create_Post_CreatesOdobrenuEntity_WhenAdmin()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, "/Ruta/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Admin testna ruta MVC",
            ["Pocetak"] = "Polaziste Admin",
            ["Kraj"] = "Odrediste Admin",
            ["IdKontrolnaTocka"] = TestData.KontrolnaTockaId.ToString(),
            ["VrijemeHodaMin"] = "90",
            ["DuljinaKm"] = "3.2",
            ["TezinaRute"] = nameof(TezinaRute.Laka)
        };

        var response = await _adminClient.PostAsync("/Ruta/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var created = db.Rute.Single(r => r.Naziv == "Admin testna ruta MVC");
        created.JeOdobreno.Should().BeTrue();
    }

    [Fact]
    public async Task Create_Post_ReturnsViewWithErrors_WhenInvalidKontrolnaTocka()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, "/Ruta/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Nevazeca KT ruta",
            ["Pocetak"] = "Polaziste",
            ["Kraj"] = "Odrediste",
            ["IdKontrolnaTocka"] = "99999",
            ["VrijemeHodaMin"] = "60",
            ["DuljinaKm"] = "2.0",
            ["TezinaRute"] = nameof(TezinaRute.Laka)
        };

        var response = await _adminClient.PostAsync("/Ruta/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ---- Edit ----

    [Fact]
    public async Task Edit_Get_Returns200_WhenCreatorPlaninar()
    {
        var id = await SeedNeodobrenuRutuAsync();

        var response = await _planinarClient.GetAsync($"/Ruta/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Get_Returns403_WhenNeodobrenaOdNekogDrugog()
    {
        var id = await SeedNeodobrenuRutuAsync();
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);

        var response = await foreignClient.GetAsync($"/Ruta/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Edit_Get_Returns200_WhenAdmin_NaNeodobrenoj()
    {
        var id = await SeedNeodobrenuRutuAsync();

        var response = await _adminClient.GetAsync($"/Ruta/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Post_UpdatesEntity_WhenAdmin_AndOdobrava()
    {
        var id = await SeedNeodobrenuRutuAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/Ruta/Edit/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Azurirana ruta MVC",
            ["Pocetak"] = "Polaziste",
            ["Kraj"] = "Odrediste",
            ["IdKontrolnaTocka"] = TestData.KontrolnaTockaId.ToString(),
            ["VrijemeHodaMin"] = "100",
            ["DuljinaKm"] = "4.0",
            ["TezinaRute"] = nameof(TezinaRute.Srednja)
        };

        var response = await _adminClient.PostAsync($"/Ruta/Edit/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var updated = db.Rute.Single(r => r.IdRuta == id);
        updated.Naziv.Should().Be("Azurirana ruta MVC");
        updated.JeOdobreno.Should().BeTrue();
    }

    [Fact]
    public async Task Edit_Post_Returns403_WhenNeodobrenaOdNekogDrugog()
    {
        var id = await SeedNeodobrenuRutuAsync();
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);
        var token = await AntiForgeryHelper.GetTokenAsync(foreignClient, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Hakirano",
            ["Pocetak"] = "Polaziste",
            ["Kraj"] = "Odrediste",
            ["IdKontrolnaTocka"] = TestData.KontrolnaTockaId.ToString(),
            ["VrijemeHodaMin"] = "100",
            ["DuljinaKm"] = "4.0",
            ["TezinaRute"] = nameof(TezinaRute.Srednja)
        };

        var response = await foreignClient.PostAsync($"/Ruta/Edit/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ---- Delete — Admin samo (soft delete) ----

    [Fact]
    public async Task Delete_Get_Returns403_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync($"/Ruta/Delete/{TestData.RutaId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_Post_SoftDeletesEntity_WhenAdmin()
    {
        var id = await SeedNeodobrenuRutuAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/Ruta/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _adminClient.PostAsync($"/Ruta/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var deleted = db.Rute.IgnoreQueryFilters().Single(r => r.IdRuta == id);
        deleted.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_Post_Returns403_WhenPlaninar()
    {
        var id = await SeedNeodobrenuRutuAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/Ruta/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _planinarClient.PostAsync($"/Ruta/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // Seeda neodobrenu rutu s vlasnistvom Planinara (IdKreator = PlaninarKorisnikId).
    private async Task<int> SeedNeodobrenuRutuAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new Ruta
        {
            IdKontrolnaTocka = TestData.KontrolnaTockaId,
            Naziv = $"Neodobrena ruta MVC test {Guid.NewGuid():N}",
            Pocetak = "Polaziste test",
            Kraj = "Odrediste test",
            VrijemeHodaMin = 60,
            DuljinaKm = 2.5m,
            TezinaRute = TezinaRute.Laka,
            JeOdobreno = false,
            IdKreator = TestData.PlaninarKorisnikId,
            DatumPrijave = DateTime.UtcNow
        };
        db.Rute.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdRuta;
    }
}
