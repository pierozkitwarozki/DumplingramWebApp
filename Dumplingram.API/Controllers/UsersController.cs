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
    public class UsersController : ControllerBase
    {
        private readonly IDumplingramRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IDumplingramRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetUsers() 
        {
            var userParams = new UserParams();
            
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await _repo.GetUser(currentUserId);

            userParams.UserId = currentUserId;

            var users = await _repo.GetUsers(userParams);

            var usersToReturn = _mapper.Map<IEnumerable<UserForDetailedDto>>(users);

            //Response.AddPagination(users.CurrentPage, users.PageSize,
             //   users.TotalCount, users.TotalPages);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id) 
        {
            var user = await _repo.GetUser(id);

            var usersToReturn = _mapper.Map<UserForDetailedDto>(user);

            return Ok(usersToReturn);
        }

        [HttpPost("{id}/follow/{followeeId}")]
        public async Task<IActionResult> FollowUser(int id, int followeeId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var follow = await _repo.GetFollow(id, followeeId);

            if(follow != null)
                return BadRequest("You already follow this user");
            
            if(await _repo.GetUser(followeeId) == null)
                return NotFound();

            follow = new Follow
            {
                FollowerId = id,
                FolloweeId = followeeId
            };

            await _repo.Add<Follow>(follow);

            if(await _repo.SaveAll())
                return Ok();
            
            return BadRequest("Failed to like user");
        }

        [HttpGet("{id}/followers")]
        public async Task<IActionResult> GetFollowers(int id)
        {
            /*if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();*/
            
            var followers = await _repo.GetFollowers(id);
            if(followers == null)
                return BadRequest("No followees");

            return Ok(followers);
        }

        [HttpGet("{id}/followees")]
        public async Task<IActionResult> GetFollowees(int id)
        {
            /*if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();*/
            
            var followees = await _repo.GetFollowees(id);

            if(followees == null)
                return BadRequest("No followees");

            return Ok(followees);
        }

        [HttpGet("{id}/getfollow/{followeeId}")]
        public async Task<IActionResult> GetFollow(int id, int followeeId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var follow = await _repo.GetFollow(id, followeeId);
            
            return Ok(follow);
        }


        [HttpDelete("{id}/unfollow/{followeeId}")]
        public async Task<IActionResult> Unfollow(int id, int followeeId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var follow = await _repo.GetFollow(id, followeeId);

            if(follow == null)
                return BadRequest("Nie obserwujesz tego użytkownika.");
            
            _repo.Delete<Follow>(follow);

            if(await _repo.SaveAll())
                return NoContent();
            
            return BadRequest("Coś poszło nie tak.");
        }

        
    }
}