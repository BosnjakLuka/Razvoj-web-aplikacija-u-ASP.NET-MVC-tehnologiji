using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.KontrolnaTocka;

namespace planinarenje.Controllers.Api;

/// <summary>
/// Web API za kontrolne točke (vrhovi / vidikovci sa žigom).
/// Čitanje je javno; pisanje je dopušteno samo administratorima.
/// </summary>
[Route("api/kontrolnatocka")]
[ApiController]
public class KontrolnaTockaApiController : ControllerBase
{
    private readonly PlaninarstvoDbContext _db;
    private readonly ILogger<KontrolnaTockaApiController> _logger;

    public KontrolnaTockaApiController(PlaninarstvoDbContext db, ILogger<KontrolnaTockaApiController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // GET /api/kontrolnatocka?podrucjeId=&tip=&naziv=
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<KontrolnaTockaDto>>> Get(
        [FromQuery] int? podrucjeId,
        [FromQuery] TipKontrolneTocke? tip,
        [FromQuery] string? naziv)
    {
        var upit = _db.KontrolneTocke
            .Include(k => k.Podrucje)
            .AsQueryable();

        if (podrucjeId.HasValue)
            upit = upit.Where(k => k.IdPodrucje == podrucjeId.Value);
        if (tip.HasValue)
            upit = upit.Where(k => k.TipKontrolneTocke == tip.Value);
        if (!string.IsNullOrWhiteSpace(naziv))
            upit = upit.Where(k => k.Naziv.Contains(naziv));

        var rezultat = await upit.OrderBy(k => k.Naziv).ToListAsync();
        return Ok(rezultat.Select(ToDto).ToList());
    }

    // GET /api/kontrolnatocka/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<KontrolnaTockaDto>> GetById(int id)
    {
        var entity = await _db.KontrolneTocke
            .Include(k => k.Podrucje)
            .FirstOrDefaultAsync(k => k.IdKontrolnaTocka == id);

        if (entity == null)
            return NotFound();

        return Ok(ToDto(entity));
    }

    // POST /api/kontrolnatocka
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<KontrolnaTockaDto>> Post([FromBody] KontrolnaTockaCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _db.Podrucja.AnyAsync(p => p.IdPodrucje == dto.IdPodrucje))
            return BadRequest("Područje nije pronađeno.");

        if (await _db.KontrolneTocke.AnyAsync(k => k.GUIDOznaka == dto.GUIDOznaka))
            return BadRequest("Kontrolna točka s tom GUID oznakom već postoji.");

        var entity = new KontrolnaTocka
        {
            GUIDOznaka = dto.GUIDOznaka,
            IdPodrucje = dto.IdPodrucje,
            Naziv = dto.Naziv,
            TipKontrolneTocke = dto.TipKontrolneTocke,
            NadmorskaVisina = dto.NadmorskaVisina,
            Opis = dto.Opis,
            Koordinate = dto.Koordinate,
            OpisZiga = dto.OpisZiga
        };

        _db.KontrolneTocke.Add(entity);
        await _db.SaveChangesAsync();
        _logger.LogInformation("POST /api/kontrolnatocka - kontrolna točka {IdKontrolnaTocka} kreirana.", entity.IdKontrolnaTocka);

        var kreiran = await _db.KontrolneTocke
            .Include(k => k.Podrucje)
            .FirstAsync(k => k.IdKontrolnaTocka == entity.IdKontrolnaTocka);

        return CreatedAtAction(nameof(GetById), new { id = entity.IdKontrolnaTocka }, ToDto(kreiran));
    }

    // PUT /api/kontrolnatocka/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<KontrolnaTockaDto>> Put(int id, [FromBody] KontrolnaTockaUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = await _db.KontrolneTocke.FirstOrDefaultAsync(k => k.IdKontrolnaTocka == id);
        if (entity == null)
            return NotFound();

        if (!await _db.Podrucja.AnyAsync(p => p.IdPodrucje == dto.IdPodrucje))
            return BadRequest("Područje nije pronađeno.");

        if (await _db.KontrolneTocke.AnyAsync(k => k.GUIDOznaka == dto.GUIDOznaka && k.IdKontrolnaTocka != id))
            return BadRequest("Druga kontrolna točka s tom GUID oznakom već postoji.");

        entity.GUIDOznaka = dto.GUIDOznaka;
        entity.IdPodrucje = dto.IdPodrucje;
        entity.Naziv = dto.Naziv;
        entity.TipKontrolneTocke = dto.TipKontrolneTocke;
        entity.NadmorskaVisina = dto.NadmorskaVisina;
        entity.Opis = dto.Opis;
        entity.Koordinate = dto.Koordinate;
        entity.OpisZiga = dto.OpisZiga;

        await _db.SaveChangesAsync();
        _logger.LogInformation("PUT /api/kontrolnatocka/{IdKontrolnaTocka} - kontrolna točka ažurirana.", id);

        var azuriran = await _db.KontrolneTocke
            .Include(k => k.Podrucje)
            .FirstAsync(k => k.IdKontrolnaTocka == id);

        return Ok(ToDto(azuriran));
    }

    // DELETE /api/kontrolnatocka/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.KontrolneTocke.FirstOrDefaultAsync(k => k.IdKontrolnaTocka == id);
        if (entity == null)
            return NotFound();

        entity.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        _logger.LogInformation("DELETE /api/kontrolnatocka/{IdKontrolnaTocka} - kontrolna točka obrisana (soft delete).", id);

        return NoContent();
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
