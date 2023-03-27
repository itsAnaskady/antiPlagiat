namespace AppAntiPlagiat.Models
{
    public class Notification
    {
        public int id { get; set; }
        public string message { get; set; }
        public bool Vu { get; set; }

        public DateTime DateNotif { get; set; }

        public string UserId { get; set; }
        public Utilisateur User { get; set; }
        public string UserIdDesti { get; set; }
    }
}
