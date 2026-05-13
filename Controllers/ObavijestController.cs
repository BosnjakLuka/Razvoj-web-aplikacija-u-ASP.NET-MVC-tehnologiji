using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using System;
using System.Linq;

namespace planinarenje.Controllers;

public class ObavijestController : Controller
{
    private readonly PlaninarstvoDbContext _dbContext;

    public ObavijestController(PlaninarstvoDbContext dbContext)
    {
        _dbContext = dbContext;
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

    public IActionResult Create()
    {
        LoadKorisniciSelectList();
        return View(new Obavijest
        {
            DatumObjave = DateTime.Now,
            JeAktivna = true
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Obavijest obavijest)
    {
        if (!ModelState.IsValid)
        {
            LoadKorisniciSelectList(obavijest.IdKorisnik);
            return View(obavijest);
        }

        _dbContext.Obavijesti.Add(obavijest);
        _dbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var obavijest = _dbContext.Obavijesti.Find(id);
        if (obavijest == null) return NotFound();

        LoadKorisniciSelectList(obavijest.IdKorisnik);
        return View(obavijest);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Obavijest obavijest)
    {
        if (id != obavijest.IdObavijest) return NotFound();

        if (!ModelState.IsValid)
        {
            LoadKorisniciSelectList(obavijest.IdKorisnik);
            return View(obavijest);
        }

        _dbContext.Obavijesti.Update(obavijest);
        _dbContext.SaveChanges();
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
        TempData["Success"] = "Obavijest je uspjesno obrisana.";
        return RedirectToAction(nameof(Index));
    }

    private void LoadKorisniciSelectList(int? selectedId = null)
    {
        var korisnici = _dbContext.Korisnici
            .OrderBy(k => k.Prezime)
            .ThenBy(k => k.Ime)
            .Select(k => new
            {
                k.IdKorisnik,
                Naziv = k.Ime + " " + k.Prezime
            })
            .ToList();

        ViewBag.Korisnici = new SelectList(korisnici, "IdKorisnik", "Naziv", selectedId);
    }
}
