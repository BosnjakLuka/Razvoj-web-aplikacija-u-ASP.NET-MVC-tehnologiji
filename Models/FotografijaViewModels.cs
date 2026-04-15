using planinarenje.Entiteti;

namespace planinarenje.Models;

public class FotografijaIndexViewModel
{
    public int IdFotografija { get; set; }
    public string NazivDatoteke { get; set; } = string.Empty;
    public int IdPosjet { get; set; }
    public string PosjetNaslov { get; set; } = string.Empty;
    public DateTime DatumUploada { get; set; }
    public string TipSlike { get; set; } = string.Empty;
}

public class FotografijaDetailsViewModel
{
    public int IdFotografija { get; set; }
    public string NazivDatoteke { get; set; } = string.Empty;
    public string PutanjaDatoteke { get; set; } = string.Empty;
    public DateTime DatumUploada { get; set; }
    public string TipSlike { get; set; } = string.Empty;
    public string? Opis { get; set; }
    
    public int IdPosjet { get; set; }
    public string PosjetNaslov { get; set; } = string.Empty;
}