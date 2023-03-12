namespace AppAntiPlagiat.Models
{
    public class Encadre
    {
        public int Id { get; set; }
        public string EtudiantId { get; set; }
        public Utilisateur Etudiant { get; set; }
        public string EnseignantId { get; set; }
        public Utilisateur Enseignant { get; set; }
        public string TypeStage { get; set; }
        
    }
}
