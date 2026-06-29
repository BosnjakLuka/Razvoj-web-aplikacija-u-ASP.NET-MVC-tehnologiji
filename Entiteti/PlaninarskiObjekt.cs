using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace planinarenje.Entiteti;

public class PlaninarskiObjekt
{
    [Key]
    public int IdPlaninarskiObjekt { get; set; }

    [ForeignKey("Podrucje")]
    public int IdPodrucje { get; set; }

    [ForeignKey("PlaninarskaUdruga")]
    public int IdPlaninarskaUdruga { get; set; }

    [Required]
    [MaxLength(150)]
    public string Naziv { get; set; } = string.Empty;

    public TipObjekta TipObjekta { get; set; }
    public int? NadmorskaVisina { get; set; }
    public int? Kapacitet { get; set; }
    public string? Opis { get; set; }

    [MaxLength(150)]
    public string? ImeOdgovorneOsobe { get; set; }

    [MaxLength(30)]
    public string? Telefon { get; set; }

    [MaxLength(150)]
    public string? Email { get; set; }

    [MaxLength(255)]
    public string? Adresa { get; set; }

    public bool ImaNocenje { get; set; }
    public bool ImaHranu { get; set; }
    public string? RadnoVrijemeOpis { get; set; }

    public DateTime? DeletedAt { get; set; }

    // Odobravanje sadržaja kreiranog/uređenog od strane Planinar role — vidi AutorizacijaController.
    public bool JeOdobreno { get; set; } = true;
    public int? IdKreator { get; set; }
    public DateTime? DatumPrijave { get; set; }

    public virtual Podrucje Podrucje { get; set; } = null!;
    public virtual PlaninarskaUdruga PlaninarskaUdruga { get; set; } = null!;

    [ForeignKey("IdKreator")]
    public virtual Korisnik? Kreator { get; set; }
}
