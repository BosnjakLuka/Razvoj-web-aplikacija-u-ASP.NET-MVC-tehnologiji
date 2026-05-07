using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace planinarenje.Entiteti;

public class Posjet
{
    [Key]
    public int IdPosjet { get; set; }

    [ForeignKey("Korisnik")]
    public int IdKorisnik { get; set; }

    [ForeignKey("Knjizica")]
    public int IdKnjizica { get; set; }

    [ForeignKey("KontrolnaTocka")]
    public int IdKontrolnaTocka { get; set; }

    [ForeignKey("Ruta")]
    public int IdRuta { get; set; }

    public DateTime DatumVrijemePosjeta { get; set; }
    public int? VrijemeUsponaMin { get; set; }
    public DozivljajPosjeta DozivljajPosjeta { get; set; }
    public string? OpisIskustva { get; set; }

    [Required]
    [MaxLength(100)]
    public string UneseniGUID { get; set; } = string.Empty;

    public bool JeLiPotvrdenPosjet { get; set; }
    public DateTime DatumKreiranjaZapisa { get; set; }

    public virtual Korisnik Korisnik { get; set; } = null!;
    public virtual Knjizica Knjizica { get; set; } = null!;
    public virtual KontrolnaTocka KontrolnaTocka { get; set; } = null!;
    public virtual Ruta Ruta { get; set; } = null!;
    public virtual ICollection<Fotografija> Fotografije { get; set; } = new List<Fotografija>();
}
