using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.Fotografija;

namespace planinarenje.Controllers.Api;

/// <summary>
/// Web API za fotografije vezane uz posjet.
/// Čitanje je javno; kreiranje/izmjena/brisanje radi vlasnik pripadajućeg posjeta ili Admin.
/// </summary>
[Route("api/fotografija")]
public class FotografijaApiController : ApiBaseController
{
    public FotografijaApiController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db)
        : base(userMgr, db)
    {
    }

    // GET /api/fotografija?posjetId=&tip=
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<FotografijaDto>>> Get(
        [FromQuery] int? posjetId,
        [FromQuery] TipSlike? tip)
    {
        var upit = Db.Fotografije.AsQueryable();

        if (posjetId.HasValue)
            upit = upit.Where(f => f.IdPosjet == posjetId.Value);
        if (tip.HasValue)
            upit = upit.Where(f => f.TipSlike == tip.Value);

        var rezultat = await upit.OrderByDescending(f => f.DatumUploada).ToListAsync();
        return Ok(rezultat.Select(ToDto).ToList());
    }

    // GET /api/fotografija/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<FotografijaDto>> GetById(int id)
    {
        var entity = await Db.Fotografije.FirstOrDefaultAsync(f => f.IdFotografija == id);
        if (entity == null)
            return NotFound();

        return Ok(ToDto(entity));
    }

    // POST /api/fotografija  (kreira metapodatke fotografije; upload datoteke je u MVC PosjetController – Faza 4)
    [HttpPost]
    [Authorize(Roles = "Admin,Planinar")]
    public async Task<ActionResult<FotografijaDto>> Post([FromBody] FotografijaCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var posjet = await Db.Posjeti.FirstOrDefaultAsync(p => p.IdPosjet == dto.IdPosjet);
        if (posjet == null)
            return BadRequest("Posjet nije pronađen.");

        if (!await IsOwnerOrAdminAsync(posjet.IdKorisnik))
            return Forbid();

        var entity = new Fotografija
        {
            IdPosjet = dto.IdPosjet,
            NazivDatoteke = dto.NazivDatoteke,
            PutanjaDatoteke = dto.PutanjaDatoteke,
            TipSlike = dto.TipSlike,
            Opis = dto.Opis,
            ContentType = dto.ContentType,
            FileSize = dto.FileSize,
            DatumUploada = DateTime.UtcNow
        };

        Db.Fotografije.Add(entity);
        await Db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.IdFotografija }, ToDto(entity));
    }

    // PUT /api/fotografija/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Planinar")]
    public async Task<ActionResult<FotografijaDto>> Put(int id, [FromBody] FotografijaUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = await Db.Fotografije
            .Include(f => f.Posjet)
            .FirstOrDefaultAsync(f => f.IdFotografija == id);
        if (entity == null)
            return NotFound();

        if (!await IsOwnerOrAdminAsync(entity.Posjet.IdKorisnik))
            return Forbid();

        entity.TipSlike = dto.TipSlike;
        entity.Opis = dto.Opis;

        await Db.SaveChangesAsync();

        return Ok(ToDto(entity));
    }

    // DELETE /api/fotografija/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Planinar")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await Db.Fotografije
            .Include(f => f.Posjet)
            .FirstOrDefaultAsync(f => f.IdFotografija == id);
        if (entity == null)
            return NotFound();

        if (!await IsOwnerOrAdminAsync(entity.Posjet.IdKorisnik))
            return Forbid();

        entity.DeletedAt = DateTime.UtcNow;
        await Db.SaveChangesAsync();

        return NoContent();
    }

    private static FotografijaDto ToDto(Fotografija f) => new()
    {
        IdFotografija = f.IdFotografija,
        IdPosjet = f.IdPosjet,
        NazivDatoteke = f.NazivDatoteke,
        PutanjaDatoteke = f.PutanjaDatoteke,
        DatumUploada = f.DatumUploada,
        TipSlike = f.TipSlike.ToString(),
        Opis = f.Opis,
        ContentType = f.ContentType,
        FileSize = f.FileSize
    };
}
