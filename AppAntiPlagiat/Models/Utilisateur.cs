using Microsoft.AspNetCore.Identity;

namespace AppAntiPlagiat.Models
{
    public class Utilisateur : IdentityUser
    {
        public string Nom { get; set; }

        public string Prenom { get; set; }

        public string? CNE { get; set; }

        public string? IMGurl { get; set; }

        public string? Niveau { get; set; }
        
        public string? Departement { get; set; }

        public string? Filiere { get; set; }
        public string NomComplet { get { return Nom + " " + Prenom; } }
        public ICollection<Rapport> Rapports { get; set; }
        public ICollection<Encadre> Encadres { get; set; }
    }
}
