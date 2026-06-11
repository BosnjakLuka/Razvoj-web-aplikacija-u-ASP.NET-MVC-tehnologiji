using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models.Dto.Korisnik;

namespace planinarenje.Controllers.Api;

/// <summary>
/// Web API za planinarske profile (Korisnik).
/// Anonimno vraća javni prikaz (KorisnikPublicDto); administratoru vraća prošireni prikaz
/// (KorisnikAdminDto). OIB, JMBG i AppUserId se NIKAD ne izlažu kroz API.
/// </summary>
[Route("api/korisnik")]
public class KorisnikApiController : ApiBaseController
{
    public KorisnikApiController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db)
        : base(userMgr, db)
    {
    }

    // GET /api/korisnik?query=
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<KorisnikPublicDto>>> Get([FromQuery] string? query)
    {
        var upit = Db.Korisnici.Where(k => k.StatusAktivan);

        if (!string.IsNullOrWhiteSpace(query))
        {
            upit = upit.Where(k =>
                k.Ime.Contains(query) ||
                k.Prezime.Contains(query) ||
                k.KorisnickoIme.Contains(query));
        }

        var korisnici = await upit.OrderBy(k => k.Prezime).ThenBy(k => k.Ime).ToListAsync();

        // Admin dobiva prošireni prikaz, svi ostali javni.
        if (IsAdmin)
            return Ok(korisnici.Select(ToAdminDto).ToList());

        return Ok(korisnici.Select(ToPublicDto).ToList());
    }

    // GET /api/korisnik/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<KorisnikPublicDto>> GetById(int id)
    {
        var korisnik = await Db.Korisnici
            .FirstOrDefaultAsync(k => k.IdKorisnik == id && k.StatusAktivan);

        if (korisnik == null)
            return NotFound();

        if (IsAdmin)
            return Ok(ToAdminDto(korisnik));

        return Ok(ToPublicDto(korisnik));
    }

    // POST /api/korisnik
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<KorisnikAdminDto>> Post([FromBody] KorisnikCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await Db.Korisnici.AnyAsync(k => k.Email == dto.Email))
            return BadRequest("Korisnik s tim emailom već postoji.");
        if (await Db.Korisnici.AnyAsync(k => k.KorisnickoIme == dto.KorisnickoIme))
            return BadRequest("Korisnik s tim korisničkim imenom već postoji.");

        var entity = new Korisnik
        {
            Ime = dto.Ime,
            Prezime = dto.Prezime,
            Email = dto.Email,
            KorisnickoIme = dto.KorisnickoIme,
            DatumRodenja = dto.DatumRodenja,
            BrojMobitela = dto.BrojMobitela,
            ProfilnaSlika = dto.ProfilnaSlika,
            DatumRegistracije = DateTime.UtcNow,
            StatusAktivan = true
        };

        Db.Korisnici.Add(entity);
        await Db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.IdKorisnik }, ToAdminDto(entity));
    }

    // PUT /api/korisnik/5
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<KorisnikAdminDto>> Put(int id, [FromBody] KorisnikUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = await Db.Korisnici.FirstOrDefaultAsync(k => k.IdKorisnik == id);
        if (entity == null)
            return NotFound();

        // Izmjenu smije raditi vlasnik profila ili Admin.
        if (!await IsOwnerOrAdminAsync(entity.IdKorisnik))
            return Forbid();

        if (await Db.Korisnici.AnyAsync(k => k.Email == dto.Email && k.IdKorisnik != id))
            return BadRequest("Drugi korisnik s tim emailom već postoji.");
        if (await Db.Korisnici.AnyAsync(k => k.KorisnickoIme == dto.KorisnickoIme && k.IdKorisnik != id))
            return BadRequest("Drugi korisnik s tim korisničkim imenom već postoji.");

        entity.Ime = dto.Ime;
        entity.Prezime = dto.Prezime;
        entity.Email = dto.Email;
        entity.KorisnickoIme = dto.KorisnickoIme;
        entity.DatumRodenja = dto.DatumRodenja;
        entity.BrojMobitela = dto.BrojMobitela;
        entity.ProfilnaSlika = dto.ProfilnaSlika;
        // Status (de)aktivacije smije mijenjati samo Admin.
        if (IsAdmin)
            entity.StatusAktivan = dto.StatusAktivan;

        await Db.SaveChangesAsync();

        return Ok(ToAdminDto(entity));
    }

    // DELETE /api/korisnik/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await Db.Korisnici.FirstOrDefaultAsync(k => k.IdKorisnik == id);
        if (entity == null)
            return NotFound();

        // Korisnik nema DeletedAt – koristi se logičko gašenje preko StatusAktivan.
        entity.StatusAktivan = false;
        await Db.SaveChangesAsync();

        return NoContent();
    }

    private static KorisnikPublicDto ToPublicDto(Korisnik k) => new()
    {
        IdKorisnik = k.IdKorisnik,
        Ime = k.Ime,
        Prezime = k.Prezime,
        KorisnickoIme = k.KorisnickoIme,
        ProfilnaSlika = k.ProfilnaSlika,
        DatumRegistracije = k.DatumRegistracije
    };

    private static KorisnikAdminDto ToAdminDto(Korisnik k) => new()
    {
        IdKorisnik = k.IdKorisnik,
        Ime = k.Ime,
        Prezime = k.Prezime,
        KorisnickoIme = k.KorisnickoIme,
        ProfilnaSlika = k.ProfilnaSlika,
        DatumRegistracije = k.DatumRegistracije,
        Email = k.Email,
        BrojMobitela = k.BrojMobitela,
        DatumRodenja = k.DatumRodenja,
        StatusAktivan = k.StatusAktivan
    };
}
