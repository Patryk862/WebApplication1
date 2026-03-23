using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Autor
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Imie { get; set; }

    [Required]
    [MaxLength(50)]
    public string Nazwisko { get; set; }

    public ICollection<KsiazkaAutor> KsiazkaAutorzy { get; set; } = new List<KsiazkaAutor>();
}