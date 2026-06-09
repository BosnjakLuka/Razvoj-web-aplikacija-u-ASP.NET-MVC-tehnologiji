using System.ComponentModel.DataAnnotations;
using planinarenje.Entiteti;

namespace planinarenje.Models.Dto.PlaninarskiObjekt;

public class PlaninarskiObjektDto
{
    public int IdPlaninarskiObjekt { get; set; }
    public int IdPodrucje { get; set; }
    public string NazivPodrucja { get; set; } = string.Empty;
    public int IdPlaninarskaUdruga { get; set; }
    public string NazivUdruge { get; set; } = string.Empty;
    public string Naziv { get; set; } = string.Empty;
    public string TipObjekta { get; set; } = string.Empty;
    public int? NadmorskaVisina { get; set; }
    public int? Kapacitet { get; set; }
    public string? Opis { get; set; }
    public string? ImeOdgovorneOsobe { get; set; }
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public string? Adresa { get; set; }
    public bool ImaNocenje { get; set; }
    public bool ImaHranu { get; set; }
    public string? RadnoVrijemeOpis { get; set; }
}

public class PlaninarskiObjektCreateDto
{
    [Required(ErrorMessage = "Područje je obavezno.")]
    public int IdPodrucje { get; set; }

    [Required(ErrorMessage = "Planinarska udruga je obavezna.")]
    public int IdPlaninarskaUdruga { get; set; }

    [Required(ErrorMessage = "Naziv je obavezan.")]
    [MaxLength(150, ErrorMessage = "Naziv ne smije biti duži od 150 znakova.")]
    public string Naziv { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tip objekta je obavezan.")]
    public TipObjekta TipObjekta { get; set; }

    [Range(0, 9000, ErrorMessage = "Nadmorska visina mora biti između 0 i 9000 m.")]
    public int? NadmorskaVisina { get; set; }

    [Range(0, 5000, ErrorMessage = "Kapacitet mora biti između 0 i 5000.")]
    public int? Kapacitet { get; set; }

    public string? Opis { get; set; }

    [MaxLength(150, ErrorMessage = "Ime odgovorne osobe ne smije biti duže od 150 znakova.")]
    public string? ImeOdgovorneOsobe { get; set; }

    [MaxLength(30, ErrorMessage = "Telefon ne smije biti duži od 30 znakova.")]
    public string? Telefon { get; set; }

    [EmailAddress(ErrorMessage = "Email nije u ispravnom formatu.")]
    [MaxLength(150, ErrorMessage = "Email ne smije biti duži od 150 znakova.")]
    public string? Email { get; set; }

    [MaxLength(255, ErrorMessage = "Adresa ne smije biti duža od 255 znakova.")]
    public string? Adresa { get; set; }

    public bool ImaNocenje { get; set; }
    public bool ImaHranu { get; set; }
    public string? RadnoVrijemeOpis { get; set; }
}

public class PlaninarskiObjektUpdateDto : PlaninarskiObjektCreateDto
{
}
