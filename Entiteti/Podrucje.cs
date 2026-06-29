using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace planinarenje.Entiteti;

public class Podrucje
{
    [Key]
    public int IdPodrucje { get; set; }

    [Required]
    [MaxLength(150)]
    public string Naziv { get; set; } = string.Empty;

    public string? Opis { get; set; }

    [MaxLength(150)]
    public string? Regija { get; set; }

    public int MinimalanBrojKTZaObilazak { get; set; }

    public DateTime? DeletedAt { get; set; }

    // Odobravanje sadržaja kreiranog/uređenog od strane Planinar role — vidi AutorizacijaController.
    public bool JeOdobreno { get; set; } = true;
    public int? IdKreator { get; set; }
    public DateTime? DatumPrijave { get; set; }

    [ForeignKey("IdKreator")]
    public virtual Korisnik? Kreator { get; set; }

    public virtual ICollection<KontrolnaTocka> KontrolneTocke { get; set; } = new List<KontrolnaTocka>();
    public virtual ICollection<PlaninarskiObjekt> PlaninarskiObjekti { get; set; } = new List<PlaninarskiObjekt>();
}
