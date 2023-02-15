using AppAntiPlagiat.Models;
using AppAntiPlagiat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppAntiPlagiat.Controllers
{
    [Authorize(Roles = "enseignant")]
    public class EnseignantController : Controller
    {
        private readonly UserManager<Utilisateur> userManager;

        public EnseignantController(UserManager<Utilisateur> userManager)
        {
            this.userManager = userManager;
        }
        [HttpPut]
        public async Task<IActionResult> Profile(string imgUrl)
        {
            ViewBag.Ltype = "enseignant";

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            ViewBag.Ltype = "enseignant";
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                UserViewModel model = new UserViewModel()
                {
                    Departement = user.Departement,
                    Email = user.Email,
                    IMGurl = user.IMGurl,
                    Nom = user.Nom,
                    Prenom= user.Prenom
                };
                return View(model);
            }

            return RedirectToAction("LogOut","Home");
        }
        public IActionResult ListeEtudiants()
        {
            ViewBag.Ltype = "enseignant";
            return View();
        }
        public IActionResult ListeRapports()
        {
            ViewBag.Ltype = "enseignant";
            return View();
        }
        public IActionResult Scann()
        {
            ViewBag.Ltype = "enseignant";
            return View();
        }


    }
}
