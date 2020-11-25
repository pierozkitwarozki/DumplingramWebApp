using System.Collections.Generic;
using System.Threading.Tasks;
using Dumplingram.API.Models;

namespace Dumplingram.API.Data
{
    public interface IConnectionGroupRepository
    {
        Task AddAsync<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAllAsync();
        //
        Task<Connection> GetConnectionAsync(string connectionId);
        Task<IEnumerable<Connection>> GetConnectionsAsync(string userId);
        Task<Group> GetGroupAsync(string name);
    }
}