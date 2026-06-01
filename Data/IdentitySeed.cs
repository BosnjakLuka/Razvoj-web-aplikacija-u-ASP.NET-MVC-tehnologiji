using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using planinarenje.Entiteti;

namespace planinarenje.Data;

public static class IdentitySeed
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        var dbContext = serviceProvider.GetRequiredService<PlaninarstvoDbContext>();

        if (!await IdentityTablesExistAsync(dbContext))
        {
            return;
        }

        var roles = new[] { "Admin", "Planinar" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = "admin@planinarenje.hr";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                OIB = "00000000000",
                JMBG = "0000000000000"
            };

            var adminResult = await userManager.CreateAsync(adminUser, "Admin@2026!");
            if (adminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");

                if (!await dbContext.Korisnici.AnyAsync(k => k.AppUserId == adminUser.Id))
                {
                    dbContext.Korisnici.Add(new Korisnik
                    {
                        Ime = "Admin",
                        Prezime = "Planinarstvo",
                        Email = adminEmail,
                        KorisnickoIme = adminEmail,
                        DatumRegistracije = DateTime.UtcNow,
                        StatusAktivan = true,
                        AppUserId = adminUser.Id
                    });

                    await dbContext.SaveChangesAsync();
                }
            }
        }

        var planinarEmail = "luka@planinarenje.hr";
        var planinarUser = await userManager.FindByEmailAsync(planinarEmail);
        if (planinarUser == null)
        {
            planinarUser = new AppUser
            {
                UserName = planinarEmail,
                Email = planinarEmail,
                EmailConfirmed = true,
                OIB = "12345678901",
                JMBG = "1234567890123"
            };

            var planinarResult = await userManager.CreateAsync(planinarUser, "Planinar@26!");
            if (planinarResult.Succeeded)
            {
                await userManager.AddToRoleAsync(planinarUser, "Planinar");

                var korisnik = await dbContext.Korisnici.FirstOrDefaultAsync(k => k.AppUserId == null && k.Email == planinarEmail);
                if (korisnik == null)
                {
                    korisnik = new Korisnik
                    {
                        Ime = "Luka",
                        Prezime = "Planinar",
                        Email = planinarEmail,
                        KorisnickoIme = planinarEmail,
                        DatumRegistracije = DateTime.UtcNow,
                        StatusAktivan = true,
                        AppUserId = planinarUser.Id
                    };

                    dbContext.Korisnici.Add(korisnik);
                }
                else
                {
                    korisnik.AppUserId = planinarUser.Id;
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }

    private static async Task<bool> IdentityTablesExistAsync(PlaninarstvoDbContext dbContext)
    {
        await dbContext.Database.OpenConnectionAsync();

        try
        {
            await using var command = dbContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = @"
                SELECT COUNT(*)
                FROM information_schema.tables
                WHERE table_schema = DATABASE()
                  AND table_name IN ('AspNetRoles', 'AspNetUsers')";

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) >= 2;
        }
        catch (MySqlException)
        {
            return false;
        }
        finally
        {
            await dbContext.Database.CloseConnectionAsync();
        }
    }
}
