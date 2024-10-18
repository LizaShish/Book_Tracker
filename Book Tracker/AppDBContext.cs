using Book_Tracker.Models;
using Microsoft.EntityFrameworkCore;

namespace Book_Tracker
{
    public class AppDBContext : DbContext 
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public AppDBContext(DbContextOptions<AppDBContext>options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Явное определение связи один ко многим
            modelBuilder.Entity<Author>()
                .HasMany(a => a.Books) // У одного автора может быть несколько книг
                .WithOne(a => a.Author)  // Каждая книга имеет одного автора
                .HasForeignKey(a => a.AuthorId);  // Внешний ключ
        }
    }
}
