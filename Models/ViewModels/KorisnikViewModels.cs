using System.ComponentModel.DataAnnotations;

namespace planinarenje.Models.ViewModels;

public class KorisnikCreateModel
{
    [Required(ErrorMessage = "Ime je obavezno.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Ime mora imati između 2 i 100 znakova.")]
    public string Ime { get; set; } = string.Empty;

    [Required(ErrorMessage = "Prezime je obavezno.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Prezime mora imati između 2 i 100 znakova.")]
    public string Prezime { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email je obavezan.")]
    [StringLength(150, ErrorMessage = "Email može imati najviše 150 znakova.")]
    [EmailAddress(ErrorMessage = "Email nije u ispravnom formatu.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Korisnicko ime je obavezno.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Korisnicko ime mora imati između 2 i 100 znakova.")]
    public string KorisnickoIme { get; set; } = string.Empty;

    [StringLength(30, ErrorMessage = "Broj mobitela može imati najviše 30 znakova.")]
    [Phone(ErrorMessage = "Broj mobitela nije u ispravnom formatu.")]
    public string? BrojMobitela { get; set; }

    public DateTime? DatumRodenja { get; set; }
}

public class KorisnikEditModel
{
    [Required(ErrorMessage = "Ime je obavezno.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Ime mora imati između 2 i 100 znakova.")]
    public string Ime { get; set; } = string.Empty;

    [Required(ErrorMessage = "Prezime je obavezno.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Prezime mora imati između 2 i 100 znakova.")]
    public string Prezime { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email je obavezan.")]
    [StringLength(150, ErrorMessage = "Email može imati najviše 150 znakova.")]
    [EmailAddress(ErrorMessage = "Email nije u ispravnom formatu.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Korisnicko ime je obavezno.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Korisnicko ime mora imati između 2 i 100 znakova.")]
    public string KorisnickoIme { get; set; } = string.Empty;

    [StringLength(30, ErrorMessage = "Broj mobitela može imati najviše 30 znakova.")]
    [Phone(ErrorMessage = "Broj mobitela nije u ispravnom formatu.")]
    public string? BrojMobitela { get; set; }

    public DateTime? DatumRodenja { get; set; }
}
