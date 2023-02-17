using System.ComponentModel.DataAnnotations;

namespace AppAntiPlagiat.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Le nom est requis.")]
        public string Nom { get; set; }
        [Required(ErrorMessage = "Le prénom est requis.")]
        public string Prenom { get; set; }
        [Required(ErrorMessage = "L'email est requis.")]
        [EmailAddress(ErrorMessage = "Veuillez saisir un email valide.")]
        public string Email { get; set; }
        
        public string Subject { get; set; }
        [Required(ErrorMessage = "Veuillez écrire votre message.")]
        [MaxLength(350,ErrorMessage ="Le message ne pas dépasser 350 caractères.")]
        public string Message { get; set; }
    }
}
