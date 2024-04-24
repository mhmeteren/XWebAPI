using Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace Repositories.EFCore.Config
{
    public class UsersConfig : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            builder.ToTable(nameof(Users));
            builder.Property(u => u.Birthday).IsRequired(false);
            builder.Property(u => u.About).IsRequired(false).HasMaxLength(200);
            builder.Property(u => u.Location).IsRequired(false).HasMaxLength(50);
            builder.Property(u => u.IsPrivateAccount).IsRequired(true).HasDefaultValue(false);
            builder.Property(u => u.IsVerifiedAccount).IsRequired(true).HasDefaultValue(false);
            builder.Property(u => u.FollowerCount).IsRequired(true).HasDefaultValue(0);
            builder.Property(u => u.FollowingCount).IsRequired(true).HasDefaultValue(0);
        }
    }
}
