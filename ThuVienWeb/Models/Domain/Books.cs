using System.ComponentModel.DataAnnotations;

namespace ThuVienWeb.Models.Domain
{
    public class Books
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public int Rate { get; set; }
        public string? Genre { get; set; }
        public string? CoverUrl { get; set; }
        public DateTime DateAdded { get; set; }
        //navigation property
        public int PublisherId { get; set; }
        // Initialize navigation properties to satisfy nullable/reference checks
        public Publisher Publisher { get; set; } = null!;
        //navigation property
        public List<Book_Author> Book_Authors { get; set; } = new List<Book_Author>();
    }
}
    