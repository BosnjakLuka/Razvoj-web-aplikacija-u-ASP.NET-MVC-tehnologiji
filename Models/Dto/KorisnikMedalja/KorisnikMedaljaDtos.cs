using System.ComponentModel.DataAnnotations;

namespace planinarenje.Models.Dto.KorisnikMedalja;

public class KorisnikMedaljaDto
{
    public int IdKorisnikMedalja { get; set; }
    public int IdKorisnik { get; set; }
    public string ImePrezimeKorisnika { get; set; } = string.Empty;
    public int IdMedalja { get; set; }
    public string NazivMedalje { get; set; } = string.Empty;
    public DateTime DatumDodjele { get; set; }
    public string? Napomena { get; set; }
}

public class KorisnikMedaljaCreateDto
{
    [Required(ErrorMessage = "Korisnik je obavezan.")]
    public int IdKorisnik { get; set; }

    [Required(ErrorMessage = "Medalja je obavezna.")]
    public int IdMedalja { get; set; }

    [Required(ErrorMessage = "Datum dodjele je obavezan.")]
    public DateTime DatumDodjele { get; set; }

    public string? Napomena { get; set; }
}

public class KorisnikMedaljaUpdateDto
{
    [Required(ErrorMessage = "Datum dodjele je obavezan.")]
    public DateTime DatumDodjele { get; set; }

    public string? Napomena { get; set; }
}
