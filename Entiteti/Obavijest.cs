using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace planinarenje.Entiteti;

public class Obavijest
{
    [Key]
    public int IdObavijest { get; set; }

    [Required]
    [MaxLength(200)]
    public string Naslov { get; set; } = string.Empty;

    public string? Sadrzaj { get; set; }

    [Required]
    public DateTime DatumObjave { get; set; }

    [Required]
    public bool JeAktivna { get; set; }

    [ForeignKey("Korisnik")]
    public int IdKorisnik { get; set; }

    // ASP.NET Core implicitno tretira non-nullable referentne tipove kao required.
    // Korisnik se ne postavlja iz forme (samo IdKorisnik), pa bi inače ModelState.IsValid
    // bio false na svaki Create/Edit POST iako su sva stvarna polja ispravno popunjena.
    [ValidateNever]
    public virtual Korisnik Korisnik { get; set; } = null!;
}
