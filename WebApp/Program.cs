using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Dependency;

var builder = WebApplication.CreateBuilder(args);
var dataProtectionDirectory = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "Keys"));
dataProtectionDirectory.Create();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MangaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MangaDatabase")
        ?? "Server=DESKTOP-GB6LURR;Database=Manga;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;"));
builder.Services.AddScoped<DAO.DAO.MangaDAO, DAO.DAO.DAOImpl.MangaDAOImpl>();
builder.Services.AddScoped<Service.Service.MangaService, Service.Service.ServiceImpl.MangaServiceImpl>();
builder.Services.AddScoped<Service.Service.ChapterUrlService, Service.Service.ServiceImpl.ChapterUrlServiceImpl>();
builder.Services.AddScoped<Service.Service.CoverUrlService, Service.Service.ServiceImpl.CoverUrlServiceImpl>();
builder.Services.AddScoped<DAO.DAO.UserDAO, DAO.DAO.DAOImpl.UserDAOImpl>();
builder.Services.AddScoped<Service.Service.UserService, Service.Service.ServiceImpl.UserServiceImpl>();

builder.Services.AddScoped<DAO.DAO.ChapterDAO, DAO.DAO.DAOImpl.ChapterDAOImpl>();
builder.Services.AddScoped<Service.Service.ChapterService, Service.Service.ServiceImpl.ChapterServiceImpl>();



builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(dataProtectionDirectory)
    .SetApplicationName("MangaVerse");
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "MangaVerse.Auth";
    });
builder.Services.AddAuthorization();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Manga}/{action=Index}/{id?}");

app.Run();
