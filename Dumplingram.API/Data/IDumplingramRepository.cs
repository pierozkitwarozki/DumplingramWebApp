using System.Collections.Generic;
using System.Threading.Tasks;
using Dumplingram.API.Models;
using Dumplingram.API.Helpers;

namespace Dumplingram.API.Data
{
    public interface IDumplingramRepository
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

         // Photos
         Task<IEnumerable<Photo>> GetPhotosAsync(int id); 
         Task<Photo> GetPhotoAsync(int id);
         Task<IEnumerable<PhotoLike>> GetPhotoLikesAsync(int id);
         Task<PhotoLike> GetPhotoLikeAsync(int id, int userId);
         Task<Photo> GetMainPhotoForUserAsync(int userId);
         Task<IEnumerable<Photo>> GetPhotosForUserAsync(int id);

         //Messages
         Task<Message> GetMessageAsync(int id);
         Task<IEnumerable<Message>> GetMessagesForUserAsync(int id);
         Task<IEnumerable<Message>> GetMessageThreadAsync(int currentUserId, int recipientId);

         //
         Task<Connection> GetConnectionAsync(string connectionId);
         Task<IEnumerable<Connection>> GetConnectionsAsync(string userId);
         Task<Group> GetGroupAsync(string name);

    }
}