namespace planinarenje.IntegrationTests.Helpers;

/// <summary>
/// Kreira HttpClient s unaprijed postavljenim X-Test-UserId / X-Test-Roles zaglavljima
/// koje TestAuthHandler čita umjesto pravih Identity kolačića.
/// </summary>
public static class AuthHelper
{
    public static HttpClient CreateAdminClient(CustomWebAppFactory factory)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-UserId", TestData.AdminAppUserId);
        client.DefaultRequestHeaders.Add("X-Test-Roles", "Admin");
        return client;
    }

    public static HttpClient CreatePlaninarClient(CustomWebAppFactory factory)
    {
        var client = factory.CreateClient();
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
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-UserId", "test-foreign-planinar-id-999");
        client.DefaultRequestHeaders.Add("X-Test-Roles", "Planinar");
        return client;
    }

    public static HttpClient CreateAnonymousClient(CustomWebAppFactory factory)
        => factory.CreateClient();
}
