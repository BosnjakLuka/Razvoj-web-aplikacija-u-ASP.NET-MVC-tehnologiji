using System.ComponentModel.DataAnnotations;
using planinarenje.Entiteti;

namespace planinarenje.Models.ViewModels;

public class KontrolnaTockaCreateModel
{
    [Required(ErrorMessage = "Naziv je obavezan.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Naziv mora imati između 2 i 150 znakova.")]
    public string Naziv { get; set; } = string.Empty;

    [Required(ErrorMessage = "GUID oznaka je obavezna.")]
    [StringLength(100, ErrorMessage = "GUID oznaka može imati najviše 100 znakova.")]
    public string GUIDOznaka { get; set; } = string.Empty;

    [Required(ErrorMessage = "Podrucje je obavezno.")]
    public int IdPodrucje { get; set; }

    [Required(ErrorMessage = "Tip kontrolne tocke je obavezan.")]
    public TipKontrolneTocke TipKontrolneTocke { get; set; }

    [Range(0, 9000, ErrorMessage = "Nadmorska visina mora biti između 0 i 9000.")]
    public int? NadmorskaVisina { get; set; }

    [StringLength(500, ErrorMessage = "Opis može imati najviše 500 znakova.")]
    public string? Opis { get; set; }

    [StringLength(100, ErrorMessage = "Koordinate mogu imati najviše 100 znakova.")]
    public string? Koordinate { get; set; }

    [StringLength(500, ErrorMessage = "Opis ziga može imati najviše 500 znakova.")]
    public string? OpisZiga { get; set; }
}

public class KontrolnaTockaEditModel : KontrolnaTockaCreateModel
{
}
