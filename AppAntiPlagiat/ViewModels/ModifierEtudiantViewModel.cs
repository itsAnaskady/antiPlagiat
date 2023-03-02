using System.ComponentModel.DataAnnotations;

namespace AppAntiPlagiat.ViewModels
{
    public class ModifierEtudiantViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Le nom est requis.", AllowEmptyStrings = false)]
        public string Nom { set; get; }

        [Required(ErrorMessage = "Le prénom est requis.", AllowEmptyStrings = false)]
        public string Prenom { set; get; }
        [Required(ErrorMessage = "Le CNE est requis.", AllowEmptyStrings = false)]
        public string? CNE { set; get; }

        [Required(ErrorMessage = "L'Email est requis.", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "")]
        public string? Email { set; get; }

        [Required(ErrorMessage = "Selectinner une filière.", AllowEmptyStrings = false)]
        public string? Filiere { set; get; }
        [Required(ErrorMessage = "Selectinner un niveau.", AllowEmptyStrings = false)]
        public string? Niveau { set; get; }
    }
}
