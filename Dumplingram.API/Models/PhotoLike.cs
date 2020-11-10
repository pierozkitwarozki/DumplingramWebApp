namespace Dumplingram.API.Models
{
    public class PhotoLike
    {
        public int UserId { get; set; }
        public int PhotoId { get; set; }
        public User Liker { get; set; }
        public Photo Photo { get; set; }
    }
}