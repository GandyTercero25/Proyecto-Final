using ECommerceArtesanos.Data;
using ECommerceArtesanos.Models;
using ECommerceArtesanos.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var host = builder.Configuration["Email:Host"];
var port = builder.Configuration.GetValue<int>("Email:Port");

// Verifica que la configuración se cargue correctamente
try
{
    Console.WriteLine("Configuración cargada:");
    Console.WriteLine($"ConnectionString: {builder.Configuration.GetConnectionString("DefaultConnection")}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error cargando configuración: {ex.Message}");
    throw;
}
// 1. Configuración de servicios esenciales
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages();

// 2. Configuración de la base de datos
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Configuración de Identity con opciones personalizadas
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Configuración de usuario
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = true;

    // Configuración de contraseña
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders(); // Para generación de tokens de confirmación

builder.Services.AddTransient<IEmailService, EmailService>();

// 4. Configuración de autenticación (Cookies + JWT)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// 5. Configuración de políticas de autorización
builder.Services.AddAuthorization(options =>
{
    // Política para email verificado
    options.AddPolicy("VerifiedEmailOnly", policy =>
        policy.RequireClaim(CustomClaimTypes.EmailVerified, "true"));

    // Política para artesanos (dominio @art.ec)
    options.AddPolicy("ArtesanoOnly", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                c.Type == ClaimTypes.Email &&
                c.Value.EndsWith("@art.ec"))));

    // Política para administradores
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

// 6. Configuración de servicios personalizados
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddHttpContextAccessor();

// 7. Configuración de Swagger (solo para desarrollo)
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "ECommerceArtesanos API",
            Version = "v1",
            Description = "API para el sistema ECommerce de Artesanos"
        });

        // Configuración de seguridad JWT en Swagger
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });
}

// Construir la aplicación
var app = builder.Build();

// 8. Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// 9. Configuración de endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

// 10. Inicialización de la base de datos (roles iniciales)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        // Crear roles iniciales
        var roles = new[] { "Admin", "Artesano", "Cliente" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Crear usuario admin inicial (solo en desarrollo)
        if (app.Environment.IsDevelopment())
        {
            const string adminEmail = "admin@art.ec";
            const string adminPassword = "Admin123!";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    await userManager.AddClaimAsync(adminUser,
                        new Claim(CustomClaimTypes.EmailVerified, "true"));
                }
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al inicializar la base de datos");
    }


}

app.Run();