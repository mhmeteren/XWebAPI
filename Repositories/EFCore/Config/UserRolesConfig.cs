

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Entities.UtilityClasses;

namespace Repositories.EFCore.Config
{
    public class UserRolesConfig : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Name = Roles.User,
                    NormalizedName = Roles.User.CustomToUpper(),
                },
                new IdentityRole
                {
                    Name = Roles.Admin,
                    NormalizedName = Roles.Admin.CustomToUpper(),
                }
                );
        }
    }
}
