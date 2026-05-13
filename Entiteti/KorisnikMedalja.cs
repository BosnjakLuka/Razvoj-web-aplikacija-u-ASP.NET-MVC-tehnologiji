using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace planinarenje.Entiteti;

public class KorisnikMedalja
{
    [Key]
    public int IdKorisnikMedalja { get; set; }

    [ForeignKey("Korisnik")]
    public int IdKorisnik { get; set; }

    [ForeignKey("Medalja")]
    public int IdMedalja { get; set; }

    public DateTime DatumDodjele { get; set; }
    public string? Napomena { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Korisnik Korisnik { get; set; } = null!;
    public virtual Medalja Medalja { get; set; } = null!;
}
