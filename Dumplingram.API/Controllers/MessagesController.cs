using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Dumplingram.API.Services;
using System;

namespace Dumplingram.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }


        [HttpGet("received")]
        public async Task<IActionResult> GetMessagesForUser()
        {
            try
            {
                var id =
                    int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                return Ok(await _messageService.GetMessagesForUserAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
    }
}