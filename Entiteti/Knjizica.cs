using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace planinarenje.Entiteti;

public class Knjizica
{
    [Key]
    public int IdKnjizica { get; set; }

    [ForeignKey("Korisnik")]
    public int IdKorisnik { get; set; }

    public DateTime DatumKreiranja { get; set; }
    public string? Napomena { get; set; }
    public bool StatusAktivna { get; set; }

    public virtual Korisnik Korisnik { get; set; } = null!;
    public virtual ICollection<Posjet> Posjeti { get; set; } = new List<Posjet>();
}
