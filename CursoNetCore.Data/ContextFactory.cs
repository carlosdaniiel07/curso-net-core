using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CursoNetCore.Data
{
    public class ContextFactory : IDesignTimeDbContextFactory<Context>
    {
        public Context CreateDbContext(string[] args)
        {
            var connectionString = "Server=192.168.99.100; Port=3306; Database=curso_net_core; Uid=root; Pwd=root;";
            var optionsBuilder = new DbContextOptionsBuilder<Context>();

            optionsBuilder.UseMySql(connectionString, ServerVersion.Parse("8.0.22"));

            return new Context(optionsBuilder.Options);
        }
    }
}
