using AutoMapper;
using CursoNetCore.Domain.Dtos.User;
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
        private readonly IMapper _mapper;

        public UserService(IRepository<User> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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

        public async Task<User> Save(SaveUserDto saveUserDto)
        {
            var user = _mapper.Map<User>(saveUserDto);
            var isEmailAlreadyExists  = await IsEmailAlreadyExists(user);

            if (isEmailAlreadyExists)
            {
                throw new ApiException(HttpStatusCode.BadRequest, $"Já existe um usuário cadastrado com o e-mail {user.Email}");
            }

            await _repository.AddAsync(user);
            await _repository.CommitAsync();

            return user;
        }

        public async Task Update(Guid id, UpdateUserDto updateUserDto)
        {
            var user = await GetById(id);

            user.Name = updateUserDto.Name;
            user.Email = updateUserDto.Email;

            var isEmailAlreadyExists = await IsEmailAlreadyExists(user);

            if (isEmailAlreadyExists)
            {
                throw new ApiException(HttpStatusCode.BadRequest, $"Já existe um usuário cadastrado com o e-mail {user.Email}");
            }

            _repository.Update(user);
            await _repository.CommitAsync();
        }

        public async Task Delete(Guid id)
        {
            var user = await GetById(id);
            
            _repository.Delete(user);
            await _repository.CommitAsync();
        }

        private async Task<bool> IsEmailAlreadyExists(User user)
        {
            return await _repository.ExistsAsync(u => u.Email == user.Email && (u.Id != user.Id || user.Id == Guid.Empty));
        }
    }
}
