using ENDE.Models.User;
using Walnut.Models;

namespace Walnut.Data.Repositories
{
    public interface IMessageRepository
    {
        public Task<List<Message>> GetAllMessages(Guid senderId, Guid receiverId);
        public Task<Message?> AddMessageAsync(User sender, User receiver, string content);
    }
}
