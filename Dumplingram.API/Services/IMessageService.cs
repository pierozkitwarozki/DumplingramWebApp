using System.Collections.Generic;
using System.Threading.Tasks;
using Dumplingram.API.Dtos;

namespace Dumplingram.API.Services
{
    public interface IMessageService
    {
         Task<IEnumerable<MessageDto>> GetMessagesForUserAsync(int id);
    }
}