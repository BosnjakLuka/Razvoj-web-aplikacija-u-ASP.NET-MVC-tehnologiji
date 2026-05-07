using Microsoft.EntityFrameworkCore;
using planinarenje.Entiteti;

namespace planinarenje.Data;

public class PlaninarstvoDbContext : DbContext
{
    public PlaninarstvoDbContext(DbContextOptions<PlaninarstvoDbContext> options) : base(options)
    {
    }

    public DbSet<Korisnik> Korisnici { get; set; }
    public DbSet<Knjizica> Knjizice { get; set; }
    public DbSet<Posjet> Posjeti { get; set; }
    public DbSet<Fotografija> Fotografije { get; set; }
    public DbSet<KontrolnaTocka> KontrolneTocke { get; set; }
    public DbSet<Ruta> Rute { get; set; }
    public DbSet<Podrucje> Podrucja { get; set; }
    public DbSet<PlaninarskiObjekt> PlaninarskiObjekti { get; set; }
    public DbSet<PlaninarskaUdruga> PlaninarskeUdruge { get; set; }
    public DbSet<Medalja> Medalje { get; set; }
    public DbSet<KorisnikMedalja> KorisnikMedalje { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Knjizica>()
            .HasIndex(k => k.IdKorisnik)
            .IsUnique();

        modelBuilder.Entity<Korisnik>()
            .HasIndex(k => k.Email)
            .IsUnique();

        modelBuilder.Entity<Korisnik>()
            .HasIndex(k => k.KorisnickoIme)
            .IsUnique();

        modelBuilder.Entity<KontrolnaTocka>()
            .HasIndex(kt => kt.GUIDOznaka)
            .IsUnique();

        modelBuilder.Entity<PlaninarskaUdruga>()
            .HasIndex(pu => pu.OIB)
            .IsUnique();
    }
}
