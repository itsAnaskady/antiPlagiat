using AppAntiPlagiat.Models;
using AppAntiPlagiat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AppAntiPlagiat.Controllers
{

    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<Utilisateur> userManager;
        private readonly ApplicationDbContext applicationDbContext;

        public AdminController(UserManager<Utilisateur> userManager,ApplicationDbContext applicationDbContext)
        {
            this.userManager = userManager;
            this.applicationDbContext = applicationDbContext;
        }
        
        public IActionResult Dashboard()
        {
            ViewBag.Ltype = "admin";
            return View();
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
                Utilisateur nvEnseignant = new Utilisateur
                {
                    Email = model.Email,
                    Nom = model.Nom,
                    Prenom = model.Prenom,
                    UserName = model.Email,
                    NormalizedUserName = model.Email.ToUpper(),
                    NormalizedEmail = model.Email.ToUpper(),
                    Departement = model.Departement,
                    IMGurl = "~/images/Users/NewUser.png"
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
                foreach(var error in result.Errors)
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
                Utilisateur nvEtudiant = new Utilisateur
                {
                    Email = model.Email,
                    Nom = model.Nom,
                    Prenom = model.Prenom,
                    UserName = model.Email,
                    NormalizedUserName = model.Email.ToUpper(),
                    NormalizedEmail = model.Email.ToUpper(),
                    Niveau = model.Niveau,
                    IMGurl = "~/images/Users/NewUser.png",
                    Filiere= model.Filiere,
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
        public IActionResult Affectation()
        {
            ViewBag.Ltype = "admin";
            return View();
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
                    var etudiant = applicationDbContext.Utilisateurs.Find(Id);
                    applicationDbContext.Utilisateurs.Remove(etudiant);
                }
                applicationDbContext.SaveChanges();
            }

            // Redirect to the inbox page
            return RedirectToAction("RechSuppModEtudiant");
        }
        [HttpPost]
        public IActionResult DeleteSelectedEnseignants(string[] enseignantIds)
        {
            // Delete the selected messages from the database
            if (enseignantIds.Length != 0) {
                foreach (var Id in enseignantIds)
                {
                    var enseignant = applicationDbContext.Utilisateurs.Find(Id);
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
            if(enseignant != null) {
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
                applicationDbContext.Utilisateurs.Remove(etudiant);
                applicationDbContext.SaveChanges();
            }

            return RedirectToAction("RechSuppModEtudiant");
        }

        [HttpGet]
        public async Task<JsonResult> FiltrerEtudiant(string filiere , string nv)
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
            else {
                var users = applicationDbContext.Utilisateurs.Where(x => x.Filiere == filiere && x.Niveau == nv).ToList();
                return Json(users);
            }
        }

        public IActionResult ModifyEnseignant(string enseigId)
        {
            ViewBag.Ltype = "admin";
            var enseignant = applicationDbContext.Utilisateurs.Find(enseigId);
            if(enseignant != null)
            {
                ModifierEnseignantViewModel model = new ModifierEnseignantViewModel()
                {
                    Departement = enseignant.Departement,
                    Email = enseignant.Email,
                    Id= enseignant.Id,
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
            if(enseignant != null) {
                enseignant.Email = model.Email;
                enseignant.UserName = model.Email;
                enseignant.NormalizedEmail = model.Email.ToUpper();
                enseignant.Departement = model.Departement;
                enseignant.Prenom= model.Prenom;
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
            if (msg != null && msg.Count!=0)
            {
                
                Message m = msg.Find(x => x.Id == currentMsgId);
                if (m != null) { 
                    if (msg.IndexOf(m) != msg.Count - 1 && msg.IndexOf(m) != 0)
                    {
                        int prev = msg[msg.IndexOf(m) - 1].Id;
                        ViewBag.prev = prev;
                        int suiv = msg[msg.IndexOf(m) + 1].Id;
                        ViewBag.suiv = suiv;
                   
                    }
                    else if(msg.IndexOf(m) == 0)
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
            if (op != "tout") {
                var users = applicationDbContext.Utilisateurs.Where(x => x.Departement == op).ToList();
                return Json(users);
            }
            else
            {
                var users = await userManager.GetUsersInRoleAsync("enseignant");
                return Json(users);
            }
        }
    }

}
