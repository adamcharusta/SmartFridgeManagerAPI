using SmartFridgeManagerAPI.Domain.Entities;

namespace SmartFridgeManagerAPI.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.Username)
            .HasMaxLength(126)
            .IsRequired();

        builder.Property(x => x.Password)
            .IsRequired();

        builder.Property(x => x.Email)
            .IsRequired();
    }
}
