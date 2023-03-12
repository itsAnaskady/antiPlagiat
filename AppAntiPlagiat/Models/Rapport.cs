using System.ComponentModel.DataAnnotations.Schema;

namespace AppAntiPlagiat.Models
{
    public class Rapport
    {
        public int Id { get; set; }
        public string Intitulé { get; set; }

        public string Type { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }

        public bool Validé { get; set; }
        public string Chemin { get; set; }
        public string EtudiantId { get; set; }
        public DateTime DateDepot;
      public Utilisateur Etudiant { get; set; }
        public Utilisateur Enseignant { get; set; }
       // public string EnseignantID { get; set; }
       
    }
}
