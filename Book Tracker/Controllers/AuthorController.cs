using Book_Tracker.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace Book_Tracker.Controllers
{
    public class AuthorController : Controller
    {
        private readonly AppDBContext _dbContext;

        public AuthorController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Create()
        {
            ViewBag.Authors = _dbContext.Authors.ToList(); 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAuthorDTO createAuthorDTO)
        {
            if (!ModelState.IsValid)
            {
                var author = new Author
                {                           
                    Name = createAuthorDTO.Name,
                    BirthDate = createAuthorDTO.BirthDate,

                };
               
                _dbContext.Authors.Add(author);   
                await _dbContext.SaveChangesAsync(); 
                return RedirectToAction(nameof(Index)); 
            }

            return View(createAuthorDTO);
        }
       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _dbContext.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            var editAuthorDTO = new EditAuthorDTO
            {
                Id = author.Id,
                Name = author.Name,
                BirthDate = author.BirthDate,
            };

            return View(editAuthorDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditAuthorDTO editAuthorDTO)
        {
            if (id != editAuthorDTO.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {

                var author = await _dbContext.Authors.FindAsync(id);
                if (author == null)
                {
                    return NotFound();
                }

                author.Name = editAuthorDTO.Name;
                author.BirthDate = editAuthorDTO.BirthDate;

                _dbContext.Update(author);
                await _dbContext.SaveChangesAsync();

            }
            return View(editAuthorDTO);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _dbContext.Authors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            var deleteAuthorDTO = new DeleteAuthorDTO  
            {
                Id = author.Id,
                Name = author.Name,
                
            };

            return View(deleteAuthorDTO);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _dbContext.Authors.FindAsync(id);
            if (author != null)
            {
                _dbContext.Authors.Remove(author);
                await _dbContext.SaveChangesAsync();
            }
            if (author == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Index()
        {
            var authors = await _dbContext.Authors.ToListAsync();
            var authorDTOs = authors.Select(author => new AuthorDTO  
            {
                Id = author.Id,
                Name = author.Name,
                BirthDate = author.BirthDate,

            }).ToList();

            return View(authorDTOs);
        }

        public async Task<IActionResult> BooksByAuthor(int id)
        {
            var author = await _dbContext.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if(author == null)
            {
                return NotFound("Автор не найден.");
            }

            return View(author.Books);
        }
    }

}
    