using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.Ruta;

namespace planinarenje.Mcp;

[McpServerToolType]
public class RutaMcpTools
{
    private readonly PlaninarstvoDbContext _db;

    public RutaMcpTools(PlaninarstvoDbContext db)
    {
        _db = db;
    }

    [McpServerTool, Description("Searches hiking routes (Ruta) by control point id and/or name. Returns an empty list if nothing matches.")]
    public async Task<List<RutaDto>> PretraziRute(
        [Description("Optional control point (KontrolnaTocka) id to filter by")] int? idKontrolnaTocka = null,
        [Description("Optional case-insensitive substring match on the route name")] string? naziv = null)
    {
        var upit = _db.Rute
            .Include(r => r.KontrolnaTocka)
            .Where(r => r.DeletedAt == null);

        if (idKontrolnaTocka.HasValue)
            upit = upit.Where(r => r.IdKontrolnaTocka == idKontrolnaTocka.Value);
        if (!string.IsNullOrWhiteSpace(naziv))
            upit = upit.Where(r => r.Naziv.Contains(naziv));

        var rezultat = await upit.OrderBy(r => r.Naziv).ToListAsync();
        return rezultat.Select(ToDto).ToList();
    }

    [McpServerTool, Description("Gets a single hiking route by its id, including the control point name. Returns null if not found.")]
    public async Task<RutaDto?> DohvatiRutu(
        [Description("Id of the route (IdRuta)")] int id)
    {
        var entity = await _db.Rute
            .Include(r => r.KontrolnaTocka)
            .FirstOrDefaultAsync(r => r.IdRuta == id && r.DeletedAt == null);

        return entity is null ? null : ToDto(entity);
    }

    private static RutaDto ToDto(Ruta r) => new()
    {
        IdRuta = r.IdRuta,
        IdKontrolnaTocka = r.IdKontrolnaTocka,
        NazivKontrolneTocke = r.KontrolnaTocka?.Naziv ?? string.Empty,
        Naziv = r.Naziv,
        Pocetak = r.Pocetak,
        Kraj = r.Kraj,
        VrijemeHodaMin = r.VrijemeHodaMin,
        DuljinaKm = r.DuljinaKm,
        VisinskaRazlikaM = r.VisinskaRazlikaM,
        Opis = r.Opis,
        OznakaNaTerenu = r.OznakaNaTerenu,
        GodinaObnove = r.GodinaObnove,
        Napomena = r.Napomena,
        TezinaRute = r.TezinaRute.ToString(),
        GPXPath = r.GPXPath
    };
}
