using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using System;
using AutoMapper;
using Dumplingram.API.Data;
using Dumplingram.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dumplingram.API.Helpers;
using Dumplingram.API.Models;
using Microsoft.Extensions.Options;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Linq;

namespace Dumplingram.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {

        private readonly IDumplingramRepository _repo;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;
        private readonly IMapper _mapper;

        public PhotosController(IDumplingramRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
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

        [HttpGet("{id}/likes")]
        public async Task<IActionResult> GetPhotoLikes(int id)
        {
            var likes = await _repo.GetPhotoLikes(id);

            var likesToReturn = _mapper.Map<IEnumerable<PhotoLikeDto>>(likes);

            if (likes == null)
                return BadRequest("Coś poszło nie tak.");

            return Ok(likesToReturn);
        }

        [HttpPost("{id}/like")]
        public async Task<IActionResult> LikePhoto(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var photoLike = new PhotoLike
            {
                UserId = userId,
                PhotoId = id
            };

            if (await _repo.GetPhoto(photoLike.PhotoId) == null)
                return BadRequest("Zdjęcie nie istnieje.");

            if (await _repo.GetPhotoLike(id, userId) != null)
                return BadRequest("Już lubisz to zdjęcie");

            await _repo.Add(photoLike);

            if (await _repo.SaveAll())
                return NoContent();

            return BadRequest("Coś poszło nie tak");
        }

        [HttpGet]
        public async Task<IActionResult> GetPhotos()
        {
            var currentUserId =
                int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var photos = await _repo.GetPhotos(currentUserId);

            if (photos == null)
                return BadRequest("Coś poszło nie tak.");


            return Ok(photos);
        }

        [HttpGet("{photoId}/getlike/{userId}")]
        public async Task<IActionResult> GetPhotoLike(int photoId, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like = await _repo.GetPhotoLike(photoId, userId);
            return Ok(like);
        }

        [HttpDelete("{id}/unlike/{userId}")]
        public async Task<IActionResult> UnlikePhoto(int id, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like = await _repo.GetPhotoLike(id, userId);

            if (like == null)
                return BadRequest("Nie lubisz tego zdjęcia");

            _repo.Delete<PhotoLike>(like);

            if (await _repo.SaveAll())
                return NoContent();

            return BadRequest("Coś poszło nie tak.");
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo == null)
                return BadRequest("Nie ma takiego zdjęcia");

            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> AddPhotoForUser(int userId,
           [FromForm] PhotoForCreationDto photoForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

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
                return CreatedAtRoute("GetPhoto", new { userId = userId, id = photo.ID }, photoForReturn);
            }

            return BadRequest("Coś poszło nie tak.");
        }

        [HttpDelete("{userId}/delete/{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(userId);

            if (!userFromRepo.Photos.Any(p => p.ID == id))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id);

            /*if (photoFromRepo.GottenLikes != null && photoFromRepo.GottenLikes.Count > 0)
            {
                DeleteBehaviour.Cascade deletes all connected to photo likes.
                
                foreach (var like in photoFromRepo.GottenLikes)
                {
                    _repo.Delete(like);
                }

                if (!await _repo.SaveAll())
                    return BadRequest("Coś poszło nie tak.");
            }*/

            if (photoFromRepo.IsMain)
                return BadRequest("Nie możesz usunąć zdjęcia głównego.");

            if (!string.IsNullOrEmpty(photoFromRepo.PublicId))
             {
                 var deleteParams = new DeletionParams(photoFromRepo.PublicId);

                 var result = await _cloudinary.DestroyAsync(deleteParams);

                 if (result.Result == "ok")
                     _repo.Delete(photoFromRepo);
             }
             else _repo.Delete(photoFromRepo);

            if (await _repo.SaveAll())
                return Ok();

            return BadRequest("Coś poszło nie tak.");
        }

        [HttpPost("{userId}/setMain/{id}")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var userFromRepo = await _repo.GetUser(userId);

            if(!userFromRepo.Photos.Any(p => p.ID == id))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("To zdjęcie jest już zdjęciem głównym.");
            
            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            
            currentMainPhoto.IsMain = false;
            photoFromRepo.IsMain = true;

            if(await _repo.SaveAll())
                return NoContent();

            return BadRequest("Coś poszło nie tak.");
        }
    }
}