using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.PlaninarskiObjekt;

namespace planinarenje.Controllers.Api;

/// <summary>
/// Web API za planinarske objekte (domovi / kuće / skloništa).
/// Čitanje je javno; pisanje je dopušteno samo administratorima.
/// </summary>
[Route("api/planinarskiobjekt")]
[ApiController]
public class PlaninarskiObjektApiController : ControllerBase
{
    private readonly PlaninarstvoDbContext _db;
    private readonly ILogger<PlaninarskiObjektApiController> _logger;

    public PlaninarskiObjektApiController(PlaninarstvoDbContext db, ILogger<PlaninarskiObjektApiController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // GET /api/planinarskiobjekt?podrucjeId=&udrugaId=&tip=&naziv=
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<PlaninarskiObjektDto>>> Get(
        [FromQuery] int? podrucjeId,
        [FromQuery] int? udrugaId,
        [FromQuery] TipObjekta? tip,
        [FromQuery] string? naziv)
    {
        var upit = _db.PlaninarskiObjekti
            .Include(o => o.Podrucje)
            .Include(o => o.PlaninarskaUdruga)
            .AsQueryable();

        if (podrucjeId.HasValue)
            upit = upit.Where(o => o.IdPodrucje == podrucjeId.Value);
        if (udrugaId.HasValue)
            upit = upit.Where(o => o.IdPlaninarskaUdruga == udrugaId.Value);
        if (tip.HasValue)
            upit = upit.Where(o => o.TipObjekta == tip.Value);
        if (!string.IsNullOrWhiteSpace(naziv))
            upit = upit.Where(o => o.Naziv.Contains(naziv));

        var rezultat = await upit.OrderBy(o => o.Naziv).ToListAsync();
        return Ok(rezultat.Select(ToDto).ToList());
    }

    // GET /api/planinarskiobjekt/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<PlaninarskiObjektDto>> GetById(int id)
    {
        var entity = await _db.PlaninarskiObjekti
            .Include(o => o.Podrucje)
            .Include(o => o.PlaninarskaUdruga)
            .FirstOrDefaultAsync(o => o.IdPlaninarskiObjekt == id);

        if (entity == null)
            return NotFound();

        return Ok(ToDto(entity));
    }

    // POST /api/planinarskiobjekt
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PlaninarskiObjektDto>> Post([FromBody] PlaninarskiObjektCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _db.Podrucja.AnyAsync(p => p.IdPodrucje == dto.IdPodrucje))
            return BadRequest("Područje nije pronađeno.");
        if (!await _db.PlaninarskeUdruge.AnyAsync(u => u.IdPlaninarskaUdruga == dto.IdPlaninarskaUdruga))
            return BadRequest("Planinarska udruga nije pronađena.");

        var entity = new PlaninarskiObjekt
        {
            IdPodrucje = dto.IdPodrucje,
            IdPlaninarskaUdruga = dto.IdPlaninarskaUdruga,
            Naziv = dto.Naziv,
            TipObjekta = dto.TipObjekta,
            NadmorskaVisina = dto.NadmorskaVisina,
            Kapacitet = dto.Kapacitet,
            Opis = dto.Opis,
            ImeOdgovorneOsobe = dto.ImeOdgovorneOsobe,
            Telefon = dto.Telefon,
            Email = dto.Email,
            Adresa = dto.Adresa,
            ImaNocenje = dto.ImaNocenje,
            ImaHranu = dto.ImaHranu,
            RadnoVrijemeOpis = dto.RadnoVrijemeOpis
        };

        _db.PlaninarskiObjekti.Add(entity);
        await _db.SaveChangesAsync();
        _logger.LogInformation("POST /api/planinarskiobjekt - objekt {IdPlaninarskiObjekt} kreiran.", entity.IdPlaninarskiObjekt);

        var kreiran = await _db.PlaninarskiObjekti
            .Include(o => o.Podrucje)
            .Include(o => o.PlaninarskaUdruga)
            .FirstAsync(o => o.IdPlaninarskiObjekt == entity.IdPlaninarskiObjekt);

        return CreatedAtAction(nameof(GetById), new { id = entity.IdPlaninarskiObjekt }, ToDto(kreiran));
    }

    // PUT /api/planinarskiobjekt/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PlaninarskiObjektDto>> Put(int id, [FromBody] PlaninarskiObjektUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = await _db.PlaninarskiObjekti.FirstOrDefaultAsync(o => o.IdPlaninarskiObjekt == id);
        if (entity == null)
            return NotFound();

        if (!await _db.Podrucja.AnyAsync(p => p.IdPodrucje == dto.IdPodrucje))
            return BadRequest("Područje nije pronađeno.");
        if (!await _db.PlaninarskeUdruge.AnyAsync(u => u.IdPlaninarskaUdruga == dto.IdPlaninarskaUdruga))
            return BadRequest("Planinarska udruga nije pronađena.");

        entity.IdPodrucje = dto.IdPodrucje;
        entity.IdPlaninarskaUdruga = dto.IdPlaninarskaUdruga;
        entity.Naziv = dto.Naziv;
        entity.TipObjekta = dto.TipObjekta;
        entity.NadmorskaVisina = dto.NadmorskaVisina;
        entity.Kapacitet = dto.Kapacitet;
        entity.Opis = dto.Opis;
        entity.ImeOdgovorneOsobe = dto.ImeOdgovorneOsobe;
        entity.Telefon = dto.Telefon;
        entity.Email = dto.Email;
        entity.Adresa = dto.Adresa;
        entity.ImaNocenje = dto.ImaNocenje;
        entity.ImaHranu = dto.ImaHranu;
        entity.RadnoVrijemeOpis = dto.RadnoVrijemeOpis;

        await _db.SaveChangesAsync();
        _logger.LogInformation("PUT /api/planinarskiobjekt/{IdPlaninarskiObjekt} - objekt ažuriran.", id);

        var azuriran = await _db.PlaninarskiObjekti
            .Include(o => o.Podrucje)
            .Include(o => o.PlaninarskaUdruga)
            .FirstAsync(o => o.IdPlaninarskiObjekt == id);

        return Ok(ToDto(azuriran));
    }

    // DELETE /api/planinarskiobjekt/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.PlaninarskiObjekti.FirstOrDefaultAsync(o => o.IdPlaninarskiObjekt == id);
        if (entity == null)
            return NotFound();

        entity.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        _logger.LogInformation("DELETE /api/planinarskiobjekt/{IdPlaninarskiObjekt} - objekt obrisan (soft delete).", id);

        return NoContent();
    }

    private static PlaninarskiObjektDto ToDto(PlaninarskiObjekt o) => new()
    {
        IdPlaninarskiObjekt = o.IdPlaninarskiObjekt,
        IdPodrucje = o.IdPodrucje,
        NazivPodrucja = o.Podrucje?.Naziv ?? string.Empty,
        IdPlaninarskaUdruga = o.IdPlaninarskaUdruga,
        NazivUdruge = o.PlaninarskaUdruga?.Naziv ?? string.Empty,
        Naziv = o.Naziv,
        TipObjekta = o.TipObjekta.ToString(),
        NadmorskaVisina = o.NadmorskaVisina,
        Kapacitet = o.Kapacitet,
        Opis = o.Opis,
        ImeOdgovorneOsobe = o.ImeOdgovorneOsobe,
        Telefon = o.Telefon,
        Email = o.Email,
        Adresa = o.Adresa,
        ImaNocenje = o.ImaNocenje,
        ImaHranu = o.ImaHranu,
        RadnoVrijemeOpis = o.RadnoVrijemeOpis
    };
}
