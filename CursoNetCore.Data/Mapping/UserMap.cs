using CursoNetCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CursoNetCore.Data.Mapping
{
    public class UserMap : BaseEntityMap<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.ToTable("user");

            builder.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("uq_user_email");

            builder.Property(e => e.Name)
                .HasMaxLength(60)
                .IsRequired()
                .HasColumnName("name");

            builder.Property(e => e.Email)
                .HasMaxLength(100)
                .IsRequired()
                .HasColumnName("email");
        }
    }
}
