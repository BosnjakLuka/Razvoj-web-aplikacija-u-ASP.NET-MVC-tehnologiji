namespace planinarenje.Entiteti;

public class Ruta
{
    public int IdRuta { get; set; }
    public int IdKontrolnaTocka { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string Pocetak { get; set; } = string.Empty;
    public string Kraj { get; set; } = string.Empty;
    public int VrijemeHodaMin { get; set; }
    public decimal DuljinaKm { get; set; }
    public int? VisinskaRazlikaM { get; set; }
    public string? Opis { get; set; }
    public string? OznakaNaTerenu { get; set; }
    public int? GodinaObnove { get; set; }
    public string? Napomena { get; set; }
    public TezinaRute TezinaRute { get; set; }
    public string? GPXPath { get; set; }

    public KontrolnaTocka? KontrolnaTocka { get; set; }
    public List<Posjet> Posjeti { get; set; } = new();
}
