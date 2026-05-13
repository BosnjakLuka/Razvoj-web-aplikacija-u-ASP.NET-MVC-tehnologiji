using System.ComponentModel.DataAnnotations;

namespace planinarenje.Models.ViewModels;

public class KnjizicaCreateModel
{
    [Required(ErrorMessage = "Korisnik je obavezan.")]
    public int IdKorisnik { get; set; }

    [StringLength(500, ErrorMessage = "Napomena može imati najviše 500 znakova.")]
    public string? Napomena { get; set; }
}

public class KnjizicaEditModel : KnjizicaCreateModel
{
}
