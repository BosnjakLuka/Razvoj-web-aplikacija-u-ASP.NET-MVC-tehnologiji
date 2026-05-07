using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;
using planinarenje.Models;

namespace planinarenje.Controllers
{
    public class KontrolnaTockaController : Controller
    {
        private readonly PlaninarstvoDbContext _dbContext;

        public KontrolnaTockaController(PlaninarstvoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var model = _dbContext.KontrolneTocke
                .Include(k => k.Podrucje)
                .OrderBy(k => k.Naziv)
                .Select(k => new KontrolnaTockaIndexCardViewModel
                {
                    IdKontrolnaTocka = k.IdKontrolnaTocka,
                    Naziv = k.Naziv,
                    TipKontrolneTockeNaziv = MapTip(k.TipKontrolneTocke),
                    NadmorskaVisina = k.NadmorskaVisina,
                    PodrucjeNaziv = k.Podrucje != null ? k.Podrucje.Naziv : "Nepoznato podrucje",
                    OpisPreview = TrimOpis(k.Opis, 140),
                    GUIDOznaka = k.GUIDOznaka
                })
                .ToList();

            ViewData["Title"] = "Kontrolne tocke";
            return View(model);
        }

        [Route("vrh/{id:int}")]
        [Route("[controller]/[action]/{id:int}")]
        public IActionResult Details(int id)
        {
            var kontrolnaTocka = _dbContext.KontrolneTocke
                .Include(k => k.Podrucje)
                .FirstOrDefault(k => k.IdKontrolnaTocka == id);
            if (kontrolnaTocka is null)
            {
                return NotFound();
            }
            ViewData["Title"] = kontrolnaTocka.Naziv;
            return View(kontrolnaTocka);
        }

        private static string MapTip(TipKontrolneTocke tip)
        {
            return tip switch
            {
                TipKontrolneTocke.Vrh => "Vrh",
                TipKontrolneTocke.Vidikovac => "Vidikovac",
                TipKontrolneTocke.KontrolnaTocka => "Kontrolna tocka",
                _ => "Tip nije definiran"
            };
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
