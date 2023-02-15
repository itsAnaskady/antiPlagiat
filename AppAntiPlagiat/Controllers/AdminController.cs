using AppAntiPlagiat.Models;
using AppAntiPlagiat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppAntiPlagiat.Controllers
{

    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<Utilisateur> userManager;

        public AdminController(UserManager<Utilisateur> userManager)
        {
            this.userManager = userManager;
        }
        public IActionResult Dashboard()
        {
            ViewBag.Ltype = "admin";
            return View();
        }
        public IActionResult AjouterEnseignant()
        {
            ViewBag.Ltype = "admin";
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AjouterEnseignant(AjouterEnseignantViewModel model)
        {
            ViewBag.Ltype = "admin";
            if (ModelState.IsValid)
            {
                Utilisateur nvEnseignant = new Utilisateur
                {
                    Email = model.Email,
                    Nom = model.Nom,
                    Prenom = model.Prenom,
                    UserName = model.Email,
                    NormalizedUserName = model.Email.ToUpper(),
                    NormalizedEmail = model.Email.ToUpper(),
                    Departement = model.Departement,
                    IMGurl = "~/images/Users/NewUser.png"
                };
                //add user
                var result = await userManager.CreateAsync(nvEnseignant, model.Password);

                //add user to role
                var result1 = await userManager.AddToRoleAsync(await userManager.FindByEmailAsync(model.Email), "admin");

                if (result.Succeeded && result1.Succeeded)
                {
                    ViewBag.ajoutNV = "ajouté";
                    return View(model);
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }
        public IActionResult RechSuppModEnseignant()
        {
            ViewBag.Ltype = "admin";
            return View();
        }
        public IActionResult AjouterEtudiant()
        {
            ViewBag.Ltype = "admin";
            return View();
        }
        public IActionResult RechSuppModEtudiant()
        {
            ViewBag.Ltype = "admin";
            return View();
        }
        public IActionResult Affectation()
        {
            ViewBag.Ltype = "admin";
            return View();
        }
        public IActionResult Messages()
        {
            ViewBag.Ltype = "admin";
            return View();
        }
    }
}
