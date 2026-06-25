using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.Podrucje;

namespace planinarenje.Mcp;

[McpServerToolType]
public class PodrucjeMcpTools
{
    private readonly PlaninarstvoDbContext _db;

    public PodrucjeMcpTools(PlaninarstvoDbContext db)
    {
        _db = db;
    }

    [McpServerTool, Description("Searches mountain areas/regions (Podrucje) by name. Returns an empty list if nothing matches.")]
    public async Task<List<PodrucjeDto>> PretraziPodrucja(
        [Description("Optional case-insensitive substring match on the area name")] string? naziv = null)
    {
        var upit = _db.Podrucja
            .Include(p => p.KontrolneTocke)
            .Where(p => p.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(naziv))
            upit = upit.Where(p => p.Naziv.Contains(naziv));

        var rezultat = await upit.OrderBy(p => p.Naziv).ToListAsync();
        return rezultat.Select(ToDto).ToList();
    }

    [McpServerTool, Description("Gets a single mountain area/region by its id, including its control point count. Returns null if not found.")]
    public async Task<PodrucjeDto?> DohvatiPodrucje(
        [Description("Id of the area (IdPodrucje)")] int id)
    {
        var entity = await _db.Podrucja
            .Include(p => p.KontrolneTocke)
            .FirstOrDefaultAsync(p => p.IdPodrucje == id && p.DeletedAt == null);

        return entity is null ? null : ToDto(entity);
    }

    private static PodrucjeDto ToDto(Podrucje p) => new()
    {
        IdPodrucje = p.IdPodrucje,
        Naziv = p.Naziv,
        Opis = p.Opis,
        Regija = p.Regija,
        MinimalanBrojKTZaObilazak = p.MinimalanBrojKTZaObilazak,
        BrojKontrolnihTocaka = p.KontrolneTocke?.Count ?? 0
    };
}
