namespace planinarenje.Entiteti;

public class PlaninarskiObjekt
{
    public int IdPlaninarskiObjekt { get; set; }
    public int IdPodrucje { get; set; }
    public int IdPlaninarskaUdruga { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public TipObjekta TipObjekta { get; set; }
    public int? NadmorskaVisina { get; set; }
    public int? Kapacitet { get; set; }
    public string? Opis { get; set; }
    public string? ImeOdgovorneOsobe { get; set; }
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public string? Adresa { get; set; }
    public bool ImaNocenje { get; set; }
    public bool ImaHranu { get; set; }
    public string? RadnoVrijemeOpis { get; set; }

    public Podrucje? Podrucje { get; set; }
    public PlaninarskaUdruga? PlaninarskaUdruga { get; set; }
}
