using System.ComponentModel.DataAnnotations;

namespace planinarenje.Models.Dto.Medalja;

public class MedaljaDto
{
    public int IdMedalja { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string? Opis { get; set; }
    public int MinimalanBrojKontrolnihTocaka { get; set; }
    public int MinimalanBrojPodrucja { get; set; }
}

public class MedaljaCreateDto
{
    [Required(ErrorMessage = "Naziv je obavezan.")]
    [MaxLength(100, ErrorMessage = "Naziv ne smije biti duži od 100 znakova.")]
    public string Naziv { get; set; } = string.Empty;

    public string? Opis { get; set; }

    [Range(0, 1000, ErrorMessage = "Minimalan broj kontrolnih točaka mora biti između 0 i 1000.")]
    public int MinimalanBrojKontrolnihTocaka { get; set; }

    [Range(0, 100, ErrorMessage = "Minimalan broj područja mora biti između 0 i 100.")]
    public int MinimalanBrojPodrucja { get; set; }
}

public class MedaljaUpdateDto : MedaljaCreateDto
{
}
