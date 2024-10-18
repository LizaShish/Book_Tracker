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



        // GET: Author/Create
        public IActionResult Create()
        {
            ViewBag.Authors = _dbContext.Authors.ToList(); // Передаем список авторов в представление
            return View();
        }

        // POST: Author/Create получает данные из формы и сохраняет новую книгу в базу данных.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAuthorDTO createAuthorDTO)
        {
           
            //ModelState.IsValid: проверяет, прошла ли форма валидацию.
            if (!ModelState.IsValid)
            {
              

                var author = new Author
                {
                    Id = createAuthorDTO.Id,
                    Name = createAuthorDTO.Name,
                    BirthDate = createAuthorDTO.BirthDate,

                };
               
                _dbContext.Authors.Add(author);   // Добавляем сущность Author в контекст базы данных
                await _dbContext.SaveChangesAsync(); // Сохраняем изменения в базу данных
                return RedirectToAction(nameof(Index)); // Перенаправляем на список авторов
            }
            return View(createAuthorDTO);
        }
       

        // GET: Author/Edit/5
        //Метод редактирования также состоит из двух частей:
        //GET Edit: загружает авторов из базы данных по идентификатору (id) и передает их в представление для редактирования.
        //POST Edit: получает измененные данные из формы и сохраняет их в базу данных.

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

        // POST: Author/Edit/5
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

                try
                {
                    // Преобразуем AuthorDTO обратно в Author
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
                catch (DbUpdateConcurrencyException)
                {
                    if (!_dbContext.Authors.Any(e => e.Id == editAuthorDTO.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(editAuthorDTO);
        }
        // GET: author/Delete/5
        //GET Delete: отображает информацию об авторе, которая будет удалена, для подтверждения.
        //POST DeleteConfirmed: удаляет автора из базы данных.

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

            var deleteAuthorDTO = new DeleteAuthorDTO   // Преобразуем его в AuthorDTO перед передачей в представление
            {
                Id = author.Id,
                Name = author.Name,
                
            };
            return View(deleteAuthorDTO);
        }

        // POST: author/Delete/5
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
        // GET: Author/Index (список всех авторов)
        public async Task<IActionResult> Index()
        {
            var authors = await _dbContext.Authors.ToListAsync();
            var authorDTOs = authors.Select(author => new AuthorDTO     // Преобразуем его в AuthorDTO перед передачей в представление
            {
                Id = author.Id,
                Name = author.Name,
                BirthDate = author.BirthDate,
                // Другие поля, если они есть
            }).ToList();

            // Передаем список AuthorDTO в представление
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
    