using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Globalization;
using WmsCore.Data;
using WmsCore.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<WmsCoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WmsCoreConnection") ?? throw new InvalidOperationException("Connection string 'WmsCoreConnection' not found.")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;         // Nie wymagaj cyfry
    options.Password.RequireLowercase = false;     // Nie wymagaj ma³ej litery
    options.Password.RequireUppercase = false;     // Nie wymagaj du¿ej litery
    options.Password.RequireNonAlphanumeric = false; // Nie wymagaj znaku specjalnego 
    options.Password.RequiredLength = 3;           // Minimalna d³ugoœæ (domyœlnie jest 6)
})
    .AddEntityFrameworkStores<WmsCoreContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

 await SeedService.SeedDatabase(app.Services);


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapRazorPages();


app.Run();
