namespace planinarenje.Models;

public class PodrucjeIndexCardViewModel
{
    public int IdPodrucje { get; init; }
    public string Naziv { get; init; } = string.Empty;
    public string Regija { get; init; } = string.Empty;
    public string? OpisPreview { get; init; }
    public int MinimalanBrojKTZaObilazak { get; init; }
    public int UkupanBrojKT { get; init; }
    public bool JeOdobreno { get; init; } = true;
}
