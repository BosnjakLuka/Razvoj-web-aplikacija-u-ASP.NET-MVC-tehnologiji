using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;

namespace planinarenje.Controllers;

public abstract class BaseController : Controller
{
    protected readonly UserManager<AppUser> UserMgr;
    protected readonly PlaninarstvoDbContext Db;

    protected BaseController(UserManager<AppUser> userMgr, PlaninarstvoDbContext db)
    {
        UserMgr = userMgr;
        Db = db;
    }

    protected string? AppUserId => UserMgr.GetUserId(User);

    protected bool IsAdmin => User.IsInRole("Admin");

    protected async Task<Korisnik?> GetCurrentKorisnikAsync()
    {
        var id = AppUserId;
        if (id is null) return null;
        return await Db.Korisnici.FirstOrDefaultAsync(k => k.AppUserId == id);
    }

    protected async Task<bool> IsOwnerAsync(int idKorisnik)
    {
        var k = await GetCurrentKorisnikAsync();
        return k != null && k.IdKorisnik == idKorisnik;
    }
}
