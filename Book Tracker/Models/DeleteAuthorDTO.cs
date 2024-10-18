using System.ComponentModel.DataAnnotations;

namespace Book_Tracker.Models
{
    public class DeleteAuthorDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите имя автора")]
        [StringLength(100, ErrorMessage = "Имя автора не должно превышать 100 символов")]
        public string Name { get; set; }
    }
}
