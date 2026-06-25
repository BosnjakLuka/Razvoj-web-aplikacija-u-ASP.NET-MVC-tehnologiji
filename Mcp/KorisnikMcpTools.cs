using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.Korisnik;

namespace planinarenje.Mcp;

[McpServerToolType]
public class KorisnikMcpTools
{
    private readonly PlaninarstvoDbContext _db;

    public KorisnikMcpTools(PlaninarstvoDbContext db)
    {
        _db = db;
    }

    [McpServerTool, Description("Searches active hiker profiles (Korisnik) by first/last/user name. Returns only public fields. Returns an empty list if nothing matches.")]
    public async Task<List<KorisnikPublicDto>> PretraziKorisnike(
        [Description("Optional case-insensitive substring match on first name, last name, or username")] string? naziv = null)
    {
        var upit = _db.Korisnici.Where(k => k.StatusAktivan);

        if (!string.IsNullOrWhiteSpace(naziv))
            upit = upit.Where(k =>
                k.Ime.Contains(naziv) ||
                k.Prezime.Contains(naziv) ||
                k.KorisnickoIme.Contains(naziv));

        var rezultat = await upit.OrderBy(k => k.Prezime).ThenBy(k => k.Ime).ToListAsync();
        return rezultat.Select(ToDto).ToList();
    }

    [McpServerTool, Description("Gets a single active hiker profile by its id. Returns only public fields. Returns null if not found.")]
    public async Task<KorisnikPublicDto?> DohvatiKorisnika(
        [Description("Id of the hiker profile (IdKorisnik)")] int id)
    {
        var entity = await _db.Korisnici.FirstOrDefaultAsync(k => k.IdKorisnik == id && k.StatusAktivan);
        return entity is null ? null : ToDto(entity);
    }

    private static KorisnikPublicDto ToDto(Korisnik k) => new()
    {
        IdKorisnik = k.IdKorisnik,
        Ime = k.Ime,
        Prezime = k.Prezime,
        KorisnickoIme = k.KorisnickoIme,
        ProfilnaSlika = k.ProfilnaSlika,
        DatumRegistracije = k.DatumRegistracije
    };
}
