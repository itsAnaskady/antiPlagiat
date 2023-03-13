using System.ComponentModel.DataAnnotations.Schema;

namespace AppAntiPlagiat.Models
{
    public class Rapport
    {
        public int Id { get; set; }
        public string Intitulé { get; set; }

        public string Type { get; set; }

        public bool Validé { get; set; }
        public byte[] data { get; set; }

        public DateTime DateDepot { get; set; }
        public string EtudiantId { get; set; }
        public Utilisateur Etudiant { get; set; }



    }
}
