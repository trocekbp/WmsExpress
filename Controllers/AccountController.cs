using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Diagnostics;
using WmsCore.Models;
using WmsCore.ViewModels;

namespace WmsCore.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Login() { 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model) {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Nieudana próba logowania");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register() {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model) {
            if (!ModelState.IsValid) { 
                return View(model);
            }

            var roleExist = await roleManager.RoleExistsAsync(model.UserRole);
            if (!roleExist) {
                ModelState.AddModelError(string.Empty, $"Podana rola \"{model.UserRole}\" nie istnieje.");
                return View(model);
            }

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded) {
                await userManager.AddToRoleAsync(user, model.UserRole);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors) { 
                ModelState.AddModelError (string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username)){
                // Tworzymy model błędu, żeby widok nie dostał NULL-a
                var errorModel = new ErrorViewModel
                {  
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                };
                return View("Error", errorModel);
            }

            return View(new ChangePasswordViewModel { UserName = username });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model) {
            if (!ModelState.IsValid) {
                ModelState.AddModelError(string.Empty, "Coś poszło nie tak");
                return View(model);
            }

            var user = await userManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Nie znaleziono operatora");
                return View(model);
            }

            var result = await userManager.RemovePasswordAsync(user);

            if (result.Succeeded)
            {
                result = await userManager.AddPasswordAsync(user, model.NewPassword);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
