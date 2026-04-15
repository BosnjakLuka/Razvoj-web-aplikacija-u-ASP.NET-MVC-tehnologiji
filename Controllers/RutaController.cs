using Microsoft.AspNetCore.Mvc;
using planinarenje.Entiteti;
using planinarenje.Models;
using planinarenje.Repositories;

namespace planinarenje.Controllers;

public class RutaController : Controller
{
    private readonly IRutaMockRepository _rutaRepository;
    private readonly IKontrolnaTockaMockRepository _kontrolnaTockaRepository;

    public RutaController(IRutaMockRepository rutaRepository, IKontrolnaTockaMockRepository kontrolnaTockaRepository)
    {
        _rutaRepository = rutaRepository;
        _kontrolnaTockaRepository = kontrolnaTockaRepository;
    }

    public IActionResult Index()
    {
        var kontrolneTocke = _kontrolnaTockaRepository.GetAll();

        var m = _rutaRepository.GetAll()
            .OrderBy(r => r.Naziv)
            .Select(r => new RutaIndexCardViewModel
            {
                IdRuta = r.IdRuta,
                Naziv = r.Naziv,
                PovezanaKontrolnaTocka = kontrolneTocke.FirstOrDefault(kt => kt.IdKontrolnaTocka == r.IdKontrolnaTocka)?.Naziv ?? "Nije prijavljena KT",
                Pocetak = r.Pocetak,
                Kraj = r.Kraj,
                Trajanje = FormatTrajanje(r.VrijemeHodaMin),
                DuljinaKm = r.DuljinaKm,
                VisinskaRazlikaM = r.VisinskaRazlikaM,
                TezinaTekst = DajNazivTezine(r.TezinaRute),
                TezinaCssClass = DajCssKlasuObziromNaTezinu(r.TezinaRute),
                OpisPreview = TrimOpis(r.Opis, 150)
            })
            .ToList();

        ViewData["Title"] = "Rute";
        return View(m);
    }

    public IActionResult Details(int id)
    {
        var ruta = _rutaRepository.GetById(id);
        
        if (ruta == null)
            return NotFound();

        // Pridruzimo ako nije
        ruta.KontrolnaTocka = _kontrolnaTockaRepository.GetById(ruta.IdKontrolnaTocka);

        ViewData["Title"] = ruta.Naziv;
        return View(ruta);
    }

    private static string FormatTrajanje(int minute)
    {
        if (minute < 60) return $"{minute} min";
        int h = minute / 60;
        int m = minute % 60;
        return m > 0 ? $"{h}h {m}min" : $"{h}h";
    }

    private static string DajNazivTezine(TezinaRute t)
    {
        return t switch
        {
            TezinaRute.Laka => "Laka",
            TezinaRute.Srednja => "Srednja",
            TezinaRute.Teska => "Teška",
            _ => "Nepoznato"
        };
    }

    private static string DajCssKlasuObziromNaTezinu(TezinaRute t)
    {
        return t switch
        {
            TezinaRute.Laka => "success",
            TezinaRute.Srednja => "warning",
            TezinaRute.Teska => "danger",
            _ => "subtle"
        };
    }

    private static string? TrimOpis(string? opis, int max)
    {
        if (string.IsNullOrWhiteSpace(opis)) return null;
        var o = opis.Trim();
        if (o.Length <= max) return o;
        return o[..max].TrimEnd() + "...";
    }
}