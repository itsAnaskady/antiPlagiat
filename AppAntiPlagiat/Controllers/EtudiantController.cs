using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppAntiPlagiat.Controllers
{
    //[Authorize(Roles = "etudiant")]
    public class EtudiantController : Controller
    {
        public IActionResult Profile()
        {
            ViewBag.Ltype = "etudiant";
            return View();
        }
        public IActionResult ImporterRapport()
        {
            ViewBag.Ltype = "etudiant";
            return View();
        }
        public IActionResult VosRapports()
        {
            ViewBag.Ltype = "etudiant";
            return View();
        }
        public IActionResult ListeEnseignants()
        {
            ViewBag.Ltype = "etudiant";
            return View();
        }


    }
}
