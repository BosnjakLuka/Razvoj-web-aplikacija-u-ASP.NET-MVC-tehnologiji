using planinarenje.Entiteti;

var builder = WebApplication.CreateBuilder(args);

// Lab 1: inicijalno punjenje objekata podacima iz dataset dokumenta.
var lab1Podaci = Lab1PodaciFactory.Kreiraj();
Console.WriteLine($"Lab1 podaci ucitani: Podrucja={lab1Podaci.Podrucja.Count}, Korisnici={lab1Podaci.Korisnici.Count}, Posjeti={lab1Podaci.Posjeti.Count}");

var podrucja = lab1Podaci.Podrucja;
var korisnici = lab1Podaci.Korisnici;
var knjizice = lab1Podaci.Knjizice;
var medalje = lab1Podaci.Medalje;
var kontrolneTocke = lab1Podaci.KontrolneTocke;
var rute = lab1Podaci.Rute;
var posjeti = lab1Podaci.Posjeti;
var fotografije = lab1Podaci.Fotografije;
var korisnikMedalje = lab1Podaci.KorisnikMedalje;

Console.WriteLine("\n===== LINQ DEMO - PLANINARSKA APLIKACIJA =====");

// 1) Dohvat svih korisnika
var upit1 = korisnici;
Console.WriteLine("\n1) Svi korisnici:");
foreach (var k in upit1)
{
    Console.WriteLine($"- {k.IdKorisnik}: {k.Ime} {k.Prezime}");
}

// 2) Svi posjeti jednog korisnika
var upit2 = posjeti.Where(p => p.IdKorisnik == 1);
Console.WriteLine("\n2) Posjeti korisnika 1:");
foreach (var p in upit2)
{
    Console.WriteLine($"- Posjet {p.IdPosjet}, KT={p.IdKontrolnaTocka}, Ruta={p.IdRuta}");
}

// 3) Kontrolne tocke iz podrucja
var upit3 = kontrolneTocke.Where(kt => kt.IdPodrucje == 5);
Console.WriteLine("\n3) Kontrolne tocke iz podrucja 5:");
foreach (var kt in upit3)
{
    Console.WriteLine($"- {kt.Naziv}");
}

// 4) Rute za kontrolnu tocku
var upit4 = rute.Where(r => r.IdKontrolnaTocka == 3);
Console.WriteLine("\n4) Rute za KT 3:");
foreach (var r in upit4)
{
    Console.WriteLine($"- {r.Naziv}");
}

// 5) Fotografije za posjet
var upit5 = fotografije.Where(f => f.IdPosjet == 1);
Console.WriteLine("\n5) Fotografije za posjet 1:");
foreach (var f in upit5)
{
    Console.WriteLine($"- {f.NazivDatoteke}");
}

// 6) Broj posjeta po korisniku
var upit6 = posjeti
    .GroupBy(p => p.IdKorisnik)
    .Select(g => new { IdKorisnik = g.Key, Broj = g.Count() });
Console.WriteLine("\n6) Broj posjeta po korisniku:");
foreach (var x in upit6)
{
    Console.WriteLine($"- Korisnik {x.IdKorisnik}: {x.Broj}");
}

// 7) Korisnici s medaljom
var upit7 = korisnici.Where(k => korisnikMedalje.Any(km => km.IdKorisnik == k.IdKorisnik));
Console.WriteLine("\n7) Korisnici s medaljom:");
foreach (var k in upit7)
{
    Console.WriteLine($"- {k.Ime} {k.Prezime}");
}

// 8) KT koje korisnik nije obisao
var upit8 = kontrolneTocke.Where(kt =>
    !posjeti.Any(p => p.IdKorisnik == 1 && p.IdKontrolnaTocka == kt.IdKontrolnaTocka));
Console.WriteLine("\n8) KT koje korisnik 1 nije obisao:");
foreach (var kt in upit8)
{
    Console.WriteLine($"- {kt.Naziv}");
}

// 9) Najteze rute
var upit9 = rute.OrderByDescending(r => r.TezinaRute);
Console.WriteLine("\n9) Rute poredane po tezini:");
foreach (var r in upit9)
{
    Console.WriteLine($"- {r.Naziv} ({r.TezinaRute})");
}

// 10) Podrucja koja je korisnik obisao (po minimalnom pragu KT)
var upit10 = podrucja.Where(p =>
    posjeti.Count(pos => pos.IdKorisnik == 1 &&
        kontrolneTocke.Any(kt =>
            kt.IdKontrolnaTocka == pos.IdKontrolnaTocka &&
            kt.IdPodrucje == p.IdPodrucje)) >= p.MinimalanBrojKTZaObilazak);
Console.WriteLine("\n10) Podrucja koja je korisnik 1 obisao prema pravilima:");
foreach (var p in upit10)
{
    Console.WriteLine($"- {p.Naziv}");
}

// 11) DODATNI: FirstOrDefault - prvi potvrdeni posjet korisnika
var upit11 = posjeti
    .Where(p => p.IdKorisnik == 1 && p.JeLiPotvrdenPosjet)
    .OrderBy(p => p.DatumVrijemePosjeta)
    .FirstOrDefault();
Console.WriteLine("\n11) Prvi potvrdeni posjet korisnika 1 (FirstOrDefault):");
if (upit11 is null)
{
    Console.WriteLine("- Nema potvrdenih posjeta.");
}
else
{
    Console.WriteLine($"- Posjet {upit11.IdPosjet} na datum {upit11.DatumVrijemePosjeta:yyyy-MM-dd HH:mm}");
}

// 12) DODATNI: Projekcija - detaljan prikaz posjeta
var upit12 = posjeti
    .Select(p => new
    {
        Korisnik = $"{korisnici.Single(k => k.IdKorisnik == p.IdKorisnik).Ime} {korisnici.Single(k => k.IdKorisnik == p.IdKorisnik).Prezime}",
        KontrolnaTocka = kontrolneTocke.Single(kt => kt.IdKontrolnaTocka == p.IdKontrolnaTocka).Naziv,
        Podrucje = podrucja.Single(pd => pd.IdPodrucje == kontrolneTocke.Single(kt => kt.IdKontrolnaTocka == p.IdKontrolnaTocka).IdPodrucje).Naziv,
        Ruta = rute.Single(r => r.IdRuta == p.IdRuta).Naziv,
        p.DozivljajPosjeta
    });
Console.WriteLine("\n12) Projekcija detalja posjeta:");
foreach (var redak in upit12)
{
    Console.WriteLine($"- {redak.Korisnik} | {redak.KontrolnaTocka} | {redak.Podrucje} | {redak.Ruta} | {redak.DozivljajPosjeta}");
}

var _ = knjizice.Count + medalje.Count;

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
