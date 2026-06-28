using System.Text.RegularExpressions;

namespace planinarenje.IntegrationTests.Helpers;

/// <summary>
/// Izvlači __RequestVerificationToken iz HTML forme vraćene GET akcijom.
/// Koristi se za MVC (ne-API) Create/Edit/Delete POST testove jer te akcije
/// imaju [ValidateAntiForgeryToken]. HttpClient iz WebApplicationFactory
/// automatski drži kolačiće između poziva (HandleCookies = true po defaultu),
/// pa GET + parsirani token + POST na istom klijentu rade kao prava sesija.
/// </summary>
public static class AntiForgeryHelper
{
    private static readonly Regex TokenRegex = new(
        "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"",
        RegexOptions.Compiled);

    public static async Task<string> GetTokenAsync(HttpClient client, string getUrl)
    {
        var html = await client.GetStringAsync(getUrl);
        var match = TokenRegex.Match(html);
        if (!match.Success)
            throw new InvalidOperationException($"__RequestVerificationToken nije pronađen na {getUrl}.");

        return match.Groups[1].Value;
    }
}
