using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Ksiazka
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Tytul { get; set; }

    public int WydawnictwoId { get; set; }
    public Wydawnictwo Wydawnictwo { get; set; }
    public int? UserId { get; set; }

    public ICollection<KsiazkaAutor> KsiazkaAutorzy { get; set; } = new List<KsiazkaAutor>();
}