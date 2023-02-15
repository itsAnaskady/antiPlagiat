using System.ComponentModel.DataAnnotations;

namespace AppAntiPlagiat.ViewModels
{
    public class ContactViewModel
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        public string Subject { get; set; }
        [Required]
        [MaxLength(350)]
        public string Message { get; set; }
    }
}
