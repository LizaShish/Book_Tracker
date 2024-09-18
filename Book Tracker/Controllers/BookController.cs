using Book_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Book_Tracker.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDBContext _dbContext;

        public BookController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Действие для отображения избранных книг
        public IActionResult Favorites()
        {
            // Получаем только книги, которые помечены как избранные
            var favoriteBooks = _dbContext.Books.Where(b => b.IsFavorite).ToList();
            return View(favoriteBooks);
        }

        // Действие для пометки книги как "избранной" (через кнопку)
        public IActionResult MarkAsFavorite(int id)
        {
            var book = _dbContext.Books.FirstOrDefault(b => b.Id == id);
            if(book != null)
            {
                book.IsFavorite = !book.IsFavorite;  // Переключаем статус избранного
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Index(string searchString, string bookStatus)
        {
            // Создаем запрос для выборки всех кни
            var books = from b in _dbContext.Books
                        select b;

            // Если строка поиска не пуста, добавляем фильтрацию по названию или автору
            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Title.Contains(searchString) || s.Author.Contains(searchString));
            }

            //Фильтрации по статусу (прочитано/не прочитано)
            if (!string.IsNullOrEmpty(bookStatus))
            {
                if(bookStatus == "Прочитано")
                {
                    books=books.Where(b => b.IsRead == true);
                }
                else if (bookStatus == "Непрочитано")
                {
                    books = books.Where(b => b.IsRead == false);
                }

            }

            // Сохраняем текущий фильтр в ViewData, чтобы отобразить его в представлении
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentFilter"] = bookStatus;

            return View(await books.ToListAsync());
        }

        // GET: Book/Create  : отображает форму для добавления книги.
        public IActionResult Create()
        {
            return View();
        }

        // POST: Book/Create получает данные из формы и сохраняет новую книгу в базу данных.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            //[Bind]: определяет, какие поля из формы должны быть связаны с моделью Book.
            //ModelState.IsValid: проверяет, прошла ли форма валидацию.
            if (ModelState.IsValid)
            {
                _dbContext.Add(book);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }


        // GET: Book/Edit/5
        //Метод редактирования также состоит из двух частей:
        //GET Edit: загружает данные книги из базы данных по идентификатору (id) и передает их в представление для редактирования.
        //POST Edit: получает измененные данные из формы и сохраняет их в базу данных.

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _dbContext.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Book/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  Book book)
        {
            if(id !=book.Id)
            {
                return NotFound();
            }
             if(ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(book);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExisis(book.Id))
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
             return View(book);
        }

        private bool BookExisis(int id)
        {
            return _dbContext.Books.Any(e => e.Id == id);
        }



        // GET: Book/Delete/5
        //GET Delete: отображает информацию о книге, которая будет удалена, для подтверждения.
        //POST DeleteConfirmed: удаляет книгу из базы данных.

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _dbContext.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _dbContext.Books.FindAsync(id);
            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
