using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.Knjizica;

namespace planinarenje.Controllers.Api;

/// <summary>
/// Web API za digitalne planinarske knjižice (1:1 s korisnikom).
/// Knjižicu vidi njezin vlasnik ili Admin; izmjenu radi vlasnik, brisanje Admin.
/// </summary>
[Route("api/knjizica")]
public class KnjizicaApiController : ApiBaseController
{
    public KnjizicaApiController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db)
        : base(userMgr, db)
    {
    }

    // GET /api/knjizica  (vlasnik vidi svoje; Admin vidi sve)
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<KnjizicaDto>>> Get()
    {
        var upit = Db.Knjizice
            .Include(k => k.Korisnik)
            .Include(k => k.Posjeti)
            .Where(k => k.StatusAktivna);

        if (!IsAdmin)
        {
            var korisnik = await GetCurrentKorisnikAsync();
            if (korisnik == null)
                return Forbid();
            upit = upit.Where(k => k.IdKorisnik == korisnik.IdKorisnik);
        }

        var rezultat = await upit.OrderBy(k => k.IdKnjizica).ToListAsync();
        return Ok(rezultat.Select(ToDto).ToList());
    }

    // GET /api/knjizica/5
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<KnjizicaDto>> GetById(int id)
    {
        var entity = await Db.Knjizice
            .Include(k => k.Korisnik)
            .Include(k => k.Posjeti)
            .FirstOrDefaultAsync(k => k.IdKnjizica == id && k.StatusAktivna);

        if (entity == null)
            return NotFound();

        if (!await IsOwnerOrAdminAsync(entity.IdKorisnik))
            return Forbid();

        return Ok(ToDto(entity));
    }

    // POST /api/knjizica
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<KnjizicaDto>> Post([FromBody] KnjizicaCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await Db.Korisnici.AnyAsync(k => k.IdKorisnik == dto.IdKorisnik && k.StatusAktivan))
            return BadRequest("Korisnik nije pronađen ili nije aktivan.");

        // Knjižica je 1:1 s korisnikom.
        if (await Db.Knjizice.AnyAsync(k => k.IdKorisnik == dto.IdKorisnik))
            return BadRequest("Korisnik već ima knjižicu.");

        var entity = new Knjizica
        {
            IdKorisnik = dto.IdKorisnik,
            Napomena = dto.Napomena,
            StatusAktivna = dto.StatusAktivna,
            DatumKreiranja = DateTime.UtcNow
        };

        Db.Knjizice.Add(entity);
        await Db.SaveChangesAsync();

        var kreirana = await Db.Knjizice
            .Include(k => k.Korisnik)
            .Include(k => k.Posjeti)
            .FirstAsync(k => k.IdKnjizica == entity.IdKnjizica);

        return CreatedAtAction(nameof(GetById), new { id = entity.IdKnjizica }, ToDto(kreirana));
    }

    // PUT /api/knjizica/5
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<KnjizicaDto>> Put(int id, [FromBody] KnjizicaUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = await Db.Knjizice.FirstOrDefaultAsync(k => k.IdKnjizica == id);
        if (entity == null)
            return NotFound();

        if (!await IsOwnerOrAdminAsync(entity.IdKorisnik))
            return Forbid();

        entity.Napomena = dto.Napomena;
        entity.StatusAktivna = dto.StatusAktivna;

        await Db.SaveChangesAsync();

        var azurirana = await Db.Knjizice
            .Include(k => k.Korisnik)
            .Include(k => k.Posjeti)
            .FirstAsync(k => k.IdKnjizica == id);

        return Ok(ToDto(azurirana));
    }

    // DELETE /api/knjizica/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await Db.Knjizice.FirstOrDefaultAsync(k => k.IdKnjizica == id);
        if (entity == null)
            return NotFound();

        // Knjizica nema DeletedAt – koristi se logičko gašenje preko StatusAktivna.
        entity.StatusAktivna = false;
        await Db.SaveChangesAsync();

        return NoContent();
    }

    private static KnjizicaDto ToDto(Knjizica k) => new()
    {
        IdKnjizica = k.IdKnjizica,
        IdKorisnik = k.IdKorisnik,
        ImePrezimeKorisnika = k.Korisnik == null ? string.Empty : $"{k.Korisnik.Ime} {k.Korisnik.Prezime}",
        DatumKreiranja = k.DatumKreiranja,
        Napomena = k.Napomena,
        StatusAktivna = k.StatusAktivna,
        BrojPosjeta = k.Posjeti?.Count ?? 0
    };
}
