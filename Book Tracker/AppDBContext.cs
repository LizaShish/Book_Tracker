using Book_Tracker.Models;
using Microsoft.EntityFrameworkCore;

namespace Book_Tracker
{
    public class AppDBContext : DbContext 
    {
        public DbSet<Book> Books { get; set; }
        public AppDBContext(DbContextOptions<AppDBContext>options) : base(options) { }
    }
}
