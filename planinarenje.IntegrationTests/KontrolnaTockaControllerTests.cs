using System.Net;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.IntegrationTests.Helpers;

namespace planinarenje.IntegrationTests;

/// <summary>
/// Integracijski testovi za MVC KontrolnaTockaController — po roli: guest, Planinar (vlasnik/strani), Admin.
/// Index/Details su javni; Create/Edit dopusteni Adminu i Planinaru (Planinar-create ide na
/// odobravanje: JeOdobreno=false); Delete je Admin samo. KontrolnaTocka ima globalni query filter
/// (DeletedAt == null), pa se soft delete provjerava s IgnoreQueryFilters().
/// </summary>
public class KontrolnaTockaControllerTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _planinarClient = null!;
    private HttpClient _adminClient = null!;

    public KontrolnaTockaControllerTests(CustomWebAppFactory factory)
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
        var response = await _anonClient.GetAsync("/KontrolnaTocka/Index");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns200_WhenExists_Anonymous()
    {
        var response = await _anonClient.GetAsync($"/KontrolnaTocka/Details/{TestData.KontrolnaTockaId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns404_WhenMissing()
    {
        var response = await _anonClient.GetAsync("/KontrolnaTocka/Details/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---- Create — Admin i Planinar ----

    [Fact]
    public async Task Create_Get_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.GetAsync("/KontrolnaTocka/Create");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_Get_Returns200_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync("/KontrolnaTocka/Create");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_Post_RedirectsToIndex_AndCreatesUnodobrenuEntity_WhenPlaninar()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_planinarClient, "/KontrolnaTocka/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Testni vrh MVC",
            ["GUIDOznaka"] = "MVC-TEST-GUID-001",
            ["IdPodrucje"] = TestData.PodrucjeId.ToString(),
            ["TipKontrolneTocke"] = nameof(TipKontrolneTocke.Vrh)
        };

        var response = await _planinarClient.PostAsync("/KontrolnaTocka/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var created = db.KontrolneTocke.SingleOrDefault(k => k.GUIDOznaka == "MVC-TEST-GUID-001");
        created.Should().NotBeNull();
        created!.JeOdobreno.Should().BeFalse();
    }

    [Fact]
    public async Task Create_Post_CreatesOdobrenuEntity_WhenAdmin()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, "/KontrolnaTocka/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Admin testni vrh MVC",
            ["GUIDOznaka"] = "MVC-TEST-GUID-ADMIN",
            ["IdPodrucje"] = TestData.PodrucjeId.ToString(),
            ["TipKontrolneTocke"] = nameof(TipKontrolneTocke.Vrh)
        };

        var response = await _adminClient.PostAsync("/KontrolnaTocka/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var created = db.KontrolneTocke.Single(k => k.GUIDOznaka == "MVC-TEST-GUID-ADMIN");
        created.JeOdobreno.Should().BeTrue();
    }

    [Fact]
    public async Task Create_Post_ReturnsViewWithErrors_WhenGuidDuplicate()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, "/KontrolnaTocka/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Duplikat GUID test",
            ["GUIDOznaka"] = "KT-HPO-2-1-VIS", // već postoji na seediranoj KT 1
            ["IdPodrucje"] = TestData.PodrucjeId.ToString(),
            ["TipKontrolneTocke"] = nameof(TipKontrolneTocke.Vrh)
        };

        var response = await _adminClient.PostAsync("/KontrolnaTocka/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ---- Edit ----

    [Fact]
    public async Task Edit_Get_Returns200_WhenCreatorPlaninar()
    {
        var id = await SeedNeodobrenuKontrolnuTockuAsync();

        var response = await _planinarClient.GetAsync($"/KontrolnaTocka/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Get_Returns403_WhenNeodobrenaOdNekogDrugog()
    {
        var id = await SeedNeodobrenuKontrolnuTockuAsync();
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);

        var response = await foreignClient.GetAsync($"/KontrolnaTocka/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Edit_Get_Returns200_WhenAdmin_NaNeodobrenoj()
    {
        var id = await SeedNeodobrenuKontrolnuTockuAsync();

        var response = await _adminClient.GetAsync($"/KontrolnaTocka/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Post_UpdatesEntity_WhenAdmin_AndOdobrava()
    {
        var id = await SeedNeodobrenuKontrolnuTockuAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/KontrolnaTocka/Edit/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Azurirani naziv MVC",
            ["GUIDOznaka"] = "MVC-NEODOBRENA-001",
            ["IdPodrucje"] = TestData.PodrucjeId.ToString(),
            ["TipKontrolneTocke"] = nameof(TipKontrolneTocke.Vrh)
        };

        var response = await _adminClient.PostAsync($"/KontrolnaTocka/Edit/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var updated = db.KontrolneTocke.Single(k => k.IdKontrolnaTocka == id);
        updated.Naziv.Should().Be("Azurirani naziv MVC");
        updated.JeOdobreno.Should().BeTrue();
    }

    [Fact]
    public async Task Edit_Post_Returns403_WhenNeodobrenaOdNekogDrugog()
    {
        var id = await SeedNeodobrenuKontrolnuTockuAsync();
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);
        var token = await AntiForgeryHelper.GetTokenAsync(foreignClient, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Hakirano",
            ["GUIDOznaka"] = "MVC-NEODOBRENA-001",
            ["IdPodrucje"] = TestData.PodrucjeId.ToString(),
            ["TipKontrolneTocke"] = nameof(TipKontrolneTocke.Vrh)
        };

        var response = await foreignClient.PostAsync($"/KontrolnaTocka/Edit/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ---- Delete — Admin samo (soft delete) ----

    [Fact]
    public async Task Delete_Get_Returns403_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync($"/KontrolnaTocka/Delete/{TestData.KontrolnaTockaId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_Post_SoftDeletesEntity_WhenAdmin()
    {
        var id = await SeedNeodobrenuKontrolnuTockuAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/KontrolnaTocka/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _adminClient.PostAsync($"/KontrolnaTocka/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var deleted = db.KontrolneTocke.IgnoreQueryFilters().Single(k => k.IdKontrolnaTocka == id);
        deleted.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_Post_Returns403_WhenPlaninar()
    {
        var id = await SeedNeodobrenuKontrolnuTockuAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/KontrolnaTocka/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _planinarClient.PostAsync($"/KontrolnaTocka/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // Seeda neodobrenu KT s vlasnistvom Planinara (IdKreator = PlaninarKorisnikId).
    private async Task<int> SeedNeodobrenuKontrolnuTockuAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new KontrolnaTocka
        {
            Naziv = "Neodobrena KT MVC test",
            GUIDOznaka = $"MVC-NEODOBRENA-{Guid.NewGuid():N}".Substring(0, 30),
            IdPodrucje = TestData.PodrucjeId,
            TipKontrolneTocke = TipKontrolneTocke.Vrh,
            JeOdobreno = false,
            IdKreator = TestData.PlaninarKorisnikId,
            DatumPrijave = DateTime.UtcNow
        };
        db.KontrolneTocke.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdKontrolnaTocka;
    }
}
