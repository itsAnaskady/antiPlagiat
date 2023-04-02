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
            string id = userManager.GetUserId(User);
            var encadre = applicationDbContext.Encadre.Where(x => x.EnseignantId == id).ToList();
            List<RapportViewModel> rapportModel = new List<RapportViewModel>();
            if(encadre.Count() != 0) { 
                foreach (Encadre e in encadre)
                {
                    RapportViewModel model = new RapportViewModel()
                    {
                        rapport = applicationDbContext.Rapports.Where(x => x.EtudiantId == e.EtudiantId && x.Type == e.TypeStage).FirstOrDefault()
                    };
                    if (model.rapport != null) { 
                        model.pourcentagePlagiat = plagiatAuto(model.rapport.Id);
                        rapportModel.Add(model);
                    }
                }
            }
            return View(rapportModel);
        }
        public IActionResult PlagiatManuelle(int id)
        {
            ViewBag.Ltype = "enseignant";
            PlagiarismDetection p = new PlagiarismDetection(applicationDbContext);
            var rapport = applicationDbContext.Rapports.Find(id);
            List<rapportEtPourcentage> modelList = new List<rapportEtPourcentage>();
            if (rapport == null) { return NotFound(); }

            modelList = p.rapportEtPourcentages(rapport.Id);

            List<PlagiatManuelleViewModel> Liste = new List<PlagiatManuelleViewModel>();

            if (applicationDbContext.Rapports.Count()<=5)
            {
                modelList = modelList.OrderByDescending(x => x.Pplagiat).Take(applicationDbContext.Rapports.Count()).ToList();
            }
            else
            {
                modelList = modelList.OrderByDescending(x => x.Pplagiat).Take(5).ToList();
            }
            

            foreach (var model in modelList)
            {
                model.Pplagiat = model.Pplagiat * 100;
                PlagiatManuelleViewModel m = new PlagiatManuelleViewModel();
                var r = applicationDbContext.Rapports.Find(model.IdRapport);
                m.etudiant = applicationDbContext.Utilisateurs.Find(r.EtudiantId);
                m.rapportEtPourcentage = model;
                Liste.Add(m);
            }
            

            return View(Liste);
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
                        Pplagiat = plagiatAuto(item.Id).ToString("0.00") + "%",
                    };
                    if (model.Encadre != null)
                    {
                        model.filière = applicationDbContext.Utilisateurs.Where(x => x.Id == model.Encadre.EtudiantId).FirstOrDefault().Filiere;
                        model.niveau = applicationDbContext.Utilisateurs.Where(x => x.Id == model.Encadre.EtudiantId).FirstOrDefault().Niveau;
                        if (applicationDbContext.Users.Where(x => x.Id == model.Encadre.EnseignantId).FirstOrDefault() != null)
                        {
                            model.enseignantnom = model.Encadre.Enseignant.Nom + " " + model.Encadre.Enseignant.Prenom;
                        }
                    }
                    else
                    {
                        model.filière = "Inconnue";
                        model.niveau = "Inconnu";
                        model.enseignantnom = "Inconnu";
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
                        Pplagiat = plagiatAuto(item.Id).ToString("0.00") + "%",
                    };
                    if (model.Encadre != null)
                    {
                        model.filière = applicationDbContext.Utilisateurs.Where(x => x.Id == model.Encadre.EtudiantId).FirstOrDefault().Filiere;
                        model.niveau = applicationDbContext.Utilisateurs.Where(x => x.Id == model.Encadre.EtudiantId).FirstOrDefault().Niveau;
                    }
                    else
                    {
                        model.filière = "Inconnue";
                        model.niveau = "Inconnu";
                    }
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
                        Pplagiat = plagiatAuto(item.Id).ToString("0.00") + "%",
                    };
                    if (model.Encadre != null)
                    {
                        model.filière = applicationDbContext.Utilisateurs.Where(x => x.Id == model.Encadre.EtudiantId).FirstOrDefault().Filiere;
                        model.niveau = applicationDbContext.Utilisateurs.Where(x => x.Id == model.Encadre.EtudiantId).FirstOrDefault().Niveau;
                    }
                    else
                    {
                        model.filière = "Inconnue";
                        model.niveau = "Inconnu";
                    }
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
        public double plagiatAuto(int id)
        {
            PlagiarismDetection aP = new PlagiarismDetection(applicationDbContext);
            double poucentagePlagiat = aP.Plagiat(id);

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
                    if (item.EtudiantId != null)
                    {
                        GestionRapportViewModel model = new GestionRapportViewModel()
                        {
                            rapport = item,
                            Encadre = applicationDbContext.Encadre.Where(x => x.EtudiantId == item.EtudiantId && item.Type == x.TypeStage).FirstOrDefault(),
                            filière = applicationDbContext.Utilisateurs.Where(x => x.Id == item.EtudiantId).FirstOrDefault().Filiere,
                            niveau = applicationDbContext.Utilisateurs.Where(x => x.Id == item.EtudiantId).FirstOrDefault().Niveau,
                            Pplagiat = plagiatAuto(item.Id).ToString("0.00") + "%"
                        };
                        gr.Add(model);
                    }
                    else
                    {
                        GestionRapportViewModel model = new GestionRapportViewModel()
                        {
                            rapport = item,
                            Encadre = null,
                            filière = "Inconnue",
                            niveau = "Inconnu",
                            Pplagiat = plagiatAuto(item.Id).ToString("0.00") + "%"
                        };
                        gr.Add(model);
                    }
                }
            }

            return View(gr);
        }

        public IActionResult ValiderRapport(int id)
        {
            var rapport = applicationDbContext.Rapports.Find(id);
            if (rapport == null) { return NotFound(); }
           

            var u = applicationDbContext.Utilisateurs.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            if (u == null) { return NotFound(); }

            rapport.Validé = true;

            Notification notification = new Notification()
            {
                message = "L'enseignant "+u.NomComplet+" a validé le rapport "+rapport.Intitulé+" du type " + rapport.Type+". Veuillez consultez votre liste des rapports.",
                User = applicationDbContext.Utilisateurs.Find(rapport.EtudiantId),
                UserId = u.Id,
                UserIdDesti = rapport.EtudiantId,
                Vu = false,
                DateNotif = DateTime.Now
            };
            applicationDbContext.Notifications.Add(notification);
            applicationDbContext.SaveChanges();

            return RedirectToAction("ListeRapports");
        }

        public ActionResult Notification(int Page = 1)
        {
            ViewBag.Ltype = "enseignant";
            int pageSize = 25;
            int skip = (Page - 1) * pageSize;
            var liste = new List<NotificationViewModel>();
            List<Notification> N = GetNotificationsSubset(skip, pageSize);

            foreach (Notification item in N)
            {

                NotificationViewModel notif = new NotificationViewModel()
                {
                    emetteur = applicationDbContext.Utilisateurs.Find(item.UserId),
                    notification = item,
                    tempPasse = TimeAgo(item.DateNotif)
                };

                liste.Add(notif);
            }

            int TotalMessageCount = applicationDbContext.Messages.Count();
            ViewBag.TotalMessageCount = TotalMessageCount;

            bool hasMoreMessages = (applicationDbContext.Messages.Count() >= skip + pageSize);
            ViewBag.HasMoreMessages = hasMoreMessages;

            ViewBag.Page = Page;
            return View(liste);
        }
        private List<Notification> GetNotificationsSubset(int skip, int pageSize)
        {
            string id = userManager.GetUserId(User);
            var notifications = applicationDbContext.Notifications
                                        .Where(x => x.UserIdDesti == id)
                                        .OrderByDescending(m => m.DateNotif)
                                        .Skip(skip)
                                        .Take(pageSize)
                                        .ToList();
            foreach (Notification item in notifications)
            {
                item.Vu = true;
            }
            applicationDbContext.SaveChanges();

            return notifications;
        }
        public string TimeAgo(DateTime dateTime)
        {
            string result = string.Empty;
            var timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("Il y a {0} secondes", timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    String.Format("Il y a {0} minutes", timeSpan.Minutes) :
                    "Il y a une minute";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    String.Format("Il y a {0} heures", timeSpan.Hours) :
                    "Il y a une heure";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    String.Format("Il y a {0} jours", timeSpan.Days) :
                    "Hier";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    String.Format("Il y a {0} mois", timeSpan.Days / 30) :
                    "Il y a un mois";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    String.Format("Il y a {0} ans", timeSpan.Days / 365) :
                    "Il y a une ans";
            }

            return result;
        }
        public IActionResult DeleteSelectedNotifications(int[] Ids)
        {
            foreach (int i in Ids)
            {
                var a = applicationDbContext.Notifications.Find(i);
                applicationDbContext.Remove(a);
            }
            applicationDbContext.SaveChanges();

            return RedirectToAction("Notifications"); ;
        }

    }
}
