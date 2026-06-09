using System.ComponentModel.DataAnnotations;

namespace planinarenje.Models.Dto.PlaninarskaUdruga;

public class PlaninarskaUdrugaDto
{
    public int IdPlaninarskaUdruga { get; set; }
    public string OIB { get; set; } = string.Empty;
    public string Naziv { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? BrojTelefona { get; set; }
    public string? Adresa { get; set; }
    public string? PostanskiBroj { get; set; }
    public string? Grad { get; set; }
    public string? Zupanija { get; set; }
    public int? BrojClanova { get; set; }
}

public class PlaninarskaUdrugaCreateDto
{
    [Required(ErrorMessage = "OIB je obavezan.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "OIB mora imati točno 11 znamenki.")]
    [RegularExpression("^[0-9]{11}$", ErrorMessage = "OIB smije sadržavati samo znamenke.")]
    public string OIB { get; set; } = string.Empty;

    [Required(ErrorMessage = "Naziv je obavezan.")]
    [MaxLength(150, ErrorMessage = "Naziv ne smije biti duži od 150 znakova.")]
    public string Naziv { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email nije u ispravnom formatu.")]
    [MaxLength(150, ErrorMessage = "Email ne smije biti duži od 150 znakova.")]
    public string? Email { get; set; }

    [MaxLength(30, ErrorMessage = "Broj telefona ne smije biti duži od 30 znakova.")]
    public string? BrojTelefona { get; set; }

    [MaxLength(255, ErrorMessage = "Adresa ne smije biti duža od 255 znakova.")]
    public string? Adresa { get; set; }

    [MaxLength(20, ErrorMessage = "Poštanski broj ne smije biti duži od 20 znakova.")]
    public string? PostanskiBroj { get; set; }

    [MaxLength(100, ErrorMessage = "Grad ne smije biti duži od 100 znakova.")]
    public string? Grad { get; set; }

    [MaxLength(100, ErrorMessage = "Županija ne smije biti duža od 100 znakova.")]
    public string? Zupanija { get; set; }

    [Range(0, 100000, ErrorMessage = "Broj članova mora biti između 0 i 100000.")]
    public int? BrojClanova { get; set; }
}

public class PlaninarskaUdrugaUpdateDto : PlaninarskaUdrugaCreateDto
{
}
