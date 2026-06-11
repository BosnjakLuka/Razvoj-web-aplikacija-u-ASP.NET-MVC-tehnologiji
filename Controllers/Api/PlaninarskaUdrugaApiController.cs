using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.PlaninarskaUdruga;

namespace planinarenje.Controllers.Api;

/// <summary>
/// Web API za planinarske udruge.
/// Čitanje je javno; pisanje je dopušteno samo administratorima.
/// </summary>
[Route("api/planinarskaudruga")]
[ApiController]
public class PlaninarskaUdrugaApiController : ControllerBase
{
    private readonly PlaninarstvoDbContext _db;

    public PlaninarskaUdrugaApiController(PlaninarstvoDbContext db)
    {
        _db = db;
    }

    // GET /api/planinarskaudruga?naziv=&grad=
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<PlaninarskaUdrugaDto>>> Get(
        [FromQuery] string? naziv,
        [FromQuery] string? grad)
    {
        var upit = _db.PlaninarskeUdruge.AsQueryable();

        if (!string.IsNullOrWhiteSpace(naziv))
            upit = upit.Where(u => u.Naziv.Contains(naziv));
        if (!string.IsNullOrWhiteSpace(grad))
            upit = upit.Where(u => u.Grad != null && u.Grad.Contains(grad));

        var rezultat = await upit.OrderBy(u => u.Naziv).ToListAsync();
        return Ok(rezultat.Select(ToDto).ToList());
    }

    // GET /api/planinarskaudruga/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<PlaninarskaUdrugaDto>> GetById(int id)
    {
        var entity = await _db.PlaninarskeUdruge.FirstOrDefaultAsync(u => u.IdPlaninarskaUdruga == id);
        if (entity == null)
            return NotFound();

        return Ok(ToDto(entity));
    }

    // POST /api/planinarskaudruga
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PlaninarskaUdrugaDto>> Post([FromBody] PlaninarskaUdrugaCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _db.PlaninarskeUdruge.AnyAsync(u => u.OIB == dto.OIB))
            return BadRequest("Udruga s tim OIB-om već postoji.");

        var entity = new PlaninarskaUdruga
        {
            OIB = dto.OIB,
            Naziv = dto.Naziv,
            Email = dto.Email,
            BrojTelefona = dto.BrojTelefona,
            Adresa = dto.Adresa,
            PostanskiBroj = dto.PostanskiBroj,
            Grad = dto.Grad,
            Zupanija = dto.Zupanija,
            BrojClanova = dto.BrojClanova
        };

        _db.PlaninarskeUdruge.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.IdPlaninarskaUdruga }, ToDto(entity));
    }

    // PUT /api/planinarskaudruga/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PlaninarskaUdrugaDto>> Put(int id, [FromBody] PlaninarskaUdrugaUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = await _db.PlaninarskeUdruge.FirstOrDefaultAsync(u => u.IdPlaninarskaUdruga == id);
        if (entity == null)
            return NotFound();

        if (await _db.PlaninarskeUdruge.AnyAsync(u => u.OIB == dto.OIB && u.IdPlaninarskaUdruga != id))
            return BadRequest("Druga udruga s tim OIB-om već postoji.");

        entity.OIB = dto.OIB;
        entity.Naziv = dto.Naziv;
        entity.Email = dto.Email;
        entity.BrojTelefona = dto.BrojTelefona;
        entity.Adresa = dto.Adresa;
        entity.PostanskiBroj = dto.PostanskiBroj;
        entity.Grad = dto.Grad;
        entity.Zupanija = dto.Zupanija;
        entity.BrojClanova = dto.BrojClanova;

        await _db.SaveChangesAsync();

        return Ok(ToDto(entity));
    }

    // DELETE /api/planinarskaudruga/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.PlaninarskeUdruge.FirstOrDefaultAsync(u => u.IdPlaninarskaUdruga == id);
        if (entity == null)
            return NotFound();

        entity.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private static PlaninarskaUdrugaDto ToDto(PlaninarskaUdruga u) => new()
    {
        IdPlaninarskaUdruga = u.IdPlaninarskaUdruga,
        OIB = u.OIB,
        Naziv = u.Naziv,
        Email = u.Email,
        BrojTelefona = u.BrojTelefona,
        Adresa = u.Adresa,
        PostanskiBroj = u.PostanskiBroj,
        Grad = u.Grad,
        Zupanija = u.Zupanija,
        BrojClanova = u.BrojClanova
    };
}
