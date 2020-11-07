using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dumplingram.API.Models;
using Microsoft.EntityFrameworkCore;
using Dumplingram.API.Helpers;

namespace Dumplingram.API.Data
{
    public class DumplingramRepository : IDumplingramRepository
    {
        private readonly DataContxt _context;
        public DumplingramRepository(DataContxt context)
        {
            _context = context;
        }

        public async Task Add<T>(T entity) where T : class
        {
            await _context.AddAsync(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(u => u.ID == id);

            return user;
        }

        public async Task<IEnumerable<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(p => p.Photos)
                .OrderBy(u => u.Username).AsQueryable();

            users = users.Where(u => u.ID != userParams.UserId);

            return await users.ToListAsync<User>();
        }

        public Task<bool> SaveAll()
        {
            throw new System.NotImplementedException();
        }
    }
}