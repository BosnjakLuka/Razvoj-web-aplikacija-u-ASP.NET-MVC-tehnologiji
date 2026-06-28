using System.Text;
using System.Text.Json;
using planinarenje.Entiteti;
using planinarenje.Models.Ai;

namespace planinarenje.Services;

/// <summary>
/// Implementacija <see cref="IAiUnosService"/> nad Google Gemini API-jem.
///
/// Koristi "controlled generation" (responseMimeType: application/json + responseSchema)
/// pa model vraća JSON koji garantirano sjeda u očekivani oblik — uključujući
/// <see cref="DozivljajPosjeta"/> kao enum vrijednost. Time se izbjegava krhko parsiranje
/// slobodnog teksta. API ključ se NE drži u kodu — čita se iz konfiguracije (user secrets).
/// </summary>
public class GeminiAiUnosService : IAiUnosService
{
    // Brzi, jeftini model s besplatnim tierom — dovoljno za ekstrakciju iz kratkog opisa.
    private const string Model = "gemini-2.5-flash";
    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/";

    private readonly HttpClient _http;
    private readonly ILogger<GeminiAiUnosService> _logger;
    private readonly string? _apiKey;

    public GeminiAiUnosService(HttpClient http, IConfiguration config, ILogger<GeminiAiUnosService> logger)
    {
        _http = http;
        _logger = logger;
        _apiKey = config["Gemini:ApiKey"];
    }

    public bool JeDostupan => !string.IsNullOrWhiteSpace(_apiKey);

    public async Task<AiPosjetPrijedlog> IzvuciPodatkeAsync(string upit, CancellationToken cancellationToken = default)
    {
        if (!JeDostupan)
        {
            return new AiPosjetPrijedlog { Napomena = "AI servis nije konfiguriran (nedostaje API ključ)." };
        }

        if (string.IsNullOrWhiteSpace(upit))
        {
            return new AiPosjetPrijedlog { Napomena = "Upit je prazan." };
        }

        try
        {
            var zahtjev = SagradiZahtjev(upit);
            var json = JsonSerializer.Serialize(zahtjev);

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{BaseUrl}{Model}:generateContent");
            request.Headers.Add("x-goog-api-key", _apiKey);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _http.SendAsync(request, cancellationToken);
            var tijelo = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Gemini API vratio status {Status}: {Tijelo}", (int)response.StatusCode, tijelo);
                return new AiPosjetPrijedlog { Napomena = "AI servis trenutno nije dostupan. Pokušajte ponovno ili popunite ručno." };
            }

            return ParsirajOdgovor(tijelo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri pozivu Gemini API-ja za AI unos posjeta.");
            return new AiPosjetPrijedlog { Napomena = "Došlo je do greške pri obradi AI upita. Popunite formu ručno." };
        }
    }

    // Gradi Gemini request s responseSchema. DozivljajPosjeta vrijednosti dolaze iz enuma
    // pa se prompt automatski drži usklađen s domenom.
    private static object SagradiZahtjev(string upit)
    {
        var dozivljajVrijednosti = Enum.GetNames<DozivljajPosjeta>();

        var systemInstruction =
            "Ti si pomoćnik u planinarskoj aplikaciji. Iz korisnikova opisa na hrvatskom jeziku izvuci " +
            "strukturirane podatke o jednom planinarskom posjetu kontrolnoj točki. " +
            $"Današnji datum je {DateTime.Now:yyyy-MM-dd}. Relativne izraze poput 'jučer', 'prošlu subotu' " +
            "ili 'prije dva dana' pretvori u konkretan datum u ISO 8601 formatu. " +
            "Polje 'datum' MORA sadržavati i vrijeme dana, ne samo datum (puni oblik 'yyyy-MM-ddTHH:mm:ss'). " +
            "Ako je u opisu spomenuto doba dana (npr. 'u 11 sati', 'ujutro', 'navečer', 'oko podneva'), pretvori ga " +
            "u konkretno vrijeme i uključi ga u 'datum'. Ako vrijeme dana nije spomenuto, koristi 00:00:00. " +
            "Za polja koja nisu spomenuta u opisu vrati null — nemoj izmišljati vrijednosti. " +
            "Naziv kontrolne točke i rute prepiši onako kako ih korisnik spominje (npr. vrh, vidikovac, staza). " +
            "Polje 'dozivljaj' mora biti jedna od ponuđenih enum vrijednosti koja najbolje opisuje težinu/iskustvo. " +
            "GUID oznaku i podatke o korisniku NE izvlači.";

