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

public class KorisnikMedaljaEligibleUserViewModel
{
    public int IdKorisnik { get; set; }
    public string ImePrezimeKorisnika { get; set; } = string.Empty;
    public string KorisnickoIme { get; set; } = string.Empty;
    public int BrojKontrolnihTocaka { get; set; }
    public int BrojPodrucja { get; set; }
}

public class MedaljaEligibilityCardViewModel
{
    public int IdMedalja { get; set; }
    public string NazivMedalje { get; set; } = string.Empty;
    public string? Opis { get; set; }
    public int MinimalanBrojKontrolnihTocaka { get; set; }
    public int MinimalanBrojPodrucja { get; set; }
    public List<KorisnikMedaljaEligibleUserViewModel> EligibleUsers { get; set; } = new();
}

public class KorisnikMedaljaEligibilityPageViewModel
{
    public List<MedaljaEligibilityCardViewModel> Medalje { get; set; } = new();
}