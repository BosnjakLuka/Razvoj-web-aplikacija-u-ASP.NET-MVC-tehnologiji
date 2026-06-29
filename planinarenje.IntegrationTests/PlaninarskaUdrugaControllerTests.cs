using System.Net;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.IntegrationTests.Helpers;

namespace planinarenje.IntegrationTests;

/// <summary>
/// Integracijski testovi za MVC PlaninarskaUdrugaController — po roli: guest, Planinar (vlasnik/strani), Admin.
/// Index/Details javni; Create/Edit dopusteni Adminu i Planinaru (Planinar-create ide na odobravanje);
/// Delete Admin samo. PlaninarskaUdruga ima globalni query filter (DeletedAt == null) -> IgnoreQueryFilters().
/// </summary>
public class PlaninarskaUdrugaControllerTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _planinarClient = null!;
    private HttpClient _adminClient = null!;

    public PlaninarskaUdrugaControllerTests(CustomWebAppFactory factory)
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
        var response = await _anonClient.GetAsync("/PlaninarskaUdruga/Index");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns200_WhenExists_Anonymous()
    {
        var response = await _anonClient.GetAsync($"/PlaninarskaUdruga/Details/{TestData.PlaninarskaUdrugaId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns404_WhenMissing()
    {
        var response = await _anonClient.GetAsync("/PlaninarskaUdruga/Details/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---- Create — Admin i Planinar ----

    [Fact]
    public async Task Create_Get_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.GetAsync("/PlaninarskaUdruga/Create");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_Get_Returns200_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync("/PlaninarskaUdruga/Create");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_Post_RedirectsToIndex_AndCreatesUnodobrenuEntity_WhenPlaninar()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_planinarClient, "/PlaninarskaUdruga/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["OIB"] = "11111111111",
            ["Naziv"] = "Testna udruga MVC"
        };

        var response = await _planinarClient.PostAsync("/PlaninarskaUdruga/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var created = db.PlaninarskeUdruge.SingleOrDefault(u => u.OIB == "11111111111");
        created.Should().NotBeNull();
        created!.JeOdobreno.Should().BeFalse();
    }

    [Fact]
    public async Task Create_Post_CreatesOdobrenuEntity_WhenAdmin()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, "/PlaninarskaUdruga/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["OIB"] = "22222222222",
            ["Naziv"] = "Admin testna udruga MVC"
        };

        var response = await _adminClient.PostAsync("/PlaninarskaUdruga/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var created = db.PlaninarskeUdruge.Single(u => u.OIB == "22222222222");
        created.JeOdobreno.Should().BeTrue();
    }

    [Fact]
    public async Task Create_Post_ReturnsViewWithErrors_WhenOibDuplicate()
    {
        using var scope0 = _factory.Services.CreateScope();
        var db0 = scope0.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var postojeciOib = db0.PlaninarskeUdruge.First().OIB;

        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, "/PlaninarskaUdruga/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["OIB"] = postojeciOib,
            ["Naziv"] = "Duplikat OIB test"
        };

        var response = await _adminClient.PostAsync("/PlaninarskaUdruga/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ---- Edit ----

    [Fact]
    public async Task Edit_Get_Returns200_WhenCreatorPlaninar()
    {
        var id = await SeedNeodobrenuUdruguAsync();

        var response = await _planinarClient.GetAsync($"/PlaninarskaUdruga/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Get_Returns403_WhenNeodobrenaOdNekogDrugog()
    {
        var id = await SeedNeodobrenuUdruguAsync();
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);

        var response = await foreignClient.GetAsync($"/PlaninarskaUdruga/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Edit_Get_Returns200_WhenAdmin_NaNeodobrenoj()
    {
        var id = await SeedNeodobrenuUdruguAsync();

        var response = await _adminClient.GetAsync($"/PlaninarskaUdruga/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Post_UpdatesEntity_WhenAdmin_AndOdobrava()
    {
        var id = await SeedNeodobrenuUdruguAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/PlaninarskaUdruga/Edit/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["OIB"] = "33333333333",
            ["Naziv"] = "Azurirana udruga MVC"
        };

        var response = await _adminClient.PostAsync($"/PlaninarskaUdruga/Edit/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var updated = db.PlaninarskeUdruge.Single(u => u.IdPlaninarskaUdruga == id);
        updated.Naziv.Should().Be("Azurirana udruga MVC");
        updated.JeOdobreno.Should().BeTrue();
    }

    [Fact]
    public async Task Edit_Post_Returns403_WhenNeodobrenaOdNekogDrugog()
    {
        var id = await SeedNeodobrenuUdruguAsync();
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);
        var token = await AntiForgeryHelper.GetTokenAsync(foreignClient, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["OIB"] = "44444444444",
            ["Naziv"] = "Hakirano"
        };

        var response = await foreignClient.PostAsync($"/PlaninarskaUdruga/Edit/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ---- Delete — Admin samo (soft delete) ----

    [Fact]
    public async Task Delete_Get_Returns403_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync($"/PlaninarskaUdruga/Delete/{TestData.PlaninarskaUdrugaId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_Post_SoftDeletesEntity_WhenAdmin()
    {
        var id = await SeedNeodobrenuUdruguAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/PlaninarskaUdruga/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _adminClient.PostAsync($"/PlaninarskaUdruga/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var deleted = db.PlaninarskeUdruge.IgnoreQueryFilters().Single(u => u.IdPlaninarskaUdruga == id);
        deleted.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_Post_Returns403_WhenPlaninar()
    {
        var id = await SeedNeodobrenuUdruguAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/PlaninarskaUdruga/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _planinarClient.PostAsync($"/PlaninarskaUdruga/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // Seeda neodobrenu udrugu s vlasnistvom Planinara (IdKreator = PlaninarKorisnikId).
    private async Task<int> SeedNeodobrenuUdruguAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new PlaninarskaUdruga
        {
            OIB = Guid.NewGuid().ToString("N")[..11],
            Naziv = $"Neodobrena udruga MVC test {Guid.NewGuid():N}",
            JeOdobreno = false,
            IdKreator = TestData.PlaninarKorisnikId,
            DatumPrijave = DateTime.UtcNow
        };
        db.PlaninarskeUdruge.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdPlaninarskaUdruga;
    }
}
