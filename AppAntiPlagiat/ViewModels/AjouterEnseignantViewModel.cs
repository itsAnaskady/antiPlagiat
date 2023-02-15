using System.ComponentModel.DataAnnotations;

namespace AppAntiPlagiat.ViewModels
{
    public class AjouterEnseignantViewModel
    {
        [Required(ErrorMessage = "Le nom est requis.",AllowEmptyStrings =false)]
        public string Nom { set; get; }

        [Required(ErrorMessage = "Le prénom est requis.", AllowEmptyStrings =false)]
        public string Prenom { set; get; }

        [Required(ErrorMessage = "L'Email est requis.", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "")]
        public string Email { set; get; }

        [Required(ErrorMessage = "Le mot de passe est requis.", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { set; get; }

        [Compare("Password",ErrorMessage ="Ce champs n'est pas identique au mdp.")]
        [Required(ErrorMessage = "La confirmation du mot de passe est requise.", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string RetryPassword { set; get; }

        [Required(ErrorMessage = "Selectinner un département.", AllowEmptyStrings = false)]
        public string Departement { set; get; }


    }
}

