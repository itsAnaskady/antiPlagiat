﻿using AppAntiPlagiat.Models;
using AppAntiPlagiat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;


namespace AppAntiPlagiat.Controllers
{

    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<Utilisateur> userManager;
        private readonly ApplicationDbContext applicationDbContext;

        public AdminController(UserManager<Utilisateur> userManager, ApplicationDbContext applicationDbContext)
        {
            this.userManager = userManager;
            this.applicationDbContext = applicationDbContext;
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
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Users/NewUser.png");
                var imageData = System.IO.File.ReadAllBytes(imagePath);
                Utilisateur nvEnseignant = new Utilisateur
                {
                    Email = model.Email,
                    Nom = model.Nom,
                    Prenom = model.Prenom,
                    UserName = model.Email,
                    NormalizedUserName = model.Email.ToUpper(),
                    NormalizedEmail = model.Email.ToUpper(),
                    Departement = model.Departement,
                    imgData = imageData,
                    imgType = "image/png"
                };
                //add user
                var result = await userManager.CreateAsync(nvEnseignant, model.Password);

                //add user to role
                var result1 = await userManager.AddToRoleAsync(nvEnseignant, "enseignant");

                if (result.Succeeded && result1.Succeeded)
                {
                    ViewBag.ajoutNV = "ajouté";
                    return View(model);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        public async Task<IActionResult> RechSuppModEnseignant()
        {
            ViewBag.Ltype = "admin";
            var users = await userManager.GetUsersInRoleAsync("enseignant");

            return View(users);
        }
        public IActionResult AjouterEtudiant()
        {
            ViewBag.Ltype = "admin";

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AjouterEtudiant(AjouterEtudiantViewModel model)
        {
            ViewBag.Ltype = "admin";
            if (ModelState.IsValid)
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Users/NewUser.png");
                var imageData = System.IO.File.ReadAllBytes(imagePath);
                Utilisateur nvEtudiant = new Utilisateur
                {
                    Email = model.Email,
                    Nom = model.Nom,
                    Prenom = model.Prenom,
                    UserName = model.Email,
                    NormalizedUserName = model.Email.ToUpper(),
                    NormalizedEmail = model.Email.ToUpper(),
                    Niveau = model.Niveau,
                    imgData = imageData,
                    imgType = "image/png",
                    Filiere = model.Filiere,
                    CNE = model.CNE
                };
                //add user
                var result = await userManager.CreateAsync(nvEtudiant, model.Password);

                //add user to role
                var result1 = await userManager.AddToRoleAsync(nvEtudiant, "etudiant");

                if (result.Succeeded && result1.Succeeded)
                {
                    ViewBag.ajoutNV = "ajouté";
                    return View(model);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        public async Task<IActionResult> RechSuppModEtudiant()
        {
            ViewBag.Ltype = "admin";
            var etudiants = await userManager.GetUsersInRoleAsync("etudiant");
            return View(etudiants);
        }

        [HttpPost]
        public ActionResult SupprimerAffectation(int id)
        {
            Encadre encadrement = applicationDbContext.Encadre.Find(id);
            if (encadrement != null)
            {
                applicationDbContext.Encadre.Remove(encadrement);
                applicationDbContext.SaveChanges();
                TempData["SuccessMessage"] = "L'étudiant a été supprimé avec succès.";
            }

            return RedirectToAction("ListeAffectation");
        }
        [HttpGet]
        public ActionResult ListeAffectation(int id)
        {
            ViewBag.Ltype = "admin";

            var encadreList = from Encadre in applicationDbContext.Encadre
                              join etudiant in applicationDbContext.Utilisateurs on Encadre.EtudiantId equals etudiant.Id
                              join enseignant in applicationDbContext.Utilisateurs on Encadre.EnseignantId equals enseignant.Id
                              select new
                              {
                                  Id = Encadre.Id,
                                  etudiantID = etudiant.Id,
                                  enseignantID = enseignant.Id,
                                  EtudiantNom = etudiant.Nom,
                                  EtudiantPrenom = etudiant.Prenom,
                                  EnseignantNom = enseignant.Nom,
                                  EnseignantPrenom = enseignant.Prenom,
                                  Encadre.TypeStage,
                                  EtudiantFiliere = etudiant.Filiere,
                              };

            return View(encadreList);
        }
        [HttpPost]
        public ActionResult AffecterEncadrement(string EtudiantId, string EnseignantId, string TypeStage)
        {
            if (ModelState.IsValid)
            {
                // Créer un nouvel objet Encadre avec les valeurs des champs
                Encadre newEncadre = new Encadre
                {
                    EtudiantId = EtudiantId,
                    EnseignantId = EnseignantId,
                    TypeStage = TypeStage
                };
                if (newEncadre != null)
                {   // Ajouter l'objet Encadre à la base de données
                    applicationDbContext.Encadre.Add(newEncadre);
                    applicationDbContext.SaveChanges();
                    TempData["SuccesMessage"] = "Affectation réussie.";
                }

                else
                {
                    TempData["ErrorMessage"] = "Vous devez sélectionner un étudiant et un enseignant pour effectuer une affectation.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Vous devez sélectionner un étudiant et un enseignant pour effectuer une affectation.";
            }


            return RedirectToAction("Affectation");
        }
        public async Task<IActionResult> Affectation(string EtudiantId, string EnseignantId, string TypeStage)
        {


            ViewBag.Ltype = "admin";

            var etudiants = await userManager.GetUsersInRoleAsync("etudiant");

            var etudiantList = new SelectList(etudiants, "Id", "NomComplet");
            ViewBag.EtudiantList = etudiantList;
            ViewBag.Etudiants = new SelectList(etudiants, "Id", "NomComplet");

            //recup enseig
            var enseignants = await userManager.GetUsersInRoleAsync("enseignant");
            var enseignantList = new SelectList(enseignants, "Id", "NomComplet");
            ViewBag.EnseignantList = enseignantList;
            ViewBag.Enseignants = new SelectList(enseignants, "Id", "NomComplet");


            return View();
        }
        public async Task<IActionResult> AffectationAlleatoire(string TypeStage, String Niveau, string Filiere)
        {

            ViewBag.Ltype = "admin";
            // Récupération des données nécessaires depuis la base de données
            var etudiants = await userManager.GetUsersInRoleAsync("etudiant");
            etudiants = etudiants.Where(e => e.Niveau == Niveau).ToList();
            etudiants = etudiants.Where(e => e.Filiere == Filiere).ToList();
            //enseign
            var enseignants = await userManager.GetUsersInRoleAsync("enseignant");
            // enseignants = enseignants.Where(e => e.Niveau == Niveau).ToList();

            var affectations = new List<Encadre>();
            // Affectation aléatoire des étudiants aux enseignants
            Random random = new Random();
            foreach (var etudiant in etudiants)
            {
                int index = random.Next(0, enseignants.Count);
                var enseignant = enseignants[index];
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(TypeStage))
                    {
                        // Enregistrement de l'affectation dans la base de données

                        applicationDbContext.Encadre.Add(new Encadre
                        {
                            EtudiantId = etudiant.Id,
                            EnseignantId = enseignant.Id,
                            TypeStage = TypeStage // Récupération du type de stage spécifié par l'utilisateur

                        });
                        TempData["SuccesMessage"] = "Affectation réussie.";
                        applicationDbContext.SaveChanges();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Vous devez sélectionner un étudiant et un enseignant pour effectuer une affectation.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Vous devez sélectionner un étudiant et un enseignant pour effectuer une affectation.";

                }
                // Retrait de l'enseignant de la liste pour éviter les doublons
                //enseignants.RemoveAt(index);
            }

            return View("AffectationAlleatoire"); // Redirection vers la page d'affichage des affectations
        }

        [HttpGet]
        public async Task<IActionResult> Modifier(int id)// modifier une affectation
        {
            var affectation = await applicationDbContext.Encadre.FindAsync(id);
            if (affectation == null)
            {
                return NotFound();
            }

            var etudiants = await userManager.GetUsersInRoleAsync("etudiant");
            var etudiantList = new SelectList(etudiants, "Id", "NomComplet", affectation.EtudiantId);

            var enseignants = await userManager.GetUsersInRoleAsync("enseignant");
            var enseignantList = new SelectList(enseignants, "Id", "NomComplet", affectation.EnseignantId);


            ViewBag.EtudiantList = etudiantList;
            ViewBag.EnseignantList = enseignantList;
            applicationDbContext.Encadre.Remove(affectation);
            applicationDbContext.SaveChanges();
            ViewBag.Ltype = "admin";

            return View("Affectation");

        }
        [HttpPost]
        public async Task<IActionResult> Modifier(Encadre encadre)
        {
            if (ModelState.IsValid)
            {
                applicationDbContext.Update(encadre);
                await applicationDbContext.SaveChangesAsync();

                return RedirectToAction("ListeAffectations");
            }

            var etudiants = await userManager.GetUsersInRoleAsync("etudiant");
            var etudiantList = new SelectList(etudiants, "Id", "NomComplet", encadre.EtudiantId);

            var enseignants = await userManager.GetUsersInRoleAsync("enseignant");
            var enseignantList = new SelectList(enseignants, "Id", "NomComplet", encadre.EnseignantId);


            ViewBag.EtudiantList = etudiantList;
            ViewBag.EnseignantList = enseignantList;
            ViewBag.Ltype = "admin";

            return View(encadre);
        }
      
        public IActionResult Messages(int Page = 1)
        {
            ViewBag.Ltype = "admin";
            int pageSize = 25;
            int skip = (Page - 1) * pageSize;
            var liste = new List<ListeMessagesViewModel>();
            List<Message> d = GetMessageSubset(skip, pageSize);
            foreach (Message message in d)
            {
                ListeMessagesViewModel msg = new ListeMessagesViewModel();
                msg.Email = message.Email;
                msg.Emetteur = message.Emetteur;
                msg.Id = message.Id;
                msg.Msg = FirstPhrase(message.Msg);
                msg.Subject = message.Subject;
                msg.tempsPasse = TimeAgo(message.dateEnvoie);
                liste.Add(msg);
            }

            int TotalMessageCount = applicationDbContext.Messages.Count();
            ViewBag.TotalMessageCount = TotalMessageCount;

            bool hasMoreMessages = (applicationDbContext.Messages.Count() >= skip + pageSize);
            ViewBag.HasMoreMessages = hasMoreMessages;

            ViewBag.Page = Page;
            return View(liste);
        }
        private List<Message> GetMessageSubset(int skip, int pageSize)
        {
            List<Message> messages = applicationDbContext.Messages
                                        .OrderByDescending(m => m.dateEnvoie)
                                        .Skip(skip)
                                        .Take(pageSize)
                                        .ToList();
            return messages;
        }
        [HttpPost]
        public IActionResult DeleteSelectedMessages(int[] messageIds)
        {
            // Delete the selected messages from the database
            foreach (var messageId in messageIds)
            {
                var message = applicationDbContext.Messages.Find(messageId);
                applicationDbContext.Messages.Remove(message);
            }
            applicationDbContext.SaveChanges();

            // Redirect to the inbox page
            return RedirectToAction("Messages");
        }
        [HttpPost]
        public IActionResult DeleteSelectedEtudiants(string[] etudiantIds)
        {
            // Delete the selected messages from the database
            if (etudiantIds.Length != 0)
            {
                foreach (var Id in etudiantIds)
                {
                    var listeEncadre = applicationDbContext.Encadre.Where(x => x.EtudiantId == Id).ToList();
                    if (listeEncadre != null)
                    {
                        foreach (Encadre u in listeEncadre)
                        {
                            applicationDbContext.Encadre.Remove(u);
                        }
                    }
                    var etudiant = applicationDbContext.Utilisateurs.Find(Id);
                    applicationDbContext.Utilisateurs.Remove(etudiant);
                }
                
            }
            applicationDbContext.SaveChanges();
            // Redirect to the inbox page
            return RedirectToAction("RechSuppModEtudiant");
        }
        [HttpPost]
        public IActionResult DeleteSelectedEnseignants(string[] enseignantIds)
        {
            // Delete the selected messages from the database
            if (enseignantIds.Length != 0)
            {
                foreach (var Id in enseignantIds)
                {
                    var enseignant = applicationDbContext.Utilisateurs.Find(Id);
                    var listeEncadre = applicationDbContext.Encadre.Where(x => x.EnseignantId == Id).ToList();
                    foreach (Encadre u in listeEncadre)
                    {
                        applicationDbContext.Encadre.Remove(u);
                    }
                    applicationDbContext.Utilisateurs.Remove(enseignant);
                }
                applicationDbContext.SaveChanges();
            }

            // Redirect to the inbox page
            return RedirectToAction("RechSuppModEnseignant");
        }
        [HttpPost]
        public IActionResult DeleteEnseignant(string enseigId)
        {
            var enseignant = applicationDbContext.Utilisateurs.Find(enseigId);
            if (enseignant != null)
            {
                
                var listeEncadre = applicationDbContext.Encadre.Where(x => x.EnseignantId == enseigId).ToList();
                if (listeEncadre != null)
                {
                    foreach (Encadre u in listeEncadre)
                    {
                        applicationDbContext.Encadre.Remove(u);
                    }
                }
                applicationDbContext.Utilisateurs.Remove(enseignant);
                applicationDbContext.SaveChanges();
            }

            return RedirectToAction("RechSuppModEnseignant");
        }
        [HttpPost]
        public IActionResult DeleteEtudiant(string etudId)
        {
            var etudiant = applicationDbContext.Utilisateurs.Find(etudId);
            if (etudiant != null)
            {
                var listeEncadre = applicationDbContext.Encadre.Where(x => x.EtudiantId == etudId).ToList();
                if (listeEncadre != null)
                {
                    foreach (Encadre u in listeEncadre)
                    {
                        applicationDbContext.Encadre.Remove(u);
                    }
                }
                var listeRapports = applicationDbContext.Rapports.Where(x => x.EtudiantId == etudId).ToList();
                if (listeRapports != null)
                {
                    foreach(Rapport r in listeRapports)
                    {
                        r.Etudiant = null;
                        r.EtudiantId = null;
                    }
                }
                applicationDbContext.Utilisateurs.Remove(etudiant);
                applicationDbContext.SaveChanges();
            }

            return RedirectToAction("RechSuppModEtudiant");
        }

        [HttpGet]
        public async Task<JsonResult> FiltrerEtudiant(string filiere, string nv)
        {
            if (filiere == "tout" && nv == "tout")
            {
                var users = await userManager.GetUsersInRoleAsync("etudiant");
                return Json(users);
            }
            else if (filiere == "tout" && nv != "tout")
            {
                var users = applicationDbContext.Utilisateurs.Where(x => x.Niveau == nv).ToList();
                return Json(users);
            }
            else if (filiere != "tout" && nv == "tout")
            {
                var users = applicationDbContext.Utilisateurs.Where(x => x.Filiere == filiere).ToList();
                return Json(users);
            }
            else
            {
                var users = applicationDbContext.Utilisateurs.Where(x => x.Filiere == filiere && x.Niveau == nv).ToList();
                return Json(users);
            }
        }

        public IActionResult ModifyEnseignant(string enseigId)
        {
            ViewBag.Ltype = "admin";
            var enseignant = applicationDbContext.Utilisateurs.Find(enseigId);
            if (enseignant != null)
            {
                ModifierEnseignantViewModel model = new ModifierEnseignantViewModel()
                {
                    Departement = enseignant.Departement,
                    Email = enseignant.Email,
                    Id = enseignant.Id,
                    Nom = enseignant.Nom,
                    Prenom = enseignant.Prenom
                };
                return View(model);
            }
            return RedirectToAction("RechSuppModEnseignant");
        }
        [HttpPost]
        public IActionResult ModifyEnseignant(ModifierEnseignantViewModel model)
        {
            var enseignant = applicationDbContext.Utilisateurs.Find(model.Id);
            if (enseignant != null)
            {
                enseignant.Email = model.Email;
                enseignant.UserName = model.Email;
                enseignant.NormalizedEmail = model.Email.ToUpper();
                enseignant.Departement = model.Departement;
                enseignant.Prenom = model.Prenom;
                enseignant.Nom = model.Nom;
                applicationDbContext.SaveChanges();
            }


            return RedirectToAction("RechSuppModEnseignant");
        }
        public IActionResult ModifyEtud(string etudId)
        {
            ViewBag.Ltype = "admin";
            var etudiant = applicationDbContext.Utilisateurs.Find(etudId);
            if (etudiant != null)
            {
                ModifierEtudiantViewModel model = new ModifierEtudiantViewModel()
                {
                    Filiere = etudiant.Filiere,
                    Email = etudiant.Email,
                    Niveau = etudiant.Niveau,
                    CNE = etudiant.CNE,
                    Id = etudiant.Id,
                    Nom = etudiant.Nom,
                    Prenom = etudiant.Prenom
                };
                return View(model);
            }
            return RedirectToAction("RechSuppModEtudiant");
        }
        [HttpPost]
        public IActionResult ModifyEtud(ModifierEtudiantViewModel model)
        {
            var etudiant = applicationDbContext.Utilisateurs.Find(model.Id);
            if (etudiant != null)
            {
                etudiant.Email = model.Email;
                etudiant.UserName = model.Email;
                etudiant.NormalizedEmail = model.Email.ToUpper();
                etudiant.Filiere = model.Filiere;
                etudiant.CNE = model.CNE;
                etudiant.Prenom = model.Prenom;
                etudiant.Nom = model.Nom;
                applicationDbContext.SaveChanges();
            }


            return RedirectToAction("RechSuppModEtudiant");
        }
        [HttpPost]
        public IActionResult DeleteMessage(int Id)
        {
            // Delete the selected messages from the database

            var message = applicationDbContext.Messages.Find(Id);
            applicationDbContext.Messages.Remove(message);

            applicationDbContext.SaveChanges();

            // Redirect to the inbox page
            return RedirectToAction("Messages");
        }
        public IActionResult ReadMessage(int currentMsgId)
        {
            ViewBag.Ltype = "admin";
            List<Message> msg = applicationDbContext.Messages.ToList();
            if (msg != null && msg.Count != 0)
            {

                Message m = msg.Find(x => x.Id == currentMsgId);
                if (m != null)
                {
                    if (msg.IndexOf(m) != msg.Count - 1 && msg.IndexOf(m) != 0)
                    {
                        int prev = msg[msg.IndexOf(m) - 1].Id;
                        ViewBag.prev = prev;
                        int suiv = msg[msg.IndexOf(m) + 1].Id;
                        ViewBag.suiv = suiv;

                    }
                    else if (msg.IndexOf(m) == 0)
                    {
                        int suiv = msg[msg.IndexOf(m) + 1].Id;
                        ViewBag.suiv = suiv;
                        ViewBag.prev = null;
                    }
                    else if (msg.IndexOf(m) == (msg.Count - 1))
                    {
                        int prev = msg[msg.IndexOf(m) - 1].Id;
                        ViewBag.prev = prev;
                        ViewBag.suiv = null;
                    }
                    return View(m);
                }
            }

            return RedirectToAction("Messages");
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
        public string FirstPhrase(string msg)
        {
            if (msg.Length < 45)
            {
                return msg;
            }
            int l = 0;
            StringBuilder stringBuilder = new StringBuilder();
            while (l < 46)
            {
                stringBuilder.Append(msg[l]);
                l++;
            }
            return stringBuilder.ToString();
        }

        [HttpGet]
        public async Task<JsonResult> FindByDepartement(string op)
        {
            if (op != "tout")
            {
                var users = applicationDbContext.Utilisateurs.Where(x => x.Departement == op).ToList();
                return Json(users);
            }
            else
            {
                var users = await userManager.GetUsersInRoleAsync("enseignant");
                return Json(users);
            }
        }
    
        public IActionResult RechercherRapport()
        {
            ViewBag.Ltype = "admin";
            var rapport = applicationDbContext.Rapports.ToList();
            List<GestionRapportViewModel> gr = new List<GestionRapportViewModel>();

            if (rapport.Count() != 0)
            {
                foreach (Rapport item in rapport)
                {
                    if(item.EtudiantId != null)
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
        public ActionResult DownloadPdf(int id)
        {
            var pdf = applicationDbContext.Rapports.Find(id);
            if (pdf == null)
            {
                return RedirectToAction("RechercherRapport");
            }
            return File(pdf.data, "application/pdf", pdf.Intitulé + ".pdf");
        }
        public ActionResult DeleteSelectedRapports(string[] rapportIds)
        {
            foreach (string id in rapportIds)
            {
                applicationDbContext.Rapports.Remove(applicationDbContext.Rapports.Find(int.Parse(id)));
                applicationDbContext.SaveChanges();
            }
            return RedirectToAction("RechercherRapport");
        }
        public double plagiatAuto(int id)
        {
            PlagiarismDetection aP = new PlagiarismDetection(applicationDbContext);
            double poucentagePlagiat = aP.Plagiat(id);

            return poucentagePlagiat * 100;
        }
        
        public async Task<JsonResult> fitrageRapport1(string type,string niveau,string filiere)
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
                        model.dateModif = ""+model.rapport.DateModif;
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
            if(rapport1.Count() == 0)
            {
                var encadre = applicationDbContext.Encadre
                                .Where(x =>
                                    x.Enseignant.Nom.Trim().ToLower().Contains(input) ||
                                    x.Enseignant.Prenom.Trim().ToLower().Contains(input) ||
                                    x.Enseignant.Departement.Trim().ToLower().Contains(input) ||
                                    x.Enseignant.Email.Trim().ToLower().Contains(input)
                                ).ToList();

                foreach( var item in encadre)
                {
                    var r = applicationDbContext.Rapports.Where(x => x.EtudiantId == item.EtudiantId).FirstOrDefault();
                    if(r != null)
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
        
        public ActionResult PourcentagePlagiat()
        {
            ViewBag.Ltype = "admin";
            var pp = applicationDbContext.pourcentagePlagiats.FirstOrDefault();
            return View(pp);
        }

        [HttpPost]
        public ActionResult PourcentagePlagiat(PourcentagePlagiat model)
        {
            ViewBag.Ltype = "admin";
            if (ModelState.IsValid)
            {
                model.Pourcentage = model.Pourcentage / 100;
                var p = applicationDbContext.pourcentagePlagiats.Find(model.id);
                if (p != null)
                {
                    p.Pourcentage = model.Pourcentage;
                    applicationDbContext.SaveChanges();
                }
            }
            
            return View(model);
        }
    }

}