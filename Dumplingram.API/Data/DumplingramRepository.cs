using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dumplingram.API.Models;
using Microsoft.EntityFrameworkCore;
using Dumplingram.API.Helpers;
using Dumplingram.API.Dtos;
using System;

namespace Dumplingram.API.Data
{
    public class DumplingramRepository : IDumplingramRepository
    {
        private readonly DataContxt _context;
        public DumplingramRepository(DataContxt context)
        {
            _context = context;
        }

        public async Task Add<T>(T entity) where T : class
        {
            await _context.AddAsync(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }


        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(u => u.ID == id);
            user.Photos = user.Photos.OrderByDescending(x => x.DateAdded).ToList();
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(p => p.Photos)
                .OrderBy(u => u.Username).Where(u => u.ID != userParams.UserId).AsQueryable();

            if (!string.IsNullOrEmpty(userParams.Word))
            {
                users = users.Where(u => (u.Name.ToLower().Contains(userParams.Word.ToLower()))
                    || u.Surname.ToLower().Contains(userParams.Word.ToLower())
                    || u.Username.ToLower().Contains(userParams.Word.ToLower()));
            }

            return await users.ToListAsync<User>();
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0; //if eq 0 then successful, otherwise no changes
        }

        public async Task<Follow> GetFollow(int id, int followeeId)
        {
            return await _context.Follow
                .FirstOrDefaultAsync(u =>
                    u.FollowerId == id && u.FolloweeId == followeeId);
        }

        public async Task<IEnumerable<Follow>> GetFollowers(int id)
        {
            var list = await _context.Follow
                .Include(u => u.Follower).ThenInclude(p => p.Photos).Where(f => f.FolloweeId == id).ToListAsync();


            foreach (var user in list)
            {
                user.Follower.PasswordHash = null;
                user.Follower.PasswordSalt = null;
                var photo = user.Follower.Photos.FirstOrDefault(p => p.IsMain == true).Url;
                user.Follower.Description = photo;
                user.Follower.Photos = null;
            }

            return list;
        }

        public async Task<IEnumerable<Follow>> GetFollowees(int id)
        {
            var list = await _context.Follow
                .Include(u => u.Followee).ThenInclude(p => p.Photos).Where(f => f.FollowerId == id).ToListAsync();

            foreach (var user in list)
            {
                user.Followee.PasswordHash = null;
                user.Followee.PasswordSalt = null;
                var photo = user.Followee.Photos.FirstOrDefault(p => p.IsMain == true).Url;
                user.Followee.Description = photo;
                user.Followee.Photos = null;
            }

            return list;
        }

        public async Task<IEnumerable<Photo>> GetPhotos(int id)
        {
            var follows = await _context.Follow.Where(f => f.FollowerId == id).Select(u => u.FolloweeId).ToListAsync();
            var photos = await _context.Photo.Where(p => follows.Contains(p.UserId)).Include(u => u.User).ToListAsync();

            foreach (var photo in photos)
            {
                photo.User.PasswordHash = null;
                photo.User.PasswordSalt = null;
            }

            return photos.OrderByDescending(d => d.DateAdded);
        }

        public async Task<IEnumerable<PhotoLike>> GetPhotoLikes(int id)
        {
            var likes = await _context.PhotoLikes
                .Where(p => p.PhotoId == id).Include(u => u.Liker).ThenInclude(u => u.Photos).ToListAsync();

            return likes;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var list = await _context.Photo.Where(x => x.ID == id).Include(x => x.GottenLikes).ToListAsync();
            return list[0];
        }

        public async Task<PhotoLike> GetPhotoLike(int id, int userId)
        {
            return await _context.PhotoLikes.FirstOrDefaultAsync(x => x.PhotoId == id && x.UserId == userId);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photo
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync(p => p.IsMain == true);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<IEnumerable<Message>> GetMessagesForUser(int id)
        {
            var messages = await _context.Messages
                .Where(x => x.RecipientId == id || x.SenderId == id).Include(p => p.Sender)
                .ThenInclude(p => p.Photos).Include(p => p.Recipient).ThenInclude(p => p.Photos)
                .OrderByDescending(x => x.MessageSent).ToListAsync();

            int otherUserId = 0;
            var listToReturn = new List<Message>();

            foreach (var message in messages)
            {
                if (message.RecipientId == id) otherUserId = message.SenderId;
                else otherUserId = message.RecipientId;

                if (listToReturn.FirstOrDefault(x => (x.RecipientId == otherUserId || x.SenderId == otherUserId)) != null)
                    continue;

                listToReturn.Add(message);
            }

            return listToReturn;
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int currentUserId, int recipientId)
        {
            var messages = await _context.Messages.Where(x => (x.RecipientId == currentUserId && x.SenderId == recipientId)
            || (x.SenderId == currentUserId && x.RecipientId == recipientId)).ToListAsync();

            return messages.OrderBy(x => x.MessageSent);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<IEnumerable<Connection>> GetConnections(string userId)
        {
            return await _context.Connections.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<Group> GetGroup(string name)
        {
            return await _context.Groups.Include(x => x.Connections).FirstOrDefaultAsync(x => x.Name == name);
        }

    }
}