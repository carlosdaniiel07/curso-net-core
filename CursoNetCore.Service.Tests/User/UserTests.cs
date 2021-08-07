using AutoFixture;
using CursoNetCore.Domain.Dtos.User;
using CursoNetCore.Domain.Interfaces;
using CursoNetCore.Service.Exceptions;
using CursoNetCore.Service.Services;
using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using UserEntity = CursoNetCore.Domain.Entities.User;

namespace CursoNetCore.Service.Tests.User
{
    public class UserTests : BaseTest
    {
        [Fact(DisplayName = "É possível listar todos os usuários")]
        public async Task Can_Get_All()
        {
            // Arrange
            var users = _fixture.CreateMany<UserEntity>(10);
            var repositoryMock = new Mock<IRepository<UserEntity>>();

            repositoryMock.Setup(m => m.GetAllAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .ReturnsAsync(users);

            var userService = new UserService(repositoryMock.Object, null);

            // Act
            var result = await userService.GetAll();

            // Assert

            repositoryMock
                .Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()), Times.Once);
            
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(users.Count(), result.Count());
        }

        [Fact(DisplayName = "É possível obter um usuário através do id")]
        public async Task Can_Get_By_Id()
        {
            // Arrange
            var user = _fixture.Create<UserEntity>();
            var repositoryMock = new Mock<IRepository<UserEntity>>();

            repositoryMock.Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);

            var userService = new UserService(repositoryMock.Object, null);

            // Act
            var result = await userService.GetById(Guid.NewGuid());

            // Assert
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
            // Arrange
            var user = _fixture.Create<UserEntity>();
            var repositoryMock = new Mock<IRepository<UserEntity>>();

            repositoryMock.Setup(m => m.GetAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .ReturnsAsync(user);

            var userService = new UserService(repositoryMock.Object, null);

            // Act
            var result = await userService.GetByEmail(user.Email);

            // Assert
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
            // Arrange
            var exception = new ApiException(HttpStatusCode.NotFound, "Usuário não encontrado");
            var repositoryMock = new Mock<IRepository<UserEntity>>();

            repositoryMock.Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((UserEntity) null);

            var userService = new UserService(repositoryMock.Object, null);

            // Act
            var action = userService.GetById(Guid.NewGuid());

            // Assert
            var result = await Assert.ThrowsAsync<ApiException>(() => action);

            Assert.Equal(exception.StatusCode, result.StatusCode);
            Assert.Equal(exception.Message, result.Message);
        }

        [Fact(DisplayName = "É possível cadastrar um novo usuário")]
        public async Task Can_Save_New_User()
        {
            // Arrange
            var saveUserDto = _fixture.Create<SaveUserDto>();
            var repositoryMock = new Mock<IRepository<UserEntity>>();

            repositoryMock.Setup(m => m.ExistsAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .ReturnsAsync(false);

            repositoryMock.Setup(m => m.AddAsync(It.IsAny<UserEntity>()));

            repositoryMock.Setup(m => m.CommitAsync());

            var userService = new UserService(repositoryMock.Object, _mapper);

            // Act
            var result = await userService.Save(saveUserDto);

            // Assert
            repositoryMock
                .Verify(m => m.ExistsAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()), Times.Once);
            repositoryMock
                .Verify(m => m.AddAsync(It.IsAny<UserEntity>()), Times.Once);
            repositoryMock
                .Verify(m => m.CommitAsync(), Times.Once);

            Assert.NotNull(result);
            Assert.Equal(saveUserDto.Name, result.Name);
            Assert.Equal(saveUserDto.Email, result.Email);
        }

