namespace CursoNetCore.Domain.Dtos.User
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; }
        public Entities.User User { get; set; }
    }
}
