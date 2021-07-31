using CursoNetCore.Domain.Dtos.User;
using CursoNetCore.Domain.Interfaces.Services;
using CursoNetCore.Service.Exceptions;
using System.Net;
using System.Threading.Tasks;

namespace CursoNetCore.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthService(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        public async Task<LoginResponseDto> Login(LoginDto loginDto)
        {
            var email = loginDto.Email;
            var user = await _userService.GetByEmail(email);

            if (user == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, $"Não foi encontrado um usuário com o e-mail {email}");
            }

            return new LoginResponseDto
            {
                AccessToken = _tokenService.Generate(user),
                User = user
            };
        }
    }
}
