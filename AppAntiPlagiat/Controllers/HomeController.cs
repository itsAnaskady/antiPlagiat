using AppAntiPlagiat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using AppAntiPlagiat.ViewModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace AppAntiPlagiat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly SignInManager<Utilisateur> signInManager;
        private readonly UserManager<Utilisateur> userManager;

        public HomeController(ApplicationDbContext dbContext,SignInManager<Utilisateur> signInManager,UserManager<Utilisateur> userManager)
        {
            this.dbContext = dbContext;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        public IActionResult Accueil()
        {
            
            return View();
        }
        public IActionResult LoginEnseignant()
        {
            if(signInManager.IsSignedIn(User) && User.IsInRole("admin"))
            {
                return RedirectToAction("Dashboard", "admin");
            }
            else if(signInManager.IsSignedIn(User) && User.IsInRole("enseignant"))
			{
				return RedirectToAction("Profile", "enseignant");
			}
            else if(signInManager.IsSignedIn(User) && User.IsInRole("etudiant"))
            {
				return RedirectToAction("Profile", "etudiant");
            }
            else
            {
                return View();
            }
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
			if (signInManager.IsSignedIn(User) && User.IsInRole("admin"))
			{
				return RedirectToAction("Dashboard", "admin");
			}
			else if (signInManager.IsSignedIn(User) && User.IsInRole("enseignant"))
			{
				return RedirectToAction("Profile", "enseignant");
			}
			else if (signInManager.IsSignedIn(User) && User.IsInRole("etudiant"))
			{
				return RedirectToAction("Profile", "etudiant");
			}
			else
			{
				return View();
			}
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
						return RedirectToAction("Profile", "Etudiant");
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
        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                Message m = new Message()
                {
                    Email= model.Email,
                    Subject = model.Subject,
                    Emetteur = model.Nom + " " + model.Prenom,
                    Msg = model.Message,
                    dateEnvoie = DateTime.Now
                };
                dbContext.Messages.Add(m);
                dbContext.SaveChanges();
                ViewBag.ajoutNV = "ajouté";
                return View();
            }
            return View(model);
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