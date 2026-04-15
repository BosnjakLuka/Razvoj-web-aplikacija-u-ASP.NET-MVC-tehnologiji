namespace planinarenje.Models;

public class PlaninarskiObjektIndexCardViewModel
{
    public int IdPlaninarskiObjekt { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string TipObjektaNaziv { get; set; } = string.Empty;
    public string PodrucjeNaziv { get; set; } = string.Empty;
    public string? UdrugaNaziv { get; set; }
    public string NadmorskaVisinaTekst { get; set; } = string.Empty;
    public string? KapacitetTekst { get; set; }
    public string? OdgovornaOsoba { get; set; }
    public bool ImaNocenje { get; set; }
    public bool ImaHranu { get; set; }
    public string? OpisPreview { get; set; }
}