using planinarenje.Entiteti;

namespace planinarenje.Models;

public class PlaninarskaUdrugaIndexViewModel
{
    public int IdPlaninarskaUdruga { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string OIB { get; set; } = string.Empty;
    public string? Grad { get; set; }
    public string? Zupanija { get; set; }
    public int? BrojClanova { get; set; }
}

public class PlaninarskaUdrugaDetailsViewModel
{
    public int IdPlaninarskaUdruga { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string OIB { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? BrojTelefona { get; set; }
    public string? Adresa { get; set; }
    public string? PostanskiBroj { get; set; }
    public string? Grad { get; set; }
    public string? Zupanija { get; set; }
    public int? BrojClanova { get; set; }

    public List<ObjektUdrugeViewModel> PlaninarskiObjekti { get; set; } = new();
}

public class ObjektUdrugeViewModel
{
    public int IdPlaninarskiObjekt { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string TipObjekta { get; set; } = string.Empty;
    public int? NadmorskaVisina { get; set; }
    public bool ImaNocenje { get; set; }
}