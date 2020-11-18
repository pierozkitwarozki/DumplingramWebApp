using System.Collections.Generic;

namespace Dumplingram.API.Dtos
{
    public class UserForDetailedDto
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Surname { get; set; }
        public string City { get; set; }
        public string Country { get; set; } 
        public string PhotoUrl { get; set; }
        public ICollection<PhotoForDetailed> Photos { get; set; } 
        //public ICollection<PhotoLikeDto> SendLikes { get; set; }
    }
}