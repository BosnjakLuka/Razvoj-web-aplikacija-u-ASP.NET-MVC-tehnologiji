using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace planinarenje.Entiteti;

public class Ruta
{
    [Key]
    public int IdRuta { get; set; }

    [ForeignKey("KontrolnaTocka")]
    public int IdKontrolnaTocka { get; set; }

    [Required]
    [MaxLength(200)]
    public string Naziv { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Pocetak { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Kraj { get; set; } = string.Empty;

    public int VrijemeHodaMin { get; set; }
    public decimal DuljinaKm { get; set; }
    public int? VisinskaRazlikaM { get; set; }
    public string? Opis { get; set; }

    [MaxLength(50)]
    public string? OznakaNaTerenu { get; set; }

    public int? GodinaObnove { get; set; }
    public string? Napomena { get; set; }
    public TezinaRute TezinaRute { get; set; }

    [MaxLength(255)]
    public string? GPXPath { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual KontrolnaTocka KontrolnaTocka { get; set; } = null!;
    public virtual ICollection<Posjet> Posjeti { get; set; } = new List<Posjet>();
}
