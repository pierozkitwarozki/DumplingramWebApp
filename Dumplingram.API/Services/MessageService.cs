using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Dumplingram.API.Data;
using Dumplingram.API.Dtos;

namespace Dumplingram.API.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repo;
        private readonly IMapper _mapper;
        public MessageService(IMessageRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesForUserAsync(int id)
        {
            // For returning last message of each conversation
            
            var messages = await _repo.GetMessagesForUserAsync(id);

            if (messages == null)
                throw new Exception("Nie znaleziono.");

            var messagesToReturn = _mapper.Map<IEnumerable<MessageDto>>(messages);

            return messagesToReturn;
        }

    }
}