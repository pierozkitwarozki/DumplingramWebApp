

using System;
using System.Collections.Generic;

namespace Dumplingram.API.Models
{
    public class Photo
    {
        public int ID { get; set; }
        public string Url { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public ICollection<PhotoLike> GottenLikes { get; set; }
    }
}