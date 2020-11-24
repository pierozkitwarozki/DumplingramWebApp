using System;
using System.Threading.Tasks;
using AutoMapper;
using Dumplingram.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dumplingram.API.Data;
using Dumplingram.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Dumplingram.API.Services;

namespace Dumplingram.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            try
            {
                return Ok(await _authService.RegisterAsync(userForRegisterDto));
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            try 
            {
                return Ok(await _authService.LoginAsync(userForLoginDto));
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}