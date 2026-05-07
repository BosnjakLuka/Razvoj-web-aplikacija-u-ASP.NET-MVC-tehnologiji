using System.ComponentModel.DataAnnotations;

namespace planinarenje.Entiteti;

public class Korisnik
{
    [Key]
    public int IdKorisnik { get; set; }

    [Required]
    [MaxLength(100)]
    public string Ime { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Prezime { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string KorisnickoIme { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime? DatumRodenja { get; set; }
    public DateTime DatumRegistracije { get; set; }

    [MaxLength(30)]
    public string? BrojMobitela { get; set; }

    [MaxLength(255)]
    public string? ProfilnaSlika { get; set; }

    public bool StatusAktivan { get; set; }

    public virtual Knjizica Knjizica { get; set; } = null!;
    public virtual ICollection<Posjet> Posjeti { get; set; } = new List<Posjet>();
    public virtual ICollection<KorisnikMedalja> KorisnikMedalje { get; set; } = new List<KorisnikMedalja>();
}
