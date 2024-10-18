using System.ComponentModel.DataAnnotations;

namespace Book_Tracker.Models
{
    public class AuthorDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите имя автора")]
        [StringLength(100, ErrorMessage = "Имя автора не должно превышать 100 символов")]
        public string Name { get; set; }

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)] 
        public DateTime? BirthDate { get; set; }
    }
}
