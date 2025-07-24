using ENDE.Data;
using ENDE.Models.User;
using Microsoft.EntityFrameworkCore;
using Walnut.Models;

namespace Walnut.Data.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _dataContext;

        public MessageRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Message?> AddMessageAsync(User sender, User receiver, string content)
        {
            Message message = new Message
            {
                SenderId = sender.Id,
                Sender = sender,
                ReceiverId = receiver.Id,
                Receiver = receiver,
                Content = content,
                Date = DateTime.UtcNow
            };
            _dataContext.Messages.Add(message);
            await _dataContext.SaveChangesAsync();
            return message;
        }

        public async Task<List<Message>> GetAllMessages(Guid senderId, Guid receiverId)
        {
            return await _dataContext.Messages
                .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                            (m.SenderId == receiverId && m.ReceiverId == senderId))
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToListAsync();
        }
    }
}
