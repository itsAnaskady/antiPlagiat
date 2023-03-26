using AppAntiPlagiat.Models;
using AppAntiPlagiat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppAntiPlagiat.Controllers
{
    [Authorize(Roles = "enseignant")]
    public class EnseignantController : Controller
    {
        private readonly UserManager<Utilisateur> userManager;
        private readonly ApplicationDbContext applicationDbContext;

        public EnseignantController(UserManager<Utilisateur> userManager,ApplicationDbContext applicationDbContext)
        {
            this.userManager = userManager;
            this.applicationDbContext = applicationDbContext;
        }
        
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
        public async Task<JsonResult> fitrageRapport1(string type, string niveau, string filiere)
        {
            List<Rapport> rapportByType = new List<Rapport>();
            List<Rapport> rapportByNv = new List<Rapport>();
            List<Rapport> rapportByFil = new List<Rapport>();

            if (type != "tout")
                rapportByType = applicationDbContext.Rapports.Where(x => x.Type == type).ToList();
            else
                rapportByType = applicationDbContext.Rapports.ToList();

            if (niveau != "tout")
                rapportByNv = applicationDbContext.Rapports.Where(x => x.Etudiant.Niveau == niveau).ToList();
            else
                rapportByNv = applicationDbContext.Rapports.ToList();

            if (filiere != "tout")
                rapportByFil = applicationDbContext.Rapports.Where(x => x.Etudiant.Filiere == filiere).ToList();
            else
                rapportByFil = applicationDbContext.Rapports.ToList();


            var rapport = rapportByFil.Intersect(rapportByNv).Intersect(rapportByType).ToList();

            List<GestionRapportViewModel1> gr = new List<GestionRapportViewModel1>();
            if (rapport.Count() != 0)
            {
                foreach (var item in rapport)
                {
                    GestionRapportViewModel1 model = new GestionRapportViewModel1()
                    {
                        rapport = item,
                        Encadre = applicationDbContext.Encadre.Where(x => x.EtudiantId == item.EtudiantId && x.TypeStage == item.Type).FirstOrDefault(),
                        Pplagiat = plagiatAuto(item.data).ToString("0.00") + "%",
                    };
                    model.filière = applicationDbContext.Utilisateurs.Where(x => x.Id == model.Encadre.EtudiantId).FirstOrDefault().Filiere;
                    model.niveau = applicationDbContext.Utilisateurs.Where(x => x.Id == model.Encadre.EtudiantId).FirstOrDefault().Niveau;
                    if (applicationDbContext.Users.Where(x => x.Id == model.Encadre.EnseignantId).FirstOrDefault() != null)
                    {
                        model.enseignantnom = model.Encadre.Enseignant.Nom + " " + model.Encadre.Enseignant.Prenom;
                    }
                    model.dateDepot = "" + model.rapport.DateDepot;

                    if (model.rapport.DateModif != null)
                    {
                        model.dateModif = "" + model.rapport.DateModif;
                    }

                    gr.Add(model);
                }

            }
            return Json(gr);
        }
        public JsonResult filtrageParInput(string input)
        {
            input = input.Trim().ToLower();
            var rapport1 = applicationDbContext.Rapports
                            .Where(x =>
                                    x.DateModif.ToString().Trim().ToLower().Contains(input) ||
                                    x.DateDepot.ToString().Trim().ToLower().Contains(input) ||
                                    x.Intitulé.Trim().ToLower().Contains(input) ||
                                    x.Type.Trim().ToLower().Contains(input) ||
                                    x.Etudiant.Nom.Trim().ToLower().Contains(input) ||
                                    x.Etudiant.Prenom.Trim().ToLower().Contains(input) ||
                                    x.Etudiant.Filiere.Trim().ToLower().Contains(input) ||
                                    x.Etudiant.Niveau.Trim().ToLower().Contains(input)
                            ).ToList();

            var rapport = new List<Rapport>();
            if (rapport1.Count() == 0)
            {
                var encadre = applicationDbContext.Encadre
                                .Where(x =>
                                    x.Enseignant.Nom.Trim().ToLower().Contains(input) ||
                                    x.Enseignant.Prenom.Trim().ToLower().Contains(input) ||
                                    x.Enseignant.Departement.Trim().ToLower().Contains(input) ||
                                    x.Enseignant.Email.Trim().ToLower().Contains(input)
                                ).ToList();

                foreach (var item in encadre)
                {
                    var r = applicationDbContext.Rapports.Where(x => x.EtudiantId == item.EtudiantId).FirstOrDefault();
                    if (r != null)
                        rapport.Add(r);
                }
            }
            else
            {
                rapport = rapport1;
            }

            rapport = rapport.Distinct().ToList();

            List<GestionRapportViewModel1> gr = new List<GestionRapportViewModel1>();
            if (rapport.Count() != 0)
            {
                foreach (var item in rapport)
                {
                    GestionRapportViewModel1 model = new GestionRapportViewModel1()
                    {
                        rapport = item,
                        Encadre = applicationDbContext.Encadre.Where(x => x.EtudiantId == item.EtudiantId && x.TypeStage == item.Type).FirstOrDefault(),
                        Pplagiat = plagiatAuto(item.data).ToString("0.00") + "%",
                    };
                    model.filière = applicationDbContext.Utilisateurs.Where(x => x.Id == model.Encadre.EtudiantId).FirstOrDefault().Filiere;
                    model.niveau = applicationDbContext.Utilisateurs.Where(x => x.Id == model.Encadre.EtudiantId).FirstOrDefault().Niveau;
                    if (applicationDbContext.Users.Where(x => x.Id == model.Encadre.EnseignantId).FirstOrDefault() != null)
                    {
                        model.enseignantnom = model.Encadre.Enseignant.Nom + " " + model.Encadre.Enseignant.Prenom;
                    }
                    model.dateDepot = "" + model.rapport.DateDepot;

                    if (model.rapport.DateModif != null)
                    {
                        model.dateModif = "" + model.rapport.DateModif;
                    }

                    gr.Add(model);
                }

            }
            gr = gr.Distinct().ToList();
            return Json(gr);
        }
        public JsonResult getAllRapport()
        {
            var rapport = applicationDbContext.Rapports.ToList();
            List<GestionRapportViewModel1> gr = new List<GestionRapportViewModel1>();
            if (rapport.Count() != 0)
            {
                foreach (var item in rapport)
                {
                    GestionRapportViewModel1 model = new GestionRapportViewModel1()
                    {
                        rapport = item,
                        Encadre = applicationDbContext.Encadre.Where(x => x.EtudiantId == item.EtudiantId && x.TypeStage == item.Type).FirstOrDefault(),
                        Pplagiat = plagiatAuto(item.data).ToString("0.00") + "%",
                    };
                    model.filière = applicationDbContext.Utilisateurs.Where(x => x.Id == model.Encadre.EtudiantId).FirstOrDefault().Filiere;
                    model.niveau = applicationDbContext.Utilisateurs.Where(x => x.Id == model.Encadre.EtudiantId).FirstOrDefault().Niveau;
                    if (applicationDbContext.Users.Where(x => x.Id == model.Encadre.EnseignantId).FirstOrDefault() != null)
                    {
                        model.enseignantnom = model.Encadre.Enseignant.Nom + " " + model.Encadre.Enseignant.Prenom;
                    }
                    model.dateDepot = "" + model.rapport.DateDepot;

                    if (model.rapport.DateModif != null)
                    {
                        model.dateModif = "" + model.rapport.DateModif;
                    }

                    gr.Add(model);
                }

            }
            return Json(gr);
        }
        public double plagiatAuto(byte[] pdf)
        {
            PlagiarismDetection aP = new PlagiarismDetection(applicationDbContext);
            double poucentagePlagiat = aP.Plagiat(pdf);

            return poucentagePlagiat * 100;
        }
        public ActionResult DownloadPdf(int id)
        {
            var pdf = applicationDbContext.Rapports.Find(id);
            if (pdf == null)
            {
                return RedirectToAction("VosRapports");
            }
            return File(pdf.data, "application/pdf", pdf.Intitulé + ".pdf");
        }
        public IActionResult RechercherRapport()
        {
            ViewBag.Ltype = "enseignant";
            var rapport = applicationDbContext.Rapports.ToList();
            List<GestionRapportViewModel> gr = new List<GestionRapportViewModel>();

            if (rapport.Count() != 0)
            {
                foreach (Rapport item in rapport)
                {
                    GestionRapportViewModel model = new GestionRapportViewModel()
                    {
                        rapport = item,
                        Encadre = applicationDbContext.Encadre.Where(x => x.EtudiantId == item.EtudiantId && item.Type == x.TypeStage).FirstOrDefault(),
                        filière = applicationDbContext.Utilisateurs.Where(x => x.Id == item.EtudiantId).FirstOrDefault().Filiere,
                        niveau = applicationDbContext.Utilisateurs.Where(x => x.Id == item.EtudiantId).FirstOrDefault().Niveau,
                        Pplagiat = plagiatAuto(item.data).ToString("0.00") + "%"
                    };
                    gr.Add(model);
                }
            }

            return View(gr);
        }

    }
}
