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

    public static HttpClient CreateAnonymousClient(CustomWebAppFactory factory)
        => factory.CreateClient();
}
