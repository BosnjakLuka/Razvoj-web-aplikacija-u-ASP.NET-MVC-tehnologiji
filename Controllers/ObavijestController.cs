using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;

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
}
