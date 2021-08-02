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
        private readonly IEnumerable<UserEntity> _mockedUsers;

        public UserTests()
        {
            _mockedUsers = GetMockUsers();
            _repository = SetupMockRepository();
            _service = new UserService(_repository.Object, _mapper);
        }

        [Fact]
        public async Task Can_Get_All()
        {
            var data = await _service.GetAll();

            _repository.Verify(x => x.GetAllAsync(null), Times.Once);
            
            Assert.NotNull(data);
            Assert.NotEmpty(data);
        }

        [Fact]
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

        [Fact]
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

        [Fact]
        public async Task Cannot_Get_Non_Existent_User()
        {
            var randomId = Guid.NewGuid();
            var exception = await Assert.ThrowsAsync<ApiException>(() => _service.GetById(randomId));

            Assert.NotNull(exception.Message);
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

            return repository;
        }

        private IEnumerable<UserEntity> GetMockUsers()
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
