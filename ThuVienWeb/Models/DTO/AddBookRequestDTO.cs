using System.ComponentModel.DataAnnotations;
using ThuVienWeb.Models.Domain;

namespace ThuVienWeb.Models.DTO
{
    public class AddBookRequestDTO
    {
        [Required]
        [MinLength(10)]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        [Range(0, 5, ErrorMessage = "Must be from 0 and 5.")]
        public int Rate { get; set; }
        public string? Genre { get; set; }
        public string? CoverUrl { get; set; }
        public DateTime DateAdded { get; set; }
        //navigation property
        public int PublisherId { get; set; }
        public required List<int> AuthorIds { get; set; }
    }
}
