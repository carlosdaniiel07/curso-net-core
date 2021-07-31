using System.ComponentModel.DataAnnotations;

namespace CursoNetCore.Domain.Dtos.User
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }
    }
}
