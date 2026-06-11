using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.Ruta;

namespace planinarenje.Controllers.Api;

/// <summary>
/// Web API za rute (staze do kontrolnih točaka).
/// Čitanje je javno; pisanje je dopušteno samo administratorima.
/// </summary>
[Route("api/ruta")]
[ApiController]
public class RutaApiController : ControllerBase
{
    private readonly PlaninarstvoDbContext _db;

    public RutaApiController(PlaninarstvoDbContext db)
    {
        _db = db;
    }

    // GET /api/ruta?tezina=&kontrolnaTockaId=&naziv=
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<RutaDto>>> Get(
        [FromQuery] TezinaRute? tezina,
        [FromQuery] int? kontrolnaTockaId,
        [FromQuery] string? naziv)
    {
        var upit = _db.Rute
            .Include(r => r.KontrolnaTocka)
            .AsQueryable();

        if (tezina.HasValue)
            upit = upit.Where(r => r.TezinaRute == tezina.Value);
        if (kontrolnaTockaId.HasValue)
            upit = upit.Where(r => r.IdKontrolnaTocka == kontrolnaTockaId.Value);
        if (!string.IsNullOrWhiteSpace(naziv))
            upit = upit.Where(r => r.Naziv.Contains(naziv));

        var rezultat = await upit.OrderBy(r => r.Naziv).ToListAsync();
        return Ok(rezultat.Select(ToDto).ToList());
    }

    // GET /api/ruta/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<RutaDto>> GetById(int id)
    {
        var entity = await _db.Rute
            .Include(r => r.KontrolnaTocka)
            .FirstOrDefaultAsync(r => r.IdRuta == id);

        if (entity == null)
            return NotFound();

        return Ok(ToDto(entity));
    }

    // POST /api/ruta
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RutaDto>> Post([FromBody] RutaCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _db.KontrolneTocke.AnyAsync(k => k.IdKontrolnaTocka == dto.IdKontrolnaTocka))
            return BadRequest("Kontrolna točka nije pronađena.");

        var entity = new Ruta
        {
            IdKontrolnaTocka = dto.IdKontrolnaTocka,
            Naziv = dto.Naziv,
            Pocetak = dto.Pocetak,
            Kraj = dto.Kraj,
            VrijemeHodaMin = dto.VrijemeHodaMin,
            DuljinaKm = dto.DuljinaKm,
            VisinskaRazlikaM = dto.VisinskaRazlikaM,
            Opis = dto.Opis,
            OznakaNaTerenu = dto.OznakaNaTerenu,
            GodinaObnove = dto.GodinaObnove,
            Napomena = dto.Napomena,
            TezinaRute = dto.TezinaRute,
            GPXPath = dto.GPXPath
        };

        _db.Rute.Add(entity);
        await _db.SaveChangesAsync();

        var kreiran = await _db.Rute
            .Include(r => r.KontrolnaTocka)
            .FirstAsync(r => r.IdRuta == entity.IdRuta);

        return CreatedAtAction(nameof(GetById), new { id = entity.IdRuta }, ToDto(kreiran));
    }

    // PUT /api/ruta/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RutaDto>> Put(int id, [FromBody] RutaUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = await _db.Rute.FirstOrDefaultAsync(r => r.IdRuta == id);
        if (entity == null)
            return NotFound();

        if (!await _db.KontrolneTocke.AnyAsync(k => k.IdKontrolnaTocka == dto.IdKontrolnaTocka))
            return BadRequest("Kontrolna točka nije pronađena.");

        entity.IdKontrolnaTocka = dto.IdKontrolnaTocka;
        entity.Naziv = dto.Naziv;
        entity.Pocetak = dto.Pocetak;
        entity.Kraj = dto.Kraj;
        entity.VrijemeHodaMin = dto.VrijemeHodaMin;
        entity.DuljinaKm = dto.DuljinaKm;
        entity.VisinskaRazlikaM = dto.VisinskaRazlikaM;
        entity.Opis = dto.Opis;
        entity.OznakaNaTerenu = dto.OznakaNaTerenu;
        entity.GodinaObnove = dto.GodinaObnove;
        entity.Napomena = dto.Napomena;
        entity.TezinaRute = dto.TezinaRute;
        entity.GPXPath = dto.GPXPath;

        await _db.SaveChangesAsync();

        var azuriran = await _db.Rute
            .Include(r => r.KontrolnaTocka)
            .FirstAsync(r => r.IdRuta == id);

        return Ok(ToDto(azuriran));
    }

    // DELETE /api/ruta/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Rute.FirstOrDefaultAsync(r => r.IdRuta == id);
        if (entity == null)
            return NotFound();

        entity.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return NoContent();
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
