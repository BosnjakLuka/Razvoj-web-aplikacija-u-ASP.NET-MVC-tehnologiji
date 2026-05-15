using System.ComponentModel.DataAnnotations;

namespace planinarenje.Models.ViewModels;

public class KorisnikMedaljaCreateModel
{
    [Required(ErrorMessage = "Korisnik je obavezan.")]
    public int IdKorisnik { get; set; }

    [Required(ErrorMessage = "Medalja je obavezna.")]
    public int IdMedalja { get; set; }

    [Required(ErrorMessage = "Datum dodjele je obavezan.")]
    public DateTime DatumDodjele { get; set; }

    [StringLength(500, ErrorMessage = "Napomena može imati najviše 500 znakova.")]
    public string? Napomena { get; set; }
}

public class KorisnikMedaljaEditModel : KorisnikMedaljaCreateModel
{
}
