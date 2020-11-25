using System.Threading.Tasks;
using System.Security.Claims;
using System;
using AutoMapper;
using Dumplingram.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dumplingram.API.Helpers;
using Microsoft.Extensions.Options;
using Dumplingram.API.Services;

namespace Dumplingram.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IPhotoService _photoService;

        public PhotosController(IPhotoService photoService, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _photoService = photoService;
        }

        [HttpGet("{id}/likes")]
        public async Task<IActionResult> GetPhotoLikes(int id)
        {
            try
            {
                return Ok(await _photoService.GetPhotoLikesAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{id}/like")]
        public async Task<IActionResult> LikePhoto(int id)
        {
            try
            {
                var userId =
                    int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                await _photoService.LikePhotoAsync(userId, id);
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPhotos()
        {
            try
            {
                var currentUserId =
                    int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                return Ok(await _photoService.GetPhotosAsync(currentUserId));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("{photoId}/getlike")]
        public async Task<IActionResult> GetPhotoLike(int photoId)
        {
            try
            {
                var id =
                    int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                return Ok(await _photoService.GetPhotoLikeAsync(photoId, id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}/unlike/{userId}")]
        public async Task<IActionResult> UnlikePhoto(int id, int userId)
        {
            try
            {
                var myId =
                    int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                await _photoService.UnlikePhotoAsync(id, myId);

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            try
            {
                return Ok(await _photoService.GetPhotoAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("forUser/{id}")]
        public async Task<IActionResult> GetPhotosForUser(int id)
        {
            try
            {
                return Ok(await _photoService.GetPhotosForUserAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(
           [FromForm] PhotoForCreationDto photoForCreationDto)
        {
            try
            {
                var myID =
                    int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                return Ok(await _photoService.AddPhotoForUserAsync(myID, photoForCreationDto));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            try
            {
                var userId =
                    int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                await _photoService.DeletePhotoAsync(userId, id);

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPost("setMain/{id}")]
        public async Task<IActionResult> SetMainPhoto(int id)
        {
            try
            {
                var userId =
                    int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                await _photoService.SetMainPhotoAsync(userId, id);

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}