using System.ComponentModel.DataAnnotations;

namespace ThuVienWeb.Models.Domain
{
    public class Book_Author
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        //navigation property
        public Books Book { get; set; } = null!;
        public int AuthorId { get; set; }
        //navigation property
        public Author Author { get; set; } = null!;
    }
}
