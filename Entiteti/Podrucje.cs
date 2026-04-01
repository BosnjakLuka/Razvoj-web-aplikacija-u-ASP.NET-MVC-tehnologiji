namespace planinarenje.Entiteti;

public class Podrucje
{
    public int IdPodrucje { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string? Opis { get; set; }
    public string? Regija { get; set; }
    public int MinimalanBrojKTZaObilazak { get; set; }

    public List<KontrolnaTocka> KontrolneTocke { get; set; } = new();
    public List<PlaninarskiObjekt> PlaninarskiObjekti { get; set; } = new();
}
