using System.Net;
using planinarenje.Data;
using planinarenje.IntegrationTests.Helpers;

namespace planinarenje.IntegrationTests;

/// <summary>
/// Integracijski testovi za MVC KorisnikController — CRUD kroz HTML forme,
/// po roli: guest, Planinar (vlasnik/strani), Admin.
/// Pravila: Index = Admin/Planinar; Create/Delete = Admin samo; Edit = vlasnik ili Admin;
/// Details = vlasnik, Admin ili bilo koji Planinar (vidi KorisnikController.Details).
/// </summary>
public class KorisnikControllerTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _planinarClient = null!;
    private HttpClient _adminClient = null!;

    public KorisnikControllerTests(CustomWebAppFactory factory)
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

    // ---- Index — Admin/Planinar samo ----

    [Fact]
    public async Task Index_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.GetAsync("/Korisnik/Index");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Index_Returns200_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync("/Korisnik/Index");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Index_Returns200_WhenAdmin()
    {
        var response = await _adminClient.GetAsync("/Korisnik/Index");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ---- Create — Admin samo ----

    [Fact]
    public async Task Create_Get_Returns403_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync("/Korisnik/Create");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_Get_Returns200_WhenAdmin()
    {
        var response = await _adminClient.GetAsync("/Korisnik/Create");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_Post_RedirectsToIndex_AndCreatesEntity_WhenAdmin()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, "/Korisnik/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Ime"] = "Novi",
            ["Prezime"] = "Korisnik",
            ["Email"] = "novi.korisnik.mvc@test.hr",
            ["KorisnickoIme"] = "novi_korisnik_mvc"
        };

        var response = await _adminClient.PostAsync("/Korisnik/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var created = db.Korisnici.SingleOrDefault(k => k.Email == "novi.korisnik.mvc@test.hr");
        created.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_Post_ReturnsViewWithErrors_WhenEmailDuplicate()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, "/Korisnik/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Ime"] = "Duplikat",
            ["Prezime"] = "Test",
            ["Email"] = "luka.bosnjak92@gmail.com", // već postoji (seedirani Korisnik 1)
            ["KorisnickoIme"] = "duplikat_test"
        };

        var response = await _adminClient.PostAsync("/Korisnik/Create", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ---- Edit — vlasnik ili Admin ----

    [Fact]
    public async Task Edit_Get_Returns200_WhenOwnerPlaninar()
    {
        var response = await _planinarClient.GetAsync($"/Korisnik/Edit/{TestData.PlaninarKorisnikId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Get_Returns403_WhenForeignPlaninar()
    {
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);

        var response = await foreignClient.GetAsync($"/Korisnik/Edit/{TestData.PlaninarKorisnikId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Edit_Get_Returns200_WhenAdmin_NotOwner()
    {
        var response = await _adminClient.GetAsync($"/Korisnik/Edit/{TestData.PlaninarKorisnikId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Post_UpdatesEntity_WhenOwnerPlaninar()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_planinarClient, $"/Korisnik/Edit/{TestData.PlaninarKorisnikId}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Ime"] = "Luka",
            ["Prezime"] = "Bošnjak",
            ["Email"] = "luka.bosnjak92@gmail.com",
            ["KorisnickoIme"] = "Boss-Azurirano"
        };

        var response = await _planinarClient.PostAsync($"/Korisnik/Edit/{TestData.PlaninarKorisnikId}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var updated = db.Korisnici.Single(k => k.IdKorisnik == TestData.PlaninarKorisnikId);
        updated.KorisnickoIme.Should().Be("Boss-Azurirano");
    }

    [Fact]
    public async Task Edit_Post_Returns403_WhenForeignPlaninar()
    {
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);
        // Korisnik/Details nema formu (read-only); /Posjet/Create je [Authorize] bez
        // provjere vlasništva na GET-u, pa je dovoljan kao izvor antiforgery tokena
        // za bilo kojeg autenticiranog korisnika (token nije vezan na akciju/kontroler).
        var token = await AntiForgeryHelper.GetTokenAsync(foreignClient, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Ime"] = "Luka",
            ["Prezime"] = "Bošnjak",
            ["Email"] = "luka.bosnjak92@gmail.com",
            ["KorisnickoIme"] = "Hakirano"
        };

        var response = await foreignClient.PostAsync($"/Korisnik/Edit/{TestData.PlaninarKorisnikId}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ---- Delete — Admin samo (soft delete: StatusAktivan = false) ----

    [Fact]
    public async Task Delete_Get_Returns403_WhenOwnerPlaninar()
    {
        var response = await _planinarClient.GetAsync($"/Korisnik/Delete/{TestData.PlaninarKorisnikId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_Get_Returns200_WhenAdmin()
    {
        var id = await SeedKorisnikAsync();

        var response = await _adminClient.GetAsync($"/Korisnik/Delete/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_Post_SoftDeletesEntity_WhenAdmin()
    {
        var id = await SeedKorisnikAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/Korisnik/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _adminClient.PostAsync($"/Korisnik/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var deleted = db.Korisnici.Single(k => k.IdKorisnik == id);
        deleted.StatusAktivan.Should().BeFalse();
    }

    [Fact]
    public async Task Delete_Post_Returns403_WhenPlaninar()
    {
        var id = await SeedKorisnikAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/Korisnik/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _planinarClient.PostAsync($"/Korisnik/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ---- Details — vlasnik, Admin, ili bilo koji autenticirani Planinar ----

    [Fact]
    public async Task Details_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.GetAsync($"/Korisnik/Details/{TestData.PlaninarKorisnikId}");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Details_Returns200_WhenForeignPlaninar()
    {
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);

        var response = await foreignClient.GetAsync($"/Korisnik/Details/{TestData.PlaninarKorisnikId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns200_WhenAdmin()
    {
        var response = await _adminClient.GetAsync($"/Korisnik/Details/{TestData.PlaninarKorisnikId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns404_WhenMissing()
    {
        var response = await _adminClient.GetAsync("/Korisnik/Details/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<int> SeedKorisnikAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new planinarenje.Entiteti.Korisnik
        {
            Ime = "Throwaway",
            Prezime = "Test",
            Email = $"throwaway-{Guid.NewGuid()}@test.hr",
            KorisnickoIme = $"throwaway_{Guid.NewGuid():N}",
            DatumRegistracije = DateTime.UtcNow,
            StatusAktivan = true
        };
        db.Korisnici.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdKorisnik;
    }
}
