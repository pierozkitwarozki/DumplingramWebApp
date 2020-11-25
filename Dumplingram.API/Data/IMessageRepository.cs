using System.Collections.Generic;
using System.Threading.Tasks;
using Dumplingram.API.Models;

namespace Dumplingram.API.Data
{
    public interface IMessageRepository
    {
         //Messages
         Task AddAsync<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         Task<bool> SaveAllAsync();
         Task<Message> GetMessageAsync(int id);
         Task<IEnumerable<Message>> GetMessagesForUserAsync(int id);
         Task<IEnumerable<Message>> GetMessageThreadAsync(int currentUserId, int recipientId);
    }
}