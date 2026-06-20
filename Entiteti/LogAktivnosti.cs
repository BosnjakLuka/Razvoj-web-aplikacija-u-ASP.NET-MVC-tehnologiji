using System.ComponentModel.DataAnnotations;

namespace planinarenje.Entiteti;

// Audit log zapis - automatski popunjen u PlaninarstvoDbContext.SaveChanges/SaveChangesAsync
// za svaku Added/Modified/Deleted promjenu nad pratim entitetima.
public class LogAktivnosti
{
    [Key]
    public int IdLogAktivnosti { get; set; }

    [Required]
    public DateTime VrijemeDogadaja { get; set; }

    [Required]
    [MaxLength(450)]
    public string AppUserId { get; set; } = string.Empty;

    [MaxLength(256)]
    public string? KorisnickoIme { get; set; }

    [Required]
    [MaxLength(100)]
    public string NazivEntiteta { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string IdEntiteta { get; set; } = string.Empty;

    [Required]
    public TipAkcijeLoga Akcija { get; set; }

    // Za Kreirano/Obrisano: čitljiv sažetak entiteta.
    // Za Azurirano: lista promijenjenih polja u obliku "Polje: 'staro' -> 'novo'; ...".
    public string? Detalji { get; set; }
}
