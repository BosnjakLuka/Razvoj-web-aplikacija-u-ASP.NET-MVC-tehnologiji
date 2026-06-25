using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.Knjizica;

namespace planinarenje.Mcp;

[McpServerToolType]
public class KnjizicaMcpTools
{
    private readonly PlaninarstvoDbContext _db;

    public KnjizicaMcpTools(PlaninarstvoDbContext db)
    {
        _db = db;
    }

    [McpServerTool, Description("Searches active hiking logbooks (Knjizica), optionally filtered by owner id. Returns an empty list if nothing matches.")]
    public async Task<List<KnjizicaDto>> PretraziKnjizice(
        [Description("Optional owner (Korisnik) id to filter by")] int? idKorisnik = null)
    {
        var upit = _db.Knjizice
            .Include(k => k.Korisnik)
            .Include(k => k.Posjeti)
            .Where(k => k.StatusAktivna);

        if (idKorisnik.HasValue)
            upit = upit.Where(k => k.IdKorisnik == idKorisnik.Value);

        var rezultat = await upit.OrderBy(k => k.IdKnjizica).ToListAsync();
        return rezultat.Select(ToDto).ToList();
    }

    [McpServerTool, Description("Gets a single active hiking logbook by its id, including owner name and visit count. Returns null if not found.")]
    public async Task<KnjizicaDto?> DohvatiKnjizicu(
        [Description("Id of the logbook (IdKnjizica)")] int id)
    {
        var entity = await _db.Knjizice
            .Include(k => k.Korisnik)
            .Include(k => k.Posjeti)
            .FirstOrDefaultAsync(k => k.IdKnjizica == id && k.StatusAktivna);

        return entity is null ? null : ToDto(entity);
    }

    private static KnjizicaDto ToDto(Knjizica k) => new()
    {
        IdKnjizica = k.IdKnjizica,
        IdKorisnik = k.IdKorisnik,
        ImePrezimeKorisnika = k.Korisnik == null ? string.Empty : $"{k.Korisnik.Ime} {k.Korisnik.Prezime}",
        DatumKreiranja = k.DatumKreiranja,
        Napomena = k.Napomena,
        StatusAktivna = k.StatusAktivna,
        BrojPosjeta = k.Posjeti?.Count ?? 0
    };
}
