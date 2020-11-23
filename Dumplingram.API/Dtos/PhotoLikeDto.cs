

namespace Dumplingram.API.Dtos
{
    public class PhotoLikeDto
    {   
        public int PhotoId { get; set; }
        public int UserId { get; set; }
        public string UserPhotoUrl { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
