using System;

namespace Dumplingram.API.Dtos
{
    public class CommentForAddDto
    {
        public int CommenterId { get; set; }
        public int PhotoId { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }

        public CommentForAddDto()
        {
            Date = DateTime.Now;
        }
    }
}