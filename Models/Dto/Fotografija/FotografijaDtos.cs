using System.ComponentModel.DataAnnotations;
using planinarenje.Entiteti;

namespace planinarenje.Models.Dto.Fotografija;

public class FotografijaDto
{
    public int IdFotografija { get; set; }
    public int IdPosjet { get; set; }
    public string NazivDatoteke { get; set; } = string.Empty;
    public string PutanjaDatoteke { get; set; } = string.Empty;
    public DateTime DatumUploada { get; set; }
    public string TipSlike { get; set; } = string.Empty;
    public string? Opis { get; set; }
    public string? ContentType { get; set; }
    public long FileSize { get; set; }
}

public class FotografijaCreateDto
{
    [Required(ErrorMessage = "Posjet je obavezan.")]
    public int IdPosjet { get; set; }

    [Required(ErrorMessage = "Naziv datoteke je obavezan.")]
    [MaxLength(255, ErrorMessage = "Naziv datoteke ne smije biti duži od 255 znakova.")]
    public string NazivDatoteke { get; set; } = string.Empty;

    [Required(ErrorMessage = "Putanja datoteke je obavezna.")]
    [MaxLength(255, ErrorMessage = "Putanja datoteke ne smije biti duža od 255 znakova.")]
    public string PutanjaDatoteke { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tip slike je obavezan.")]
    public TipSlike TipSlike { get; set; }

    public string? Opis { get; set; }

    [MaxLength(100, ErrorMessage = "ContentType ne smije biti duži od 100 znakova.")]
    public string? ContentType { get; set; }

    [Range(0, long.MaxValue, ErrorMessage = "Veličina datoteke ne smije biti negativna.")]
    public long FileSize { get; set; }
}

public class FotografijaUpdateDto
{
    [Required(ErrorMessage = "Tip slike je obavezan.")]
    public TipSlike TipSlike { get; set; }

    public string? Opis { get; set; }
}
