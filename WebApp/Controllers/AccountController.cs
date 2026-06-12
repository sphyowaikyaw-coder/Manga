using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Business_Model;
using Service.Service;
using WebApp.View_Model;

namespace WebApp.Controllers;

public class AccountController(UserService userService, MangaService mangaService) : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await userService.Login(model.Email, model.Password);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        await SignInAsync(user.Email, user.Role, user.Name);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return user.Role == "Admin"
            ? RedirectToAction("Index", "Admin")
            : RedirectToAction("Dashboard", "Account");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var saved = await userService.Register(new BM_UserItem
        {
            Name = model.UserName,
            Email = model.Email,
            Password = model.Password,
            Role = "User"
        });

        if (!saved)
        {
            ModelState.AddModelError(nameof(model.Email), "This email is already registered.");
            return View(model);
        }

        await SignInAsync(model.Email, "User", model.UserName);

        return RedirectToAction(nameof(Dashboard));
    }

    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Dashboard()
    {
        var manga = await mangaService.GetAllManga();

        return View(manga.Take(4).Select(MapToViewModel).ToList());
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Manga");
    }

    private async Task SignInAsync(string email, string role, string? name = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, name ?? email.Split('@')[0]),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

    private static MangaItem MapToViewModel(BM_MangaItem manga)
    {
        return new MangaItem
        {
            Id = manga.Id,
            Title = manga.Title,
            Author = manga.Author,
            Status = manga.Status,
            Description = manga.Description,
            CoverImageUrl = manga.CoverImage,
            ReleaseYear = manga.ReleaseYear,
            CreatedAt = manga.CreatedAt,
            Genres = manga.Genres,
            Chapters = manga.Chapters,
            Views = manga.Views,
            Rating = manga.Rating,
            Featured = manga.Featured
        };
    }
}
