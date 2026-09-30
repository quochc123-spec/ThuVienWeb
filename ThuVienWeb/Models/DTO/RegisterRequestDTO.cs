using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace ThuVienWeb.Models.DTO
{
    public class RegisterRequestDTO
    {
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [Required]
        public List<string>? Roles { get; set; }
    }
}
