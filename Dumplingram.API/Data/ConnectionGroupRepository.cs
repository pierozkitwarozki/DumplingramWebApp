using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dumplingram.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dumplingram.API.Data
{
    public class ConnectionGroupRepository : IConnectionGroupRepository
    {
        private readonly DataContxt _context;
        public ConnectionGroupRepository(DataContxt context)
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
  
        public async Task<Connection> GetConnectionAsync(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<IEnumerable<Connection>> GetConnectionsAsync(string userId)
        {
            return await _context.Connections.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<Group> GetGroupAsync(string name)
        {
            return await _context.Groups.Include(x => x.Connections).FirstOrDefaultAsync(x => x.Name == name);
        }
    }
}