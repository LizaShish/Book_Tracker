using Book_Tracker.Interface;
using Book_Tracker.Models;
using Microsoft.EntityFrameworkCore;

namespace Book_Tracker.Repository 

{
    public class BookRepository : IRepository<Book>
    {
        private readonly AppDBContext _dbcontext;

        public BookRepository(AppDBContext DBcontext)
        {
            _dbcontext = DBcontext;
        }


        public IEnumerable<Book> GetAll()
        {
            return _dbcontext.Books.ToList();
        }

        public Book GetById(int id)
        {
            return _dbcontext.Books.Find(id);
        }

        public void Add(Book book)
        {
            _dbcontext.Books.Add(book);
        }

        public void Update(Book book)
        {
            _dbcontext.Books.Update(book);
        }

        public void Delete(Book book)
        {
            _dbcontext.Books.Remove(book);
        }

        public void Save()
        {
            _dbcontext.SaveChanges();
        }
    }
}
