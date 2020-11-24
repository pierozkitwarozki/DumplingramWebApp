using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dumplingram.API.Helpers;
using Dumplingram.API.Models;
using Microsoft.Extensions.Options;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Linq;
using Dumplingram.API.Dtos;
using Dumplingram.API.Data;
using AutoMapper;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
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