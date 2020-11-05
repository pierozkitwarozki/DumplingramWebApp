using Dumplingram.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dumplingram.API.Data
{
    public class DataContxt : DbContext
    {
        public DataContxt(DbContextOptions<DataContxt> options) : base(options) {}
        public DbSet<User> Users { get; set; }   
    }
}