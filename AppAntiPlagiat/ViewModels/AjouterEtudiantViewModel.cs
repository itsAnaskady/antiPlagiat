using System.ComponentModel.DataAnnotations;

namespace AppAntiPlagiat.ViewModels
{
    public class AjouterEtudiantViewModel
    {
        [Required(ErrorMessage = "Le nom est requis.",AllowEmptyStrings =false)]
        public string Nom { set; get; }

        [Required(ErrorMessage = "Le prénom est requis.", AllowEmptyStrings =false)]
        public string Prenom { set; get; }
        [Required(ErrorMessage = "Le CNE est requis.", AllowEmptyStrings = false)]
        public string CNE { set; get; }

        [Required(ErrorMessage = "L'Email est requis.", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "Cette format n'est pas validée.")]
        public string Email { set; get; }

        [Required(ErrorMessage = "Le mot de passe est requis.", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { set; get; }

        [Compare("Password",ErrorMessage ="Ce champs n'est pas identique au mdp.")]
        [DataType(DataType.Password)]
        public string RetryPassword { set; get; }
        [Required(ErrorMessage = "Selectinner une Filière.", AllowEmptyStrings = false)]
        public string Filiere { set; get; }
        [Required(ErrorMessage = "Selectinner le niveau.", AllowEmptyStrings = false)]
        public string Niveau { set; get; }


    }
}

