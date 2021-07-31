using System.ComponentModel.DataAnnotations;

namespace CursoNetCore.Domain.Dtos.User
{
    public class UpdateUserDto
    {
        [Required]
        [MaxLength(60)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }
    }
}
