namespace planinarenje.Entiteti;

public class KontrolnaTocka
{
    public int IdKontrolnaTocka { get; set; }
    public string GUIDOznaka { get; set; } = string.Empty;
    public int IdPodrucje { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public TipKontrolneTocke TipKontrolneTocke { get; set; }
    public int? NadmorskaVisina { get; set; }
    public string? Opis { get; set; }
    public string? Koordinate { get; set; }
    public string? OpisZiga { get; set; }

    public Podrucje? Podrucje { get; set; }
    public List<Posjet> Posjeti { get; set; } = new();
    public List<Ruta> Rute { get; set; } = new();
}
