using System.ComponentModel.DataAnnotations;

namespace planinarenje.Models.ViewModels;

public class MedaljaCreateModel
{
    [Required(ErrorMessage = "Naziv je obavezan.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Naziv mora imati između 2 i 100 znakova.")]
    public string Naziv { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Opis može imati najviše 500 znakova.")]
    public string? Opis { get; set; }

    [Required(ErrorMessage = "Minimalan broj KT je obavezan.")]
    [Range(1, 1000, ErrorMessage = "Minimalan broj KT mora biti između 1 i 1000.")]
    public int MinimalanBrojKontrolnihTocaka { get; set; }

    [Required(ErrorMessage = "Minimalan broj podrucja je obavezan.")]
    [Range(1, 100, ErrorMessage = "Minimalan broj podrucja mora biti između 1 i 100.")]
    public int MinimalanBrojPodrucja { get; set; }
}

public class MedaljaEditModel : MedaljaCreateModel
{
}
