using System.Net;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.IntegrationTests.Helpers;

namespace planinarenje.IntegrationTests;

/// <summary>
/// Integracijski testovi za MVC PlaninarskiObjektController — po roli: guest, Planinar (vlasnik/strani), Admin.
/// Index/Details javni; Create/Edit dopusteni Adminu i Planinaru (Planinar-create ide na odobravanje);
/// Delete Admin samo. PlaninarskiObjekt ima globalni query filter (DeletedAt == null) -> IgnoreQueryFilters().
/// </summary>
public class PlaninarskiObjektControllerTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _planinarClient = null!;
    private HttpClient _adminClient = null!;

    public PlaninarskiObjektControllerTests(CustomWebAppFactory factory)
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
        var response = await _anonClient.GetAsync("/PlaninarskiObjekt/Index");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns200_WhenExists_Anonymous()
    {
        var response = await _anonClient.GetAsync($"/PlaninarskiObjekt/Details/{TestData.PlaninarskiObjektId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns404_WhenMissing()
    {
        var response = await _anonClient.GetAsync("/PlaninarskiObjekt/Details/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---- Create — Admin i Planinar ----

    [Fact]
    public async Task Create_Get_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.GetAsync("/PlaninarskiObjekt/Create");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_Get_Returns200_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync("/PlaninarskiObjekt/Create");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_Post_RedirectsToIndex_AndCreatesUnodobrenuEntity_WhenPlaninar()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_planinarClient, "/PlaninarskiObjekt/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Testni objekt MVC",
            ["TipObjekta"] = nameof(TipObjekta.Skloniste),
            ["IdPodrucje"] = TestData.PodrucjeId.ToString(),
            ["IdPlaninarskaUdruga"] = TestData.PlaninarskaUdrugaId.ToString()
        };

        var response = await _planinarClient.PostAsync("/PlaninarskiObjekt/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var created = db.PlaninarskiObjekti.SingleOrDefault(po => po.Naziv == "Testni objekt MVC");
        created.Should().NotBeNull();
        created!.JeOdobreno.Should().BeFalse();
    }

    [Fact]
    public async Task Create_Post_CreatesOdobrenuEntity_WhenAdmin()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, "/PlaninarskiObjekt/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Admin testni objekt MVC",
            ["TipObjekta"] = nameof(TipObjekta.Dom),
            ["IdPodrucje"] = TestData.PodrucjeId.ToString(),
            ["IdPlaninarskaUdruga"] = TestData.PlaninarskaUdrugaId.ToString()
        };

        var response = await _adminClient.PostAsync("/PlaninarskiObjekt/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var created = db.PlaninarskiObjekti.Single(po => po.Naziv == "Admin testni objekt MVC");
        created.JeOdobreno.Should().BeTrue();
    }

    [Fact]
    public async Task Create_Post_ReturnsViewWithErrors_WhenUdrugaNevazeca()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, "/PlaninarskiObjekt/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Objekt s nevazecom udrugom",
            ["TipObjekta"] = nameof(TipObjekta.Dom),
            ["IdPodrucje"] = TestData.PodrucjeId.ToString(),
            ["IdPlaninarskaUdruga"] = "99999"
        };

        var response = await _adminClient.PostAsync("/PlaninarskiObjekt/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ---- Edit ----

    [Fact]
    public async Task Edit_Get_Returns200_WhenCreatorPlaninar()
    {
        var id = await SeedNeodobrenObjektAsync();

        var response = await _planinarClient.GetAsync($"/PlaninarskiObjekt/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Get_Returns403_WhenNeodobrenOdNekogDrugog()
    {
        var id = await SeedNeodobrenObjektAsync();
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);

        var response = await foreignClient.GetAsync($"/PlaninarskiObjekt/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Edit_Get_Returns200_WhenAdmin_NaNeodobrenom()
    {
        var id = await SeedNeodobrenObjektAsync();

        var response = await _adminClient.GetAsync($"/PlaninarskiObjekt/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Post_UpdatesEntity_WhenAdmin_AndOdobrava()
    {
        var id = await SeedNeodobrenObjektAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/PlaninarskiObjekt/Edit/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Azurirani objekt MVC",
            ["TipObjekta"] = nameof(TipObjekta.Kuca),
            ["IdPodrucje"] = TestData.PodrucjeId.ToString(),
            ["IdPlaninarskaUdruga"] = TestData.PlaninarskaUdrugaId.ToString()
        };

        var response = await _adminClient.PostAsync($"/PlaninarskiObjekt/Edit/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var updated = db.PlaninarskiObjekti.Single(po => po.IdPlaninarskiObjekt == id);
        updated.Naziv.Should().Be("Azurirani objekt MVC");
        updated.JeOdobreno.Should().BeTrue();
    }

    [Fact]
    public async Task Edit_Post_Returns403_WhenNeodobrenOdNekogDrugog()
    {
        var id = await SeedNeodobrenObjektAsync();
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);
        var token = await AntiForgeryHelper.GetTokenAsync(foreignClient, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Hakirano",
            ["TipObjekta"] = nameof(TipObjekta.Kuca),
            ["IdPodrucje"] = TestData.PodrucjeId.ToString(),
            ["IdPlaninarskaUdruga"] = TestData.PlaninarskaUdrugaId.ToString()
        };

        var response = await foreignClient.PostAsync($"/PlaninarskiObjekt/Edit/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ---- Delete — Admin samo (soft delete) ----

    [Fact]
    public async Task Delete_Get_Returns403_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync($"/PlaninarskiObjekt/Delete/{TestData.PlaninarskiObjektId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_Post_SoftDeletesEntity_WhenAdmin()
    {
        var id = await SeedNeodobrenObjektAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/PlaninarskiObjekt/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _adminClient.PostAsync($"/PlaninarskiObjekt/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var deleted = db.PlaninarskiObjekti.IgnoreQueryFilters().Single(po => po.IdPlaninarskiObjekt == id);
        deleted.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_Post_Returns403_WhenPlaninar()
    {
        var id = await SeedNeodobrenObjektAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/PlaninarskiObjekt/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _planinarClient.PostAsync($"/PlaninarskiObjekt/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // Seeda neodobren objekt s vlasnistvom Planinara (IdKreator = PlaninarKorisnikId).
    private async Task<int> SeedNeodobrenObjektAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new PlaninarskiObjekt
        {
            Naziv = $"Neodobren objekt MVC test {Guid.NewGuid():N}",
            TipObjekta = TipObjekta.Skloniste,
            IdPodrucje = TestData.PodrucjeId,
            IdPlaninarskaUdruga = TestData.PlaninarskaUdrugaId,
            JeOdobreno = false,
            IdKreator = TestData.PlaninarKorisnikId,
            DatumPrijave = DateTime.UtcNow
        };
        db.PlaninarskiObjekti.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdPlaninarskiObjekt;
    }
}
