using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.Medalja;

namespace planinarenje.Controllers.Api;

/// <summary>
/// Web API za medalje (nagrade za broj obilazaka).
/// Čitanje je javno; pisanje je dopušteno samo administratorima.
/// </summary>
[Route("api/medalja")]
[ApiController]
public class MedaljaApiController : ControllerBase
{
    private readonly PlaninarstvoDbContext _db;
    private readonly ILogger<MedaljaApiController> _logger;

    public MedaljaApiController(PlaninarstvoDbContext db, ILogger<MedaljaApiController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // GET /api/medalja?naziv=
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<MedaljaDto>>> Get([FromQuery] string? naziv)
    {
        var upit = _db.Medalje.AsQueryable();

        if (!string.IsNullOrWhiteSpace(naziv))
            upit = upit.Where(m => m.Naziv.Contains(naziv));

        var rezultat = await upit.OrderBy(m => m.MinimalanBrojKontrolnihTocaka).ToListAsync();
        return Ok(rezultat.Select(ToDto).ToList());
    }

    // GET /api/medalja/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<MedaljaDto>> GetById(int id)
    {
        var entity = await _db.Medalje.FirstOrDefaultAsync(m => m.IdMedalja == id);
        if (entity == null)
            return NotFound();

        return Ok(ToDto(entity));
    }

    // POST /api/medalja
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<MedaljaDto>> Post([FromBody] MedaljaCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = new Medalja
        {
            Naziv = dto.Naziv,
            Opis = dto.Opis,
            MinimalanBrojKontrolnihTocaka = dto.MinimalanBrojKontrolnihTocaka,
            MinimalanBrojPodrucja = dto.MinimalanBrojPodrucja
        };

        _db.Medalje.Add(entity);
        await _db.SaveChangesAsync();
        _logger.LogInformation("POST /api/medalja - medalja {IdMedalja} kreirana.", entity.IdMedalja);

        return CreatedAtAction(nameof(GetById), new { id = entity.IdMedalja }, ToDto(entity));
    }

    // PUT /api/medalja/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<MedaljaDto>> Put(int id, [FromBody] MedaljaUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = await _db.Medalje.FirstOrDefaultAsync(m => m.IdMedalja == id);
        if (entity == null)
            return NotFound();

        entity.Naziv = dto.Naziv;
        entity.Opis = dto.Opis;
        entity.MinimalanBrojKontrolnihTocaka = dto.MinimalanBrojKontrolnihTocaka;
        entity.MinimalanBrojPodrucja = dto.MinimalanBrojPodrucja;

        await _db.SaveChangesAsync();
        _logger.LogInformation("PUT /api/medalja/{IdMedalja} - medalja ažurirana.", id);

        return Ok(ToDto(entity));
    }

    // DELETE /api/medalja/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Medalje.FirstOrDefaultAsync(m => m.IdMedalja == id);
        if (entity == null)
            return NotFound();

        entity.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        _logger.LogInformation("DELETE /api/medalja/{IdMedalja} - medalja obrisana (soft delete).", id);

        return NoContent();
    }

    private static MedaljaDto ToDto(Medalja m) => new()
    {
        IdMedalja = m.IdMedalja,
        Naziv = m.Naziv,
        Opis = m.Opis,
        MinimalanBrojKontrolnihTocaka = m.MinimalanBrojKontrolnihTocaka,
        MinimalanBrojPodrucja = m.MinimalanBrojPodrucja
    };
}
