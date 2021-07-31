using CursoNetCore.Domain.Entities;

namespace CursoNetCore.Domain.Interfaces.Services
{
    public interface ITokenService
    {
        string Generate(User user);
    }
}
