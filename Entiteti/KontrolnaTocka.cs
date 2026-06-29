using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace planinarenje.Entiteti;

public class KontrolnaTocka
{
    [Key]
    public int IdKontrolnaTocka { get; set; }

    [Required]
    [MaxLength(100)]
    public string GUIDOznaka { get; set; } = string.Empty;

    [ForeignKey("Podrucje")]
    public int IdPodrucje { get; set; }

    [Required]
    [MaxLength(150)]
    public string Naziv { get; set; } = string.Empty;

    public TipKontrolneTocke TipKontrolneTocke { get; set; }
    public int? NadmorskaVisina { get; set; }
    public string? Opis { get; set; }

    [MaxLength(100)]
    public string? Koordinate { get; set; }

    public string? OpisZiga { get; set; }

    public DateTime? DeletedAt { get; set; }

    // Odobravanje sadržaja kreiranog/uređenog od strane Planinar role — vidi AutorizacijaController.
    public bool JeOdobreno { get; set; } = true;
    public int? IdKreator { get; set; }
    public DateTime? DatumPrijave { get; set; }

    public virtual Podrucje Podrucje { get; set; } = null!;

    [ForeignKey("IdKreator")]
    public virtual Korisnik? Kreator { get; set; }

    public virtual ICollection<Posjet> Posjeti { get; set; } = new List<Posjet>();
    public virtual ICollection<Ruta> Rute { get; set; } = new List<Ruta>();
}
