using Microsoft.AspNetCore.Mvc;
using planinarenje.Entiteti;
using planinarenje.Models;

namespace planinarenje.Controllers;

public class RutaController : Controller
{
    public IActionResult Index()
    {
        var podaci = Lab1PodaciFactory.Kreiraj();

        var m = podaci.Rute
            .OrderBy(r => r.Naziv)
            .Select(r => new RutaIndexCardViewModel
            {
                IdRuta = r.IdRuta,
                Naziv = r.Naziv,
                PovezanaKontrolnaTocka = podaci.KontrolneTocke.FirstOrDefault(kt => kt.IdKontrolnaTocka == r.IdKontrolnaTocka)?.Naziv ?? "Nije prijavljena KT",
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
        var podaci = Lab1PodaciFactory.Kreiraj();
        var ruta = podaci.Rute.SingleOrDefault(r => r.IdRuta == id);
        
        if (ruta == null)
            return NotFound();

        // Pridruzimo ako nije
        ruta.KontrolnaTocka = podaci.KontrolneTocke.SingleOrDefault(kt => kt.IdKontrolnaTocka == ruta.IdKontrolnaTocka);

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