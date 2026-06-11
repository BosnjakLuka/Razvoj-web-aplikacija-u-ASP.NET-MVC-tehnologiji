using System.ComponentModel.DataAnnotations;

namespace planinarenje.Models.Dto.Knjizica;

public class KnjizicaDto
{
    public int IdKnjizica { get; set; }
    public int IdKorisnik { get; set; }
    public string ImePrezimeKorisnika { get; set; } = string.Empty;
    public DateTime DatumKreiranja { get; set; }
    public string? Napomena { get; set; }
    public bool StatusAktivna { get; set; }
    public int BrojPosjeta { get; set; }
}

public class KnjizicaCreateDto
{
    [Required(ErrorMessage = "Korisnik je obavezan.")]
    public int IdKorisnik { get; set; }

    public string? Napomena { get; set; }

    public bool StatusAktivna { get; set; } = true;
}

public class KnjizicaUpdateDto
{
    public string? Napomena { get; set; }

    public bool StatusAktivna { get; set; } = true;
}
