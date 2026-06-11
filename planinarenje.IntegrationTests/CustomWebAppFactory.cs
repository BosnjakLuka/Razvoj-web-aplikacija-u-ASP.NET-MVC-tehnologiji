using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.IntegrationTests.Helpers;

namespace planinarenje.IntegrationTests;

/// <summary>
/// Testni host koji zamjenjuje MySQL s InMemory bazom i pravu autentikaciju s TestAuthHandler-om.
/// Svaka instanca dobiva jedinstveni naziv baze kako bi testovi bili međusobno izolirani.
/// </summary>
public class CustomWebAppFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Ukloni SVE registracije vezane uz PlaninarstvoDbContext koje je dodao Program.cs (MySQL/Pomelo).
            // U EF Core 9 AddDbContext registrira i IDbContextOptionsConfiguration<TContext> (config delegat) kao
            // zaseban descriptor — ako se ne ukloni, drugi AddDbContext samo DODA InMemory konfiguraciju na MySQL,
            // pa se oba provajdera primijene na isti options objekt → "dva provajdera" greška.
            var toRemove = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<PlaninarstvoDbContext>) ||
                d.ServiceType == typeof(DbContextOptions) ||
                d.ServiceType == typeof(PlaninarstvoDbContext) ||
                (d.ServiceType.IsGenericType &&
                 d.ServiceType.GetGenericArguments().Contains(typeof(PlaninarstvoDbContext))))
                .ToList();
            foreach (var d in toRemove)
                services.Remove(d);

            // Zamijeni s InMemory providerom (izolirana baza po instanci factoryja).
            services.AddDbContext<PlaninarstvoDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            // Postavi TestAuthHandler kao defaultnu auth shemu umjesto Identity kolačića.
            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName, _ => { });
        });
    }

    /// <summary>
    /// Pokreni EnsureCreated (primijeni HasData seedove) i dodaj testne AppUserId na korisnicima.
    /// Pozivati jednom na početku svake test klase.
    /// </summary>
    public async Task SeedDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlaninarstvoDbContext>();
        await db.Database.EnsureCreatedAsync();
        await TestDataSeeder.SeedAsync(db);
    }
}
