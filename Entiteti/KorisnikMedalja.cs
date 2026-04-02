namespace planinarenje.Entiteti;

public class KorisnikMedalja
{
    public int IdKorisnikMedalja { get; set; }
    public int IdKorisnik { get; set; }
    public int IdMedalja { get; set; }
    public DateTime DatumDodjele { get; set; }
    public string? Napomena { get; set; }

    public Korisnik? Korisnik { get; set; }
    public Medalja? Medalja { get; set; }
}
