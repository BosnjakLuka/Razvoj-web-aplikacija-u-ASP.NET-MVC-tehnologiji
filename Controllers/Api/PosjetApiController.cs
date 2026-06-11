using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.Posjet;

namespace planinarenje.Controllers.Api;

/// <summary>
/// Web API za posjete (glavni radni entitet aplikacije).
/// Čitanje je javno; kreiranje/izmjena/brisanje traži prijavu i vlasništvo (ili Admina).
/// </summary>
[Route("api/posjet")]
public class PosjetApiController : ApiBaseController
{
    public PosjetApiController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db)
        : base(userMgr, db)
    {
    }

    // GET /api/posjet?korisnikId=&kontrolnaTockaId=&datumOd=&datumDo=&dozivljaj=
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<PosjetDto>>> Get(
        [FromQuery] int? korisnikId,
        [FromQuery] int? kontrolnaTockaId,
        [FromQuery] DateTime? datumOd,
        [FromQuery] DateTime? datumDo,
        [FromQuery] DozivljajPosjeta? dozivljaj)
    {
        var upit = Db.Posjeti
            .Include(p => p.Korisnik)
            .Include(p => p.KontrolnaTocka)
            .Include(p => p.Ruta)
            .Include(p => p.Fotografije)
            .AsQueryable();

        if (korisnikId.HasValue)
            upit = upit.Where(p => p.IdKorisnik == korisnikId.Value);
        if (kontrolnaTockaId.HasValue)
            upit = upit.Where(p => p.IdKontrolnaTocka == kontrolnaTockaId.Value);
        if (datumOd.HasValue)
            upit = upit.Where(p => p.DatumVrijemePosjeta >= datumOd.Value);
        if (datumDo.HasValue)
            upit = upit.Where(p => p.DatumVrijemePosjeta <= datumDo.Value);
        if (dozivljaj.HasValue)
            upit = upit.Where(p => p.DozivljajPosjeta == dozivljaj.Value);

        var rezultat = await upit
            .OrderByDescending(p => p.DatumVrijemePosjeta)
            .ToListAsync();

        return Ok(rezultat.Select(ToDto).ToList());
    }

    // GET /api/posjet/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<PosjetDto>> GetById(int id)
    {
        var posjet = await Db.Posjeti
            .Include(p => p.Korisnik)
            .Include(p => p.KontrolnaTocka)
            .Include(p => p.Ruta)
            .Include(p => p.Fotografije)
            .FirstOrDefaultAsync(p => p.IdPosjet == id);

        if (posjet == null)
            return NotFound();

        return Ok(ToDto(posjet));
    }

    // POST /api/posjet
    [HttpPost]
    [Authorize(Roles = "Admin,Planinar")]
    public async Task<ActionResult<PosjetDto>> Post([FromBody] PosjetCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Vlasnik se postavlja iz prijavljenog korisnika, NIKAD iz DTO-a.
        var korisnik = await GetCurrentKorisnikAsync();
        if (korisnik == null)
            return Forbid();

        var knjizica = await Db.Knjizice
            .FirstOrDefaultAsync(k => k.IdKnjizica == dto.IdKnjizica && k.StatusAktivna);
        if (knjizica == null)
            return BadRequest("Knjižica nije pronađena ili nije aktivna.");

        // Knjižica mora pripadati prijavljenom korisniku (osim ako je Admin).
        if (!IsAdmin && knjizica.IdKorisnik != korisnik.IdKorisnik)
            return Forbid();

        var kontrolnaTocka = await Db.KontrolneTocke
            .FirstOrDefaultAsync(k => k.IdKontrolnaTocka == dto.IdKontrolnaTocka);
        if (kontrolnaTocka == null)
            return BadRequest("Kontrolna točka nije pronađena.");

        var ruta = await Db.Rute
            .FirstOrDefaultAsync(r => r.IdRuta == dto.IdRuta);
        if (ruta == null)
            return BadRequest("Ruta nije pronađena.");

        var entity = new Posjet
        {
            IdKorisnik = knjizica.IdKorisnik,
            IdKnjizica = dto.IdKnjizica,
            IdKontrolnaTocka = dto.IdKontrolnaTocka,
            IdRuta = dto.IdRuta,
            DatumVrijemePosjeta = dto.DatumVrijemePosjeta,
            VrijemeUsponaMin = dto.VrijemeUsponaMin,
            DozivljajPosjeta = dto.DozivljajPosjeta,
            OpisIskustva = dto.OpisIskustva,
            UneseniGUID = dto.UneseniGUID,
            // Posjet je potvrđen ako se uneseni GUID poklapa s oznakom žiga na kontrolnoj točki.
            JeLiPotvrdenPosjet = string.Equals(
                dto.UneseniGUID, kontrolnaTocka.GUIDOznaka, StringComparison.OrdinalIgnoreCase),
            DatumKreiranjaZapisa = DateTime.UtcNow
        };

        Db.Posjeti.Add(entity);
        await Db.SaveChangesAsync();

        var kreiran = await Db.Posjeti
            .Include(p => p.Korisnik)
            .Include(p => p.KontrolnaTocka)
            .Include(p => p.Ruta)
            .Include(p => p.Fotografije)
            .FirstAsync(p => p.IdPosjet == entity.IdPosjet);

        return CreatedAtAction(nameof(GetById), new { id = entity.IdPosjet }, ToDto(kreiran));
    }

    // PUT /api/posjet/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Planinar")]
    public async Task<ActionResult<PosjetDto>> Put(int id, [FromBody] PosjetUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = await Db.Posjeti.FirstOrDefaultAsync(p => p.IdPosjet == id);
        if (entity == null)
            return NotFound();

        if (!await IsOwnerOrAdminAsync(entity.IdKorisnik))
            return Forbid();

        var kontrolnaTocka = await Db.KontrolneTocke
            .FirstOrDefaultAsync(k => k.IdKontrolnaTocka == dto.IdKontrolnaTocka);
        if (kontrolnaTocka == null)
            return BadRequest("Kontrolna točka nije pronađena.");

        if (!await Db.Rute.AnyAsync(r => r.IdRuta == dto.IdRuta))
            return BadRequest("Ruta nije pronađena.");

        if (!await Db.Knjizice.AnyAsync(k => k.IdKnjizica == dto.IdKnjizica && k.StatusAktivna))
            return BadRequest("Knjižica nije pronađena ili nije aktivna.");

        entity.IdKnjizica = dto.IdKnjizica;
        entity.IdKontrolnaTocka = dto.IdKontrolnaTocka;
        entity.IdRuta = dto.IdRuta;
        entity.DatumVrijemePosjeta = dto.DatumVrijemePosjeta;
        entity.VrijemeUsponaMin = dto.VrijemeUsponaMin;
        entity.DozivljajPosjeta = dto.DozivljajPosjeta;
        entity.OpisIskustva = dto.OpisIskustva;
        entity.UneseniGUID = dto.UneseniGUID;
        entity.JeLiPotvrdenPosjet = string.Equals(
            dto.UneseniGUID, kontrolnaTocka.GUIDOznaka, StringComparison.OrdinalIgnoreCase);

        await Db.SaveChangesAsync();

        var azuriran = await Db.Posjeti
            .Include(p => p.Korisnik)
            .Include(p => p.KontrolnaTocka)
            .Include(p => p.Ruta)
            .Include(p => p.Fotografije)
            .FirstAsync(p => p.IdPosjet == id);

        return Ok(ToDto(azuriran));
    }

    // DELETE /api/posjet/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Planinar")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await Db.Posjeti.FirstOrDefaultAsync(p => p.IdPosjet == id);
        if (entity == null)
            return NotFound();

        if (!await IsOwnerOrAdminAsync(entity.IdKorisnik))
            return Forbid();

        // Soft delete – ne brišemo fizički iz baze.
        entity.DeletedAt = DateTime.UtcNow;
        await Db.SaveChangesAsync();

        return NoContent();
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
