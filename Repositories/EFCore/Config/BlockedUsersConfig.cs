
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.EFCore.Config
{
    public class BlockedUsersConfig : IEntityTypeConfiguration<BlockedUsers>
    {


        public void Configure(EntityTypeBuilder<BlockedUsers> builder)
        {
            builder.ToTable(nameof(BlockedUsers));
            builder.HasKey(b => b.Id); 

           
            builder.HasOne(b => b.BlockerUser)
                   .WithMany()
                   .HasForeignKey(b => b.BlockerUserId)
                   .IsRequired().OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(b => b.BlockedUser)
                   .WithMany(b => b.BlockedUsers)
                   .HasForeignKey(b => b.BlockedUserId)
                   .IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
