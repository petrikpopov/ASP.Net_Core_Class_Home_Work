using ASP_.Net_Core_Class_Home_Work.Data;
using ASP_.Net_Core_Class_Home_Work.Models;
using ASP_.Net_Core_Class_Home_Work.Services.Hash;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Додаємо власні сервіси до контейнера builder.Services
//це можна робити у довільному порядку , але жо команди
// var app = builder.Services();
// Сервіси , створені з дотриманням DIP реєструються як зьязка (binding) між інтерфейсом та классом , що його реалізує
// Інжекціі IHashService контейнер має повернути обьект класу Md5HashService

//builder.Services.AddSingleton<IHashService, Md5HashService>();
// перехід між різними реалізаціями одного сервісу - одін рядок змін
builder.Services.AddSingleton<IHashService, ShaHashService>();
//
builder.Services.AddSingleton<IRandomService, RandomService>();

string connectionString = builder.Configuration.GetConnectionString("LocalMySQL");
MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
builder.Services.AddDbContext<DataContext>(option =>
    option.UseMySql(connectionString, ServerVersion.AutoDetect(mySqlConnection)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();