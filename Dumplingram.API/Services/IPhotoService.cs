using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dumplingram.API.Dtos;
using Dumplingram.API.Models;

namespace Dumplingram.API.Services
{
    public interface IPhotoService
    {
         Task<IEnumerable<PhotoLikeDto>> GetPhotoLikesAsync(int id);
         Task<IAsyncResult> LikePhotoAsync(int userId, int id);
         Task<IEnumerable<PhotoForDashboardDto>> GetPhotosAsync(int currentUserId);
         Task<PhotoLike> GetPhotoLikeAsync(int photoId, int userId);
         Task<IAsyncResult> UnlikePhotoAsync(int id, int userId);
         Task<PhotoForReturnDto> GetPhotoAsync(int id);
         Task<PhotoForReturnDto> AddPhotoForUserAsync(int userId, PhotoForCreationDto photoForCreationDto);
         Task<IEnumerable<PhotoForDashboardDto>> GetPhotosForUserAsync(int userId);
         Task<IAsyncResult> DeletePhotoAsync(int userId, int id);
         Task<IAsyncResult> SetMainPhotoAsync(int userId, int id);
         Task<IAsyncResult> DeleteCommentAsync(int commentId);
         Task<IAsyncResult> AddCommentAsync(CommentForAddDto commentForAddDto);
         Task<IEnumerable<CommentForReturnDto>> GetCommentsForPhoto(int photoId);
         
    }
}