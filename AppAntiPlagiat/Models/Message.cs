namespace AppAntiPlagiat.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Emetteur { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Msg { get; set; }
        public DateTime dateEnvoie { get; set; }

    }
}
