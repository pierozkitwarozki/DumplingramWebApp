using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dumplingram.API.Helpers;
using Dumplingram.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dumplingram.API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContxt _context;
        public UserRepository(DataContxt context)
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


        public async Task<User> GetUserAsync(int id)
        {
            var user = await _context.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(u => u.ID == id);
            user.Photos = user.Photos.OrderByDescending(x => x.DateAdded).ToList();
            return user;
        }

        public async Task<IEnumerable<User>> GetUsersAsync(UserParams userParams)
        {
            var users = _context.Users.Include(p => p.Photos)
                .OrderBy(u => u.Username).Where(u => u.ID != userParams.UserId).AsQueryable();

            if (!string.IsNullOrEmpty(userParams.Word))
            {
                users = users.Where(u => (u.Name.ToLower().Contains(userParams.Word.ToLower()))
                    || u.Surname.ToLower().Contains(userParams.Word.ToLower())
                    || u.Username.ToLower().Contains(userParams.Word.ToLower()));
            }

            return await users.ToListAsync<User>();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0; //if eq 0 then successful, otherwise no changes
        }

        public async Task<Follow> GetFollowAsync(int id, int followeeId)
        {
            return await _context.Follow
                .FirstOrDefaultAsync(u =>
                    u.FollowerId == id && u.FolloweeId == followeeId);
        }

        public async Task<IEnumerable<Follow>> GetFollowersAsync(int id)
        {
            var list = await _context.Follow
                .Include(u => u.Follower).ThenInclude(p => p.Photos).Where(f => f.FolloweeId == id).ToListAsync();

            return list;
        }

        public async Task<IEnumerable<Follow>> GetFolloweesAsync(int id)
        {
            var list = await _context.Follow
                .Include(u => u.Followee).ThenInclude(p => p.Photos).Where(f => f.FollowerId == id).ToListAsync();
            return list;
        }
    }
}