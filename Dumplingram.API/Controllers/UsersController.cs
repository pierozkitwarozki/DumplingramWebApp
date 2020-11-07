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
        public async Task<IActionResult> GetUsers(UserParams userParams) 
        {
            
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

        
    }
}