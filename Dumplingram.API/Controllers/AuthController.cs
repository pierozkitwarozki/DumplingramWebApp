using System;
using System.Threading.Tasks;
using Dumplingram.API.Dtos;
using Microsoft.AspNetCore.Mvc;
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