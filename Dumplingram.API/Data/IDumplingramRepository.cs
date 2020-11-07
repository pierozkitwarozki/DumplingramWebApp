using System.Collections.Generic;
using System.Threading.Tasks;
using Dumplingram.API.Models;
using Dumplingram.API.Helpers;

namespace Dumplingram.API.Data
{
    public interface IDumplingramRepository
    {
         Task Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         Task<bool> SaveAll();
         Task<IEnumerable<User>> GetUsers(UserParams userParams);
         Task<User> GetUser(int id);
    }
}