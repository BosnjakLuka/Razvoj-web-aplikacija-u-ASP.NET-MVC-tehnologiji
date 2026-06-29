namespace planinarenje.Models.ViewModels;

public class PosjetNaCekanjuViewModel
{
    public int IdPosjet { get; set; }
    public string ImePrezimeKorisnika { get; set; } = string.Empty;
    public string NazivKontrolneTocke { get; set; } = string.Empty;
    public string NazivRute { get; set; } = string.Empty;
    public DateTime DatumVrijemePosjeta { get; set; }
    public DateTime DatumKreiranjaZapisa { get; set; }
}

public class EntitetNaCekanjuViewModel
{
    public string TipEntiteta { get; set; } = string.Empty;
    public string TipEntitetaNaziv { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string? Podnaslov { get; set; }
    public string? ImePrezimeKreatora { get; set; }
    public DateTime? DatumPrijave { get; set; }
}

public class AutorizacijaViewModel
{
    public List<PosjetNaCekanjuViewModel> PosjetiNaCekanju { get; set; } = new();
    public List<EntitetNaCekanjuViewModel> EntitetiNaCekanju { get; set; } = new();

    // Planinar smije autorizirati samo posjete; sekcija prijedloga sadržaja je vidljiva samo Adminu.
    public bool PrikaziPrijedlogeSadrzaja { get; set; }
}