        [Fact(DisplayName = "Não é possível cadastrar um novo usuário utilizando um e-mail já utilizado")]
        public async Task Cannot_Save_User_With_Already_Used_Email()
        {
            // Arrange
            var saveUserDto = _fixture.Create<SaveUserDto>();
            var exception = new ApiException(HttpStatusCode.BadRequest, $"Já existe um usuário cadastrado com o e-mail {saveUserDto.Email}");
            var repositoryMock = new Mock<IRepository<UserEntity>>();

            repositoryMock.Setup(m => m.ExistsAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .ReturnsAsync(true);

            var userService = new UserService(repositoryMock.Object, _mapper);

            // Act
            var action = userService.Save(saveUserDto);

            // Assert
            var result = await Assert.ThrowsAsync<ApiException>(() => action);

            Assert.Equal(exception.StatusCode, result.StatusCode);
            Assert.Equal(exception.Message, result.Message);
        }

        [Fact(DisplayName = "É possível atualizar um usuário já existente")]
        public async Task Can_Update_User()
        {
            // Arrange
            var userId = _fixture.Create<Guid>();
            var updateUserDto = _fixture.Create<UpdateUserDto>();
            var user = _fixture.Create<UserEntity>();
            var repositoryMock = new Mock<IRepository<UserEntity>>();

            repositoryMock.Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
            repositoryMock.Setup(m => m.ExistsAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .ReturnsAsync(false);
            repositoryMock.Setup(m => m.Update(It.IsAny<UserEntity>()));
            repositoryMock.Setup(m => m.CommitAsync());

            var userService = new UserService(repositoryMock.Object, _mapper);

            // Act
            await userService.Update(userId, updateUserDto);

            // Assert
            repositoryMock
                .Verify(m => m.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            repositoryMock
                .Verify(m => m.ExistsAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()), Times.Once);
            repositoryMock
                .Verify(m => m.Update(It.IsAny<UserEntity>()), Times.Once);
            repositoryMock
                .Verify(m => m.CommitAsync(), Times.Once);
        }

        [Fact(DisplayName = "Não é possível atualizar um usuário informando um e-mail já utilizado")]
        public async Task Cannot_Update_User_With_Already_Used_Email()
        {
            // Arrange
            var userId = _fixture.Create<Guid>();
            var updateUserDto = _fixture.Create<UpdateUserDto>();
            var exception = new ApiException(HttpStatusCode.BadRequest, $"Já existe um usuário cadastrado com o e-mail {updateUserDto.Email}");
            var user = _fixture.Create<UserEntity>();
            var repositoryMock = new Mock<IRepository<UserEntity>>();

            repositoryMock.Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
            repositoryMock.Setup(m => m.ExistsAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .ReturnsAsync(true);

            var userService = new UserService(repositoryMock.Object, _mapper);

            // Act
            var action = userService.Update(userId, updateUserDto);

            // Assert
            repositoryMock
                .Verify(m => m.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            repositoryMock
                .Verify(m => m.ExistsAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()), Times.Once);
            repositoryMock
                .Verify(m => m.Update(It.IsAny<UserEntity>()), Times.Never);
            repositoryMock
                .Verify(m => m.CommitAsync(), Times.Never);

            var result = await Assert.ThrowsAsync<ApiException>(() => action);

            Assert.Equal(exception.StatusCode, result.StatusCode);
            Assert.Equal(exception.Message, result.Message);
        }

        [Fact(DisplayName = "Não é possível atualizar um usuário inexistente")]
        public async Task Cannot_Update_Non_Existent_User()
        {
            // Arrange
            var userId = _fixture.Create<Guid>();
            var updateUserDto = _fixture.Create<UpdateUserDto>();
            var exception = new ApiException(HttpStatusCode.NotFound, "Usuário não encontrado");
            var repositoryMock = new Mock<IRepository<UserEntity>>();

            repositoryMock.Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((UserEntity) null);

            var userService = new UserService(repositoryMock.Object, _mapper);

            // Act
            var action = userService.Update(userId, updateUserDto);

            // Assert
            repositoryMock
                .Verify(m => m.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            repositoryMock
                .Verify(m => m.Update(It.IsAny<UserEntity>()), Times.Never);
            repositoryMock
                .Verify(m => m.CommitAsync(), Times.Never);

            var result = await Assert.ThrowsAsync<ApiException>(() => action);

            Assert.Equal(exception.StatusCode, result.StatusCode);
            Assert.Equal(exception.Message, result.Message);
        }

        [Fact(DisplayName = "É possível deletar um usuário")]
        public async Task Can_Delete_User()
        {
            // Arrange
            var userId = _fixture.Create<Guid>();
            var user = _fixture.Create<UserEntity>();
            var repositoryMock = new Mock<IRepository<UserEntity>>();

            repositoryMock.Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
            repositoryMock.Setup(m => m.Delete(It.IsAny<UserEntity>()));
            repositoryMock.Setup(m => m.CommitAsync());

            var userService = new UserService(repositoryMock.Object, null);

            // Action
            await userService.Delete(userId);

            // Assert
            repositoryMock
                .Verify(m => m.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            repositoryMock
                .Verify(m => m.Delete(It.IsAny<UserEntity>()), Times.Once);
            repositoryMock
                .Verify(m => m.CommitAsync(), Times.Once);
        }

        [Fact(DisplayName = "Não é possível deletar um usuário inexistente")]
        public async Task Cannot_Delete_Non_Existent_User()
        {
            // Arrange
            var userId = _fixture.Create<Guid>();
            var exception = new ApiException(HttpStatusCode.NotFound, "Usuário não encontrado");
            var repositoryMock = new Mock<IRepository<UserEntity>>();

            repositoryMock.Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((UserEntity) null);

            var userService = new UserService(repositoryMock.Object, null);

            // Action
            var action = userService.Delete(userId);

            // Assert
            repositoryMock
                .Verify(m => m.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            repositoryMock
                .Verify(m => m.Delete(It.IsAny<UserEntity>()), Times.Never);
            repositoryMock
                .Verify(m => m.CommitAsync(), Times.Never);

            var result = await Assert.ThrowsAsync<ApiException>(() => action);

            Assert.Equal(exception.StatusCode, result.StatusCode);
            Assert.Equal(exception.Message, result.Message);
        }
    }
}
