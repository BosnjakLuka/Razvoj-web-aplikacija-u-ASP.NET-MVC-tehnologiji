using System.ComponentModel.DataAnnotations;

namespace planinarenje.Models.Dto.Podrucje;

public class PodrucjeDto
{
    public int IdPodrucje { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string? Opis { get; set; }
    public string? Regija { get; set; }
    public int MinimalanBrojKTZaObilazak { get; set; }
    public int BrojKontrolnihTocaka { get; set; }
}

public class PodrucjeCreateDto
{
    [Required(ErrorMessage = "Naziv je obavezan.")]
    [MaxLength(150, ErrorMessage = "Naziv ne smije biti duži od 150 znakova.")]
    public string Naziv { get; set; } = string.Empty;

    public string? Opis { get; set; }

    [MaxLength(150, ErrorMessage = "Regija ne smije biti duža od 150 znakova.")]
    public string? Regija { get; set; }

    [Range(0, 100, ErrorMessage = "Minimalan broj KT mora biti između 0 i 100.")]
    public int MinimalanBrojKTZaObilazak { get; set; }
}

public class PodrucjeUpdateDto : PodrucjeCreateDto
{
}
