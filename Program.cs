using ECommerceArtesanos.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// 🔧 1. Configurar cadena de conexión y DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔐 2. Configurar Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Opcional para evitar email
})
.AddEntityFrameworkStores<AppDbContext>();

// 🧱 3. Habilitar MVC con vistas + Razor Pages + recarga en caliente
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages();

var app = builder.Build();

// 🌐 4. Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // ❗ Estaba faltando esto

app.UseRouting();

app.UseAuthentication(); // Siempre antes de Authorization
app.UseAuthorization();

// 🚀 5. Rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Necesario para /Identity/Account/Login, etc.

app.Run();
