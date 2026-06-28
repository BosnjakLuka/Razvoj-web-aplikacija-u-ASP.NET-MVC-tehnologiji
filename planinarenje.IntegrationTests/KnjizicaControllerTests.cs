using System.Net;
using planinarenje.Data;
using planinarenje.IntegrationTests.Helpers;

namespace planinarenje.IntegrationTests;

/// <summary>
/// Integracijski testovi za MVC KnjizicaController — po roli: guest, Planinar (vlasnik/strani), Admin.
/// Knjizica nema standardni Create POST; admin dodjeljuje (Dodijeli), a bilo koji autenticirani
/// korisnik smije sam sebi kreirati knjižicu (KreirajVlastitu) ako je još nema.
/// </summary>
public class KnjizicaControllerTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private HttpClient _anonClient = null!;
    private HttpClient _planinarClient = null!;
    private HttpClient _adminClient = null!;

    public KnjizicaControllerTests(CustomWebAppFactory factory)
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

    // ---- Index ----

    [Fact]
    public async Task Index_Returns401_WhenAnonymous()
    {
        var response = await _anonClient.GetAsync("/Knjizica/Index");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Index_Returns200_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync("/Knjizica/Index");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Index_Returns200_WhenAdmin()
    {
        var response = await _adminClient.GetAsync("/Knjizica/Index");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ---- Create (Admin) / Dodijeli ----

    [Fact]
    public async Task Create_Get_Returns403_WhenPlaninar()
    {
        var response = await _planinarClient.GetAsync("/Knjizica/Create");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_Get_Returns200_WhenAdmin()
    {
        var response = await _adminClient.GetAsync("/Knjizica/Create");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Dodijeli_Post_RedirectsToIndex_AndCreatesEntity_WhenAdmin()
    {
        var korisnikId = await SeedKorisnikBezKnjiziceAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, "/Knjizica/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["korisnikId"] = korisnikId.ToString()
        };

        var response = await _adminClient.PostAsync("/Knjizica/Dodijeli", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var created = db.Knjizice.SingleOrDefault(k => k.IdKorisnik == korisnikId && k.StatusAktivna);
        created.Should().NotBeNull();
    }

    [Fact]
    public async Task Dodijeli_Post_Returns403_WhenPlaninar()
    {
        var korisnikId = await SeedKorisnikBezKnjiziceAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_planinarClient, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["korisnikId"] = korisnikId.ToString()
        };

        var response = await _planinarClient.PostAsync("/Knjizica/Dodijeli", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task KreirajVlastitu_Post_RedirectsToIndex_AndCreatesEntity_WhenKorisnikBezKnjizice()
    {
        // Svjez Korisnik bez knjizice, povezan s novim testnim identitetom da simuliramo
        // prijavljenog korisnika koji sam sebi kreira knjižicu.
        var appUserId = $"test-bezknjizice-{Guid.NewGuid():N}";
        var korisnikId = await SeedKorisnikBezKnjiziceAsync(appUserId);

        using var client = AuthHelper.CreateClient(_factory, appUserId, "Planinar");
        var token = await AntiForgeryHelper.GetTokenAsync(client, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await client.PostAsync("/Knjizica/KreirajVlastitu", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var created = verifyDb.Knjizice.SingleOrDefault(k => k.IdKorisnik == korisnikId && k.StatusAktivna);
        created.Should().NotBeNull();
    }

    [Fact]
    public async Task KreirajVlastitu_Post_Returns403_WhenForeignPlaninarBezKorisnickogProfila()
    {
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);
        var token = await AntiForgeryHelper.GetTokenAsync(foreignClient, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await foreignClient.PostAsync("/Knjizica/KreirajVlastitu", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ---- Edit ----

    [Fact]
    public async Task Edit_Get_Returns200_WhenOwnerPlaninar()
    {
        var response = await _planinarClient.GetAsync($"/Knjizica/Edit/{TestData.KnjizicaId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Get_Returns403_WhenForeignPlaninar()
    {
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);

        var response = await foreignClient.GetAsync($"/Knjizica/Edit/{TestData.KnjizicaId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Edit_Get_Returns200_WhenAdmin_NotOwner()
    {
        var response = await _adminClient.GetAsync($"/Knjizica/Edit/{TestData.KnjizicaId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Edit_Post_UpdatesEntity_WhenOwnerPlaninar()
    {
        var token = await AntiForgeryHelper.GetTokenAsync(_planinarClient, $"/Knjizica/Edit/{TestData.KnjizicaId}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["IdKorisnik"] = TestData.PlaninarKorisnikId.ToString(),
            ["Napomena"] = "Azurirana napomena MVC testa."
        };

        var response = await _planinarClient.PostAsync($"/Knjizica/Edit/{TestData.KnjizicaId}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var updated = db.Knjizice.Single(k => k.IdKnjizica == TestData.KnjizicaId);
        updated.Napomena.Should().Be("Azurirana napomena MVC testa.");
    }

    [Fact]
    public async Task Edit_Post_Returns403_WhenForeignPlaninar()
    {
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);
        var token = await AntiForgeryHelper.GetTokenAsync(foreignClient, "/Posjet/Create");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["IdKorisnik"] = TestData.PlaninarKorisnikId.ToString(),
            ["Napomena"] = "Hakirano."
        };

        var response = await foreignClient.PostAsync($"/Knjizica/Edit/{TestData.KnjizicaId}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ---- Delete — Admin samo (soft delete) ----

    [Fact]
    public async Task Delete_Get_Returns403_WhenOwnerPlaninar()
    {
        var response = await _planinarClient.GetAsync($"/Knjizica/Delete/{TestData.KnjizicaId}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_Post_SoftDeletesEntity_WhenAdmin()
    {
        var id = await SeedKnjizicaAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/Knjizica/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _adminClient.PostAsync($"/Knjizica/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        var deleted = db.Knjizice.Single(k => k.IdKnjizica == id);
        deleted.StatusAktivna.Should().BeFalse();
    }

    [Fact]
    public async Task Delete_Post_Returns403_WhenPlaninar()
    {
        var id = await SeedKnjizicaAsync();
        var token = await AntiForgeryHelper.GetTokenAsync(_adminClient, $"/Knjizica/Delete/{id}");

        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token
        };

        var response = await _planinarClient.PostAsync($"/Knjizica/Delete/{id}", new FormUrlEncodedContent(form));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ---- Details ----

    [Fact]
    public async Task Details_Returns200_WhenOwnerPlaninar()
    {
        var response = await _planinarClient.GetAsync($"/Knjizica/Details/{TestData.KnjizicaId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Details_Returns403_WhenForeignPlaninar()
    {
        using var foreignClient = AuthHelper.CreateForeignPlaninarClient(_factory);

        var response = await foreignClient.GetAsync($"/Knjizica/Details/{TestData.KnjizicaId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Details_Returns404_WhenMissing()
    {
        var response = await _adminClient.GetAsync("/Knjizica/Details/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // Seeda svjeziog throwaway korisnika (bez knjizice) i njegovu knjizicu, za Delete testove.
    // IdKorisnik ima jedinstveni indeks na Knjizica, pa svaki test mora dobiti svog korisnika.
    private async Task<int> SeedKnjizicaAsync()
    {
        var korisnikId = await SeedKorisnikBezKnjiziceAsync();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new planinarenje.Entiteti.Knjizica
        {
            IdKorisnik = korisnikId,
            DatumKreiranja = DateTime.UtcNow,
            StatusAktivna = true
        };
        db.Knjizice.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdKnjizica;
    }

    // Seeda svjeziog throwaway korisnika bez aktivne knjizice (svaki test treba svog
    // jer Knjizica.IdKorisnik ima jedinstveni indeks, a baza je zajednicka po test-klasi).
    private async Task<int> SeedKorisnikBezKnjiziceAsync(string? appUserId = null)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();

        var entity = new planinarenje.Entiteti.Korisnik
        {
            Ime = "BezKnjizice",
            Prezime = "Test",
            Email = $"bezknjizice-{Guid.NewGuid()}@test.hr",
            KorisnickoIme = $"bezknjizice_{Guid.NewGuid():N}",
            DatumRegistracije = DateTime.UtcNow,
            StatusAktivan = true,
            AppUserId = appUserId
        };
        db.Korisnici.Add(entity);
        await db.SaveChangesAsync();
        return entity.IdKorisnik;
    }
}
