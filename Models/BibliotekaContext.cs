using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class BibliotekaContext : DbContext 
    {
        public BibliotekaContext(DbContextOptions<BibliotekaContext> options) : base(options) { }

        public DbSet<Uzytkownik> Uzytkownicy { get; set; } 
        public DbSet<Ksiazka> Ksiazki { get; set; }
        public DbSet<Autor> Autorzy { get; set; }
        public DbSet<Wydawnictwo> Wydawnictwa { get; set; }
        public DbSet<KsiazkaAutor> KsiazkaAutorzy { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<KsiazkaAutor>()
                .HasKey(ka => new { ka.KsiazkaId, ka.AutorId });

            modelBuilder.Entity<Ksiazka>()
                .HasOne(k => k.Wydawnictwo)
                .WithMany(w => w.Ksiazki)
                .HasForeignKey(k => k.WydawnictwoId);

            modelBuilder.Entity<KsiazkaAutor>()
                .HasOne(ka => ka.Ksiazka)
                .WithMany(k => k.KsiazkaAutorzy)
                .HasForeignKey(ka => ka.KsiazkaId);

            modelBuilder.Entity<KsiazkaAutor>()
                .HasOne(ka => ka.Autor)
                .WithMany(a => a.KsiazkaAutorzy)
                .HasForeignKey(ka => ka.AutorId);
        }
    }
}