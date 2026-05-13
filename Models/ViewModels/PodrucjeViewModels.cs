using System.ComponentModel.DataAnnotations;

namespace planinarenje.Models.ViewModels;

public class PodrucjeCreateModel
{
    [Required(ErrorMessage = "Naziv je obavezan.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Naziv mora imati između 2 i 150 znakova.")]
    public string Naziv { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Opis može imati najviše 500 znakova.")]
    public string? Opis { get; set; }

    [StringLength(150, ErrorMessage = "Regija može imati najviše 150 znakova.")]
    public string? Regija { get; set; }

    [Required(ErrorMessage = "Minimalan broj KT je obavezan.")]
    [Range(1, 100, ErrorMessage = "Minimalan broj KT mora biti između 1 i 100.")]
    public int MinimalanBrojKTZaObilazak { get; set; }
}

public class PodrucjeEditModel : PodrucjeCreateModel
{
}
