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
        private readonly IDumplingramRepository _repo;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;
        private readonly IMapper _mapper;

        public PhotoService(IDumplingramRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _repo = repo;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<IEnumerable<PhotoLikeDto>> GetPhotoLikes(int id)
        {
            var likes = await _repo.GetPhotoLikes(id);

            var likesToReturn = _mapper.Map<IEnumerable<PhotoLikeDto>>(likes);

            return likesToReturn;
        }

        public async Task<IAsyncResult> LikePhoto(int userId, int id)
        {
            var photoLike = new PhotoLike
            {
                UserId = userId,
                PhotoId = id
            };

            if (await _repo.GetPhoto(photoLike.PhotoId) == null)
                throw new Exception("Zdjęcie nie istnieje.");

            if (await _repo.GetPhotoLike(id, userId) != null)
                throw new Exception("Już lubisz to zdjęcie");

            await _repo.Add(photoLike);

            if (await _repo.SaveAll())
                return Task.CompletedTask;

            throw new Exception("Coś poszło nie tak");
        }

        public async Task<IEnumerable<PhotoForDashboardDto>> GetPhotos(int currentUserId)
        {
            var photos = await _repo.GetPhotos(currentUserId);

            var photosToReturn = _mapper.Map<IEnumerable<PhotoForDashboardDto>>(photos);

            return photosToReturn;
        }

        public async Task<PhotoLike> GetPhotoLike(int photoId, int userId)
        {
            var like = await _repo.GetPhotoLike(photoId, userId);
            return like;
        }

        public async Task<IAsyncResult> UnlikePhoto(int id, int userId)
        {

            var like = await _repo.GetPhotoLike(id, userId);

            if (like == null)
                throw new Exception("Nie lubisz tego zdjęcia");

            _repo.Delete<PhotoLike>(like);

            if (await _repo.SaveAll())
                return Task.CompletedTask;

            throw new Exception("Coś poszło nie tak.");
        }

        public async Task<PhotoForReturnDto> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo == null)
                throw new Exception("Nie ma takiego zdjęcia");

            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return photo;
        }

        public async Task<PhotoForReturnDto> AddPhotoForUser(int userId,
           PhotoForCreationDto photoForCreationDto)
        {
            var userFromRepo = await _repo.GetUser(userId);

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

            if (await _repo.SaveAll())
            {
                var photoForReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return photoForReturn;
            }

            throw new Exception("Coś poszło nie tak.");
        }

        public async Task<IEnumerable<PhotoForDashboardDto>> GetPhotosForUser(int userId)
        {
            var photos = await _repo.GetPhotosForUser(userId);

            var photosToReturn = _mapper.Map<IEnumerable<PhotoForDashboardDto>>(photos);

            return photosToReturn;
        }

        public async Task<IAsyncResult> DeletePhoto(int userId, int id)
        {

            var userFromRepo = await _repo.GetUser(userId);

            if (!userFromRepo.Photos.Any(p => p.ID == id))
                throw new Exception();

            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.IsMain)
                throw new Exception("Nie możesz usunąć zdjęcia głównego.");

            if (!string.IsNullOrEmpty(photoFromRepo.PublicId))
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);

                var result = await _cloudinary.DestroyAsync(deleteParams);

                if (result.Result == "ok")
                    _repo.Delete(photoFromRepo);
            }
            else _repo.Delete(photoFromRepo);

            if (await _repo.SaveAll())
                return Task.CompletedTask;

            throw new Exception("Coś poszło nie tak.");
        }

        public async Task<IAsyncResult> SetMainPhoto(int userId, int id)
        {
            var userFromRepo = await _repo.GetUser(userId);

            if (!userFromRepo.Photos.Any(p => p.ID == id))
                throw new Exception();

            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.IsMain)
                throw new Exception("To zdjęcie jest już zdjęciem głównym.");

            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);

            currentMainPhoto.IsMain = false;
            photoFromRepo.IsMain = true;

            if (await _repo.SaveAll())
                return Task.CompletedTask;

            throw new Exception("Coś poszło nie tak.");
        }
    }
}