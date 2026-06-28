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
        var model = _dbContext.Obavijesti
            .Include(o => o.Korisnik)
            .OrderByDescending(o => o.DatumObjave)
            .ToList();

        return View(model);
    }

    public IActionResult Search(string? searchTerm)
    {
        var query = _dbContext.Obavijesti
            .Include(o => o.Korisnik)
            .AsQueryable();

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

    public IActionResult Edit(int id)
    {
        var obavijest = _dbContext.Obavijesti
            .Include(o => o.Korisnik)
            .FirstOrDefault(o => o.IdObavijest == id);
        if (obavijest == null) return NotFound();

        ViewData["KorisnikText"] = obavijest.Korisnik != null
            ? obavijest.Korisnik.Ime + " " + obavijest.Korisnik.Prezime + " (@" + obavijest.Korisnik.KorisnickoIme + ")"
            : GetKorisnikLabel(obavijest.IdKorisnik);
        return View(obavijest);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Obavijest obavijest)
    {
        if (id != obavijest.IdObavijest) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewData["KorisnikText"] = GetKorisnikLabel(obavijest.IdKorisnik);
            return View(obavijest);
        }

        _dbContext.Obavijesti.Update(obavijest);
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

        return View(obavijest);
    }

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
