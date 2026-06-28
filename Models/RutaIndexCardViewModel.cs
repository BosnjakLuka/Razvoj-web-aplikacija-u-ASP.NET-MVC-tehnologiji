namespace planinarenje.Models;

public class RutaIndexCardViewModel
{
    public int IdRuta { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string PovezanaKontrolnaTocka { get; set; } = string.Empty;
    public string Pocetak { get; set; } = string.Empty;
    public string Kraj { get; set; } = string.Empty;
    public string Trajanje { get; set; } = string.Empty;
    public decimal DuljinaKm { get; set; }
    public int? VisinskaRazlikaM { get; set; }
    public string TezinaTekst { get; set; } = string.Empty;
    public string TezinaCssClass { get; set; } = string.Empty;
    public string? OpisPreview { get; set; }
    public bool JeOdobreno { get; set; } = true;
}