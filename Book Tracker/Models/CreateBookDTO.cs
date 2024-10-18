using System.ComponentModel.DataAnnotations;

namespace Book_Tracker.Models
{
    public class CreateBookDTO
    {
        [Required(ErrorMessage = "Введите название книги")]
        [StringLength(100, ErrorMessage = "Название книги не должно превышать 100 символов")]
        public string Title { get; set; }

        public int Id { get; set; }
        public bool IsRead { get; set; }


        //[DataType(DataType.Date)]
        //public DateTime? DateRead { get; set; }


        [StringLength(100, ErrorMessage = "Описание не должно превышать 1000 символов")]
        public string Description { get; set; }

        public bool IsFavorite { get; set; }

        public int AuthorId { get; set; }

        public IFormFile? UploadFile { get; set; }
    }
}
