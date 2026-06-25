using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.PlaninarskiObjekt;

namespace planinarenje.Mcp;

[McpServerToolType]
public class PlaninarskiObjektMcpTools
{
    private readonly PlaninarstvoDbContext _db;

    public PlaninarskiObjektMcpTools(PlaninarstvoDbContext db)
    {
        _db = db;
    }

    [McpServerTool, Description("Searches mountain huts/cabins/shelters (PlaninarskiObjekt) by area id, association id, and/or name. Returns an empty list if nothing matches.")]
    public async Task<List<PlaninarskiObjektDto>> PretraziPlaninarskeObjekte(
        [Description("Optional area (Podrucje) id to filter by")] int? idPodrucje = null,
        [Description("Optional association (PlaninarskaUdruga) id to filter by")] int? idPlaninarskaUdruga = null,
        [Description("Optional case-insensitive substring match on the object name")] string? naziv = null)
    {
        var upit = _db.PlaninarskiObjekti
            .Include(o => o.Podrucje)
            .Include(o => o.PlaninarskaUdruga)
            .Where(o => o.DeletedAt == null);

        if (idPodrucje.HasValue)
            upit = upit.Where(o => o.IdPodrucje == idPodrucje.Value);
        if (idPlaninarskaUdruga.HasValue)
            upit = upit.Where(o => o.IdPlaninarskaUdruga == idPlaninarskaUdruga.Value);
        if (!string.IsNullOrWhiteSpace(naziv))
            upit = upit.Where(o => o.Naziv.Contains(naziv));

        var rezultat = await upit.OrderBy(o => o.Naziv).ToListAsync();
        return rezultat.Select(ToDto).ToList();
    }

    [McpServerTool, Description("Gets a single mountain hut/cabin/shelter by its id, including area and association names. Returns null if not found.")]
    public async Task<PlaninarskiObjektDto?> DohvatiPlaninarskiObjekt(
        [Description("Id of the object (IdPlaninarskiObjekt)")] int id)
    {
        var entity = await _db.PlaninarskiObjekti
            .Include(o => o.Podrucje)
            .Include(o => o.PlaninarskaUdruga)
            .FirstOrDefaultAsync(o => o.IdPlaninarskiObjekt == id && o.DeletedAt == null);

        return entity is null ? null : ToDto(entity);
    }

    private static PlaninarskiObjektDto ToDto(PlaninarskiObjekt o) => new()
    {
        IdPlaninarskiObjekt = o.IdPlaninarskiObjekt,
        IdPodrucje = o.IdPodrucje,
        NazivPodrucja = o.Podrucje?.Naziv ?? string.Empty,
        IdPlaninarskaUdruga = o.IdPlaninarskaUdruga,
        NazivUdruge = o.PlaninarskaUdruga?.Naziv ?? string.Empty,
        Naziv = o.Naziv,
        TipObjekta = o.TipObjekta.ToString(),
        NadmorskaVisina = o.NadmorskaVisina,
        Kapacitet = o.Kapacitet,
        Opis = o.Opis,
        ImeOdgovorneOsobe = o.ImeOdgovorneOsobe,
        Telefon = o.Telefon,
        Email = o.Email,
        Adresa = o.Adresa,
        ImaNocenje = o.ImaNocenje,
        ImaHranu = o.ImaHranu,
        RadnoVrijemeOpis = o.RadnoVrijemeOpis
    };
}
