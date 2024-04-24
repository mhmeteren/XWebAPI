using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Repositories.EFCore.Config
{
    public class TweetLikesConfig : IEntityTypeConfiguration<TweetLikes>
    {
        public void Configure(EntityTypeBuilder<TweetLikes> builder)
        {
            builder.ToTable(nameof(TweetLikes));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreateDate).IsRequired().HasDefaultValue(DateTime.Now);

            builder.HasOne(x => x.Tweets)
                .WithMany(x => x.TweetLikes)
                .HasForeignKey(x => x.TweetId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.User)
                 .WithMany(x => x.TweetLikes)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
