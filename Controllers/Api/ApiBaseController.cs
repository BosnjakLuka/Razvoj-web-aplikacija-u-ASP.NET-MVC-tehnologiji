using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;

namespace planinarenje.Controllers.Api;

/// <summary>
/// Bazni razred za Web API kontrolere. Sadrži zajedničke pomoćne metode za
/// dohvat trenutno prijavljenog planinarskog profila (Korisnik) i provjeru vlasništva.
/// </summary>
[ApiController]
public abstract class ApiBaseController : ControllerBase
{
    protected readonly UserManager<AppUser> UserMgr;
    protected readonly PlaninarstvoDbContext Db;

    protected ApiBaseController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db)
    {
        UserMgr = userMgr;
        Db = db;
    }

    /// <summary>Identity Id (string GUID) trenutno prijavljenog korisnika ili null.</summary>
    protected string? AppUserId => UserMgr.GetUserId(User);

    /// <summary>Je li trenutni korisnik u roli Admin.</summary>
    protected bool IsAdmin => User.IsInRole("Admin");

    /// <summary>Dohvaća planinarski profil (Korisnik) povezan s prijavljenim Identity korisnikom.</summary>
    protected async Task<Korisnik?> GetCurrentKorisnikAsync()
    {
        var id = AppUserId;
        if (id is null) return null;
        return await Db.Korisnici.FirstOrDefaultAsync(k => k.AppUserId == id);
    }

    /// <summary>True ako prijavljeni korisnik posjeduje zapis s danim IdKorisnik (ili je Admin).</summary>
    protected async Task<bool> IsOwnerOrAdminAsync(int idKorisnik)
    {
        if (IsAdmin) return true;
        var k = await GetCurrentKorisnikAsync();
        return k != null && k.IdKorisnik == idKorisnik;
    }
}
