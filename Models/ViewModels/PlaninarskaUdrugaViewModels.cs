using System.ComponentModel.DataAnnotations;

namespace planinarenje.Models.ViewModels;

public class PlaninarskaUdrugaCreateModel
{
    [Required(ErrorMessage = "OIB je obavezan.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "OIB mora imati tocno 11 znakova.")]
    public string OIB { get; set; } = string.Empty;

    [Required(ErrorMessage = "Naziv je obavezan.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Naziv mora imati između 2 i 150 znakova.")]
    public string Naziv { get; set; } = string.Empty;

    [StringLength(150, ErrorMessage = "Email može imati najviše 150 znakova.")]
    [EmailAddress(ErrorMessage = "Email nije u ispravnom formatu.")]
    public string? Email { get; set; }

    [StringLength(30, ErrorMessage = "Broj telefona može imati najviše 30 znakova.")]
    [Phone(ErrorMessage = "Broj telefona nije u ispravnom formatu.")]
    public string? BrojTelefona { get; set; }

    [StringLength(255, ErrorMessage = "Adresa može imati najviše 255 znakova.")]
    public string? Adresa { get; set; }

    [StringLength(20, ErrorMessage = "Postanski broj može imati najviše 20 znakova.")]
    public string? PostanskiBroj { get; set; }

    [StringLength(100, ErrorMessage = "Grad može imati najviše 100 znakova.")]
    public string? Grad { get; set; }

    [StringLength(100, ErrorMessage = "Zupanija može imati najviše 100 znakova.")]
    public string? Zupanija { get; set; }

    [Range(0, 100000, ErrorMessage = "Broj clanova mora biti između 0 i 100000.")]
    public int? BrojClanova { get; set; }
}

public class PlaninarskaUdrugaEditModel : PlaninarskaUdrugaCreateModel
{
}
