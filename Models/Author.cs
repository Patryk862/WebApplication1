using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Author
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }

    // Relacja N:M
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}