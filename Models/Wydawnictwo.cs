using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Wydawnictwo
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nazwa { get; set; }
    public string? UserId { get; set; }

    public ICollection<Ksiazka> Ksiazki { get; set; } = new List<Ksiazka>();
}