using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppAntiPlagiat.Controllers
{
    [Authorize(Roles = "enseignant")]
    public class EnseignantController : Controller
    {
        public IActionResult Profile()
        {
            ViewBag.Ltype = "enseignant";
            return View();
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
