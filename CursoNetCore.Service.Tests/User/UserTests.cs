using CursoNetCore.Domain.Dtos.User;
using CursoNetCore.Domain.Interfaces;
using CursoNetCore.Domain.Interfaces.Services;
using CursoNetCore.Service.Exceptions;
using CursoNetCore.Service.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using UserEntity = CursoNetCore.Domain.Entities.User;

namespace CursoNetCore.Service.Tests.User
{
    public class UserTests : BaseTest
    {
        private readonly Mock<IRepository<UserEntity>> _repository;
        private readonly IUserService _service;
        private readonly IList<UserEntity> _mockedUsers;

        public UserTests()
        {
            _mockedUsers = GetMockUsers();
            _repository = SetupMockRepository();
            _service = new UserService(_repository.Object, _mapper);
        }

        [Fact(DisplayName = "É possível listar todos os usuários")]
        public async Task Can_Get_All()
        {
            var data = await _service.GetAll();

            _repository.Verify(x => x.GetAllAsync(null), Times.Once);
            
            Assert.NotNull(data);
            Assert.NotEmpty(data);
        }

        [Fact(DisplayName = "É possível obter um usuário através do id")]
        public async Task Can_Get_By_Id()
        {
            var user = GetRandomUser(_mockedUsers);
            var id = user.Id;
            var result = await _service.GetById(id);

            _repository.Verify(x => x.GetByIdAsync(id), Times.Once);

            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Name, result.Name);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.CreatedAt, result.CreatedAt);
            Assert.Equal(user.UpdatedAt, result.UpdatedAt);
        }

        [Fact(DisplayName = "É possível obter um usuário através do e-mail")]
        public async Task Can_Get_By_Email()
        {
            var user = GetRandomUser(_mockedUsers);
            var email = user.Email;
            var result = await _service.GetByEmail(email);

            _repository.Verify(x => x.GetAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()), Times.Once);

            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Name, result.Name);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.CreatedAt, result.CreatedAt);
            Assert.Equal(user.UpdatedAt, result.UpdatedAt);
        }

        [Fact(DisplayName = "É gerado uma exceção ao tentar obter um usuário através de um id inexistente")]
        public async Task Cannot_Get_Non_Existent_User()
        {
            var randomId = Guid.NewGuid();
            var exception = await Assert.ThrowsAsync<ApiException>(() => _service.GetById(randomId));

            Assert.NotNull(exception.Message);
            Assert.Equal(404, exception.StatusCode);
        }

        [Fact(DisplayName = "É possível cadastrar um novo usuário")]
        public async Task Can_Save_New_User()
        {
            var saveUserDto = new SaveUserDto
            {
                Name = Faker.Name.FullName(),
                Email = Faker.Internet.Email()
            };
            var createdUser = await _service.Save(saveUserDto);

            _repository.Verify(x => x.ExistsAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()), Times.Once);
            _repository.Verify(x => x.AddAsync(It.IsAny<UserEntity>()), Times.Once);
            _repository.Verify(x => x.CommitAsync(), Times.Once);

            Assert.NotNull(createdUser);
            Assert.NotEqual(Guid.Empty, createdUser.Id);
            Assert.Equal(saveUserDto.Name, createdUser.Name);
            Assert.Equal(saveUserDto.Email, createdUser.Email);
        }

        [Fact(DisplayName = "Não é possível cadastrar um novo usuário utilizando um e-mail já utilizado")]
        public async Task Cannot_Save_User_With_Already_Used_Email()
        {
            var saveUserDto = new SaveUserDto
            {
                Name = Faker.Name.FullName(),
                Email = GetRandomUser(_mockedUsers).Email,
            };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _service.Save(saveUserDto));

            Assert.NotNull(exception.Message);
            Assert.Equal(400, exception.StatusCode);
        }

        [Fact(DisplayName = "É possível atualizar um usuário já existente")]
        public async Task Can_Update_User()
        {
            var userId = GetRandomUser(_mockedUsers).Id;
            var updateUserDto = new UpdateUserDto
            {
                Name = Faker.Name.FullName(),
                Email = Faker.Internet.Email()
            };

            await _service.Update(userId, updateUserDto);

            _repository.Verify(x => x.GetByIdAsync(userId), Times.Once);
            _repository.Verify(x => x.ExistsAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()), Times.Once);
            _repository.Verify(x => x.Update(It.IsAny<UserEntity>()), Times.Once);
            _repository.Verify(x => x.CommitAsync(), Times.Once);

            var user = await _service.GetById(userId);

            Assert.NotNull(user);
            Assert.Equal(updateUserDto.Name, user.Name);
            Assert.Equal(updateUserDto.Email, user.Email);
        }

        [Fact(DisplayName = "Não é possível atualizar um usuário informando um e-mail já utilizado")]
        public async Task Cannot_Update_User_With_Already_Used_Email()
        {
            var user = GetRandomUser(_mockedUsers);
            var userId = user.Id;
            var alreadyUsedEmail = _mockedUsers.FirstOrDefault(x => x.Id != userId).Email;
            var updateUserDto = new UpdateUserDto
            {
                Name = user.Name,
                Email = alreadyUsedEmail
            };
            var exception = await Assert.ThrowsAsync<ApiException>(() => _service.Update(userId, updateUserDto));

            Assert.NotNull(exception.Message);
            Assert.Equal(400, exception.StatusCode);
        }

        [Fact(DisplayName = "É possível deletar um usuário")]
        public async Task Can_Delete_User()
        {
            var id = GetRandomUser(_mockedUsers).Id;

            await _service.Delete(id);

            _repository.Verify(x => x.GetByIdAsync(id), Times.Once);
            _repository.Verify(x => x.Delete(It.IsAny<UserEntity>()), Times.Once);
            _repository.Verify(x => x.CommitAsync(), Times.Once);

            var exception = await Assert.ThrowsAsync<ApiException>(() => _service.GetById(id));

            Assert.NotNull(exception.Message);
            Assert.Equal(404, exception.StatusCode);
        }

        [Fact(DisplayName = "Não é possível deletar um usuário inexistente")]
        public async Task Cannot_Delete_Non_Existent_User()
        {
            var id = Guid.NewGuid();
            var exception = await Assert.ThrowsAsync<ApiException>(() => _service.Delete(id));

            Assert.NotNull(exception.Message);
            Assert.Equal(404, exception.StatusCode);
        }

        private UserEntity GetRandomUser(IEnumerable<UserEntity> source)
        {
            var random = new Random();
            var index = random.Next(source.Count());

            return source.ElementAt(index);
        }

        private Mock<IRepository<UserEntity>> SetupMockRepository()
        {
            var repository = new Mock<IRepository<UserEntity>>();

            repository.Setup(x => x.GetAllAsync(null))
                .ReturnsAsync(_mockedUsers);
            
            repository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => _mockedUsers.FirstOrDefault(user => user.Id == id));
            
            repository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .ReturnsAsync((Expression<Func<UserEntity, bool>> predicate) => _mockedUsers.FirstOrDefault(predicate.Compile()));

            repository.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .ReturnsAsync((Expression<Func<UserEntity, bool>> predicate) => _mockedUsers.Any(predicate.Compile()));

            repository.Setup(x => x.AddAsync(It.IsAny<UserEntity>()))
                .Callback((UserEntity user) =>
                {
                    user.Id = Guid.NewGuid();
                    user.CreatedAt = DateTime.UtcNow;
                    user.UpdatedAt = DateTime.UtcNow;

                    _mockedUsers.Add(user);
                });

            repository.Setup(x => x.Update(It.IsAny<UserEntity>()))
                .Callback((UserEntity user) =>
                {
                    var index = _mockedUsers.IndexOf(user);
                    _mockedUsers[index] = user;
                });

            repository.Setup(x => x.Delete(It.IsAny<UserEntity>()))
                .Callback((UserEntity user) => _mockedUsers.Remove(user));

            return repository;
        }

        private IList<UserEntity> GetMockUsers()
        {
            return Enumerable.Range(1, 10)
                .Select(x => GetMockUser())
                .ToList();
        }

        private UserEntity GetMockUser()
        {
            return new UserEntity
            {
                Id = Guid.NewGuid(),
                Name = Faker.Name.FullName(),
                Email = Faker.Internet.Email(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
        }
    }
}
