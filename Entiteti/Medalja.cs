using System.ComponentModel.DataAnnotations;

namespace planinarenje.Entiteti;

public class Medalja
{
    [Key]
    public int IdMedalja { get; set; }

    [Required]
    [MaxLength(100)]
    public string Naziv { get; set; } = string.Empty;

    public string? Opis { get; set; }
    public int MinimalanBrojKontrolnihTocaka { get; set; }
    public int MinimalanBrojPodrucja { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<KorisnikMedalja> KorisnikMedalje { get; set; } = new List<KorisnikMedalja>();
}
