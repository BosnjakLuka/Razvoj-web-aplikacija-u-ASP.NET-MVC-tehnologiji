using System.ComponentModel.DataAnnotations;
using planinarenje.Entiteti;

namespace planinarenje.Models.Dto.KontrolnaTocka;

public class KontrolnaTockaDto
{
    public int IdKontrolnaTocka { get; set; }
    public string GUIDOznaka { get; set; } = string.Empty;
    public int IdPodrucje { get; set; }
    public string NazivPodrucja { get; set; } = string.Empty;
    public string Naziv { get; set; } = string.Empty;
    public string TipKontrolneTocke { get; set; } = string.Empty;
    public int? NadmorskaVisina { get; set; }
    public string? Opis { get; set; }
    public string? Koordinate { get; set; }
    public string? OpisZiga { get; set; }
}

public class KontrolnaTockaCreateDto
{
    [Required(ErrorMessage = "GUID oznaka je obavezna.")]
    [MaxLength(100, ErrorMessage = "GUID oznaka ne smije biti duža od 100 znakova.")]
    public string GUIDOznaka { get; set; } = string.Empty;

    [Required(ErrorMessage = "Područje je obavezno.")]
    public int IdPodrucje { get; set; }

    [Required(ErrorMessage = "Naziv je obavezan.")]
    [MaxLength(150, ErrorMessage = "Naziv ne smije biti duži od 150 znakova.")]
    public string Naziv { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tip kontrolne točke je obavezan.")]
    public TipKontrolneTocke TipKontrolneTocke { get; set; }

    [Range(0, 9000, ErrorMessage = "Nadmorska visina mora biti između 0 i 9000 m.")]
    public int? NadmorskaVisina { get; set; }

    public string? Opis { get; set; }

    [MaxLength(100, ErrorMessage = "Koordinate ne smiju biti duže od 100 znakova.")]
    public string? Koordinate { get; set; }

    public string? OpisZiga { get; set; }
}

public class KontrolnaTockaUpdateDto : KontrolnaTockaCreateDto
{
}
