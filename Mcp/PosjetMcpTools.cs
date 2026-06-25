using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.Posjet;

namespace planinarenje.Mcp;

[McpServerToolType]
public class PosjetMcpTools
{
    private readonly PlaninarstvoDbContext _db;

    public PosjetMcpTools(PlaninarstvoDbContext db)
    {
        _db = db;
    }

    [McpServerTool, Description("Searches logged visits (Posjet) to control points, optionally filtered by hiker id, control point id, or date range. Returns an empty list if nothing matches.")]
    public async Task<List<PosjetDto>> PretraziPosjete(
        [Description("Optional hiker (Korisnik) id to filter by")] int? idKorisnik = null,
        [Description("Optional control point (KontrolnaTocka) id to filter by")] int? idKontrolnaTocka = null,
        [Description("Optional lower bound (inclusive) for visit date/time")] DateTime? datumOd = null,
        [Description("Optional upper bound (inclusive) for visit date/time")] DateTime? datumDo = null)
    {
        var upit = _db.Posjeti
            .Include(p => p.Korisnik)
            .Include(p => p.KontrolnaTocka)
            .Include(p => p.Ruta)
            .Include(p => p.Fotografije)
            .Where(p => p.DeletedAt == null);

        if (idKorisnik.HasValue)
            upit = upit.Where(p => p.IdKorisnik == idKorisnik.Value);
        if (idKontrolnaTocka.HasValue)
            upit = upit.Where(p => p.IdKontrolnaTocka == idKontrolnaTocka.Value);
        if (datumOd.HasValue)
            upit = upit.Where(p => p.DatumVrijemePosjeta >= datumOd.Value);
        if (datumDo.HasValue)
            upit = upit.Where(p => p.DatumVrijemePosjeta <= datumDo.Value);

        var rezultat = await upit.OrderByDescending(p => p.DatumVrijemePosjeta).ToListAsync();
        return rezultat.Select(ToDto).ToList();
    }

    [McpServerTool, Description("Gets a single logged visit by its id, including photos. Returns null if not found.")]
    public async Task<PosjetDto?> DohvatiPosjet(
        [Description("Id of the visit (IdPosjet)")] int id)
    {
        var entity = await _db.Posjeti
            .Include(p => p.Korisnik)
            .Include(p => p.KontrolnaTocka)
            .Include(p => p.Ruta)
            .Include(p => p.Fotografije)
            .FirstOrDefaultAsync(p => p.IdPosjet == id && p.DeletedAt == null);

        return entity is null ? null : ToDto(entity);
    }

    private static PosjetDto ToDto(Posjet p) => new()
    {
        IdPosjet = p.IdPosjet,
        IdKorisnik = p.IdKorisnik,
        ImePrezimeKorisnika = p.Korisnik == null ? string.Empty : $"{p.Korisnik.Ime} {p.Korisnik.Prezime}",
        IdKnjizica = p.IdKnjizica,
        IdKontrolnaTocka = p.IdKontrolnaTocka,
        NazivKontrolneTocke = p.KontrolnaTocka?.Naziv ?? string.Empty,
        IdRuta = p.IdRuta,
        NazivRute = p.Ruta?.Naziv ?? string.Empty,
        DatumVrijemePosjeta = p.DatumVrijemePosjeta,
        VrijemeUsponaMin = p.VrijemeUsponaMin,
        DozivljajPosjeta = p.DozivljajPosjeta.ToString(),
        OpisIskustva = p.OpisIskustva,
        UneseniGUID = p.UneseniGUID,
        JeLiPotvrdenPosjet = p.JeLiPotvrdenPosjet,
        DatumKreiranjaZapisa = p.DatumKreiranjaZapisa,
        Fotografije = p.Fotografije
            .Select(f => new FotografijaSummaryDto
            {
                IdFotografija = f.IdFotografija,
                NazivDatoteke = f.NazivDatoteke,
                PutanjaDatoteke = f.PutanjaDatoteke,
                Opis = f.Opis
            })
            .ToList()
    };
}
