using System.ComponentModel.DataAnnotations;

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

    public virtual ICollection<KontrolnaTocka> KontrolneTocke { get; set; } = new List<KontrolnaTocka>();
    public virtual ICollection<PlaninarskiObjekt> PlaninarskiObjekti { get; set; } = new List<PlaninarskiObjekt>();
}
