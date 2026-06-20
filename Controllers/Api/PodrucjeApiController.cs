using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.Podrucje;

namespace planinarenje.Controllers.Api;

/// <summary>
/// Web API za planinarska područja (regije).
/// Čitanje je javno; pisanje je dopušteno samo administratorima.
/// </summary>
[Route("api/podrucje")]
[ApiController]
public class PodrucjeApiController : ControllerBase
{
    private readonly PlaninarstvoDbContext _db;
    private readonly ILogger<PodrucjeApiController> _logger;

    public PodrucjeApiController(PlaninarstvoDbContext db, ILogger<PodrucjeApiController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // GET /api/podrucje?naziv=
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<PodrucjeDto>>> Get([FromQuery] string? naziv)
    {
        var upit = _db.Podrucja.AsQueryable();

        if (!string.IsNullOrWhiteSpace(naziv))
            upit = upit.Where(p => p.Naziv.Contains(naziv));

        var rezultat = await upit
            .OrderBy(p => p.Naziv)
            .Select(p => new PodrucjeDto
            {
                IdPodrucje = p.IdPodrucje,
                Naziv = p.Naziv,
                Opis = p.Opis,
                Regija = p.Regija,
                MinimalanBrojKTZaObilazak = p.MinimalanBrojKTZaObilazak,
                BrojKontrolnihTocaka = p.KontrolneTocke.Count
            })
            .ToListAsync();

        return Ok(rezultat);
    }

    // GET /api/podrucje/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<PodrucjeDto>> GetById(int id)
    {
        var entity = await _db.Podrucja
            .Include(p => p.KontrolneTocke)
            .FirstOrDefaultAsync(p => p.IdPodrucje == id);

        if (entity == null)
            return NotFound();

        return Ok(ToDto(entity));
    }

    // POST /api/podrucje
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PodrucjeDto>> Post([FromBody] PodrucjeCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = new Podrucje
        {
            Naziv = dto.Naziv,
            Opis = dto.Opis,
            Regija = dto.Regija,
            MinimalanBrojKTZaObilazak = dto.MinimalanBrojKTZaObilazak
        };

        _db.Podrucja.Add(entity);
        await _db.SaveChangesAsync();
        _logger.LogInformation("POST /api/podrucje - područje {IdPodrucje} kreirano.", entity.IdPodrucje);

        // Učitaj s navigacijskom kolekcijom da BrojKontrolnihTocaka bude točan.
        var kreiran = await _db.Podrucja
            .Include(p => p.KontrolneTocke)
            .FirstAsync(p => p.IdPodrucje == entity.IdPodrucje);

        return CreatedAtAction(nameof(GetById), new { id = entity.IdPodrucje }, ToDto(kreiran));
    }

    // PUT /api/podrucje/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PodrucjeDto>> Put(int id, [FromBody] PodrucjeUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = await _db.Podrucja.FirstOrDefaultAsync(p => p.IdPodrucje == id);
        if (entity == null)
            return NotFound();

        entity.Naziv = dto.Naziv;
        entity.Opis = dto.Opis;
        entity.Regija = dto.Regija;
        entity.MinimalanBrojKTZaObilazak = dto.MinimalanBrojKTZaObilazak;

        await _db.SaveChangesAsync();
        _logger.LogInformation("PUT /api/podrucje/{IdPodrucje} - područje ažurirano.", id);

        var azuriran = await _db.Podrucja
            .Include(p => p.KontrolneTocke)
            .FirstAsync(p => p.IdPodrucje == id);

        return Ok(ToDto(azuriran));
    }

    // DELETE /api/podrucje/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Podrucja.FirstOrDefaultAsync(p => p.IdPodrucje == id);
        if (entity == null)
            return NotFound();

        entity.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        _logger.LogInformation("DELETE /api/podrucje/{IdPodrucje} - područje obrisano (soft delete).", id);

        return NoContent();
    }

    private static PodrucjeDto ToDto(Podrucje p) => new()
    {
        IdPodrucje = p.IdPodrucje,
        Naziv = p.Naziv,
        Opis = p.Opis,
        Regija = p.Regija,
        MinimalanBrojKTZaObilazak = p.MinimalanBrojKTZaObilazak,
        BrojKontrolnihTocaka = p.KontrolneTocke?.Count ?? 0
    };
}
