using System.Collections.Generic;
using System.Threading.Tasks;
using Dumplingram.API.Models;

namespace Dumplingram.API.Data
{
    public interface IPhotoRepository
    {
         Task AddAsync<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         Task<bool> SaveAllAsync();

         // Photos
         Task<IEnumerable<Photo>> GetPhotosAsync(int id); 
         Task<Photo> GetPhotoAsync(int id);
         Task<IEnumerable<PhotoLike>> GetPhotoLikesAsync(int id);
         Task<PhotoLike> GetPhotoLikeAsync(int id, int userId);
         Task<Photo> GetMainPhotoForUserAsync(int userId);
         Task<IEnumerable<Photo>> GetPhotosForUserAsync(int id);
         Task<IEnumerable<PhotoComment>> GetCommentsForPhotosAsync(int photoId);
         Task<PhotoComment> GetPhotoCommentAsync(int photoId);
    }
}