using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CursoNetCore.Data.Tests
{
    public abstract class BaseTest
    {
        public BaseTest()
        {

        }
    }

    public class DbTest : IDisposable
    {
        public ServiceProvider ServiceProvider { get; private set; }

        public DbTest()
        {
            var serviceCollection = new ServiceCollection();
            var connectionString = "Server=192.168.99.100; Port=3306; Database=curso_net_core_tests; Uid=root; Pwd=root;";
            var serverVersion = ServerVersion.Parse("8.0.22");

            serviceCollection.AddDbContext<Context>(options =>
            {
                options.UseMySql(connectionString, serverVersion);
            }, 
                ServiceLifetime.Transient
            );

            ServiceProvider = serviceCollection.BuildServiceProvider();

            using (var context = ServiceProvider.GetService<Context>())
            {
                context.Database.EnsureCreated();
            }
        }

        public void Dispose()
        {
            using (var context = ServiceProvider.GetService<Context>())
            {
                context.Database.EnsureDeleted();
            }
        }
    }
}
