using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Areas.Admin.Models;
using WebApplication1.Utilities.Enums;
using WebApplication1.Utilities.Extensions;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class RoleChangerController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleChangerController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangeRole(RoleGetVM role)
        {
            if (role.KeyFile != null && role.KeyFile.Length > 0)
            {

                string fileContent;
                using (var reader = new StreamReader(role.KeyFile.OpenReadStream()))
                {
                    fileContent = await reader.ReadToEndAsync();
                }

                string hashedText = fileContent.HashText();
                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user != null)
                    {
                        var currentRoles = await _userManager.GetRolesAsync(user);

                        // To delete roles
                        foreach (var roleName in currentRoles)
                        {
                            await _userManager.RemoveFromRoleAsync(user, roleName);
                        }


                        if (hashedText == RolesHash.hashRolesDictionary["Admin"])
                        {
                            await _userManager.AddToRoleAsync(user, UserRole.Admin.ToString());
                        }
                        else if (hashedText == RolesHash.hashRolesDictionary["Moderator"])
                        {
                            await _userManager.AddToRoleAsync(user, UserRole.Moderator.ToString());
                        }
                        else if (hashedText == RolesHash.hashRolesDictionary["Member"])
                        {
                            await _userManager.AddToRoleAsync(user, UserRole.Member.ToString());
                        }
                        else if (hashedText == RolesHash.hashRolesDictionary["SuperAdmin"])
                        {
                            await _userManager.AddToRoleAsync(user, UserRole.SuperAdmin.ToString());
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, UserRole.Member.ToString());
                        }
                        for (int i = 0; i < 100; i++)
                        {
                            Console.WriteLine(hashedText);
                        }
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }

}

