using LicenseKeyServer.Data;
using LicenseKeyServer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔥 Lấy connection từ ENV hoặc appsettings
var rawConnection =
    Environment.GetEnvironmentVariable("DefaultConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found");

// 🔥 Convert MYSQL_URL -> dạng MySQL hiểu
string connectionString;

if (rawConnection.StartsWith("mysql://"))
{
    var uri = new Uri(rawConnection);
    var userInfo = uri.UserInfo.Split(':');

    var user = userInfo[0];
    var password = userInfo[1];
    var host = uri.Host;
    var port = uri.Port;
    var database = uri.AbsolutePath.Trim('/');

    connectionString =
        $"server={host};port={port};database={database};user={user};password={password}";
}
else
{
    // nếu chạy local thì dùng luôn
    connectionString = rawConnection;
}

// ✅ MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// MVC
builder.Services.AddControllersWithViews();

// Service
builder.Services.AddScoped<IKeyGenerator, KeyGenerator>();

// 🔐 Auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
    });

var app = builder.Build();

// 🔥 Tạo DB nếu chưa có
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Keys}/{action=Index}/{id?}");

app.Run();