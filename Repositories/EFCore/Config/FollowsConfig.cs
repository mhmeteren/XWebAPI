
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.EFCore.Config
{
    public class FollowsConfig : IEntityTypeConfiguration<Follows>
    {


        public void Configure(EntityTypeBuilder<Follows> builder)
        {
            builder.ToTable(nameof(Follows));
            builder.Property(x => x.RequestStatus).IsRequired().HasDefaultValue(false);
            builder.HasOne(x => x.FollowerUser).WithMany(x => x.Followers).HasForeignKey(x => x.FollowerId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.FollowingUser).WithMany(x => x.Following).HasForeignKey(x => x.FollowingId).IsRequired().OnDelete(DeleteBehavior.NoAction);
        }
    }
}
