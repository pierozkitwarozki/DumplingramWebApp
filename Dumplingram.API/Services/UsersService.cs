using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Dumplingram.API.Data;
using Dumplingram.API.Dtos;
using Dumplingram.API.Helpers;
using Dumplingram.API.Models;

namespace Dumplingram.API.Services
{
    public class UsersService : IUsersService
    {
        private readonly IDumplingramRepository _repo;
        private readonly IMapper _mapper;
        public UsersService(IDumplingramRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        public async Task<IEnumerable<UserForDetailedDto>> GetUsersAsync(UserParams userParams, int currentUserId)
        {

            var userFromRepo = await _repo.GetUserAsync(currentUserId);

            userParams.UserId = currentUserId;

            var users = await _repo.GetUsersAsync(userParams);

            var usersToReturn = _mapper.Map<IEnumerable<UserForDetailedDto>>(users);

            return usersToReturn;
        }

        public async Task<UserForDetailedDto> GetUserAsync(int id)
        {
            var user = await _repo.GetUserAsync(id);

            var usersToReturn = _mapper.Map<UserForDetailedDto>(user);

            return usersToReturn;
        }

        public async Task<IAsyncResult> FollowUserAsync(int id, int followeeId)
        {

            var follow = await _repo.GetFollowAsync(id, followeeId);

            if (follow != null)
                throw new Exception("Już obserwujesz tego użytkownika");

            if (followeeId == id)
                throw new Exception("Samouwielbienie, hatfu.");

            if (await _repo.GetUserAsync(followeeId) == null)
                throw new Exception("Nie znaleziono użytkownika");

            follow = new Follow
            {
                FollowerId = id,
                FolloweeId = followeeId
            };

            await _repo.AddAsync<Follow>(follow);

            if (await _repo.SaveAllAsync())
                return Task.CompletedTask;

            throw new Exception("Niepowodzenie.");
        }

        public async Task<IEnumerable<FollowerForReturn>> GetFollowersAsync(int id)
        {
            var followers = await _repo.GetFollowersAsync(id);

            var followersToReturn = _mapper.Map<IEnumerable<FollowerForReturn>>(followers);

            return followersToReturn;
        }

        public async Task<IEnumerable<FolloweeToReturn>> GetFolloweesAsync(int id)
        {
            var followees = await _repo.GetFolloweesAsync(id);

            var followeesToReturn = _mapper.Map<IEnumerable<FolloweeToReturn>>(followees);

            return followeesToReturn;
        }

        public async Task<Follow> GetFollowAsync(int id, int followeeId)
        {
            var follow = await _repo.GetFollowAsync(id, followeeId);

            return follow;
        }

        public async Task<IAsyncResult> UnfollowAsync(int id, int followeeId)
        {
            var follow = await _repo.GetFollowAsync(id, followeeId);

            if (follow == null)
                throw new Exception("Nie obserwujesz tego użytkownika.");

            _repo.Delete<Follow>(follow);

            if (await _repo.SaveAllAsync())
                return Task.CompletedTask;

            throw new Exception("Coś poszło nie tak.");
        }

        public async Task<IAsyncResult> UpdateUserAsync(int id, UserForUpdateDto userForUpdateDto)
        {
            var userFromRepo = await _repo.GetUserAsync(id);

            _mapper.Map(userForUpdateDto, userFromRepo);

            if (await _repo.SaveAllAsync())
                return Task.CompletedTask;

            throw new Exception($"Edycja użytkownika: {id} zakończona niepowodzeniem.");
        }

    }
}