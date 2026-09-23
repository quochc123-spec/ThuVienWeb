using System.ComponentModel.DataAnnotations;

namespace ThuVienWeb.Models.Domain
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        public string? FullName { get; set; }
        //navigation property
        public required List<Book_Author> Book_Authors { get; set; }
    }
}
