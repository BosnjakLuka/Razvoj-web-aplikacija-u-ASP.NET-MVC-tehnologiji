namespace planinarenje.Entiteti;

public class Korisnik
{
    public int IdKorisnik { get; set; }
    public string Ime { get; set; } = string.Empty;
    public string Prezime { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string KorisnickoIme { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime? DatumRodenja { get; set; }
    public DateTime DatumRegistracije { get; set; }
    public string? BrojMobitela { get; set; }
    public string? ProfilnaSlika { get; set; }
    public bool StatusAktivan { get; set; }

    public Knjizica? Knjizica { get; set; }
    public List<Posjet> Posjeti { get; set; } = new();
    public List<KorisnikMedalja> KorisnikMedalje { get; set; } = new();
}
