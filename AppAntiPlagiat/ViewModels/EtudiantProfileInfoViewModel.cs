using System.ComponentModel.DataAnnotations;

namespace AppAntiPlagiat.ViewModels
{
    public class EtudiantProfileInfoViewModel
    {
        public string Nom { set; get; }

        public string Prenom { set; get; }
        
        public string CNE { set; get; }
        public string IMGurl { set; get; }
        public string Email { set; get; }

        public string Filiere { set; get; }
        public string Niveau { set; get; }
    }
}
