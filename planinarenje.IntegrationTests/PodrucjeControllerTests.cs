using System.Net;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.IntegrationTests.Helpers;

namespace planinarenje.IntegrationTests;

/// <summary>
/// Integracijski testovi za MVC PodrucjeController — po roli: guest, Planinar (vlasnik/strani), Admin.
/// Index/Details javni; Create/Edit dopusteni Adminu i Planinaru (Planinar-create ide na odobravanje);
/// Delete Admin samo. Podrucje ima globalni query filter (DeletedAt == null) -> IgnoreQueryFilters() za soft delete.
/// </summary>
public class PodrucjeControllerTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _planinarClient = null!;
    private HttpClient _adminClient = null!;

    public PodrucjeControllerTests(CustomWebAppFactory factory)
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
        var response = await _anonClient.GetAsync("/Podrucje/Index");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns200_WhenExists_Anonymous()
    {
        var response = await _anonClient.GetAsync($"/Podrucje/Details/{TestData.PodrucjeId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns404_WhenMissing()
    {
        var response = await _anonClient.GetAsync("/Podrucje/Details/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---- Create — Admin i Planinar ----

    [Fact]
    public async Task Create_Get_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.GetAsync("/Podrucje/Create");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_Get_Returns200_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync("/Podrucje/Create");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_Post_RedirectsToIndex_AndCreatesUnodobrenuEntity_WhenPlaninar()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_planinarClient, "/Podrucje/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Testno podrucje MVC",
            ["Regija"] = "Testna regija",
            ["MinimalanBrojKTZaObilazak"] = "1",
            ["UkupanBrojKT"] = "0"
        };

        var response = await _planinarClient.PostAsync("/Podrucje/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var created = db.Podrucja.SingleOrDefault(p => p.Naziv == "Testno podrucje MVC");
        created.Should().NotBeNull();
        created!.JeOdobreno.Should().BeFalse();
    }

    [Fact]
    public async Task Create_Post_CreatesOdobrenoEntity_WhenAdmin()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, "/Podrucje/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Admin testno podrucje MVC",
            ["Regija"] = "Admin regija",
            ["MinimalanBrojKTZaObilazak"] = "2",
            ["UkupanBrojKT"] = "0"
        };

        var response = await _adminClient.PostAsync("/Podrucje/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var created = db.Podrucja.Single(p => p.Naziv == "Admin testno podrucje MVC");
        created.JeOdobreno.Should().BeTrue();
    }

    [Fact]
    public async Task Create_Post_ReturnsViewWithErrors_WhenMinimalanBrojKtIzvanRange()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, "/Podrucje/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Nevazece podrucje",
            ["Regija"] = "Regija",
            ["MinimalanBrojKTZaObilazak"] = "0", // izvan dopustenog range (1-100)
            ["UkupanBrojKT"] = "0"
        };

        var response = await _adminClient.PostAsync("/Podrucje/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ---- Edit ----

    [Fact]
    public async Task Edit_Get_Returns200_WhenCreatorPlaninar()
    {
        var id = await SeedNeodobrenoPodrucjeAsync();

        var response = await _planinarClient.GetAsync($"/Podrucje/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Get_Returns403_WhenNeodobrenoOdNekogDrugog()
    {
        var id = await SeedNeodobrenoPodrucjeAsync();
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);

        var response = await foreignClient.GetAsync($"/Podrucje/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Edit_Get_Returns200_WhenAdmin_NaNeodobrenom()
    {
        var id = await SeedNeodobrenoPodrucjeAsync();

        var response = await _adminClient.GetAsync($"/Podrucje/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Post_UpdatesEntity_WhenAdmin_AndOdobrava()
    {
        var id = await SeedNeodobrenoPodrucjeAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/Podrucje/Edit/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Azurirano podrucje MVC",
            ["Regija"] = "Azurirana regija",
            ["MinimalanBrojKTZaObilazak"] = "3",
            ["UkupanBrojKT"] = "0"
        };

        var response = await _adminClient.PostAsync($"/Podrucje/Edit/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var updated = db.Podrucja.Single(p => p.IdPodrucje == id);
        updated.Naziv.Should().Be("Azurirano podrucje MVC");
        updated.JeOdobreno.Should().BeTrue();
    }

    [Fact]
    public async Task Edit_Post_Returns403_WhenNeodobrenoOdNekogDrugog()
    {
        var id = await SeedNeodobrenoPodrucjeAsync();
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);
        var token = await AntiForgeryHelper.GetTokenAsync(foreignClient, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Hakirano",
            ["Regija"] = "Regija",
            ["MinimalanBrojKTZaObilazak"] = "3",
            ["UkupanBrojKT"] = "0"
        };

        var response = await foreignClient.PostAsync($"/Podrucje/Edit/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ---- Delete — Admin samo (soft delete) ----

    [Fact]
    public async Task Delete_Get_Returns403_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync($"/Podrucje/Delete/{TestData.PodrucjeId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_Post_SoftDeletesEntity_WhenAdmin()
    {
        var id = await SeedNeodobrenoPodrucjeAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/Podrucje/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _adminClient.PostAsync($"/Podrucje/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var deleted = db.Podrucja.IgnoreQueryFilters().Single(p => p.IdPodrucje == id);
        deleted.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_Post_Returns403_WhenPlaninar()
    {
        var id = await SeedNeodobrenoPodrucjeAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/Podrucje/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _planinarClient.PostAsync($"/Podrucje/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // Seeda neodobreno podrucje s vlasnistvom Planinara (IdKreator = PlaninarKorisnikId).
    private async Task<int> SeedNeodobrenoPodrucjeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new Podrucje
        {
            Naziv = $"Neodobreno podrucje MVC test {Guid.NewGuid():N}",
            Regija = "Test regija",
            MinimalanBrojKTZaObilazak = 1,
            JeOdobreno = false,
            IdKreator = TestData.PlaninarKorisnikId,
            DatumPrijave = DateTime.UtcNow
        };
        db.Podrucja.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdPodrucje;
    }
}
