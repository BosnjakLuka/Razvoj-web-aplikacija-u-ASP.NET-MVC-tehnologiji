using planinarenje.Data;
using planinarenje.Entiteti;

namespace planinarenje.IntegrationTests.Helpers;

/// <summary>
/// Dopunjuje InMemory bazu nakon EnsureCreated():
///  - Postavlja AppUserId na seediranim Korisnicima da TestAuthHandler može naći profil.
///  - Dodaje Korisnik bez Knjižice (za POST test KnjizicaApi-ja).
/// </summary>
public static class TestDataSeeder
{
    public static async Task SeedAsync(PlaninarstvoDbContext db)
    {
        // Poveži seedirane Korisnike s testnim AppUser identifikatorima.
        var planinar = await db.Korisnici.FindAsync(TestData.PlaninarKorisnikId);
        if (planinar != null)
            planinar.AppUserId = TestData.PlaninarAppUserId;

        var admin = await db.Korisnici.FindAsync(TestData.AdminKorisnikId);
        if (admin != null)
            admin.AppUserId = TestData.AdminAppUserId;

        // Korisnik bez Knjižice — potreban za POST /api/knjizica test.
        if (await db.Korisnici.FindAsync(TestData.KorisnikBezKnjiziceId) == null)
        {
            db.Korisnici.Add(new Korisnik
            {
                IdKorisnik = TestData.KorisnikBezKnjiziceId,
                Ime = "NoKnjizica",
                Prezime = "Test",
                Email = "noknjizica@test.hr",
                KorisnickoIme = "noknjizica_test",
                DatumRegistracije = DateTime.UtcNow,
                StatusAktivan = true
            });
        }

        await db.SaveChangesAsync();
    }
}
