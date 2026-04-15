using System;
using planinarenje.Entiteti;

namespace planinarenje.Models;

public class KorisnikMedaljaIndexViewModel
{
    public int IdKorisnikMedalja { get; set; }
    public int IdKorisnik { get; set; }
    public string ImePrezimeKorisnika { get; set; } = string.Empty;
    public int IdMedalja { get; set; }
    public string NazivMedalje { get; set; } = string.Empty;
    public DateTime DatumDodjele { get; set; }
    public string? Napomena { get; set; }
}

public class KorisnikMedaljaDetailsViewModel
{
    public int IdKorisnikMedalja { get; set; }
    
    public int IdKorisnik { get; set; }
    public string ImePrezimeKorisnika { get; set; } = string.Empty;
    public string? ProfilnaSlikaUrl { get; set; }
    
    public int IdMedalja { get; set; }
    public string NazivMedalje { get; set; } = string.Empty;
    
    public DateTime DatumDodjele { get; set; }
    public string? Napomena { get; set; }
}