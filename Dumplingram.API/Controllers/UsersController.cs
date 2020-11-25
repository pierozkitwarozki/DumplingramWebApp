using System.Threading.Tasks;
using System.Security.Claims;
using System;
using Dumplingram.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dumplingram.API.Helpers;
using Dumplingram.API.Services;

namespace Dumplingram.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            try
            {
                var id =
                    int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                return Ok(await _usersService.GetUsersAsync(userParams, id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                return Ok(await _usersService.GetUserAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("follow/{followeeId}")]
        public async Task<IActionResult> FollowUser(int followeeId)
        {          
            try
            {
                var id = 
                    int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                await _usersService.FollowUserAsync(id, followeeId);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}/followers")]
        public async Task<IActionResult> GetFollowers(int id)
        {
            try
            {
                return Ok(await _usersService.GetFollowersAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}/followees")]
        public async Task<IActionResult> GetFollowees(int id)
        {
            try
            {
                return Ok(await _usersService.GetFolloweesAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("getfollow/{followeeId}")]
        public async Task<IActionResult> GetFollow(int followeeId)
        {            
            try
            {
                var id = 
                    int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                return Ok(await _usersService.GetFollowAsync(id, followeeId));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpDelete("unfollow/{followeeId}")]
        public async Task<IActionResult> Unfollow(int followeeId)
        {
            try
            {
                var id = 
                    int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                await _usersService.UnfollowAsync(id, followeeId);

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(UserForUpdateDto userForUpdateDto)
        {     
            try 
            {
                var id = 
                    int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                await _usersService.UpdateUserAsync(id, userForUpdateDto);

                return NoContent();

            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}