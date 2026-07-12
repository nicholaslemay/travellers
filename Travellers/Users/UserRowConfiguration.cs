using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Travellers.Users;

public class UserRowConfiguration : IEntityTypeConfiguration<UserRow>
{
    public void Configure(EntityTypeBuilder<UserRow> builder)
    {
        builder.ToTable("users");
        builder.HasKey(user => user.UserId);
        builder.Property(user => user.UserId).HasColumnName("user_id").ValueGeneratedOnAdd();
        builder.Property(user => user.Email).HasColumnName("email");
        builder.Property(user => user.CreatedAt).HasColumnName("created_at");
        builder.Property(user => user.UpdatedAt).HasColumnName("updated_at");
        builder.HasIndex(user => user.Email).IsUnique();
    }
}
