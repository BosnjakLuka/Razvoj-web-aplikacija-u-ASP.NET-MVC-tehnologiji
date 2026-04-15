namespace planinarenje.Models;

public class KnjizicaIndexViewModel
{
    public int IdKnjizica { get; set; }
    public int IdKorisnik { get; set; }
    public string ImePrezimeKorisnika { get; set; } = string.Empty;
    public DateTime DatumKreiranja { get; set; }
    public bool StatusAktivna { get; set; }
}

public class KnjizicaDetailsViewModel
{
    public int IdKnjizica { get; set; }
    public int IdKorisnik { get; set; }
    public string ImePrezimeKorisnika { get; set; } = string.Empty;
    public string KorisnickoIme { get; set; } = string.Empty;
    public string? ProfilnaSlikaUrl { get; set; }
    public DateTime DatumKreiranja { get; set; }
    public bool StatusAktivna { get; set; }
    public string? Napomena { get; set; }
    public List<KnjizicaPosjetViewModel> Posjeti { get; set; } = new();
}

public class KnjizicaPosjetViewModel
{
    public int IdPosjet { get; set; }
    public int IdKontrolnaTocka { get; set; }
    public string NazivKontrolneTocke { get; set; } = string.Empty;
    public DateTime DatumVrijemePosjeta { get; set; }
    public bool JeLiPotvrdenPosjet { get; set; }
}
