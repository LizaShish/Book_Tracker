using Book_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Book_Tracker.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDBContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment; // Инжектируем для работы с файловой системой

        public BookController(AppDBContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        
        public IActionResult Favorites()
        {
            
            var favoriteBooks = _dbContext.Books
                .Include(b => b.Author)  
                .Where(b => b.IsFavorite).ToList()
                .Select(b => new BookDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    IsRead = b.IsRead,
                    Description = b.Description,
                    IsFavorite= b.IsFavorite,
                    AuthorName = b.Author != null ? b.Author.Name : "Автор не указан" 
                })
                .ToList();
            return View(favoriteBooks);
        }

       
        public IActionResult MarkAsFavorite(int id)
        {
            var book = _dbContext.Books.FirstOrDefault(b => b.Id == id);
            if(book != null)
            {
                book.IsFavorite = !book.IsFavorite;  
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult DownloadFile(int id)
        {
            var book = _dbContext.Books.FirstOrDefault(b => b.Id == id);
            if (book == null || string.IsNullOrEmpty(book.FilePath))
            {
                return NotFound("Файл не найден или не задан.");
            }

            // Абсолютный путь к файлу на сервере
            var filePath = Directory.GetCurrentDirectory() + "\\wwwroot\\" + book.FilePath;
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Файл не найден.");
            }

            // Получаем MIME-тип файла
            var contentType = "application/pdf"; // Если только PDF файлы загружаются
            var fileName = Path.GetFileName(book.FilePath);

            // Возвращаем файл клиенту
            return PhysicalFile(filePath, contentType, fileName);
        }


        // GET: Book/Index
        public async Task<IActionResult> Index(string searchString, string bookStatus, int page = 1,  int pageSize = 10 )
        {
            if (page < 1)
            {
                page = 1; // Обеспечиваем, чтобы номер страницы был не меньше 1
            }
            var books = from b in _dbContext.Books.Include(b => b.Author)
                        orderby b.Id descending
                        //Переписать методами linq
                        select new BookDTO
                        {
                            Id = b.Id,
                            Title = b.Title,
                            IsRead = b.IsRead,
                            Description = b.Description,
                            IsFavorite = b.IsFavorite,
                            AuthorName = b.Author.Name, 
                            FilePath = b.FilePath 

                        };


            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title.Contains(searchString) || b.AuthorName.Contains(searchString));
            }

            
            if (!string.IsNullOrEmpty(bookStatus))
            {
                if(bookStatus == "Прочитано")
                {
                    books=books.Where(b => b.IsRead);
                }
                else if (bookStatus == "Непрочитано")
                {
                    books = books.Where(b => !b.IsRead);
                }

            }

            var totalBooks = books.Count();
            var totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);

            if (page > totalPages && totalPages > 0)
            {
                page = totalPages; // Устанавливаем page в значение totalPages, если превышает общее количество страниц
            }

            var paginatedBooks = await books
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Вычисляем начальный и конечный индекс книг на текущей странице
            int startBook = (page - 1) * pageSize + 1;
            int endBook = Math.Min(startBook + pageSize - 1, totalBooks);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalBooks = totalBooks;
            ViewBag.StartBook = startBook;
            ViewBag.EndBook = endBook;


            return View(paginatedBooks);
        }

        // GET: Book/Create  : отображает форму для добавления книги.
        public IActionResult Create()
        {
            ViewBag.Authors = _dbContext.Authors.ToList(); 
            var model = new CreateBookDTO();            
            return View(model);                         
        }

        // POST: Book/Create получает данные из формы и сохраняет новую книгу в базу данных.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookDTO createBookDTO) 
        {
           
            if (createBookDTO.AuthorId == 0 || !_dbContext.Authors.Any(a => a.Id == createBookDTO.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Выберите существующего автора или добавьте нового.");
            }
            Console.WriteLine("AuthorId: " + createBookDTO.AuthorId);
            Console.WriteLine("Title: " + createBookDTO.Title);
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                var book = new Book
                {
                    Title = createBookDTO.Title,
                    IsRead = createBookDTO.IsRead,
                    Description = createBookDTO.Description,
                    AuthorId = createBookDTO.AuthorId,
                    FilePath = createBookDTO.UploadFile != null ? string.Empty : "placeholder.pdf"
                };

                
               if (createBookDTO.UploadFile != null && Path.GetExtension(createBookDTO.UploadFile.FileName).ToLower() == ".pdf")
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(createBookDTO.UploadFile.FileName);
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await createBookDTO.UploadFile.CopyToAsync(fileStream);
                    }

                    book.FilePath = "/uploads/" + uniqueFileName;
                }
               
                _dbContext.Books.Add(book);
                _dbContext.Entry(book).State = EntityState.Added;
                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index)); 
            }
            

            ViewBag.Authors = _dbContext.Authors.ToList();
            return View(createBookDTO);

        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _dbContext.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }
           

            var editBookDTO = new EditBookDTO
            {
                Id = book.Id,
                Title = book.Title,
                IsRead = book.IsRead,
                Description = book.Description,
                AuthorId = book.AuthorId,
            };

            if (!string.IsNullOrEmpty(book.FilePath) && book.FilePath != "placeholder.pdf")
            {
                editBookDTO.ExistingFilePath = book.FilePath;
            }

            ViewBag.Authors = _dbContext.Authors.ToList(); 
            return View(editBookDTO); 
        }

        // POST: Book/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditBookDTO editBookDTO)
        {
            if (id != editBookDTO.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Authors = _dbContext.Authors.ToList();
                return View(editBookDTO);
            }

            var book = await _dbContext.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            book.Title = editBookDTO.Title;
            book.Description = editBookDTO.Description;
            book.IsRead = editBookDTO.IsRead;
            book.AuthorId = editBookDTO.AuthorId;
            

            

            // Обработка загрузки файла
            if (editBookDTO.UploadFile != null && editBookDTO.UploadFile.Length > 0 && 
                Path.GetExtension(editBookDTO.UploadFile.FileName).ToLower() == ".pdf")
            {
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(editBookDTO.UploadFile.FileName);
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await editBookDTO.UploadFile.CopyToAsync(fileStream);
                }

                book.FilePath = "/uploads/" + uniqueFileName;
            }

            _dbContext.Update(book);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Book/Delete/5 - отображает подтверждение удаления книги
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _dbContext.Books.Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            var deleteBookDTO = new DeleteBookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                AuthorId = book.AuthorId,
            };

            return View(deleteBookDTO);
        }

        // POST: Book/Delete/5 - удаляет книгу
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
           

            var book = await _dbContext.Books.FindAsync( id);
            if (book == null)
            {
                return NotFound();
            }

            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync();

            // После удаления перенаправляем на список книг
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _dbContext.Books.Any(e => e.Id == id);
        }

    }
}

