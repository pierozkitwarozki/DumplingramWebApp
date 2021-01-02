namespace Dumplingram.API.Dtos
{
    public class CommentForReturnDto
    {
        public int Id { get; set; }
        public int CommenterId { get; set; }
        public string CommenterUsername { get; set; }
        public int PhotoId { get; set; }
        public string Content { get; set; }
    }
}