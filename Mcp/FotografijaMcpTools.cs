using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.Fotografija;

namespace planinarenje.Mcp;

[McpServerToolType]
public class FotografijaMcpTools
{
    private readonly PlaninarstvoDbContext _db;

    public FotografijaMcpTools(PlaninarstvoDbContext db)
    {
        _db = db;
    }

    [McpServerTool, Description("Searches photos (Fotografija) attached to visits, optionally filtered by visit id. Returns an empty list if nothing matches.")]
    public async Task<List<FotografijaDto>> PretraziFotografije(
        [Description("Optional visit (Posjet) id to filter by")] int? idPosjet = null)
    {
        var upit = _db.Fotografije.Where(f => f.DeletedAt == null);

        if (idPosjet.HasValue)
            upit = upit.Where(f => f.IdPosjet == idPosjet.Value);

        var rezultat = await upit.OrderByDescending(f => f.DatumUploada).ToListAsync();
        return rezultat.Select(ToDto).ToList();
    }

    [McpServerTool, Description("Gets a single photo by its id. Returns null if not found.")]
    public async Task<FotografijaDto?> DohvatiFotografiju(
        [Description("Id of the photo (IdFotografija)")] int id)
    {
        var entity = await _db.Fotografije.FirstOrDefaultAsync(f => f.IdFotografija == id && f.DeletedAt == null);
        return entity is null ? null : ToDto(entity);
    }

    private static FotografijaDto ToDto(Fotografija f) => new()
    {
        IdFotografija = f.IdFotografija,
        IdPosjet = f.IdPosjet,
        NazivDatoteke = f.NazivDatoteke,
        PutanjaDatoteke = f.PutanjaDatoteke,
        DatumUploada = f.DatumUploada,
        TipSlike = f.TipSlike.ToString(),
        Opis = f.Opis,
        ContentType = f.ContentType,
        FileSize = f.FileSize
    };
}
