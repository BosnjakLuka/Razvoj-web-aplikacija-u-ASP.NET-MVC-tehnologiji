using System.ComponentModel.DataAnnotations;
using planinarenje.Entiteti;

namespace planinarenje.Models.ViewModels;

public class FotografijaCreateModel
{
    [Required(ErrorMessage = "Posjet je obavezan.")]
    public int IdPosjet { get; set; }

    [Required(ErrorMessage = "Naziv datoteke je obavezan.")]
    [StringLength(255, ErrorMessage = "Naziv datoteke može imati najviše 255 znakova.")]
    public string NazivDatoteke { get; set; } = string.Empty;

    [Required(ErrorMessage = "Putanja datoteke je obavezna.")]
    [StringLength(255, ErrorMessage = "Putanja datoteke može imati najviše 255 znakova.")]
    public string PutanjaDatoteke { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tip slike je obavezan.")]
    public TipSlike TipSlike { get; set; }

    [StringLength(500, ErrorMessage = "Opis može imati najviše 500 znakova.")]
    public string? Opis { get; set; }
}

public class FotografijaEditModel : FotografijaCreateModel
{
}
