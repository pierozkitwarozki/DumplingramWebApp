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


namespace Dumplingram.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {

        private readonly IDumplingramRepository _repo;
        private readonly IMapper _mapper;

        public PhotosController(IDumplingramRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet("{id}/likes")]
        public async Task<IActionResult> GetPhotoLikes(int id)
        {
            var likes = await _repo.GetPhotoLikes(id);

            var likesToReturn = _mapper.Map<IEnumerable<PhotoLikeDto>>(likes);

            if(likes == null)
                return BadRequest("Coś poszło nie tak.");

            return Ok(likesToReturn);
        }

        [HttpPost("{id}/like")]
        public async Task<IActionResult> LikePhoto(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var photoLike = new PhotoLike {
                UserId = userId,
                PhotoId = id
            };

            if (await _repo.GetPhoto(photoLike.PhotoId)==null)
                return BadRequest("Zdjęcie nie istnieje.");

            if(await _repo.GetPhotoLike(id, userId)!=null)
                return BadRequest("Już lubisz to zdjęcie");

            await _repo.Add(photoLike);

            if(await _repo.SaveAll())
                return NoContent();

            return BadRequest("Coś poszło nie tak");
        }

        [HttpGet]
        public async Task<IActionResult> GetPhotos()
        {
           var currentUserId = 
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            var photos = await _repo.GetPhotos(currentUserId);

            if(photos == null)
                return BadRequest("Coś poszło nie tak.");
            
            return Ok(photos);
        }

        [HttpGet("{photoId}/getlike/{userId}")]
        public async Task<IActionResult> GetPhotoLike(int photoId, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like =  await _repo.GetPhotoLike(photoId, userId);
            return Ok(like);
        }

        [HttpDelete("{id}/unlike/{userId}")]
        public async Task<IActionResult> UnlikePhoto(int id, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var like = await _repo.GetPhotoLike(id, userId);

            if(like == null)
                return BadRequest("Nie lubisz tego zdjęcia");
                
            _repo.Delete<PhotoLike>(like);

            if(await _repo.SaveAll())
                return NoContent();
            
            return BadRequest("Coś poszło nie tak.");
        }
    }
}