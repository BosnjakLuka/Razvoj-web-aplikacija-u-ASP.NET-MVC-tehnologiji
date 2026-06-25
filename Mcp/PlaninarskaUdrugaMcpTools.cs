using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.PlaninarskaUdruga;

namespace planinarenje.Mcp;

[McpServerToolType]
public class PlaninarskaUdrugaMcpTools
{
    private readonly PlaninarstvoDbContext _db;

    public PlaninarskaUdrugaMcpTools(PlaninarstvoDbContext db)
    {
        _db = db;
    }

    [McpServerTool, Description("Searches mountaineering associations (PlaninarskaUdruga) by name and/or city. Returns an empty list if nothing matches.")]
    public async Task<List<PlaninarskaUdrugaDto>> PretraziPlaninarskeUdruge(
        [Description("Optional case-insensitive substring match on the association name")] string? naziv = null,
        [Description("Optional case-insensitive substring match on the city")] string? grad = null)
    {
        var upit = _db.PlaninarskeUdruge.Where(u => u.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(naziv))
            upit = upit.Where(u => u.Naziv.Contains(naziv));
        if (!string.IsNullOrWhiteSpace(grad))
            upit = upit.Where(u => u.Grad != null && u.Grad.Contains(grad));

        var rezultat = await upit.OrderBy(u => u.Naziv).ToListAsync();
        return rezultat.Select(ToDto).ToList();
    }

    [McpServerTool, Description("Gets a single mountaineering association by its id. Returns null if not found.")]
    public async Task<PlaninarskaUdrugaDto?> DohvatiPlaninarskuUdrugu(
        [Description("Id of the association (IdPlaninarskaUdruga)")] int id)
    {
        var entity = await _db.PlaninarskeUdruge.FirstOrDefaultAsync(u => u.IdPlaninarskaUdruga == id && u.DeletedAt == null);
        return entity is null ? null : ToDto(entity);
    }

    private static PlaninarskaUdrugaDto ToDto(PlaninarskaUdruga u) => new()
    {
        IdPlaninarskaUdruga = u.IdPlaninarskaUdruga,
        OIB = u.OIB,
        Naziv = u.Naziv,
        Email = u.Email,
        BrojTelefona = u.BrojTelefona,
        Adresa = u.Adresa,
        PostanskiBroj = u.PostanskiBroj,
        Grad = u.Grad,
        Zupanija = u.Zupanija,
        BrojClanova = u.BrojClanova
    };
}
