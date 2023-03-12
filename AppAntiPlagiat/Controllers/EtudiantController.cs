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
                    IMGurl = user.IMGurl,
                    Nom = user.Nom,
                    Prenom = user.Prenom
                };
                return View(model);
            }
            return View();
            // return RedirectToAction("LogOut", "Home");
        }



        [HttpPost]
        public async Task<IActionResult> DeposerRapp(Rapport model, String TypeS)
        {
            ViewBag.Ltype = "etudiant";
            /*if (ModelState.IsValid)
            {*/
           
              
            

            if (model.File != null && model.File.Length > 0)
                {
                    var nomFichier = Path.GetFileName(model.File.FileName.Trim('"'));
                    var cheminFichier = Path.Combine(_env.WebRootPath, "rapports", nomFichier);

                    // Vérifier si le dossier de destination existe et le créer si nécessaire
                    var dossierDestination = Path.GetDirectoryName(cheminFichier);
                    if (!Directory.Exists(dossierDestination))
                    {
                        Directory.CreateDirectory(dossierDestination);
                    }

                    using (var stream = new FileStream(cheminFichier, FileMode.Create))
                    {
                        await model.File.CopyToAsync(stream);
                    }

              

                // Enregistrer le rapport dans la base de données
                var rapport = new Rapport
                    {
                        EtudiantId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                        Intitulé = nomFichier,
                        Chemin = cheminFichier,
                        DateDepot = DateTime.Now,
                        Type = TypeS,
                        Validé = false,
                      //  EnseignantID=IDEnseig
                        

                    };

                    applicationDbContext.Rapports.Add(rapport);
                    await applicationDbContext.SaveChangesAsync();


                    return RedirectToAction("VosRapports");
                    //  }
                }


                return RedirectToAction("VosRapports");


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
                List<Rapport> mesRapports = applicationDbContext.Rapports.Where(r => r.EtudiantId == etudiantId).ToList();
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
