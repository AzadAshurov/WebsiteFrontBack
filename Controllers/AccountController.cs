using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.Admin.Models;
using WebApplication1.Utilities.Enums;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM userVM)
        {
            userVM.Name = char.ToUpper(userVM.Name[0]) + userVM.Name.Substring(1).ToLower().Trim();
            userVM.Surname = char.ToUpper(userVM.Surname[0]) + userVM.Surname.Substring(1).ToLower().Trim();
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser user = new AppUser
            {
                Name = userVM.Name,
                Surname = userVM.Surname,
                Email = userVM.Email,
                UserName = userVM.Name
            };
            IdentityResult result = await _userManager.CreateAsync(user, userVM.Password);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    return View();
                }
            }
            //all new users are admin, this is only for test
            await _userManager.AddToRoleAsync(user, UserRole.Admin.ToString());
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        public IActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM, string? returnURL)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            //let me cook
            AppUser user = loginVM.UsernameOrEmail.Contains("@") ? await _userManager.Users.FirstOrDefaultAsync(x => x.Email == loginVM.UsernameOrEmail) : await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginVM.UsernameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Username, email or password is incorrect");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.IsPersistent, true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your account is locked, please try later");
                return View();
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Username, Email or Password is incorrect");
                return View();
            }
            if (returnURL is null)
            {
                RedirectToAction("Index", "Home");
            }

            Redirect(returnURL);
            //not all path have return(idk why redirect doesnt count)
            return View();
        }
        public async Task<IActionResult> CreateRoles()
        {
            foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                if (!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = role.ToString() });
                }
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
