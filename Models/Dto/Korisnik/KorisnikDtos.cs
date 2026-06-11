using System.ComponentModel.DataAnnotations;

namespace planinarenje.Models.Dto.Korisnik;

/// <summary>
/// Javni prikaz korisnika (dostupan anonimno). NE sadrži kontakt podatke ni osjetljiva polja.
/// </summary>
public class KorisnikPublicDto
{
    public int IdKorisnik { get; set; }
    public string Ime { get; set; } = string.Empty;
    public string Prezime { get; set; } = string.Empty;
    public string KorisnickoIme { get; set; } = string.Empty;
    public string? ProfilnaSlika { get; set; }
    public DateTime DatumRegistracije { get; set; }
}

/// <summary>
/// Administratorski prikaz korisnika. Dodaje kontakt podatke i status.
/// NIKAD ne sadrži OIB, JMBG ni AppUserId – ta polja se ne izlažu kroz API.
/// </summary>
public class KorisnikAdminDto : KorisnikPublicDto
{
    public string Email { get; set; } = string.Empty;
    public string? BrojMobitela { get; set; }
    public DateTime? DatumRodenja { get; set; }
    public bool StatusAktivan { get; set; }
}

public class KorisnikCreateDto
{
    [Required(ErrorMessage = "Ime je obavezno.")]
    [MaxLength(100, ErrorMessage = "Ime ne smije biti duže od 100 znakova.")]
    public string Ime { get; set; } = string.Empty;

    [Required(ErrorMessage = "Prezime je obavezno.")]
    [MaxLength(100, ErrorMessage = "Prezime ne smije biti duže od 100 znakova.")]
    public string Prezime { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email je obavezan.")]
    [EmailAddress(ErrorMessage = "Email nije u ispravnom formatu.")]
    [MaxLength(150, ErrorMessage = "Email ne smije biti duži od 150 znakova.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Korisničko ime je obavezno.")]
    [MaxLength(100, ErrorMessage = "Korisničko ime ne smije biti duže od 100 znakova.")]
    public string KorisnickoIme { get; set; } = string.Empty;

    public DateTime? DatumRodenja { get; set; }

    [MaxLength(30, ErrorMessage = "Broj mobitela ne smije biti duži od 30 znakova.")]
    public string? BrojMobitela { get; set; }

    [MaxLength(255, ErrorMessage = "Putanja profilne slike ne smije biti duža od 255 znakova.")]
    public string? ProfilnaSlika { get; set; }
}

public class KorisnikUpdateDto : KorisnikCreateDto
{
    public bool StatusAktivan { get; set; } = true;
}
