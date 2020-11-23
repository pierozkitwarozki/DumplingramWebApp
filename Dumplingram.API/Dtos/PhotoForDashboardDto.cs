using System;

namespace Dumplingram.API.Dtos
{
    public class PhotoForDashboardDto
    {
        public int ID { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public int UserId { get; set; }
        public string UserPhotoUrl { get; set; }
        public string Username { get; set; }
    }
}