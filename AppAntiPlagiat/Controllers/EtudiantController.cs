using AppAntiPlagiat.Models;
using AppAntiPlagiat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace AppAntiPlagiat.Controllers
{
    [Authorize(Roles = "etudiant")]
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
                    Prenom = user.Prenom,
                    id = user.Id
                };
                return View(model);
            }
            return View();
        }
        public IActionResult ImporterRapport()
        {
            ViewBag.Ltype = "etudiant";

            //Initialiser le model
            ImporterRapportViewModel model = new ImporterRapportViewModel();
            model.TypeStage = new List<string>();


            //Rechercher les affectations de l'étudiant
            string etuId = applicationDbContext.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault().Id;
            var affectations = applicationDbContext.Encadre.Where(af => af.EtudiantId == etuId ).ToList();

            if(affectations != null )
            {
                foreach (var affect in affectations)
                {
                    if (!vérifierTypeStage(affect.TypeStage,etuId))
                    {
                        model.TypeStage.Add(affect.TypeStage);
                    }
                }
            }

            return View(model);
        }

        public bool vérifierTypeStage(string type, string etuId)
        {
            var rapports = applicationDbContext.Rapports.Where(x => x.EtudiantId == etuId).ToList();
            foreach(Rapport r in rapports)
            {
                if(r.Type == type)
                {
                    return true;
                }
            }
            return false;
        }
        [HttpPost]
        public async Task<IActionResult> DeposerRapp(ImporterRapportViewModel model, IFormFile pdfFile)
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
                        Type = model.Rapport.Type,
                        DateModif = null,
                        Validé = false,
                    };
                    applicationDbContext.Rapports.Add(rapport);
                    var etud = applicationDbContext.Utilisateurs.Where(x=> x.UserName==User.Identity.Name).FirstOrDefault();
                    string idEnseignant = applicationDbContext.Encadre.Where(x => x.EtudiantId == etud.Id && x.TypeStage == rapport.Type).FirstOrDefault().EnseignantId; 
                    Notification notification = new Notification()
                    {
                        DateNotif = DateTime.Now,
                        message = "L'étudiant " + etud.NomComplet + " à déposer un rapport de type " + rapport.Type,
                        UserIdDesti = idEnseignant,
                        User = etud,
                        UserId = etud.Id,
                        Vu = false
                    };
                    applicationDbContext.Notifications.Add(notification);
                    applicationDbContext.SaveChanges();
                }
                return RedirectToAction("VosRapports");

            }
            return RedirectToAction("ImporterRapport");

        }
        public IActionResult VosRapports()
        {
            ViewBag.Ltype = "etudiant";
            var rapportsViewModel = new List<RapportViewModel>();

            if (User.Identity.IsAuthenticated)
            {
                string etudiantId = userManager.GetUserId(User);
                var mesRapports = applicationDbContext.Rapports.Where(r => r.EtudiantId == etudiantId).ToList();
                
                if (mesRapports != null) {
                    
                    foreach (Rapport rapport in mesRapports)
                    {
                        RapportViewModel model = new RapportViewModel()
                        {
                            rapport = rapport,
                            pourcentagePlagiat = plagiatAuto(rapport.Id)
                        };
                        rapportsViewModel.Add(model);
                        if ((model.pourcentagePlagiat / 100) >= applicationDbContext.pourcentagePlagiats.FirstOrDefault().Pourcentage)
                        {
                            ViewBag.Alert = "ICI";
                        }
                    }
                }
            }
            return View(rapportsViewModel); 
        }
        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(string id, IFormFile image)
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
       
        public ActionResult ModifierRapport(int RappId, string type)
        {
            ViewBag.Ltype = "etudiant";
            ViewBag.RapportType = type;
            ViewBag.RapportId = RappId;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ModifierRapport(int RappId,string type, IFormFile newPdf)
        {
			ViewBag.Ltype = "etudiant";
			if (newPdf != null && newPdf.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await newPdf.CopyToAsync(stream);
                    PlagiarismDetection aP = new PlagiarismDetection(applicationDbContext);
                    double poucentagePlagiat = aP.PlagiatAutoExeption(RappId,stream.ToArray());

                    if (poucentagePlagiat >= applicationDbContext.pourcentagePlagiats.ToList()[0].Pourcentage)
                    {
                        return RedirectToAction("RapportRefusé");
                    }

                    var monRapp = applicationDbContext.Rapports.Find(RappId);
                    if (monRapp == null)
                    {
                        return NotFound();
                    }

                    monRapp.data = stream.ToArray();
                    monRapp.DateModif = DateTime.Now;

                    applicationDbContext.SaveChanges();
                    ViewBag.modifier = "modifier";
                }
            }
           return View(new { RappId = RappId,type =type});
        }
       
        public ActionResult Notification(int Page = 1)
        {
            ViewBag.Ltype = "etudiant";
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
