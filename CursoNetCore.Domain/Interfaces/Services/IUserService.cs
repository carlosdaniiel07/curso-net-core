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
        Task<User> Save(User user);
        Task Update(Guid id, User user);
        Task Delete(Guid id);
    }
}
