namespace Walnut.Models.Dto
{
    public class MessageSendRequestDto
    {
        public string ReceiverUsername { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
