using AppAntiPlagiat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using AppAntiPlagiat.ViewModels;
using Microsoft.IdentityModel.Tokens;

namespace AppAntiPlagiat.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<Utilisateur> signInManager;
        private readonly UserManager<Utilisateur> userManager;

        public HomeController(SignInManager<Utilisateur> signInManager,UserManager<Utilisateur> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        public IActionResult Accueil()
        {
            return View();
        }
        public IActionResult LoginEnseignant()
        {
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginEnseignant(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email,model.Password,model.RememberMe,false);
                if (result.Succeeded)
                {
                    var u = await userManager.FindByEmailAsync(model.Email);
                    string? role = (await userManager.GetRolesAsync(u))[0];

                    if(role== "admin")
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else if(role == "enseignant")
                    {
                        return RedirectToAction("Profile", "Enseignant");
                    }
                    else
                    {
                        await signInManager.SignOutAsync();
                    }
                }
                ModelState.AddModelError(String.Empty, "Email ou mot de passe incorrecte.");
            }
            return View(model);
        }

        public IActionResult LoginEtudiant()
        {
			return View();
        }
		[HttpPost]
		public async Task<IActionResult> LoginEtudiant(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
				if (result.Succeeded)
				{
					var u = await userManager.FindByEmailAsync(model.Email);
					string? role = (await userManager.GetRolesAsync(u))[0];

					if (role == "etudiant")
					{
						return RedirectToAction("Dashboard", "Admin");
					}
					else
					{
						await signInManager.SignOutAsync();
					}
				}
				ModelState.AddModelError(String.Empty, "Email ou mot de passe incorrecte.");
			}
			return View(model);
		}
		public IActionResult Détails()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Accueil");
        }


    }
}