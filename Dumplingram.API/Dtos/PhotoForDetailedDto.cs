using System;
using System.Collections.Generic;

namespace Dumplingram.API.Dtos
{
    public class PhotoForDetailed
    {
        public int ID { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }     
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        public ICollection<PhotoLikeDto> GottenLikes { get; set;}
    }
}