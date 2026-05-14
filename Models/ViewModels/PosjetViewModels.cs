using System.ComponentModel.DataAnnotations;
using planinarenje.Entiteti;

namespace planinarenje.Models.ViewModels;

public class PosjetCreateModel
{
    [Required(ErrorMessage = "Korisnik je obavezan.")]
    [Range(1, int.MaxValue, ErrorMessage = "Korisnik je obavezan.")]
    public int IdKorisnik { get; set; }

    [Required(ErrorMessage = "Knjizica je obavezna.")]
    [Range(1, int.MaxValue, ErrorMessage = "Knjizica je obavezna.")]
    public int IdKnjizica { get; set; }

    [Required(ErrorMessage = "Kontrolna tocka je obavezna.")]
    [Range(1, int.MaxValue, ErrorMessage = "Kontrolna tocka je obavezna.")]
    public int IdKontrolnaTocka { get; set; }

    [Required(ErrorMessage = "Ruta je obavezna.")]
    [Range(1, int.MaxValue, ErrorMessage = "Ruta je obavezna.")]
    public int IdRuta { get; set; }

    [Required(ErrorMessage = "Datum i vrijeme posjeta su obavezni.")]
    public DateTime DatumVrijemePosjeta { get; set; }

    [Range(0, 10000, ErrorMessage = "Vrijeme uspona mora biti između 0 i 10000 minuta.")]
    public int? VrijemeUsponaMin { get; set; }

    [Required(ErrorMessage = "Dozivljaj je obavezan.")]
    public DozivljajPosjeta DozivljajPosjeta { get; set; }

    [StringLength(1000, ErrorMessage = "Opis iskustva može imati najviše 1000 znakova.")]
    public string? OpisIskustva { get; set; }

    [Required(ErrorMessage = "Uneseni GUID je obavezan.")]
    [StringLength(100, ErrorMessage = "Uneseni GUID može imati najviše 100 znakova.")]
    public string UneseniGUID { get; set; } = string.Empty;
}

public class PosjetEditModel : PosjetCreateModel
{
}
