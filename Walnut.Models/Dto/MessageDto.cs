using ENDE.Models.User;

namespace Walnut.Models.Dto
{
    public class MessageDto
    {
        public string SenderUsername { get; set; } = string.Empty;
        public string ReceiverUsername { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
