using System.Text;

namespace planinarenje.Helpers;

/// <summary>
/// Pomoćne funkcije za pretraživanje/podudaranje hrvatskog teksta neosjetljivo na
/// dijakritiku (č/ć/đ/š/ž) i velika/mala slova — npr. "okic" treba pronaći "Okić",
/// "Okića", "Okiću" jednako kao "okić". Koristi se i u globalnoj pretrazi i u AI unosu.
/// </summary>
public static class HrvatskiTekst
{
    public static string Normaliziraj(string s)
    {
        var sb = new StringBuilder(s.Length);
        foreach (var ch in s.ToLowerInvariant())
        {
            sb.Append(ch switch
            {
                'č' => 'c',
                'ć' => 'c',
                'đ' => 'd',
                'š' => 's',
                'ž' => 'z',
                _ => ch
            });
        }
        return sb.ToString();
    }

    /// <summary>Provjerava sadrži li <paramref name="tekst"/> <paramref name="term"/>, neosjetljivo na dijakritiku i veličinu slova.</summary>
    public static bool SadrziNormalizirano(string? tekst, string term)
    {
        if (string.IsNullOrEmpty(tekst) || string.IsNullOrEmpty(term)) return false;
        return Normaliziraj(tekst).Contains(Normaliziraj(term), StringComparison.Ordinal);
    }

    public static bool JednakoNormalizirano(string? tekst, string term)
    {
        if (tekst == null) return false;
        return Normaliziraj(tekst) == Normaliziraj(term);
    }

    public static bool PocinjeNormalizirano(string? tekst, string term)
    {
        if (string.IsNullOrEmpty(tekst)) return false;
        return Normaliziraj(tekst).StartsWith(Normaliziraj(term), StringComparison.Ordinal);
    }
}
