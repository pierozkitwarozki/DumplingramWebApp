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

        public async Task<IEnumerable<UserForDetailedDto>> GetUsers(UserParams userParams, int currentUserId)
        {

            var userFromRepo = await _repo.GetUser(currentUserId);

            userParams.UserId = currentUserId;

            var users = await _repo.GetUsers(userParams);

            var usersToReturn = _mapper.Map<IEnumerable<UserForDetailedDto>>(users);

            return usersToReturn;
        }

        public async Task<UserForDetailedDto> GetUser(int id)
        {
            var user = await _repo.GetUser(id);

            var usersToReturn = _mapper.Map<UserForDetailedDto>(user);

            return usersToReturn;
        }

        public async Task<IAsyncResult> FollowUser(int id, int followeeId)
        {

            var follow = await _repo.GetFollow(id, followeeId);

            if (follow != null)
                throw new Exception("Już obserwujesz tego użytkownika");

            if (followeeId == id)
                throw new Exception("Samouwielbienie, hatfu.");

            if (await _repo.GetUser(followeeId) == null)
                throw new Exception("Nie znaleziono użytkownika");

            follow = new Follow
            {
                FollowerId = id,
                FolloweeId = followeeId
            };

            await _repo.Add<Follow>(follow);

            if (await _repo.SaveAll())
                return Task.CompletedTask;

            throw new Exception("Niepowodzenie.");
        }

        public async Task<IEnumerable<FollowerForReturn>> GetFollowers(int id)
        {
            var followers = await _repo.GetFollowers(id);

            var followersToReturn = _mapper.Map<IEnumerable<FollowerForReturn>>(followers);

            return followersToReturn;
        }

        public async Task<IEnumerable<FolloweeToReturn>> GetFollowees(int id)
        {
            var followees = await _repo.GetFollowees(id);

            var followeesToReturn = _mapper.Map<IEnumerable<FolloweeToReturn>>(followees);

            return followeesToReturn;
        }

        public async Task<Follow> GetFollow(int id, int followeeId)
        {
            var follow = await _repo.GetFollow(id, followeeId);

            return follow;
        }

        public async Task<IAsyncResult> Unfollow(int id, int followeeId)
        {
            var follow = await _repo.GetFollow(id, followeeId);

            if (follow == null)
                throw new Exception("Nie obserwujesz tego użytkownika.");

            _repo.Delete<Follow>(follow);

            if (await _repo.SaveAll())
                return Task.CompletedTask;

            throw new Exception("Coś poszło nie tak.");
        }

        public async Task<IAsyncResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            var userFromRepo = await _repo.GetUser(id);

            _mapper.Map(userForUpdateDto, userFromRepo);

            if (await _repo.SaveAll())
                return Task.CompletedTask;

            throw new Exception($"Edycja użytkownika: {id} zakończona niepowodzeniem.");
        }

    }
}