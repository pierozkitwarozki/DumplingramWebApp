using System;
using System.Collections.Generic;

namespace Dumplingram.API.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Surname { get; set; }
        public string City { get; set; }
        public string Country { get; set; }    
        public ICollection<Photo> Photos { get; set; }
        public ICollection<Follow> Followees { get; set; }
        public ICollection<Follow> Followers { get; set; }    
        public ICollection<PhotoLike> SendLikes { get; set; }
        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<Message> MessagesReceived { get; set; }
        public ICollection<PhotoComment> Comments { get; set; }
    }
}