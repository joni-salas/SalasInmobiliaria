using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SalasInmobiliaria.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ListenAnyIP(5000); // to listen for incoming http connection on port 5001
//    
//});


// Add services to the container. 
builder.Services.AddControllersWithViews();
             // asi se solicitan los servicios de autenticacion y autorizacion en .NET 6

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>//el sitio web valida con cookie
    {
        options.LoginPath = "/Usuarios/Login";
        options.LogoutPath = "/Usuarios/Logout";
        options.AccessDeniedPath = "/Home/Restringido";

    }).AddJwtBearer(options =>//la api web valida con token
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["TokenAuthentication:Issuer"],
            ValidAudience = builder.Configuration["TokenAuthentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(
                builder.Configuration["TokenAuthentication:SecretKey"])),
        };
    });

    //Autorizaciones
builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("Empleado", policy => policy.RequireClaim(ClaimTypes.Role, "Administrador", "Empleado"));
        options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador", "SuperAdministrador"));
    });


builder.Services.AddDbContext<DataContext>(
    options => options.UseSqlServer(
        builder.Configuration["ConnectionStrings:DefaultConnection"]
    )
);


//Inyeccion de dependencia
builder.Services.AddTransient<IRepositorioPropietario, RepositorioPropietario>();
builder.Services.AddTransient<IRepositorioInquilino, RepositorioInquilino>();
builder.Services.AddTransient<IRepositorioUsuario, RepositorioUsuario>();
builder.Services.AddTransient<IRepositorioContrato, RepositorioContrato>();
builder.Services.AddTransient<IRepositorioPago, RepositorioPago>();
builder.Services.AddTransient<IRepositorioInmueble, RepositorioInmueble>();

builder.Services.AddMvc();
builder.Services.AddSignalR();//añade signalR
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//importa el orden de los middelware
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//mapControllerRoute es para generar una nueva ruta
app.MapControllerRoute(
    name:"login", 
    pattern:"entrar/{**accion}", new { controller = "Usuarios", action = "Login" });

app.MapControllerRoute(
    name: "fecha",
    pattern: "{controller=Contrato}/{action=CalcularDesdeHasta}/{desde}/{hasta}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
