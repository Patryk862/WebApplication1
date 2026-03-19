using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Book
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; }

    // Relacja 1:N
    public int PublisherId { get; set; }
    public Publisher Publisher { get; set; }

    // Relacja N:M
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}