using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Publisher
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    // Relacja 1:N
    public ICollection<Book> Books { get; set; } = new List<Book>();
}