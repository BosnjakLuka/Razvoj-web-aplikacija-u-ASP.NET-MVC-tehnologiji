using System.ComponentModel.DataAnnotations;
using planinarenje.Entiteti;

namespace planinarenje.Models.Dto.Ruta;

public class RutaDto
{
    public int IdRuta { get; set; }
    public int IdKontrolnaTocka { get; set; }
    public string NazivKontrolneTocke { get; set; } = string.Empty;
    public string Naziv { get; set; } = string.Empty;
    public string Pocetak { get; set; } = string.Empty;
    public string Kraj { get; set; } = string.Empty;
    public int VrijemeHodaMin { get; set; }
    public decimal DuljinaKm { get; set; }
    public int? VisinskaRazlikaM { get; set; }
    public string? Opis { get; set; }
    public string? OznakaNaTerenu { get; set; }
    public int? GodinaObnove { get; set; }
    public string? Napomena { get; set; }
    public string TezinaRute { get; set; } = string.Empty;
    public string? GPXPath { get; set; }
}

public class RutaCreateDto
{
    [Required(ErrorMessage = "Kontrolna točka je obavezna.")]
    public int IdKontrolnaTocka { get; set; }

    [Required(ErrorMessage = "Naziv je obavezan.")]
    [MaxLength(200, ErrorMessage = "Naziv ne smije biti duži od 200 znakova.")]
    public string Naziv { get; set; } = string.Empty;

    [Required(ErrorMessage = "Početak je obavezan.")]
    [MaxLength(150, ErrorMessage = "Početak ne smije biti duži od 150 znakova.")]
    public string Pocetak { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kraj je obavezan.")]
    [MaxLength(150, ErrorMessage = "Kraj ne smije biti duži od 150 znakova.")]
    public string Kraj { get; set; } = string.Empty;

    [Range(1, 5000, ErrorMessage = "Vrijeme hoda mora biti između 1 i 5000 minuta.")]
    public int VrijemeHodaMin { get; set; }

    [Range(0.0, 999.99, ErrorMessage = "Duljina mora biti između 0 i 999.99 km.")]
    public decimal DuljinaKm { get; set; }

    [Range(0, 5000, ErrorMessage = "Visinska razlika mora biti između 0 i 5000 m.")]
    public int? VisinskaRazlikaM { get; set; }

    public string? Opis { get; set; }

    [MaxLength(50, ErrorMessage = "Oznaka na terenu ne smije biti duža od 50 znakova.")]
    public string? OznakaNaTerenu { get; set; }

    public int? GodinaObnove { get; set; }

    public string? Napomena { get; set; }

    [Required(ErrorMessage = "Težina rute je obavezna.")]
    public TezinaRute TezinaRute { get; set; }

    [MaxLength(255, ErrorMessage = "GPX putanja ne smije biti duža od 255 znakova.")]
    public string? GPXPath { get; set; }
}

public class RutaUpdateDto : RutaCreateDto
{
}
