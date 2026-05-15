using System.ComponentModel.DataAnnotations;
using planinarenje.Entiteti;

namespace planinarenje.Models.ViewModels;

public class PlaninarskiObjektCreateModel
{
    [Required(ErrorMessage = "Naziv je obavezan.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Naziv mora imati između 2 i 150 znakova.")]
    public string Naziv { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tip objekta je obavezan.")]
    public TipObjekta TipObjekta { get; set; }

    [Required(ErrorMessage = "Podrucje je obavezno.")]
    public int IdPodrucje { get; set; }

    [Required(ErrorMessage = "Udruga je obavezna.")]
    public int IdPlaninarskaUdruga { get; set; }

    [Range(0, 9000, ErrorMessage = "Nadmorska visina mora biti između 0 i 9000.")]
    public int? NadmorskaVisina { get; set; }

    [Range(0, 1000, ErrorMessage = "Kapacitet mora biti između 0 i 1000.")]
    public int? Kapacitet { get; set; }

    [StringLength(500, ErrorMessage = "Opis može imati najviše 500 znakova.")]
    public string? Opis { get; set; }

    [StringLength(150, ErrorMessage = "Ime odgovorne osobe može imati najviše 150 znakova.")]
    public string? ImeOdgovorneOsobe { get; set; }

    [StringLength(30, ErrorMessage = "Telefon može imati najviše 30 znakova.")]
    [Phone(ErrorMessage = "Telefon nije u ispravnom formatu.")]
    public string? Telefon { get; set; }

    [StringLength(150, ErrorMessage = "Email može imati najviše 150 znakova.")]
    [EmailAddress(ErrorMessage = "Email nije u ispravnom formatu.")]
    public string? Email { get; set; }

    [StringLength(255, ErrorMessage = "Adresa može imati najviše 255 znakova.")]
    public string? Adresa { get; set; }

    public bool ImaNocenje { get; set; }
    public bool ImaHranu { get; set; }

    [StringLength(500, ErrorMessage = "Radno vrijeme može imati najviše 500 znakova.")]
    public string? RadnoVrijemeOpis { get; set; }
}

public class PlaninarskiObjektEditModel : PlaninarskiObjektCreateModel
{
}
