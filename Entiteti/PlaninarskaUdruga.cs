using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace planinarenje.Entiteti;

public class PlaninarskaUdruga
{
    [Key]
    public int IdPlaninarskaUdruga { get; set; }

    [Required]
    [MaxLength(11)]
    public string OIB { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Naziv { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? Email { get; set; }

    [MaxLength(30)]
    public string? BrojTelefona { get; set; }

    [MaxLength(255)]
    public string? Adresa { get; set; }

    [MaxLength(20)]
    public string? PostanskiBroj { get; set; }

    [MaxLength(100)]
    public string? Grad { get; set; }

    [MaxLength(100)]
    public string? Zupanija { get; set; }

    public int? BrojClanova { get; set; }

    public DateTime? DeletedAt { get; set; }

    // Odobravanje sadržaja kreiranog/uređenog od strane Planinar role — vidi AutorizacijaController.
    public bool JeOdobreno { get; set; } = true;
    public int? IdKreator { get; set; }
    public DateTime? DatumPrijave { get; set; }

    [ForeignKey("IdKreator")]
    public virtual Korisnik? Kreator { get; set; }

    public virtual ICollection<PlaninarskiObjekt> PlaninarskiObjekti { get; set; } = new List<PlaninarskiObjekt>();
}
