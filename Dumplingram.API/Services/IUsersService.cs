using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dumplingram.API.Dtos;
using Dumplingram.API.Helpers;
using Dumplingram.API.Models;

namespace Dumplingram.API.Services
{
    public interface IUsersService
    {
         Task<IEnumerable<UserForDetailedDto>> GetUsers(UserParams userParams, int currentUserId);
         Task<UserForDetailedDto> GetUser(int id);
         Task<IAsyncResult> FollowUser(int id, int followeeId);
         Task<IEnumerable<FollowerForReturn>> GetFollowers(int id);
         Task<IEnumerable<FolloweeToReturn>> GetFollowees(int id);
         Task<Follow> GetFollow(int id, int followeeId);
         Task<IAsyncResult> Unfollow(int id, int followeeId);
         Task<IAsyncResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto);
    }
}