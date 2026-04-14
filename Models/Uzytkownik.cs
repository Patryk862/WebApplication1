using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
    public class Uzytkownik : IdentityUser // <-- TO MUSI BYĆ IdentityUser
    {
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
    }
}