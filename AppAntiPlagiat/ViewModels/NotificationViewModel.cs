using AppAntiPlagiat.Models;

namespace AppAntiPlagiat.ViewModels
{
    public class NotificationViewModel
    {
        public Notification notification { get; set; }
        public Utilisateur emetteur { get; set; }
        public string tempPasse { get; set; }
    }
}
