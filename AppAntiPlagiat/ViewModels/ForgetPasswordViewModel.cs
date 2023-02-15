using System.ComponentModel.DataAnnotations;

namespace AppAntiPlagiat.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}
