using EVrtic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EVrtic.Data
{
    public class ApplicationDbContext : IdentityDbContext<Korisnik, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Users DbSet nije potreban — IdentityDbContext već registruje Users tabelu (AspNetUsers)
        public DbSet<Roditelj> Roditelji { get; set; }
        public DbSet<Odgajatelj> Odgajatelji { get; set; }
        public DbSet<Administrator> Administratori { get; set; }

        public DbSet<Dijete> Djeca { get; set; }
        public DbSet<Grupa> Grupe { get; set; }

        public DbSet<AlergijaDjeteta> AlergijeDjece { get; set; }
        public DbSet<BolestDjeteta> BolestiDjece { get; set; }

        public DbSet<DnevniIzvjestaj> DnevniIzvjestaji { get; set; }
        public DbSet<EvidencijaDolaskaOdlaska> EvidencijeDolaskaOdlaska { get; set; }
        public DbSet<DnevniQRCode> DnevniQRCodovi { get; set; }

        public DbSet<SazetakAktivnosti> SazeciAktivnosti { get; set; }
        public DbSet<Obavijest> Obavijesti { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Korisnik se mapira na AspNetUsers automatski — NE stavljaj ToTable("Korisnik")
            // Nasljeđene klase i dalje imaju svoje tabele (TPT strategija)
            modelBuilder.Entity<Roditelj>().ToTable("Roditelj");
            modelBuilder.Entity<Odgajatelj>().ToTable("Odgajatelj");
            modelBuilder.Entity<Administrator>().ToTable("Administrator");

            modelBuilder.Entity<Dijete>().ToTable("Dijete");
            modelBuilder.Entity<Grupa>().ToTable("Grupa");

            modelBuilder.Entity<AlergijaDjeteta>().ToTable("AlergijaDjeteta");
            modelBuilder.Entity<BolestDjeteta>().ToTable("BolestDjeteta");

            modelBuilder.Entity<DnevniIzvjestaj>().ToTable("DnevniIzvjestaj");
            modelBuilder.Entity<EvidencijaDolaskaOdlaska>().ToTable("EvidencijaDolaskaOdlaska");
            modelBuilder.Entity<DnevniQRCode>().ToTable("DnevniQRCode");

            modelBuilder.Entity<SazetakAktivnosti>().ToTable("SazetakAktivnosti");
            modelBuilder.Entity<Obavijest>().ToTable("Obavijest");

            // Email unique index — Identity već to osigurava, ali možemo zadržati eksplicitno
            modelBuilder.Entity<Korisnik>()
                .HasIndex(k => k.Email)
                .IsUnique();

            modelBuilder.Entity<Dijete>()
                .HasIndex(d => d.IdentifikacioniKod)
                .IsUnique();

            modelBuilder.Entity<Roditelj>()
                .HasMany(r => r.Djeca)
                .WithOne(d => d.Roditelj)
                .HasForeignKey(d => d.RoditeljId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Grupa>()
                .HasMany(g => g.Djeca)
                .WithOne(d => d.Grupa)
                .HasForeignKey(d => d.GrupaId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Odgajatelj>()
                .HasMany(o => o.Grupe)
                .WithOne(g => g.Odgajatelj)
                .HasForeignKey(g => g.OdgajateljId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Dijete>()
                .HasMany(d => d.DnevniIzvjestaji)
                .WithOne(i => i.Dijete)
                .HasForeignKey(i => i.DijeteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Dijete>()
                .HasMany(d => d.EvidencijeDolaskaOdlaska)
                .WithOne(e => e.Dijete)
                .HasForeignKey(e => e.DijeteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Dijete>()
                .HasMany(d => d.SazeciAktivnosti)
                .WithOne(s => s.Dijete)
                .HasForeignKey(s => s.DijeteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
