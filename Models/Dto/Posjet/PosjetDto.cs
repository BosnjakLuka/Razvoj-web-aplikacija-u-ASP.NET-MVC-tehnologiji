namespace planinarenje.Models.Dto.Posjet;

public class PosjetDto
{
    public int IdPosjet { get; set; }
    public int IdKorisnik { get; set; }
    public string ImePrezimeKorisnika { get; set; } = string.Empty;
    public int IdKnjizica { get; set; }
    public int IdKontrolnaTocka { get; set; }
    public string NazivKontrolneTocke { get; set; } = string.Empty;
    public int IdRuta { get; set; }
    public string NazivRute { get; set; } = string.Empty;
    public DateTime DatumVrijemePosjeta { get; set; }
    public int? VrijemeUsponaMin { get; set; }
    public string DozivljajPosjeta { get; set; } = string.Empty;
    public string? OpisIskustva { get; set; }
    public string UneseniGUID { get; set; } = string.Empty;
    public bool JeLiPotvrdenPosjet { get; set; }
    public DateTime DatumKreiranjaZapisa { get; set; }
    public List<FotografijaSummaryDto> Fotografije { get; set; } = new();
}

public class FotografijaSummaryDto
{
    public int IdFotografija { get; set; }
    public string NazivDatoteke { get; set; } = string.Empty;
    public string PutanjaDatoteke { get; set; } = string.Empty;
    public string? Opis { get; set; }
}
