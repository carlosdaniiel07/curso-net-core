using CursoNetCore.Data.Repository;
using CursoNetCore.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CursoNetCore.Data.Tests
{
    public class UserTests : BaseTest, IClassFixture<DbTest>
    {
        private readonly ServiceProvider _serviceProvider;

        public UserTests(DbTest dbTest)
        {
            _serviceProvider = dbTest.ServiceProvider;
        }

        [Fact]
        [Trait("User's CRUD", "Save")]
        public async Task Dado_um_usuario_valido_deve_ser_possivel_inserir_ele_no_banco()
        {
            using (var context = _serviceProvider.GetService<Context>())
            {
                var repository = new BaseRepository<User>(context);
                var user = new User
                {
                    Name = Faker.Name.FullName(),
                    Email = Faker.Internet.Email()
                };

                await repository.AddAsync(user);
                await repository.CommitAsync();

                Assert.NotNull(user);
                Assert.NotEqual(Guid.Empty, user.Id);
            }
        }

        [Fact]
        [Trait("User's CRUD", "Get all")]
        public async Task Deve_retornar_uma_lista_de_usuarios()
        {
            using (var context = _serviceProvider.GetService<Context>())
            {
                var repository = new BaseRepository<User>(context);
                var users = await repository.GetAllAsync();

                Assert.NotNull(users);
                Assert.IsType<List<User>>(users);
            }
        }
    }
}
