using System.ComponentModel.DataAnnotations;

namespace Book_Tracker.Models
{
    public class CreateAuthorDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите имя автора")]
        [StringLength(100, ErrorMessage = "Имя автора не должно превышать 100 символов")]
        public string Name { get; set; }

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)] // Указывает на то, что это поле должно быть отформатировано как дата
        public DateTime? BirthDate { get; set; } // Поле может быть nullable, если дата необязательна

        public ICollection<Book> Books { get; set; }
    }
}
