
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.EFCore.Config
{
    public class TweetsConfig : IEntityTypeConfiguration<Tweets>
    {
        public void Configure(EntityTypeBuilder<Tweets> builder)
        {
            builder.ToTable(nameof(Tweets));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.MainTweetID).IsRequired(false);
            builder.Property(x => x.Content).HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.LikeCount).HasMaxLength(0).IsRequired();
            builder.Property(x => x.ReTweetCount).HasMaxLength(0).IsRequired();
            builder.Property(x => x.CommentCount).HasMaxLength(0).IsRequired();
            builder.Property(x => x.IsEditing).HasDefaultValue(false);
            builder.Property(x => x.IsDeleting).HasDefaultValue(false);
            builder.Property(x => x.IsRetweet).HasDefaultValue(false);
            builder.Property(x => x.AllowedRepliers).IsRequired().HasDefaultValue(0);


            builder.HasOne(x => x.MainTweet)
                .WithMany()
                .HasForeignKey(x => x.MainTweetID)
                .OnDelete(DeleteBehavior.NoAction);



            builder.HasOne(x => x.CreaterUser)
                .WithMany()
                .HasForeignKey(x => x.CreaterUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
