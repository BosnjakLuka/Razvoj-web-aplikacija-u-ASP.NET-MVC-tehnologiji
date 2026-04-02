namespace planinarenje.Entiteti;

public class Medalja
{
    public int IdMedalja { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string? Opis { get; set; }
    public int MinimalanBrojKontrolnihTocaka { get; set; }
    public int MinimalanBrojPodrucja { get; set; }

    public List<KorisnikMedalja> KorisnikMedalje { get; set; } = new();
}
