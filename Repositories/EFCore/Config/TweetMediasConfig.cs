using Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Repositories.EFCore.Config
{
    public class TweetMediasConfig : IEntityTypeConfiguration<TweetMedias>
    {
        public void Configure(EntityTypeBuilder<TweetMedias> builder)
        {
            builder.ToTable(nameof(TweetMedias));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.path).IsRequired();

            builder.HasOne(x => x.Tweets)
                .WithMany(t => t.TweetMedias)
                .HasForeignKey(x => x.TweetId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
