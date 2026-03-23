using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models;

public class BibliotekaContext : DbContext
{
    public BibliotekaContext(DbContextOptions<BibliotekaContext> options) : base(options) { }

    public DbSet<Wydawnictwo> Wydawnictwa { get; set; }
    public DbSet<Ksiazka> Ksiazki { get; set; }
    public DbSet<Autor> Autorzy { get; set; }
    public DbSet<KsiazkaAutor> KsiazkaAutorzy { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KsiazkaAutor>()
            .HasKey(ka => new { ka.KsiazkaId, ka.AutorId });


        modelBuilder.Entity<Ksiazka>()
            .HasOne(k => k.Wydawnictwo)           // Książka ma jedno wydawnictwo
            .WithMany(w => w.Ksiazki)             // Wydawnictwo ma wiele książek
            .HasForeignKey(k => k.WydawnictwoId); // Klucz obcy

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