using System.Threading.Tasks;
using CursoNetCore.Domain.Dtos.User;

namespace CursoNetCore.Domain.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> Login(LoginDto loginDto);
    }
}
