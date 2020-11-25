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

        private readonly IUserRepository _usersRepo;
        private readonly IMapper _mapper;
        public UsersService(IMapper mapper, IUserRepository userRepo)
        {
            _mapper = mapper;
            _usersRepo = userRepo;
        }

        public async Task<IEnumerable<UserForDetailedDto>> GetUsersAsync(UserParams userParams, int currentUserId)
        {

            var userFromRepo = await _usersRepo.GetUserAsync(currentUserId);

            userParams.UserId = currentUserId;

            var users = await _usersRepo.GetUsersAsync(userParams);

            var usersToReturn = _mapper.Map<IEnumerable<UserForDetailedDto>>(users);

            return usersToReturn;
        }

        public async Task<UserForDetailedDto> GetUserAsync(int id)
        {
            var user = await _usersRepo.GetUserAsync(id);

            var usersToReturn = _mapper.Map<UserForDetailedDto>(user);

            return usersToReturn;
        }

        public async Task<IAsyncResult> FollowUserAsync(int id, int followeeId)
        {

            var follow = await _usersRepo.GetFollowAsync(id, followeeId);

            if (follow != null)
                throw new Exception("Już obserwujesz tego użytkownika");

            if (followeeId == id)
                throw new Exception("Samouwielbienie, hatfu.");

            if (await _usersRepo.GetUserAsync(followeeId) == null)
                throw new Exception("Nie znaleziono użytkownika");

            follow = new Follow
            {
                FollowerId = id,
                FolloweeId = followeeId
            };

            await _usersRepo.AddAsync<Follow>(follow);

            if (await _usersRepo.SaveAllAsync())
                return Task.CompletedTask;

            throw new Exception("Niepowodzenie.");
        }

        public async Task<IEnumerable<FollowerForReturn>> GetFollowersAsync(int id)
        {
            var followers = await _usersRepo.GetFollowersAsync(id);

            var followersToReturn = _mapper.Map<IEnumerable<FollowerForReturn>>(followers);

            return followersToReturn;
        }

        public async Task<IEnumerable<FolloweeToReturn>> GetFolloweesAsync(int id)
        {
            var followees = await _usersRepo.GetFolloweesAsync(id);

            var followeesToReturn = _mapper.Map<IEnumerable<FolloweeToReturn>>(followees);

            return followeesToReturn;
        }

        public async Task<Follow> GetFollowAsync(int id, int followeeId)
        {
            var follow = await _usersRepo.GetFollowAsync(id, followeeId);

            return follow;
        }

        public async Task<IAsyncResult> UnfollowAsync(int id, int followeeId)
        {
            var follow = await _usersRepo.GetFollowAsync(id, followeeId);

            if (follow == null)
                throw new Exception("Nie obserwujesz tego użytkownika.");

            _usersRepo.Delete<Follow>(follow);

            if (await _usersRepo.SaveAllAsync())
                return Task.CompletedTask;

            throw new Exception("Coś poszło nie tak.");
        }

        public async Task<IAsyncResult> UpdateUserAsync(int id, UserForUpdateDto userForUpdateDto)
        {
            var userFromRepo = await _usersRepo.GetUserAsync(id);

            _mapper.Map(userForUpdateDto, userFromRepo);

            if (await _usersRepo.SaveAllAsync())
                return Task.CompletedTask;

            throw new Exception($"Edycja użytkownika: {id} zakończona niepowodzeniem.");
        }

    }
}