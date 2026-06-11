namespace planinarenje.IntegrationTests.Helpers;

/// <summary>
/// Konstante za testne podatke — IDs koji se koriste u TestDataSeeder-u i test klasama.
/// </summary>
public static class TestData
{
    // AppUser identifikatori za testnu auth shemu (X-Test-UserId zaglavlje).
    // Moraju se podudarati s AppUserId vrijednostima u baznim Korisnik zapisima.
    public const string PlaninarAppUserId = "test-planinar-app-user-id-001";
    public const string AdminAppUserId = "test-admin-app-user-id-001";

    // IdKorisnik za seedirane korisnike (iz HasData).
    public const int PlaninarKorisnikId = 1;  // Luka Bošnjak
    public const int AdminKorisnikId = 2;     // Test Test

    // Seedirani IDs koji se koriste u POST testovima.
    public const int PodrucjeId = 1;
    public const int KontrolnaTockaId = 1;
    public const int RutaId = 1;
    public const int KnjizicaId = 1;
    public const int PosjetId = 1;
    public const int FotografijaId = 1;
    public const int PlaninarskiObjektId = 1;
    public const int PlaninarskaUdrugaId = 1;
    public const int MedaljaId = 1;
    public const int KorisnikMedaljaId = 1;

    // Id korisnika bez knjižice — dodan u TestDataSeeder.
    public const int KorisnikBezKnjiziceId = 100;
}