        return new
        {
            system_instruction = new
            {
                parts = new[] { new { text = systemInstruction } }
            },
            contents = new[]
            {
                new
                {
                    parts = new[] { new { text = upit } }
                }
            },
            generationConfig = new
            {
                responseMimeType = "application/json",
                responseSchema = new
                {
                    type = "OBJECT",
                    properties = new
                    {
                        kontrolnaTockaNaziv = new { type = "STRING", nullable = true },
                        rutaNaziv = new { type = "STRING", nullable = true },
                        datum = new
                        {
                            type = "STRING",
                            nullable = true,
                            description = "Puni datum i vrijeme posjeta u ISO 8601 formatu (yyyy-MM-ddTHH:mm:ss), uključujući doba dana ako je spomenuto."
                        },
                        dozivljaj = new { type = "STRING", @enum = dozivljajVrijednosti, nullable = true },
                        vrijemeUsponaMin = new { type = "INTEGER", nullable = true },
                        opisIskustva = new { type = "STRING", nullable = true }
                    }
                }
            }
        };
    }

    // Iz Gemini odgovora vadi candidates[0].content.parts[0].text (naš JSON) i mapira ga na prijedlog.
    private AiPosjetPrijedlog ParsirajOdgovor(string tijelo)
    {
        using var doc = JsonDocument.Parse(tijelo);

        if (!doc.RootElement.TryGetProperty("candidates", out var candidates) ||
            candidates.GetArrayLength() == 0)
        {
            _logger.LogWarning("Gemini odgovor ne sadrži kandidate: {Tijelo}", tijelo);
            return new AiPosjetPrijedlog { Napomena = "AI nije vratio prijedlog. Popunite formu ručno." };
        }

        var text = candidates[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        if (string.IsNullOrWhiteSpace(text))
        {
            return new AiPosjetPrijedlog { Napomena = "AI je vratio prazan odgovor." };
        }

        using var podaci = JsonDocument.Parse(text);
        var root = podaci.RootElement;

        return new AiPosjetPrijedlog
        {
            KontrolnaTockaNaziv = DohvatiString(root, "kontrolnaTockaNaziv"),
            RutaNaziv = DohvatiString(root, "rutaNaziv"),
            DatumVrijemePosjeta = DohvatiDatum(root, "datum"),
            DozivljajPosjeta = DohvatiDozivljaj(root, "dozivljaj"),
            VrijemeUsponaMin = DohvatiInt(root, "vrijemeUsponaMin"),
            OpisIskustva = DohvatiString(root, "opisIskustva")
        };
    }

    private static string? DohvatiString(JsonElement el, string naziv)
    {
        if (el.TryGetProperty(naziv, out var p) && p.ValueKind == JsonValueKind.String)
        {
            var v = p.GetString();
            return string.IsNullOrWhiteSpace(v) ? null : v.Trim();
        }
        return null;
    }

    private static int? DohvatiInt(JsonElement el, string naziv)
    {
        if (el.TryGetProperty(naziv, out var p))
        {
            if (p.ValueKind == JsonValueKind.Number && p.TryGetInt32(out var n)) return n;
            if (p.ValueKind == JsonValueKind.String && int.TryParse(p.GetString(), out var s)) return s;
        }
        return null;
    }

    private static DateTime? DohvatiDatum(JsonElement el, string naziv)
    {
        var s = DohvatiString(el, naziv);
        if (s != null && DateTime.TryParse(s, out var d)) return d;
        return null;
    }

    private static DozivljajPosjeta? DohvatiDozivljaj(JsonElement el, string naziv)
    {
        var s = DohvatiString(el, naziv);
        if (s != null && Enum.TryParse<DozivljajPosjeta>(s, ignoreCase: true, out var dozivljaj))
        {
            return dozivljaj;
        }
        return null;
    }
}
