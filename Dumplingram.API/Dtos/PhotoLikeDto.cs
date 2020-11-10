

namespace Dumplingram.API.Dtos
{
    public class PhotoLikeDto
    {
        public int UserId { get; set; }
        public int PhotoId { get; set; }
        public UserForDetailedDto Liker { get; set; }
        public PhotoForDetailed Photo { get; set; }
    }
}
