using System.Collections.Generic;
using System.Threading.Tasks;
using Dumplingram.API.Helpers;
using Dumplingram.API.Models;

namespace Dumplingram.API.Data
{
    public interface IUserRepository
    {
         Task AddAsync<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         Task<bool> SaveAllAsync();

         // Users
         Task<IEnumerable<User>> GetUsersAsync(UserParams userParams);
         Task<User> GetUserAsync(int id);

         //Follows
         Task<Follow> GetFollowAsync(int id, int followeeId);
         Task<IEnumerable<Follow>> GetFollowersAsync(int id);
         Task<IEnumerable<Follow>> GetFolloweesAsync(int id);
    }
}