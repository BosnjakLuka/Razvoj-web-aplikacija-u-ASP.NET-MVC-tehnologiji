using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.KontrolnaTocka;

namespace planinarenje.Mcp;

[McpServerToolType]
public class KontrolnaTockaMcpTools
{
    private readonly PlaninarstvoDbContext _db;

    public KontrolnaTockaMcpTools(PlaninarstvoDbContext db)
    {
        _db = db;
    }

    [McpServerTool, Description("Searches control points (mountain peaks, viewpoints, stamp locations) by name and/or area id. Returns an empty list if nothing matches.")]
    public async Task<List<KontrolnaTockaDto>> PretraziKontrolneTocke(
        [Description("Optional area (Podrucje) id to filter by")] int? idPodrucje = null,
        [Description("Optional case-insensitive substring match on the control point name")] string? naziv = null)
    {
        var upit = _db.KontrolneTocke
            .Include(k => k.Podrucje)
            .AsQueryable();

        if (idPodrucje.HasValue)
            upit = upit.Where(k => k.IdPodrucje == idPodrucje.Value);
        if (!string.IsNullOrWhiteSpace(naziv))
            upit = upit.Where(k => k.Naziv.Contains(naziv));

        var rezultat = await upit.OrderBy(k => k.Naziv).ToListAsync();
        return rezultat.Select(ToDto).ToList();
    }

    [McpServerTool, Description("Gets a single control point by its id, including its area name. Returns null if not found.")]
    public async Task<KontrolnaTockaDto?> DohvatiKontrolnuTocku(
        [Description("Id of the control point (IdKontrolnaTocka)")] int id)
    {
        var entity = await _db.KontrolneTocke
            .Include(k => k.Podrucje)
            .FirstOrDefaultAsync(k => k.IdKontrolnaTocka == id);

        return entity is null ? null : ToDto(entity);
    }

    private static KontrolnaTockaDto ToDto(KontrolnaTocka k) => new()
    {
        IdKontrolnaTocka = k.IdKontrolnaTocka,
        GUIDOznaka = k.GUIDOznaka,
        IdPodrucje = k.IdPodrucje,
        NazivPodrucja = k.Podrucje?.Naziv ?? string.Empty,
        Naziv = k.Naziv,
        TipKontrolneTocke = k.TipKontrolneTocke.ToString(),
        NadmorskaVisina = k.NadmorskaVisina,
        Opis = k.Opis,
        Koordinate = k.Koordinate,
        OpisZiga = k.OpisZiga
    };
}
