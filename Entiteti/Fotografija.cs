namespace planinarenje.Entiteti;

public class Fotografija
{
    public int IdFotografija { get; set; }
    public int IdPosjet { get; set; }
    public string NazivDatoteke { get; set; } = string.Empty;
    public string PutanjaDatoteke { get; set; } = string.Empty;
    public DateTime DatumUploada { get; set; }
    public TipSlike TipSlike { get; set; }
    public string? Opis { get; set; }

    public Posjet? Posjet { get; set; }
}
