using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.Medalja;

namespace planinarenje.Mcp;

[McpServerToolType]
public class MedaljaMcpTools
{
    private readonly PlaninarstvoDbContext _db;

    public MedaljaMcpTools(PlaninarstvoDbContext db)
    {
        _db = db;
    }

    [McpServerTool, Description("Searches medals/badges (Medalja) by name. Returns an empty list if nothing matches.")]
    public async Task<List<MedaljaDto>> PretraziMedalje(
        [Description("Optional case-insensitive substring match on the medal name")] string? naziv = null)
    {
        var upit = _db.Medalje.Where(m => m.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(naziv))
            upit = upit.Where(m => m.Naziv.Contains(naziv));

        var rezultat = await upit.OrderBy(m => m.MinimalanBrojKontrolnihTocaka).ToListAsync();
        return rezultat.Select(ToDto).ToList();
    }

    [McpServerTool, Description("Gets a single medal/badge by its id. Returns null if not found.")]
    public async Task<MedaljaDto?> DohvatiMedalju(
        [Description("Id of the medal (IdMedalja)")] int id)
    {
        var entity = await _db.Medalje.FirstOrDefaultAsync(m => m.IdMedalja == id && m.DeletedAt == null);
        return entity is null ? null : ToDto(entity);
    }

    private static MedaljaDto ToDto(Medalja m) => new()
    {
        IdMedalja = m.IdMedalja,
        Naziv = m.Naziv,
        Opis = m.Opis,
        MinimalanBrojKontrolnihTocaka = m.MinimalanBrojKontrolnihTocaka,
        MinimalanBrojPodrucja = m.MinimalanBrojPodrucja
    };
}
