using CursoNetCore.Data.Mapping;
using CursoNetCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace CursoNetCore.Data
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }

        public Context(DbContextOptions options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}
