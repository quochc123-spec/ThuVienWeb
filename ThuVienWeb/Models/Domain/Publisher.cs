using System.ComponentModel.DataAnnotations;

namespace ThuVienWeb.Models.Domain
{
    public class Publisher
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        //navigation property
        public List<Books> Books { get; set; } = new List<Books>();
    }
}
