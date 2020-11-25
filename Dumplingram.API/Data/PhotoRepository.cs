using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dumplingram.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dumplingram.API.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContxt _context;
        public PhotoRepository(DataContxt context)
        {
            _context = context;
        }

        public async Task AddAsync<T>(T entity) where T : class
        {
            await _context.AddAsync(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0; //if eq 0 then successful, otherwise no changes
        }


        public async Task<IEnumerable<Photo>> GetPhotosAsync(int id)
        {
            var follows = await _context.Follow.Where(f => f.FollowerId == id).Select(u => u.FolloweeId).ToListAsync();
            var photos = await _context.Photo.Where(p => follows.Contains(p.UserId)).Include(u => u.User).ToListAsync();

            return photos.OrderByDescending(d => d.DateAdded);
        }

        public async Task<IEnumerable<Photo>> GetPhotosForUserAsync(int id)
        {
            var photos = await _context.Photo.Where(x => x.UserId == id).Include(x => x.User).ToListAsync();
            return photos.OrderByDescending(d => d.DateAdded);
        }

        public async Task<IEnumerable<PhotoLike>> GetPhotoLikesAsync(int id)
        {
            var likes = await _context.PhotoLikes
                .Where(p => p.PhotoId == id).Include(u => u.Liker).ThenInclude(u => u.Photos).ToListAsync();

            return likes;
        }

        public async Task<Photo> GetPhotoAsync(int id)
        {
            var list = await _context.Photo.Where(x => x.ID == id).Include(x => x.GottenLikes).ToListAsync();
            return list[0];
        }

        public async Task<PhotoLike> GetPhotoLikeAsync(int id, int userId)
        {
            return await _context.PhotoLikes.FirstOrDefaultAsync(x => x.PhotoId == id && x.UserId == userId);
        }

        public async Task<Photo> GetMainPhotoForUserAsync(int userId)
        {
            return await _context.Photo
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync(p => p.IsMain == true);
        }
    }
}