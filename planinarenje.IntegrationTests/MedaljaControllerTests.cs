using System.Net;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.IntegrationTests.Helpers;

namespace planinarenje.IntegrationTests;

/// <summary>
/// Integracijski testovi za MVC MedaljaController — po roli: guest, Planinar, Admin.
/// Index/Details javni; Create/Edit/Delete Admin samo (nema vlasnistvo/odobravanje kao kod
/// kataloznih entiteta s prijavom). Medalja ima globalni query filter (DeletedAt == null).
/// </summary>
public class MedaljaControllerTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _planinarClient = null!;
    private HttpClient _adminClient = null!;

    public MedaljaControllerTests(CustomWebAppFactory factory)
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
        var response = await _anonClient.GetAsync("/Medalja/Index");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns200_WhenExists_Anonymous()
    {
        var response = await _anonClient.GetAsync($"/Medalja/Details/{TestData.MedaljaId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns404_WhenMissing()
    {
        var response = await _anonClient.GetAsync("/Medalja/Details/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---- Create — Admin samo ----

    [Fact]
    public async Task Create_Get_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.GetAsync("/Medalja/Create");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_Get_Returns403_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync("/Medalja/Create");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_Get_Returns200_WhenAdmin()
    {
        var response = await _adminClient.GetAsync("/Medalja/Create");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_Post_RedirectsToIndex_AndCreatesEntity_WhenAdmin()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, "/Medalja/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Testna medalja MVC",
            ["MinimalanBrojKontrolnihTocaka"] = "5",
            ["MinimalanBrojPodrucja"] = "2"
        };

        var response = await _adminClient.PostAsync("/Medalja/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var created = db.Medalje.SingleOrDefault(m => m.Naziv == "Testna medalja MVC");
        created.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_Post_Returns403_WhenPlaninar()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_planinarClient, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Hakirana medalja",
            ["MinimalanBrojKontrolnihTocaka"] = "5",
            ["MinimalanBrojPodrucja"] = "2"
        };

        var response = await _planinarClient.PostAsync("/Medalja/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_Post_ReturnsViewWithErrors_WhenDuplicate()
    {
        using var scope0 = _factory.Services.CreateScope();
        var db0 = scope0.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var postojeca = db0.Medalje.First();

        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, "/Medalja/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = postojeca.Naziv,
            ["MinimalanBrojKontrolnihTocaka"] = postojeca.MinimalanBrojKontrolnihTocaka.ToString(),
            ["MinimalanBrojPodrucja"] = postojeca.MinimalanBrojPodrucja.ToString()
        };

        var response = await _adminClient.PostAsync("/Medalja/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ---- Edit ----

    [Fact]
    public async Task Edit_Get_Returns403_WhenPlaninar()
    {
        var id = await SeedMedaljaAsync();

        var response = await _planinarClient.GetAsync($"/Medalja/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Edit_Get_Returns200_WhenAdmin()
    {
        var id = await SeedMedaljaAsync();

        var response = await _adminClient.GetAsync($"/Medalja/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Post_UpdatesEntity_WhenAdmin()
    {
        var id = await SeedMedaljaAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/Medalja/Edit/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Azurirana medalja MVC",
            ["MinimalanBrojKontrolnihTocaka"] = "7",
            ["MinimalanBrojPodrucja"] = "3"
        };

        var response = await _adminClient.PostAsync($"/Medalja/Edit/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var updated = db.Medalje.Single(m => m.IdMedalja == id);
        updated.Naziv.Should().Be("Azurirana medalja MVC");
    }

    [Fact]
    public async Task Edit_Post_Returns403_WhenPlaninar()
    {
        var id = await SeedMedaljaAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_planinarClient, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Naziv"] = "Hakirano",
            ["MinimalanBrojKontrolnihTocaka"] = "7",
            ["MinimalanBrojPodrucja"] = "3"
        };

        var response = await _planinarClient.PostAsync($"/Medalja/Edit/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ---- Delete — Admin samo (soft delete) ----

    [Fact]
    public async Task Delete_Get_Returns403_WhenPlaninar()
    {
        var id = await SeedMedaljaAsync();

        var response = await _planinarClient.GetAsync($"/Medalja/Delete/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_Post_SoftDeletesEntity_WhenAdmin()
    {
        var id = await SeedMedaljaAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/Medalja/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _adminClient.PostAsync($"/Medalja/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var deleted = db.Medalje.IgnoreQueryFilters().Single(m => m.IdMedalja == id);
        deleted.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_Post_Returns403_WhenPlaninar()
    {
        var id = await SeedMedaljaAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/Medalja/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _planinarClient.PostAsync($"/Medalja/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<int> SeedMedaljaAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new Medalja
        {
            Naziv = $"Throwaway medalja MVC {Guid.NewGuid():N}",
            MinimalanBrojKontrolnihTocaka = 1,
            MinimalanBrojPodrucja = 1
        };
        db.Medalje.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdMedalja;
    }
}
