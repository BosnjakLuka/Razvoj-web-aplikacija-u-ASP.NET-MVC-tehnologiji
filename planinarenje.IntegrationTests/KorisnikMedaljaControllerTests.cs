using System.Net;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.IntegrationTests.Helpers;

namespace planinarenje.IntegrationTests;

/// <summary>
/// Integracijski testovi za MVC KorisnikMedaljaController — po roli: guest, Planinar, Admin.
/// Index/Details = bilo koji autenticirani korisnik; Create/Award/Edit/Delete = Admin samo
/// (nema osobno vlasnistvo - dodjelu medalja upravlja isključivo Admin kroz eligibility stranicu).
/// KorisnikMedalja ima globalni query filter (DeletedAt == null) -> IgnoreQueryFilters().
/// </summary>
public class KorisnikMedaljaControllerTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _planinarClient = null!;
    private HttpClient _adminClient = null!;

    public KorisnikMedaljaControllerTests(CustomWebAppFactory factory)
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

    // ---- Index / Details — bilo koji autenticirani korisnik ----

    [Fact]
    public async Task Index_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.GetAsync("/KorisnikMedalja/Index");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Index_Returns200_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync("/KorisnikMedalja/Index");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Index_Returns200_WhenAdmin()
    {
        var response = await _adminClient.GetAsync("/KorisnikMedalja/Index");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns200_WhenExists_Planinar()
    {
        var response = await _planinarClient.GetAsync($"/KorisnikMedalja/Details/{TestData.KorisnikMedaljaId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns404_WhenMissing()
    {
        var response = await _adminClient.GetAsync("/KorisnikMedalja/Details/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---- Create / Award — Admin samo ----

    [Fact]
    public async Task Create_Get_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.GetAsync("/KorisnikMedalja/Create");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_Get_Returns403_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync("/KorisnikMedalja/Create");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_Get_Returns200_WhenAdmin()
    {
        var response = await _adminClient.GetAsync("/KorisnikMedalja/Create");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Award_Post_RedirectsToIndex_AndCreatesEntity_WhenAdmin_AndEligible()
    {
        // Nova medalja s niskim pragom — PlaninarKorisnikId vec ima dovoljno posjeta/podrucja
        // iz seed podataka da bude eligible, a jos nema OVU medalju.
        var medaljaId = await SeedMedaljaAsync(minKt: 1, minPodrucja: 1);
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, "/KorisnikMedalja/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["medaljaId"] = medaljaId.ToString(),
            ["korisnikId"] = TestData.PlaninarKorisnikId.ToString()
        };

        var response = await _adminClient.PostAsync("/KorisnikMedalja/Award", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var created = db.KorisnikMedalje.SingleOrDefault(km => km.IdKorisnik == TestData.PlaninarKorisnikId && km.IdMedalja == medaljaId);
        created.Should().NotBeNull();
    }

    [Fact]
    public async Task Award_Post_Returns403_WhenPlaninar()
    {
        var medaljaId = await SeedMedaljaAsync(minKt: 1, minPodrucja: 1);
        var token = await AntiForgeryHelper.GetTokenAsync(_planinarClient, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["medaljaId"] = medaljaId.ToString(),
            ["korisnikId"] = TestData.PlaninarKorisnikId.ToString()
        };

        var response = await _planinarClient.PostAsync("/KorisnikMedalja/Award", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Award_Post_RedirectsWithError_WhenKorisnikNijeEligible()
    {
        // Visok prag koji nitko od seediranih korisnika ne zadovoljava.
        var medaljaId = await SeedMedaljaAsync(minKt: 999, minPodrucja: 99);
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, "/KorisnikMedalja/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["medaljaId"] = medaljaId.ToString(),
            ["korisnikId"] = TestData.PlaninarKorisnikId.ToString()
        };

        var response = await _adminClient.PostAsync("/KorisnikMedalja/Award", new FormUrlEncodedContent(form));

        // Kontroler redirecta na Create s TempData["Error"], ne kreira zapis.
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var created = db.KorisnikMedalje.SingleOrDefault(km => km.IdKorisnik == TestData.PlaninarKorisnikId && km.IdMedalja == medaljaId);
        created.Should().BeNull();
    }

    // ---- Edit ----

    [Fact]
    public async Task Edit_Get_Returns403_WhenPlaninar()
    {
        var (id, _) = await SeedDodjelaAsync();

        var response = await _planinarClient.GetAsync($"/KorisnikMedalja/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Edit_Get_Returns200_WhenAdmin()
    {
        var (id, _) = await SeedDodjelaAsync();

        var response = await _adminClient.GetAsync($"/KorisnikMedalja/Edit/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Post_UpdatesEntity_WhenAdmin()
    {
        var (id, medaljaId) = await SeedDodjelaAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/KorisnikMedalja/Edit/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["IdKorisnik"] = TestData.PlaninarKorisnikId.ToString(),
            ["IdMedalja"] = medaljaId.ToString(),
            ["DatumDodjele"] = "2026-05-01T10:00",
            ["Napomena"] = "Azurirana napomena MVC testa."
        };

        var response = await _adminClient.PostAsync($"/KorisnikMedalja/Edit/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var updated = db.KorisnikMedalje.Single(km => km.IdKorisnikMedalja == id);
        updated.Napomena.Should().Be("Azurirana napomena MVC testa.");
    }

    [Fact]
    public async Task Edit_Post_Returns403_WhenPlaninar()
    {
        var (id, medaljaId) = await SeedDodjelaAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_planinarClient, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["IdKorisnik"] = TestData.PlaninarKorisnikId.ToString(),
            ["IdMedalja"] = medaljaId.ToString(),
            ["DatumDodjele"] = "2026-05-01T10:00",
            ["Napomena"] = "Hakirano."
        };

        var response = await _planinarClient.PostAsync($"/KorisnikMedalja/Edit/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ---- Delete — Admin samo (soft delete) ----

    [Fact]
    public async Task Delete_Get_Returns403_WhenPlaninar()
    {
        var (id, _) = await SeedDodjelaAsync();

        var response = await _planinarClient.GetAsync($"/KorisnikMedalja/Delete/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_Post_SoftDeletesEntity_WhenAdmin()
    {
        var (id, _) = await SeedDodjelaAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/KorisnikMedalja/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _adminClient.PostAsync($"/KorisnikMedalja/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var deleted = db.KorisnikMedalje.IgnoreQueryFilters().Single(km => km.IdKorisnikMedalja == id);
        deleted.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_Post_Returns403_WhenPlaninar()
    {
        var (id, _) = await SeedDodjelaAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/KorisnikMedalja/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _planinarClient.PostAsync($"/KorisnikMedalja/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<int> SeedMedaljaAsync(int minKt, int minPodrucja)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new Medalja
        {
            Naziv = $"Throwaway medalja MVC {Guid.NewGuid():N}",
            MinimalanBrojKontrolnihTocaka = minKt,
            MinimalanBrojPodrucja = minPodrucja
        };
        db.Medalje.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdMedalja;
    }

    // Seeda throwaway dodjelu medalje (nova Medalja + nova dodjela Planinaru) za Edit/Delete testove,
    // da se ne kosi s vec postojecom seediranom dodjelom (PlaninarKorisnikId + MedaljaId).
    // Vraca (IdKorisnikMedalja, IdMedalja) — IdMedalja treba caller da izbjegne duplikat-validaciju
    // kod Edit POST-a (PlaninarKorisnikId vec ima TestData.MedaljaId iz HasData seeda).
    private async Task<(int IdKorisnikMedalja, int IdMedalja)> SeedDodjelaAsync()
    {
        var medaljaId = await SeedMedaljaAsync(minKt: 1, minPodrucja: 1);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new KorisnikMedalja
        {
            IdKorisnik = TestData.PlaninarKorisnikId,
            IdMedalja = medaljaId,
            DatumDodjele = DateTime.UtcNow,
            Napomena = "Throwaway dodjela za MVC test."
        };
        db.KorisnikMedalje.Add(entity);
        await db.SaveChangesAsync();
        return (entity.IdKorisnikMedalja, medaljaId);
    }
}
