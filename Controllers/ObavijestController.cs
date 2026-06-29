using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace planinarenje.Controllers;

public class ObavijestController : BaseController
{
    private readonly PlaninarstvoDbContext _dbContext;
    private readonly ILogger<ObavijestController> _logger;

    public ObavijestController(UserManager<AppUser> userMgr, PlaninarstvoDbContext dbContext, ILogger<ObavijestController> logger)
        : base(userMgr, dbContext)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public IActionResult Index()
    {
        var query = _dbContext.Obavijesti
            .Include(o => o.Korisnik)
            .AsQueryable();

        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            query = query.Where(o => o.JeAktivna);
        }

        var model = query
            .OrderByDescending(o => o.DatumObjave)
            .ToList();

        return View(model);
    }

    public IActionResult Search(string? searchTerm)
    {
        var query = _dbContext.Obavijesti
            .Include(o => o.Korisnik)
            .AsQueryable();

        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            query = query.Where(o => o.JeAktivna);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(o =>
                o.Naslov.ToLower().Contains(term) ||
                (o.Sadrzaj != null && o.Sadrzaj.ToLower().Contains(term)) ||
                (o.Korisnik != null &&
                 (o.Korisnik.Ime + " " + o.Korisnik.Prezime).ToLower().Contains(term)));
        }

        var model = query
            .OrderByDescending(o => o.DatumObjave)
            .ToList();

        return PartialView("_ObavijestListPartial", model);
    }

    [Authorize]
    public IActionResult Create()
    {
        return View(new Obavijest
        {
            DatumObjave = DateTime.Now,
            JeAktivna = true
        });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Obavijest obavijest)
    {
        var korisnik = await GetCurrentKorisnikAsync();
        if (korisnik == null)
        {
            return Forbid();
        }

        obavijest.IdKorisnik = korisnik.IdKorisnik;
        ModelState.Remove(nameof(Obavijest.IdKorisnik));

        if (!ModelState.IsValid)
        {
            return View(obavijest);
        }

        _dbContext.Obavijesti.Add(obavijest);
        _dbContext.SaveChanges();
        _logger.LogInformation("Obavijest {IdObavijest} kreirana ({Naslov}).", obavijest.IdObavijest, obavijest.Naslov);
        TempData["NewId"] = obavijest.IdObavijest;
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var obavijest = _dbContext.Obavijesti
            .Include(o => o.Korisnik)
            .FirstOrDefault(o => o.IdObavijest == id);
        if (obavijest == null) return NotFound();

        if (!IsAdmin && !await IsOwnerAsync(obavijest.IdKorisnik))
            return Forbid();

        ViewData["IsAdmin"] = IsAdmin;
        ViewData["KorisnikText"] = obavijest.Korisnik != null
            ? obavijest.Korisnik.Ime + " " + obavijest.Korisnik.Prezime + " (@" + obavijest.Korisnik.KorisnickoIme + ")"
            : GetKorisnikLabel(obavijest.IdKorisnik);
        return View(obavijest);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Obavijest obavijest)
    {
        if (id != obavijest.IdObavijest) return NotFound();

        var postojeca = _dbContext.Obavijesti.FirstOrDefault(o => o.IdObavijest == id);
        if (postojeca == null) return NotFound();

        if (!IsAdmin && !await IsOwnerAsync(postojeca.IdKorisnik))
        {
            _logger.LogWarning("Korisnik {AppUserId} bez prava pristupa pokušao je urediti obavijest {IdObavijest}.", AppUserId, id);
            return Forbid();
        }

        // Autor se ne moze promijeniti kroz formu osim ako je Admin.
        if (!IsAdmin)
        {
            obavijest.IdKorisnik = postojeca.IdKorisnik;
        }
        ModelState.Remove(nameof(Obavijest.IdKorisnik));

        if (!ModelState.IsValid)
        {
            ViewData["IsAdmin"] = IsAdmin;
            ViewData["KorisnikText"] = GetKorisnikLabel(obavijest.IdKorisnik);
            return View(obavijest);
        }

        postojeca.Naslov = obavijest.Naslov;
        postojeca.Sadrzaj = obavijest.Sadrzaj;
        postojeca.DatumObjave = obavijest.DatumObjave;
        postojeca.JeAktivna = obavijest.JeAktivna;
        postojeca.IdKorisnik = obavijest.IdKorisnik;

        _dbContext.SaveChanges();
        _logger.LogInformation("Obavijest {IdObavijest} ažurirana.", id);
        return RedirectToAction(nameof(Index));
    }

    [Route("obavijest/{id:int}")]
    [Route("[controller]/[action]/{id:int}")]
    public IActionResult Details(int id)
    {
        var obavijest = _dbContext.Obavijesti
            .Include(o => o.Korisnik)
            .FirstOrDefault(o => o.IdObavijest == id);

        if (obavijest == null) return NotFound();

        if (!obavijest.JeAktivna && (User.Identity == null || !User.Identity.IsAuthenticated))
        {
            return NotFound();
        }

        return View(obavijest);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var obavijest = _dbContext.Obavijesti
            .Include(o => o.Korisnik)
            .FirstOrDefault(o => o.IdObavijest == id);

        if (obavijest == null) return NotFound();

        ViewData["Title"] = "Obrisi obavijest";
        return View(obavijest);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteConfirmed(int id)
    {
        var obavijest = _dbContext.Obavijesti.FirstOrDefault(o => o.IdObavijest == id);
        if (obavijest == null) return NotFound();

        _dbContext.Obavijesti.Remove(obavijest);
        _dbContext.SaveChanges();
        _logger.LogInformation("Obavijest {IdObavijest} obrisana.", id);
        TempData["Success"] = "Obavijest je uspjesno obrisana.";
        return RedirectToAction(nameof(Index));
    }

    private string? GetKorisnikLabel(int? id)
    {
        if (!id.HasValue)
        {
            return null;
        }

        return _dbContext.Korisnici
            .Where(k => k.StatusAktivan && k.IdKorisnik == id.Value)
            .Select(k => k.Ime + " " + k.Prezime + " (@" + k.KorisnickoIme + ")")
            .FirstOrDefault();
    }
}
