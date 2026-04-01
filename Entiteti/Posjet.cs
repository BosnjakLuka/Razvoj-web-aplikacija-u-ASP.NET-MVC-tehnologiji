namespace planinarenje.Entiteti;

public class Posjet
{
    public int IdPosjet { get; set; }
    public int IdKorisnik { get; set; }
    public int IdKnjizica { get; set; }
    public int IdKontrolnaTocka { get; set; }
    public int IdRuta { get; set; }
    public DateTime DatumVrijemePosjeta { get; set; }
    public int? VrijemeUsponaMin { get; set; }
    public DozivljajPosjeta DozivljajPosjeta { get; set; }
    public string? OpisIskustva { get; set; }
    public string UneseniGUID { get; set; } = string.Empty;
    public bool JeLiPotvrdenPosjet { get; set; }
    public DateTime DatumKreiranjaZapisa { get; set; }

    public Korisnik? Korisnik { get; set; }
    public Knjizica? Knjizica { get; set; }
    public KontrolnaTocka? KontrolnaTocka { get; set; }
    public Ruta? Ruta { get; set; }
    public List<Fotografija> Fotografije { get; set; } = new();
}
