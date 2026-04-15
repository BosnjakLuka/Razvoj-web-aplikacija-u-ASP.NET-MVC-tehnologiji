namespace planinarenje.Models;

public class MedaljaIndexCardViewModel
{
    public int IdMedalja { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string? OpisPreview { get; set; }
    public int MinimalanBrojKontrolnihTocaka { get; set; }
    public int MinimalanBrojPodrucja { get; set; }
    public string IkonaKlasa { get; set; } = "bi-award";
    public string BojaKlasa { get; set; } = "text-secondary";
}