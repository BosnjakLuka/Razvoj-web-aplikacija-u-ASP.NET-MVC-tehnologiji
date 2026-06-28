namespace planinarenje.Models;

public class KontrolnaTockaIndexCardViewModel
{
    public int IdKontrolnaTocka { get; init; }
    public string Naziv { get; init; } = string.Empty;
    public string TipKontrolneTockeNaziv { get; init; } = string.Empty;
    public int? NadmorskaVisina { get; init; }
    public string PodrucjeNaziv { get; init; } = string.Empty;
    public string? OpisPreview { get; init; }
    public string? GUIDOznaka { get; init; }
    public bool JeOdobreno { get; init; } = true;
}
