namespace planinarenje.Entiteti;

public class Knjizica
{
    public int IdKnjizica { get; set; }
    public int IdKorisnik { get; set; }
    public DateTime DatumKreiranja { get; set; }
    public string? Napomena { get; set; }
    public bool StatusAktivna { get; set; }

    public Korisnik? Korisnik { get; set; }
    public List<Posjet> Posjeti { get; set; } = new();
}
