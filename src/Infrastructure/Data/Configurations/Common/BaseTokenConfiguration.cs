using SmartFridgeManagerAPI.Domain.Entities.Common;

namespace SmartFridgeManagerAPI.Infrastructure.Data.Configurations.Common;

public class BaseTokenConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseTokenEntity
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(x => x.Token)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .IsRequired();
    }
}
