using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;

namespace planinarenje.Controllers
{
    public class PodrucjeController : Controller
    {
        private readonly PlaninarstvoDbContext _dbContext;

        public PodrucjeController(PlaninarstvoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var ktCountByPodrucje = _dbContext.KontrolneTocke
                .GroupBy(kt => kt.IdPodrucje)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionary(x => x.Key, x => x.Count);

            var model = _dbContext.Podrucja
                .OrderBy(p => p.IdPodrucje)
                .AsEnumerable()
                .Select(p => new PodrucjeIndexCardViewModel
                {
                    IdPodrucje = p.IdPodrucje,
                    Naziv = p.Naziv,
                    Regija = string.IsNullOrWhiteSpace(p.Regija) ? "Regija nije navedena" : p.Regija,
                    OpisPreview = TrimOpis(p.Opis, 170),
                    MinimalanBrojKTZaObilazak = p.MinimalanBrojKTZaObilazak,
                    UkupanBrojKT = ktCountByPodrucje.TryGetValue(p.IdPodrucje, out var count) ? count : 0
                })
                .ToList();

            ViewData["Title"] = "Podrucja";
            return View(model);
        }

        public IActionResult Details(int id)
        {
            var podrucje = _dbContext.Podrucja
                .Include(p => p.KontrolneTocke)
                .Include(p => p.PlaninarskiObjekti)
                .FirstOrDefault(p => p.IdPodrucje == id);
            if (podrucje is null)
            {
                return NotFound();
            }

            if (podrucje.KontrolneTocke != null)
            {
                podrucje.KontrolneTocke = podrucje.KontrolneTocke
                    .OrderBy(kt => kt.Naziv)
                    .ToList();
            }

            ViewData["Title"] = podrucje.Naziv;
            return View(podrucje);
        }

        private static string? TrimOpis(string? opis, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(opis))
            {
                return null;
            }

            var normalized = opis.Trim();
            if (normalized.Length <= maxLength)
            {
                return normalized;
            }

            return normalized[..maxLength].TrimEnd() + "...";
        }
    }
}
