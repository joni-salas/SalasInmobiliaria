using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. 
builder.Services.AddControllersWithViews();

    // asi se solicitan los servicios de autenticacion y autorizacion en .NET 6

    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>//el sitio web valida con cookie
        {
            options.LoginPath = "/Usuarios/Login";
            options.LogoutPath = "/Usuarios/Logout";
            options.AccessDeniedPath = "/Home/Restringido";
        });

    builder.Services.AddAuthorization(options =>
    {
        //options.AddPolicy("Empleado", policy => policy.RequireClaim(ClaimTypes.Role, "Administrador", "Empleado"));
        options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador", "SuperAdministrador"));
    });




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
