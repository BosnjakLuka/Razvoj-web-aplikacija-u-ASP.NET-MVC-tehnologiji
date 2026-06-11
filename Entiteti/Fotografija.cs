using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace planinarenje.Entiteti;

public class Fotografija
{
    [Key]
    public int IdFotografija { get; set; }

    [ForeignKey("Posjet")]
    public int IdPosjet { get; set; }

    [Required]
    [MaxLength(255)]
    public string NazivDatoteke { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PutanjaDatoteke { get; set; } = string.Empty;

    public DateTime DatumUploada { get; set; }
    public TipSlike TipSlike { get; set; }
    public string? Opis { get; set; }

    [StringLength(100)]
    public string? ContentType { get; set; }

    public long FileSize { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Posjet Posjet { get; set; } = null!;
}
