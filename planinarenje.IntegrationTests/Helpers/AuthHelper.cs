using Microsoft.AspNetCore.Mvc.Testing;

namespace planinarenje.IntegrationTests.Helpers;

/// <summary>
/// Kreira HttpClient s unaprijed postavljenim X-Test-UserId / X-Test-Roles zaglavljima
/// koje TestAuthHandler čita umjesto pravih Identity kolačića.
/// AllowAutoRedirect je isključen kako bi MVC CRUD testovi mogli provjeriti 302
/// (RedirectToAction) status i Location zaglavlje umjesto da klijent automatski
/// odradi sljedeći GET — API testovi to ne koriste pa promjena ne utječe na njih.
/// </summary>
public static class AuthHelper
{
    private static readonly WebApplicationFactoryClientOptions NoRedirectOptions = new()
    {
        AllowAutoRedirect = false
    };

    public static HttpClient CreateAdminClient(CustomWebAppFactory factory)
    {
        var client = factory.CreateClient(NoRedirectOptions);
        client.DefaultRequestHeaders.Add("X-Test-UserId", TestData.AdminAppUserId);
        client.DefaultRequestHeaders.Add("X-Test-Roles", "Admin");
        return client;
    }

    public static HttpClient CreatePlaninarClient(CustomWebAppFactory factory)
    {
        var client = factory.CreateClient(NoRedirectOptions);
        client.DefaultRequestHeaders.Add("X-Test-UserId", TestData.PlaninarAppUserId);
        client.DefaultRequestHeaders.Add("X-Test-Roles", "Planinar");
        return client;
    }

    /// <summary>
    /// Autentificiran korisnik s rolom Planinar koji NIJE povezan ni s jednim Korisnik profilom.
    /// Prolazi rolnu provjeru, ali pada na provjeri vlasništva (očekivani 403 Forbidden).
    /// </summary>
    public static HttpClient CreateForeignPlaninarClient(CustomWebAppFactory factory)
    {
        var client = factory.CreateClient(NoRedirectOptions);
        client.DefaultRequestHeaders.Add("X-Test-UserId", "test-foreign-planinar-id-999");
        client.DefaultRequestHeaders.Add("X-Test-Roles", "Planinar");
        return client;
    }

    public static HttpClient CreateAnonymousClient(CustomWebAppFactory factory)
        => factory.CreateClient(NoRedirectOptions);

    /// <summary>
    /// Generički klijent za proizvoljan AppUserId/role — koristan kad test treba povezati
    /// vlastiti Korisnik zapis (bez unaprijed seediranog AppUserId-a) s testnom auth shemom.
    /// </summary>
    public static HttpClient CreateClient(CustomWebAppFactory factory, string appUserId, string roles)
    {
        var client = factory.CreateClient(NoRedirectOptions);
        client.DefaultRequestHeaders.Add("X-Test-UserId", appUserId);
        client.DefaultRequestHeaders.Add("X-Test-Roles", roles);
        return client;
    }
}
