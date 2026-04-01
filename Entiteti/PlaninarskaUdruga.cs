namespace planinarenje.Entiteti;

public class PlaninarskaUdruga
{
    public int IdPlaninarskaUdruga { get; set; }
    public string OIB { get; set; } = string.Empty;
    public string Naziv { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? BrojTelefona { get; set; }
    public string? Adresa { get; set; }
    public string? PostanskiBroj { get; set; }
    public string? Grad { get; set; }
    public string? Zupanija { get; set; }
    public int? BrojClanova { get; set; }

    public List<PlaninarskiObjekt> PlaninarskiObjekti { get; set; } = new();
}
