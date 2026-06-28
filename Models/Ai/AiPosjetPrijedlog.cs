using planinarenje.Entiteti;

namespace planinarenje.Models.Ai;

/// <summary>
/// Rezultat ekstrakcije podataka o posjetu iz prirodnog jezika (AI upit).
/// Sadrži samo "sirove" nazive i vrijednosti koje AI izvuče iz korisnikova opisa;
/// mapiranje naziva na ID-eve iz baze radi kontroler, ne AI servis.
///
/// AI namjerno NE popunjava GUID (fizički je na žigu na vrhu) niti korisnika/knjižicu
/// (forsiraju se server-side na trenutnog prijavljenog korisnika).
/// </summary>
public class AiPosjetPrijedlog
{
    /// <summary>Naziv kontrolne točke kako ga je AI prepoznao iz opisa (npr. "Sljeme").</summary>
    public string? KontrolnaTockaNaziv { get; set; }

    /// <summary>Naziv rute/staze kako ga je AI prepoznao (npr. "Leustekova staza").</summary>
    public string? RutaNaziv { get; set; }

    /// <summary>Datum i vrijeme posjeta (AI razrješava relativne izraze poput "jučer" u konkretan datum).</summary>
    public DateTime? DatumVrijemePosjeta { get; set; }

    /// <summary>Procijenjeni doživljaj posjeta; dolazi kao točna enum vrijednost (responseSchema enum).</summary>
    public DozivljajPosjeta? DozivljajPosjeta { get; set; }

    /// <summary>Trajanje uspona u minutama, ako je spomenuto.</summary>
    public int? VrijemeUsponaMin { get; set; }

    /// <summary>Pročišćeni opis iskustva (može biti sažetak korisnikova teksta).</summary>
    public string? OpisIskustva { get; set; }

    /// <summary>Napomena servisa (npr. razlog zašto nešto nije prepoznato ili da je AI nedostupan).</summary>
    public string? Napomena { get; set; }
}
