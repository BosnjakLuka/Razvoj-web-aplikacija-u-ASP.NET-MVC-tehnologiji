using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace planinarenje.Entiteti;

public class Obavijest
{
    [Key]
    public int IdObavijest { get; set; }

    [Required]
    [MaxLength(200)]
    public string Naslov { get; set; } = string.Empty;

    public string? Sadrzaj { get; set; }

    [Required]
    public DateTime DatumObjave { get; set; }

    [Required]
    public bool JeAktivna { get; set; }

    [ForeignKey("Korisnik")]
    public int IdKorisnik { get; set; }

    public virtual Korisnik Korisnik { get; set; } = null!;
}
