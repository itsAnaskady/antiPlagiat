using AppAntiPlagiat.Models;
using AppAntiPlagiat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppAntiPlagiat.Controllers
{
    //[Authorize(Roles = "enseignant")]
    public class EnseignantController : Controller
    {
        private readonly UserManager<Utilisateur> userManager;
        private readonly ApplicationDbContext applicationDbContext;

        public EnseignantController(UserManager<Utilisateur> userManager,ApplicationDbContext applicationDbContext)
        {
            this.userManager = userManager;
            this.applicationDbContext = applicationDbContext;
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
                    imgData = user.imgData,
                    imgType = user.imgType,
                    Nom = user.Nom,
                    Prenom= user.Prenom,
                    id = user.Id
                };
                return View(model);
            }

            return RedirectToAction("LogOut","Home");
        }
        public async Task<IActionResult> ListeEtudiants()
        {
           
                ViewBag.Ltype = "enseignant";
                if (User.Identity.IsAuthenticated)
                {
                    string enseignantId = userManager.GetUserId(User);

                    // Récupérer les enseignants affectés à l'étudiant connecté
                    var etudiantsAffectes = applicationDbContext.Encadre
                        .Where(e => e.EnseignantId == enseignantId)
                        .Select(e => e.Etudiant)
                        .ToList();

                    // Récupérer les types de stage affectés à l'étudiant connecté
                    var typesStageAffectes = applicationDbContext.Encadre
                        .Where(e => e.EnseignantId == enseignantId)
                        .Select(e => e.TypeStage)
                        .ToList();
                    var model = new Tuple<List<Utilisateur>, List<string>>(etudiantsAffectes, typesStageAffectes);
                    return View(model);
                }
                return View();
            
        }
        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(string id,IFormFile image)
        {
            using (var stream = new MemoryStream())
            {
                await image.CopyToAsync(stream);
                var enseignant = applicationDbContext.Users.Find(id);
                if (enseignant == null)
                {
                    return NotFound();
                }

                enseignant.imgData = stream.ToArray();
                enseignant.imgType = image.ContentType;

                applicationDbContext.SaveChanges();

            }
                return RedirectToAction("Profile");
        }


        public IActionResult ListeRapports()
        {
            ViewBag.Ltype = "enseignant";
            Rapport rp = new Rapport();
            if (User.Identity.IsAuthenticated)
            {
                string enseignantId = userManager.GetUserId(User);

                // Récupérer les enseignants affectés à l'étudiant connecté
                var etudiantsAffectes = applicationDbContext.Encadre
                    .Where(e => e.EnseignantId == enseignantId)
                    .Select(e => e.Etudiant)
                    .ToList();
               
                // Récupérer les types de stage affectés à l'étudiant connecté
                var typesStageAffectes = applicationDbContext.Encadre
                    .Where(e => e.EnseignantId == enseignantId)
                    .Select(e => e.TypeStage)
                    .ToList();
                List<Rapport> rapportsEtudiant = new List<Rapport>();
                foreach (var e in etudiantsAffectes)
                {
                    var rapports = applicationDbContext.Rapports
                        .Where(r => r.EtudiantId == e.Id)
                        .ToList();
                    rapportsEtudiant.AddRange(rapports);
                }
                var Rapportslistes = new Tuple<List<Utilisateur>, List<string>, List<Rapport>>(etudiantsAffectes, typesStageAffectes, rapportsEtudiant);
                return View(Rapportslistes);
               

            }
            return View();
        }
        public IActionResult Scann()
        {
            ViewBag.Ltype = "enseignant";
            return View();
        }


    }
}
