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

         // Users
         Task<IEnumerable<User>> GetUsers(UserParams userParams);
         Task<User> GetUser(int id);

         //Follows
         Task<Follow> GetFollow(int id, int followeeId);
         Task<IEnumerable<Follow>> GetFollowers(int id);
         Task<IEnumerable<Follow>> GetFollowees(int id);

         // Photos
         Task<IEnumerable<Photo>> GetPhotos(int id);
         Task<Photo> GetPhoto(int id);
         Task<IEnumerable<PhotoLike>> GetPhotoLikes(int id);
         Task<PhotoLike> GetPhotoLike(int id, int userId);
         Task<Photo> GetMainPhotoForUser(int userId);

         //Messages
         Task<Message> GetMessage(int id);
         Task<IEnumerable<Message>> GetMessagesForUser(int id);
         Task<IEnumerable<Message>> GetMessageThread(int currentUserId, int recipientId);

         //
         Task<Connection> GetConnection(string connectionId);
         Task<IEnumerable<Connection>> GetConnections(string userId);
         Task<Group> GetGroup(string name);
    }
}