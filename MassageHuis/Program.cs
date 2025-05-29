using MassageHuis.Data;
using MassageHuis.Domains.Configuration;
using MassageHuis.Entities;
using MassageHuis.Models;
using MassageHuis.Repositories;
using MassageHuis.Repositories.Interfaces;
using MassageHuis.Services;
using MassageHuis.Services.Interfaces;
using MassageHuis.Util;
using MassageHuis.Util.Mail;
using MassageHuis.Util.Mail.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using NuGet.Configuration;
using System.Globalization;
using System.Net.Sockets;
using EmailSettings = MassageHuis.Util.Mail.EmailSettings;


var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDbContext<MassageHuisDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

builder.Services.AddRazorPages()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddSingleton<IEmailSend, EmailSend>();


builder.Services.AddTransient<IService<Masseur>, MasseurService>();
builder.Services.AddTransient<IService<Schema>, SchemaService>();
builder.Services.AddTransient<IService<RegulierTijdslot>, RegulierTijdslotService>();
builder.Services.AddTransient<IService<Reservatie>, ReservatieService>();
builder.Services.AddTransient<IService<UitzonderingTijdslot>, UitzonderingTijdslotService>();
builder.Services.AddTransient<IService<KostPrijs>, KostPrijsService>();
builder.Services.AddTransient<IService<TypeMassage>, TypeMassageService>();

builder.Services.AddTransient<IDAO<Masseur>, MasseurDAO>();
builder.Services.AddTransient<IDAO<Schema>, SchemaDAO>();
builder.Services.AddTransient<IDAO<RegulierTijdslot>, RegulierTijdslotDAO>();
builder.Services.AddTransient<IDAO<Reservatie>, ReservatieDAO>();
builder.Services.AddTransient<IDAO<UitzonderingTijdslot>, UitzonderingTijdslotDAO>();
builder.Services.AddTransient<IDAO<KostPrijs>, KostPrijsDAO>();
builder.Services.AddTransient<IDAO<TypeMassage>, TypeMassageDAO>();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "be.shop.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(10);
});


var app = builder.Build();

var supportedCultures = new[]
{
    new CultureInfo("nl-BE"),
    new CultureInfo("nl-NL"),
    new CultureInfo("en-US"),
};

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("nl-BE"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

app.UseRequestLocalization(localizationOptions);


if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapControllerRoute(
    name: "VerlofVerwijderen",
    pattern: "Uitbater/VerlofVerwijderen/{id}",
    defaults: new { controller = "Uitbater", action = "VerlofVerwijderen" });

app.MapRazorPages()
   .WithStaticAssets();

app.Run();