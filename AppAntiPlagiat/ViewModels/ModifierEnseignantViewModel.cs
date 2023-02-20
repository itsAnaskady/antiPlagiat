using System.ComponentModel.DataAnnotations;

namespace AppAntiPlagiat.ViewModels
{
    public class ModifierEnseignantViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Le nom est requis.", AllowEmptyStrings = false)]
        public string Nom { set; get; }

        [Required(ErrorMessage = "Le prénom est requis.", AllowEmptyStrings = false)]
        public string Prenom { set; get; }

        [Required(ErrorMessage = "L'Email est requis.", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "")]
        public string? Email { set; get; }

        [Required(ErrorMessage = "Selectinner un département.", AllowEmptyStrings = false)]
        public string? Departement { set; get; }
    }
}
