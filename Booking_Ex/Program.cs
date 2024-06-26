using ASP_.Net_Core_Class_Home_Work.Data;
using ASP_.Net_Core_Class_Home_Work.Data.DAL;
using ASP_.Net_Core_Class_Home_Work.Middleware;
using ASP_.Net_Core_Class_Home_Work.Models;
using ASP_.Net_Core_Class_Home_Work.Services.Email;
using ASP_.Net_Core_Class_Home_Work.Services.Hash;
using ASP_.Net_Core_Class_Home_Work.Services.Kdf;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using ASP_.Net_Core_Class_Home_Work.Services.Random;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("emailconfig.json", false);

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IHashService, ShaHashService>();

builder.Services.AddSingleton<IRandomService, RandomService>();

string connectionString = builder.Configuration.GetConnectionString("LocalMySQL");
MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
builder.Services.AddDbContext<DataContext>(option =>
    option.UseMySql(connectionString, ServerVersion.AutoDetect(mySqlConnection)),ServiceLifetime.Singleton);

builder.Services.AddSingleton<DataAccessor>();
builder.Services.AddSingleton<IKdfService, PBKDF1Service>();
builder.Services.AddSingleton<IEmailService, GmailServise>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(300);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors(builder => builder

    .AllowAnyMethod()

    .AllowAnyHeader()

    .SetIsOriginAllowed(origin => true)

    .AllowCredentials());
app.UseAuthorization();

// підключення
app.UseSession();

app.UseAuthSession();
app.UseAuthToken();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Content}/{action=Index}/{id?}");

app.Run();