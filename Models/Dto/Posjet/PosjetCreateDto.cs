using System.ComponentModel.DataAnnotations;
using planinarenje.Entiteti;

namespace planinarenje.Models.Dto.Posjet;

public class PosjetCreateDto
{
    [Required(ErrorMessage = "Knjižica je obavezna.")]
    public int IdKnjizica { get; set; }

    [Required(ErrorMessage = "Kontrolna točka je obavezna.")]
    public int IdKontrolnaTocka { get; set; }

    [Required(ErrorMessage = "Ruta je obavezna.")]
    public int IdRuta { get; set; }

    [Required(ErrorMessage = "Datum posjeta je obavezan.")]
    public DateTime DatumVrijemePosjeta { get; set; }

    [Range(1, 2000, ErrorMessage = "Vrijeme uspona mora biti između 1 i 2000 minuta.")]
    public int? VrijemeUsponaMin { get; set; }

    [Required(ErrorMessage = "Doživljaj posjeta je obavezan.")]
    public DozivljajPosjeta DozivljajPosjeta { get; set; }

    [MaxLength(2000, ErrorMessage = "Opis iskustva ne smije biti duži od 2000 znakova.")]
    public string? OpisIskustva { get; set; }

    [Required(ErrorMessage = "GUID oznaka je obavezna.")]
    [MaxLength(100, ErrorMessage = "GUID ne smije biti duži od 100 znakova.")]
    public string UneseniGUID { get; set; } = string.Empty;
}
