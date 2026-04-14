using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. POŁĄCZENIE Z BAZĄ DANYCH (Docker / SQL Server)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Nie znaleziono ciągu połączenia 'DefaultConnection'.");

builder.Services.AddDbContext<BibliotekaContext>(options =>
    options.UseSqlServer(connectionString));

// 2. KONFIGURACJA SYSTEMU LOGOWANIA (Identity)
// Ustawienia haseł zgodne z Twoją instrukcją PDF
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false; // Brak wymogu potwierdzenia e-mail
    options.Password.RequireDigit = false;          // Brak wymogu cyfr
    options.Password.RequiredLength = 6;           // Min. 6 znaków
    options.Password.RequireNonAlphanumeric = false; // Brak wymogu znaków specjalnych
    options.Password.RequireUppercase = false;       // Brak wymogu wielkich liter
    options.Password.RequireLowercase = false;       // Brak wymogu małych liter
})
.AddEntityFrameworkStores<BibliotekaContext>();

// 3. DODANIE SERWISÓW DLA MVC I RAZOR PAGES (Dla logowania)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 4. KONFIGURACJA SESJI (Zgodnie z Twoim PDF)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// 5. KONFIGURACJA POTOKU PRZETWARZANIA (Middleware)
// KLUCZOWA KOLEJNOŚĆ!
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

// KOLEJNOŚĆ TUTAJ JEST KRYTYCZNA:
app.UseAuthentication(); // 1. Kto to jest?
app.UseAuthorization();  // 2. Co może robić?
app.UseSession();        // 3. Obsługa sesji

// 6. MAPOWANIE ŚCIEŻEK
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Niezbędne dla działania stron logowania Identity

app.Run();