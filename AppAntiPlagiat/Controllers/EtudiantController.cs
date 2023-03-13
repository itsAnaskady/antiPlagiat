using AppAntiPlagiat.Models;
using AppAntiPlagiat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppAntiPlagiat.Controllers
{
    //[Authorize(Roles = "etudiant")]
    public class EtudiantController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<Utilisateur> userManager;
        private readonly ApplicationDbContext applicationDbContext;
        public EtudiantController(UserManager<Utilisateur> userManager, ApplicationDbContext applicationDbContext, IWebHostEnvironment env)
        {
            this.userManager = userManager;
            this.applicationDbContext = applicationDbContext;
            _env = env;
        }
        public async Task<IActionResult> Profile(string imgUrl)
        {
            ViewBag.Ltype = "etudiant";

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            ViewBag.Ltype = "etudiant";
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                UserViewModel model = new UserViewModel()
                {
                    Filiere = user.Filiere,
                    Niveau = user.Niveau,
                    Email = user.Email,
                    imgData = user.imgData,
                    imgType = user.imgType,
                    Nom = user.Nom,
                    Prenom = user.Prenom
                };
                return View(model);
            }
            return View();
            // return RedirectToAction("LogOut", "Home");
        }



        [HttpPost]
        public async Task<IActionResult> DeposerRapp(Rapport model, IFormFile pdfFile)
        {
            ViewBag.Ltype = "etudiant";

            if (pdfFile != null && pdfFile.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await pdfFile.CopyToAsync(stream);
                    var rapport = new Rapport
                    {
                        data = stream.ToArray(),
                        EtudiantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                        Intitulé = pdfFile.FileName,
                        DateDepot = DateTime.Now,
                        Type = model.Type,
                        Validé = false,
                    };
                    applicationDbContext.Rapports.Add(rapport);
                    applicationDbContext.SaveChanges();
                }
                return RedirectToAction("VosRapports");

            }
            return RedirectToAction("ImporterRapport");

        }
        public ActionResult DownloadPdf(int id)
        {
            var pdf = applicationDbContext.Rapports.Find(id);
            if (pdf == null)
            {
                return RedirectToAction("VosRapports");
            }
            return File(pdf.data, "application/pdf", pdf.Intitulé+".pdf");
        }

        public async Task<IActionResult> ImporterRapport()
        {
            ViewBag.Ltype = "etudiant";
            return View();
        }

        public IActionResult VosRapports()
        {
            ViewBag.Ltype = "etudiant";
            if (User.Identity.IsAuthenticated)
                {
                    string etudiantId = userManager.GetUserId(User);
                var mesRapports = applicationDbContext.Rapports.Where(r => r.EtudiantId == etudiantId).ToList();
                
                return View(mesRapports);
            }
            return View(); 
            
            // Récupérer les rapports de cet étudiant à partir de la base de données
            

            // Passer les rapports récupérés à la vue
            
          
            
        }
        public async Task<IActionResult> ListeEnseignants()
        {
            ViewBag.Ltype = "etudiant";
            if (User.Identity.IsAuthenticated)
            {
                string etudiantId = userManager.GetUserId(User);

                // Récupérer les enseignants affectés à l'étudiant connecté
                var enseignantsAffectes = applicationDbContext.Encadre
                    .Where(e => e.EtudiantId == etudiantId)
                    .Select(e => e.Enseignant)
                    .ToList();

                // Récupérer les types de stage affectés à l'étudiant connecté
                var typesStageAffectes = applicationDbContext.Encadre
                    .Where(e => e.EtudiantId == etudiantId)
                    .Select(e => e.TypeStage)
                    .ToList();
                var model = new Tuple<List<Utilisateur>, List<string>>(enseignantsAffectes, typesStageAffectes);
                return View(model);
            }
            return View();
        }


    }
}
