using System.ComponentModel.DataAnnotations;
using planinarenje.Entiteti;

namespace planinarenje.Models.ViewModels;

public class RutaCreateModel
{
    [Required(ErrorMessage = "Naziv je obavezan.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Naziv mora imati između 2 i 200 znakova.")]
    public string Naziv { get; set; } = string.Empty;

    [Required(ErrorMessage = "Pocetak je obavezan.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Pocetak mora imati između 2 i 150 znakova.")]
    public string Pocetak { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kraj je obavezan.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Kraj mora imati između 2 i 150 znakova.")]
    public string Kraj { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kontrolna tocka je obavezna.")]
    public int IdKontrolnaTocka { get; set; }

    [Required(ErrorMessage = "Vrijeme hoda je obavezno.")]
    [Range(1, 10000, ErrorMessage = "Vrijeme hoda mora biti između 1 i 10000 minuta.")]
    public int VrijemeHodaMin { get; set; }

    [Required(ErrorMessage = "Duljina je obavezna.")]
    [Range(0.1, 10000, ErrorMessage = "Duljina mora biti između 0.1 i 10000 km.")]
    public decimal DuljinaKm { get; set; }

    [Range(0, 9000, ErrorMessage = "Visinska razlika mora biti između 0 i 9000.")]
    public int? VisinskaRazlikaM { get; set; }

    [StringLength(500, ErrorMessage = "Opis može imati najviše 500 znakova.")]
    public string? Opis { get; set; }

    [StringLength(50, ErrorMessage = "Oznaka na terenu može imati najviše 50 znakova.")]
    public string? OznakaNaTerenu { get; set; }

    [Range(1900, 2100, ErrorMessage = "Godina obnove mora biti između 1900 i 2100.")]
    public int? GodinaObnove { get; set; }

    [StringLength(500, ErrorMessage = "Napomena može imati najviše 500 znakova.")]
    public string? Napomena { get; set; }

    [Required(ErrorMessage = "Tezina rute je obavezna.")]
    public TezinaRute TezinaRute { get; set; }

    [StringLength(255, ErrorMessage = "GPX putanja može imati najviše 255 znakova.")]
    public string? GPXPath { get; set; }
}

public class RutaEditModel : RutaCreateModel
{
}
