using System;

namespace Dumplingram.API.Models
{
    public class PhotoComment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int CommenterId { get; set; }
        public int PhotoId { get; set; }
        public User Commenter { get; set; }
        public Photo Photo { get; set; }
        public DateTime Date { get; set; }
    }
}