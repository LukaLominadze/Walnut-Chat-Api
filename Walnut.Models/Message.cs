using ENDE.Models.User;

namespace Walnut.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public User? Sender { get; set; }
        public Guid ReceiverId { get; set; }
        public User? Receiver { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
