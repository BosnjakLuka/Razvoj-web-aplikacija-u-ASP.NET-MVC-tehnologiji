using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.KorisnikMedalja;

namespace planinarenje.Mcp;

[McpServerToolType]
public class KorisnikMedaljaMcpTools
{
    private readonly PlaninarstvoDbContext _db;

    public KorisnikMedaljaMcpTools(PlaninarstvoDbContext db)
    {
        _db = db;
    }

    [McpServerTool, Description("Searches medal awards (KorisnikMedalja), optionally filtered by hiker id and/or medal id. Returns an empty list if nothing matches.")]
    public async Task<List<KorisnikMedaljaDto>> PretraziDodjeleMedalja(
        [Description("Optional hiker (Korisnik) id to filter by")] int? idKorisnik = null,
        [Description("Optional medal (Medalja) id to filter by")] int? idMedalja = null)
    {
        var upit = _db.KorisnikMedalje
            .Include(km => km.Korisnik)
            .Include(km => km.Medalja)
            .Where(km => km.DeletedAt == null);

        if (idKorisnik.HasValue)
            upit = upit.Where(km => km.IdKorisnik == idKorisnik.Value);
        if (idMedalja.HasValue)
            upit = upit.Where(km => km.IdMedalja == idMedalja.Value);

        var rezultat = await upit.OrderByDescending(km => km.DatumDodjele).ToListAsync();
        return rezultat.Select(ToDto).ToList();
    }

    [McpServerTool, Description("Gets a single medal award by its id, including hiker and medal names. Returns null if not found.")]
    public async Task<KorisnikMedaljaDto?> DohvatiDodjeluMedalje(
        [Description("Id of the award (IdKorisnikMedalja)")] int id)
    {
        var entity = await _db.KorisnikMedalje
            .Include(km => km.Korisnik)
            .Include(km => km.Medalja)
            .FirstOrDefaultAsync(km => km.IdKorisnikMedalja == id && km.DeletedAt == null);

        return entity is null ? null : ToDto(entity);
    }

    private static KorisnikMedaljaDto ToDto(KorisnikMedalja km) => new()
    {
        IdKorisnikMedalja = km.IdKorisnikMedalja,
        IdKorisnik = km.IdKorisnik,
        ImePrezimeKorisnika = km.Korisnik == null ? string.Empty : $"{km.Korisnik.Ime} {km.Korisnik.Prezime}",
        IdMedalja = km.IdMedalja,
        NazivMedalje = km.Medalja?.Naziv ?? string.Empty,
        DatumDodjele = km.DatumDodjele,
        Napomena = km.Napomena
    };
}
