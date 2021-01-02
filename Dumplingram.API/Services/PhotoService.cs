using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Dumplingram.API.Data;
using Dumplingram.API.Dtos;
using Dumplingram.API.Helpers;
using Dumplingram.API.Models;
using Microsoft.Extensions.Options;

namespace Dumplingram.API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IUserRepository _usersRepo;
        private readonly IPhotoRepository _photoRepo;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;
        private readonly IMapper _mapper;

        public PhotoService(IPhotoRepository photoRepo,
            IUserRepository userRepo,IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _usersRepo = userRepo;
            _photoRepo = photoRepo;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<IEnumerable<PhotoLikeDto>> GetPhotoLikesAsync(int id)
        {
            var likes = await _photoRepo.GetPhotoLikesAsync(id);

            var likesToReturn = _mapper.Map<IEnumerable<PhotoLikeDto>>(likes);

            return likesToReturn;
        }

        public async Task<IAsyncResult> LikePhotoAsync(int userId, int id)
        {
            var photoLike = new PhotoLike
            {
                UserId = userId,
                PhotoId = id
            };

            if (await _photoRepo.GetPhotoAsync(photoLike.PhotoId) == null)
                throw new Exception("Zdjęcie nie istnieje.");

            if (await _photoRepo.GetPhotoLikeAsync(id, userId) != null)
                throw new Exception("Już lubisz to zdjęcie");

            await _photoRepo.AddAsync(photoLike);

            if (await _photoRepo.SaveAllAsync())
                return Task.CompletedTask;

            throw new Exception("Coś poszło nie tak");
        }

        public async Task<IEnumerable<PhotoForDashboardDto>> GetPhotosAsync(int currentUserId)
        {
            var photos = await _photoRepo.GetPhotosAsync(currentUserId);

            var photosToReturn = _mapper.Map<IEnumerable<PhotoForDashboardDto>>(photos);

            return photosToReturn;
        }

        public async Task<PhotoLike> GetPhotoLikeAsync(int photoId, int userId)
        {
            var like = await _photoRepo.GetPhotoLikeAsync(photoId, userId);
            return like;
        }

        public async Task<IAsyncResult> UnlikePhotoAsync(int id, int userId)
        {

            var like = await _photoRepo.GetPhotoLikeAsync(id, userId);

            if (like == null)
                throw new Exception("Nie lubisz tego zdjęcia");

            _photoRepo.Delete<PhotoLike>(like);

            if (await _photoRepo.SaveAllAsync())
                return Task.CompletedTask;

            throw new Exception("Coś poszło nie tak.");
        }

        public async Task<PhotoForReturnDto> GetPhotoAsync(int id)
        {
            var photoFromRepo = await _photoRepo.GetPhotoAsync(id);

            if (photoFromRepo == null)
                throw new Exception("Nie ma takiego zdjęcia");

            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return photo;
        }

        public async Task<PhotoForReturnDto> AddPhotoForUserAsync(int userId,
           PhotoForCreationDto photoForCreationDto)
        {
            var userFromRepo = await _usersRepo.GetUserAsync(userId);

            var file = photoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            var photoId = ("user_" + userFromRepo.ID + "-").GeneratePublicId();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation()
                            .Width(500).Height(500).Crop("fill").Gravity("face"),
                        PublicId = photoId
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDto.Url = uploadResult.Url.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);

            if (!userFromRepo.Photos.Any(u => u.IsMain))
            {
                photo.IsMain = true;
            }

            userFromRepo.Photos.Add(photo);

            if (await _usersRepo.SaveAllAsync())
            {
                var photoForReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return photoForReturn;
            }

            throw new Exception("Coś poszło nie tak.");
        }

        public async Task<IEnumerable<PhotoForDashboardDto>> GetPhotosForUserAsync(int userId)
        {
            var photos = await _photoRepo.GetPhotosForUserAsync(userId);

            var photosToReturn = _mapper.Map<IEnumerable<PhotoForDashboardDto>>(photos);

            return photosToReturn;
        }

        public async Task<IAsyncResult> DeletePhotoAsync(int userId, int id)
        {

            var userFromRepo = await _usersRepo.GetUserAsync(userId);

            if (!userFromRepo.Photos.Any(p => p.ID == id))
                throw new Exception();

            var photoFromRepo = await _photoRepo.GetPhotoAsync(id);

            if (photoFromRepo.IsMain)
                throw new Exception("Nie możesz usunąć zdjęcia głównego.");

            if (!string.IsNullOrEmpty(photoFromRepo.PublicId))
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);

                var result = await _cloudinary.DestroyAsync(deleteParams);

                if (result.Result == "ok")
                    _photoRepo.Delete(photoFromRepo);
            }
            else _photoRepo.Delete(photoFromRepo);

            if (await _photoRepo.SaveAllAsync())
                return Task.CompletedTask;

            throw new Exception("Coś poszło nie tak.");
        }

        public async Task<IAsyncResult> SetMainPhotoAsync(int userId, int id)
        {
            var userFromRepo = await _usersRepo.GetUserAsync(userId);

            if (!userFromRepo.Photos.Any(p => p.ID == id))
                throw new Exception();

            var photoFromRepo = await _photoRepo.GetPhotoAsync(id);

            if (photoFromRepo.IsMain)
                throw new Exception("To zdjęcie jest już zdjęciem głównym.");

            var currentMainPhoto = await _photoRepo.GetMainPhotoForUserAsync(userId);

            currentMainPhoto.IsMain = false;
            photoFromRepo.IsMain = true;

            if (await _photoRepo.SaveAllAsync())
                return Task.CompletedTask;

            throw new Exception("Coś poszło nie tak.");
        }

        public async Task<IAsyncResult> DeleteCommentAsync(int commentId)
        {
            var comment = await _photoRepo.GetPhotoCommentAsync(commentId);

            if(comment == null) throw new Exception("Nie znaleziono.");

            _photoRepo.Delete(comment);

            if(await _photoRepo.SaveAllAsync()) return Task.CompletedTask;

            throw new Exception("Coś poszło nie tak.");
        }

        public async Task<IAsyncResult> AddCommentAsync(CommentForAddDto commentForAddDto)
        {
            var comment = _mapper.Map<PhotoComment>(commentForAddDto);

            await _photoRepo.AddAsync(comment);

            if(await _photoRepo.SaveAllAsync()) return Task.CompletedTask;

            throw new Exception("Coś poszło nie tak.");
        }

        public async Task<IEnumerable<CommentForReturnDto>> GetCommentsForPhoto(int photoId)
        {
            var comments = await _photoRepo.GetCommentsForPhotosAsync(photoId);

            if (comments == null) throw new Exception("Nie znaleziono.");

            var commentsToReturn = _mapper.Map<IEnumerable<CommentForReturnDto>>(comments);

            return commentsToReturn;
        }
    }
}