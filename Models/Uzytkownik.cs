using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Uzytkownik
    {
        [Key]
        public int Id { get; set; }
        public string Login { get; set; }
        public string HasloHash { get; set; } 
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Email { get; set; }
    }
}