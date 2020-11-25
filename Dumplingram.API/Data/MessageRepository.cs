using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dumplingram.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dumplingram.API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContxt _context;
        public MessageRepository(DataContxt context)
        {
            _context = context;
        }

        public async Task AddAsync<T>(T entity) where T : class
        {
            await _context.AddAsync(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0; //if eq 0 then successful, otherwise no changes
        }

        public async Task<Message> GetMessageAsync(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<IEnumerable<Message>> GetMessagesForUserAsync(int id)
        {
            var messages = await _context.Messages
                .Where(x => x.RecipientId == id || x.SenderId == id).Include(p => p.Sender)
                .ThenInclude(p => p.Photos).Include(p => p.Recipient).ThenInclude(p => p.Photos)
                .OrderByDescending(x => x.MessageSent).ToListAsync();

            int otherUserId = 0;
            var listToReturn = new List<Message>();

            foreach (var message in messages)
            {
                if (message.RecipientId == id) otherUserId = message.SenderId;
                else otherUserId = message.RecipientId;

                if (listToReturn.FirstOrDefault(x => (x.RecipientId == otherUserId || x.SenderId == otherUserId)) != null)
                    continue;

                listToReturn.Add(message);
            }

            return listToReturn;
        }

        public async Task<IEnumerable<Message>> GetMessageThreadAsync(int currentUserId, int recipientId)
        {
            var messages = await _context.Messages.Where(x => (x.RecipientId == currentUserId && x.SenderId == recipientId)
            || (x.SenderId == currentUserId && x.RecipientId == recipientId)).ToListAsync();

            return messages.OrderBy(x => x.MessageSent);
        }
    }
}