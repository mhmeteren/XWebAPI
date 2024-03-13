using Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Repositories.EFCore.Config
{
    public class BaseUserConfig : IEntityTypeConfiguration<BaseUser>
    {
        public void Configure(EntityTypeBuilder<BaseUser> builder)
        {
            builder.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            builder.Property(u => u.UserName).HasMaxLength(100).IsRequired();
            builder.Property(u => u.PhoneNumber).HasMaxLength(11).IsRequired(false);
            builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
            builder.Property(u => u.AccountCreateDate).IsRequired();
            builder.Property(u => u.AccountDisableDate).IsRequired(false);
            builder.Property(u => u.ProfileImageUrl).HasMaxLength(256).IsRequired(false);
            builder.Property(u => u.BackgroundImageUrl).HasMaxLength(256).IsRequired(false);
            builder.Property(u => u.Gender).HasMaxLength(10).IsRequired(false);
        }
    }
}
