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
         Task<IEnumerable<UserForDetailedDto>> GetUsersAsync(UserParams userParams, int currentUserId);
         Task<UserForDetailedDto> GetUserAsync(int id);
         Task<IAsyncResult> FollowUserAsync(int id, int followeeId);
         Task<IEnumerable<FollowerForReturn>> GetFollowersAsync(int id);
         Task<IEnumerable<FolloweeToReturn>> GetFolloweesAsync(int id);
         Task<Follow> GetFollowAsync(int id, int followeeId);
         Task<IAsyncResult> UnfollowAsync(int id, int followeeId);
         Task<IAsyncResult> UpdateUserAsync(int id, UserForUpdateDto userForUpdateDto);
    }
}