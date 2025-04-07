using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MassageHuis.Data;
using MassageHuis.Domains.Configuration;
using Microsoft.Extensions.Options;
using MassageHuis.Util;
using MassageHuis.Util.Mail.Interfaces;
using MassageHuis.Util.Mail;
using NuGet.Configuration;
using EmailSettings = MassageHuis.Util.Mail.EmailSettings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

//toevoegen van de automapper
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
/*Configuration.GetSection("EmailSettings")) zal de instellingen opvragen uit de
AppSettings.json file en vervolgens wordt er een emailsettings - object
aangemaakt en de waarden worden ge�njecteerd in het object*/
builder.Services.AddSingleton<IEmailSend, EmailSend>();
/*Als in een Constructor een IEmailSender-parameter wordt gevonden, zal een
emailSender - object worden aangemaakt.*/


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();



app.Run();
