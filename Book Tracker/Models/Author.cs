using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Book_Tracker.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите имя автора")]
        [StringLength(100, ErrorMessage = "Имя автора не должно превышать 100 символов")]
        public string Name { get; set; }

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)] 
        public DateTime? BirthDate { get; set; } 

        public ICollection<Book> Books { get; set; }

    }
}
