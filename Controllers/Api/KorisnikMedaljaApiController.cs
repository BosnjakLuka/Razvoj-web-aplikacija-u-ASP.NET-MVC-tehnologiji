using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.KorisnikMedalja;

namespace planinarenje.Controllers.Api;

/// <summary>
/// Web API za dodijeljene medalje korisnicima (veza Korisnik–Medalja).
/// Čitanje je javno; dodjelu/izmjenu/brisanje radi samo administrator.
/// </summary>
[Route("api/korisnikmedalja")]
[ApiController]
public class KorisnikMedaljaApiController : ControllerBase
{
    private readonly PlaninarstvoDbContext _db;
    private readonly ILogger<KorisnikMedaljaApiController> _logger;

    public KorisnikMedaljaApiController(PlaninarstvoDbContext db, ILogger<KorisnikMedaljaApiController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // GET /api/korisnikmedalja?korisnikId=&medaljaId=
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<KorisnikMedaljaDto>>> Get(
        [FromQuery] int? korisnikId,
        [FromQuery] int? medaljaId)
    {
        var upit = _db.KorisnikMedalje
            .Include(km => km.Korisnik)
            .Include(km => km.Medalja)
            .AsQueryable();

        if (korisnikId.HasValue)
            upit = upit.Where(km => km.IdKorisnik == korisnikId.Value);
        if (medaljaId.HasValue)
            upit = upit.Where(km => km.IdMedalja == medaljaId.Value);

        var rezultat = await upit.OrderByDescending(km => km.DatumDodjele).ToListAsync();
        return Ok(rezultat.Select(ToDto).ToList());
    }

    // GET /api/korisnikmedalja/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<KorisnikMedaljaDto>> GetById(int id)
    {
        var entity = await _db.KorisnikMedalje
            .Include(km => km.Korisnik)
            .Include(km => km.Medalja)
            .FirstOrDefaultAsync(km => km.IdKorisnikMedalja == id);

        if (entity == null)
            return NotFound();

        return Ok(ToDto(entity));
    }

    // POST /api/korisnikmedalja
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<KorisnikMedaljaDto>> Post([FromBody] KorisnikMedaljaCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _db.Korisnici.AnyAsync(k => k.IdKorisnik == dto.IdKorisnik && k.StatusAktivan))
            return BadRequest("Korisnik nije pronađen ili nije aktivan.");
        if (!await _db.Medalje.AnyAsync(m => m.IdMedalja == dto.IdMedalja))
            return BadRequest("Medalja nije pronađena.");

        var entity = new KorisnikMedalja
        {
            IdKorisnik = dto.IdKorisnik,
            IdMedalja = dto.IdMedalja,
            DatumDodjele = dto.DatumDodjele,
            Napomena = dto.Napomena
        };

        _db.KorisnikMedalje.Add(entity);
        await _db.SaveChangesAsync();
        _logger.LogInformation("POST /api/korisnikmedalja - dodjela {IdKorisnikMedalja} kreirana (korisnik {IdKorisnik}, medalja {IdMedalja}).",
            entity.IdKorisnikMedalja, entity.IdKorisnik, entity.IdMedalja);

        var kreiran = await _db.KorisnikMedalje
            .Include(km => km.Korisnik)
            .Include(km => km.Medalja)
            .FirstAsync(km => km.IdKorisnikMedalja == entity.IdKorisnikMedalja);

        return CreatedAtAction(nameof(GetById), new { id = entity.IdKorisnikMedalja }, ToDto(kreiran));
    }

    // PUT /api/korisnikmedalja/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<KorisnikMedaljaDto>> Put(int id, [FromBody] KorisnikMedaljaUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = await _db.KorisnikMedalje.FirstOrDefaultAsync(km => km.IdKorisnikMedalja == id);
        if (entity == null)
            return NotFound();

        entity.DatumDodjele = dto.DatumDodjele;
        entity.Napomena = dto.Napomena;

        await _db.SaveChangesAsync();
        _logger.LogInformation("PUT /api/korisnikmedalja/{IdKorisnikMedalja} - dodjela ažurirana.", id);

        var azuriran = await _db.KorisnikMedalje
            .Include(km => km.Korisnik)
            .Include(km => km.Medalja)
            .FirstAsync(km => km.IdKorisnikMedalja == id);

        return Ok(ToDto(azuriran));
    }

    // DELETE /api/korisnikmedalja/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.KorisnikMedalje.FirstOrDefaultAsync(km => km.IdKorisnikMedalja == id);
        if (entity == null)
            return NotFound();

        entity.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        _logger.LogInformation("DELETE /api/korisnikmedalja/{IdKorisnikMedalja} - dodjela obrisana (soft delete).", id);

        return NoContent();
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
