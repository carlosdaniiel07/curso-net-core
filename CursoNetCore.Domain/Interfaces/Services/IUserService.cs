using CursoNetCore.Domain.Dtos.User;
using CursoNetCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CursoNetCore.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(Guid id);
        Task<User> GetByEmail(string email);
        Task<User> Save(SaveUserDto saveUserDto);
        Task Update(Guid id, UpdateUserDto updateUserDto);
        Task Delete(Guid id);
    }
}
