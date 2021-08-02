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
        public async Task Can_Insert_Into_Database()
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
        public async Task Can_Get_A_Collection_Of_Results()
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
