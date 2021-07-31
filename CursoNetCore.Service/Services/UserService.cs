using CursoNetCore.Domain.Entities;
using CursoNetCore.Domain.Interfaces;
using CursoNetCore.Domain.Interfaces.Services;
using CursoNetCore.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace CursoNetCore.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _repository;

        public UserService(IRepository<User> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<User> GetById(Guid id)
        {
            var user = await _repository.GetByIdAsync(id);

            if (user == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "Usuário não encontrado");
            }

            return user;
        }

        public async Task<User> GetByEmail(string email)
        {
            return await _repository.GetAsync(user => user.Email == email);
        }

        public async Task<User> Save(User user)
        {
            var emailAlreadyExists = await EmailAlreadyExists(user);

            if (emailAlreadyExists)
            {
                throw new ApiException(HttpStatusCode.BadRequest, $"Já existe um usuário cadastrado com o e-mail {user.Email}");
            }

            await _repository.AddAsync(user);
            await _repository.CommitAsync();

            return user;
        }

        public async Task Update(Guid id, User user)
        {
            var userToUpdate = await GetById(id);

            userToUpdate.Name = user.Name;
            userToUpdate.Email = user.Email;

            var emailAlreadyExists = await EmailAlreadyExists(userToUpdate);

            if (emailAlreadyExists)
            {
                throw new ApiException(HttpStatusCode.BadRequest, $"Já existe um usuário cadastrado com o e-mail {user.Email}");
            }

            _repository.Update(userToUpdate);
            await _repository.CommitAsync();
        }

        public async Task Delete(Guid id)
        {
            var user = await GetById(id);
            
            _repository.Delete(user);
            await _repository.CommitAsync();
        }

        private async Task<bool> EmailAlreadyExists(User user)
        {
            return await _repository.ExistsAsync(u => u.Email == user.Email && (u.Id != user.Id || user.Id == Guid.Empty));
        }
    }
}
