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

namespace Dumplingram.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDumplingramRepository _repo;
        private readonly IMapper _mapper;
        public MessagesController(IDumplingramRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> CreateMessage(int id, CreateMessageDto createMessageDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var recipient = await _repo.GetUser(createMessageDto.RecipientId);

            if(id != userId)
                return Unauthorized();

            if(recipient == null)
                return NotFound("Recipient does not exist");

            if(userId == recipient.ID)
                return BadRequest("You cannot send message to urself");

            var sender = await _repo.GetUser(id);

            var message = new Message {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.Username,
                RecipientUsername = recipient.Username,
                Content = createMessageDto.Content
            };

            await _repo.Add(message);

            if(await _repo.SaveAll())
                return Ok(_mapper.Map<MessageDto>(message));
            
            return BadRequest("Failed to send message");
            
        }
        
        [HttpGet("{id}/received")]
        public async Task<IActionResult> GetMessagesForUser(int id)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            //var sender = await _repo.GetUser(id);

            var messages = await _repo.GetMessagesForUser(id);

            if(messages == null)
                return NotFound();


            var messagesToReturn = _mapper.Map<IEnumerable<MessageDto>>(messages);

            return Ok(messagesToReturn);
        }

        [HttpGet("{id}/thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int id, int recipientId)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var sender = await _repo.GetUser(id);
            var recipient = await _repo.GetUser(recipientId);

            if(recipient == null)
                return NotFound();

            var messages = await _repo.GetMessageThread(id, recipientId);

            if(messages == null)
                return NotFound();

            var messagesToReturn = _mapper.Map<IEnumerable<MessageDto>>(messages);

            return Ok(messagesToReturn);
        }
    }
}