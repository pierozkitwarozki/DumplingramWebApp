using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dumplingram.API.Dtos;
using Dumplingram.API.Models;

namespace Dumplingram.API.Services
{
    public interface IPhotoService
    {
         Task<IEnumerable<PhotoLikeDto>> GetPhotoLikes(int id);
         Task<IAsyncResult> LikePhoto(int userId, int id);
         Task<IEnumerable<PhotoForDashboardDto>> GetPhotos(int currentUserId);
         Task<PhotoLike> GetPhotoLike(int photoId, int userId);
         Task<IAsyncResult> UnlikePhoto(int id, int userId);
         Task<PhotoForReturnDto> GetPhoto(int id);
         Task<PhotoForReturnDto> AddPhotoForUser(int userId, PhotoForCreationDto photoForCreationDto);
         Task<IEnumerable<PhotoForDashboardDto>> GetPhotosForUser(int userId);
         Task<IAsyncResult> DeletePhoto(int userId, int id);
         Task<IAsyncResult> SetMainPhoto(int userId, int id);
    }
}