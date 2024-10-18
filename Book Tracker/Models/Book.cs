using System.ComponentModel.DataAnnotations;

namespace Book_Tracker.Models
{
    public class Book
    {
        public int Id { get; set; }


        //Атрибуты валидации в квадратных скобках   
        [Required(ErrorMessage ="Введите название книги")]  // проверяет, что поле обязательно для заполнения.
        [StringLength(100, ErrorMessage = "Название книги не должно превышать 100 символов")] //задает максимальную и минимальную длину строки.
        public string Title { get; set; }


        public bool IsRead { get; set; }


        [DataType(DataType.Date)]  //указывает тип данных для валидации и рендеринга (например, дата, email, телефон).
        public DateTime? DateRead { get; set; }


        [StringLength(100, ErrorMessage = "Описание не должно превышать 1000 символов")]
        public string Description { get; set; }
        
        public bool IsFavorite { get; set; }


        //Связь с автором (многие к одному)
       [Required(ErrorMessage = "Введите Имя автора")]
       public int AuthorId { get; set; }

        public Author Author { get; set; }

        public string FilePath { get; set; }
    }
}
