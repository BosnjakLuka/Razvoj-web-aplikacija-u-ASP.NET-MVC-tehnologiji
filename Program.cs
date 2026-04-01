using planinarenje.Entiteti;

var builder = WebApplication.CreateBuilder(args);

// Lab 1: inicijalno punjenje objekata podacima iz dataset dokumenta.
var lab1Podaci = Lab1PodaciFactory.Kreiraj();
Console.WriteLine($"Lab1 podaci ucitani: Podrucja={lab1Podaci.Podrucja.Count}, Korisnici={lab1Podaci.Korisnici.Count}, Posjeti={lab1Podaci.Posjeti.Count}");

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
