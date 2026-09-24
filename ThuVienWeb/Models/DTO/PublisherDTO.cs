namespace ThuVienWeb.Models.DTO
{
    public class PublisherDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<BookAuthorDTO>? BookAuthors { set; get; }
    }
    public class PublisherNoIdDTO
    {
        public string? Name { get; set; }
    }
    public class BookAuthorDTO
    {
        public string? BookName { get; set; }
        public List<string>? BookAuthors { get; set; }
    }

}

