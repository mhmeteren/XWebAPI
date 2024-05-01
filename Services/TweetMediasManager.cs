using Entities.Enums;
using Entities.Models;
using Entities.UtilityClasses.FileTransactions;
using Microsoft.AspNetCore.Http;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class TweetMediasManager(
        IRepositoryManager manager,
        IFileUploadService fileUploadService
        ) : ITweetMediasService
    {

        private readonly IFileUploadService _fileUploadService = fileUploadService;

        public async Task DeleteMediasByIds(string tweetId, List<string> mediaIds)
        {
            List<TweetMedias>? tweetMedias = await manager.TweetMedias.GetAllTweetMediasByTweet(tweetId, false);
            if (tweetMedias == null)
                return;

            var deletingTweetMedias = new List<TweetMedias>();

            foreach (string mediaId in mediaIds)
            {
                TweetMedias? tweetMedia = tweetMedias.FirstOrDefault(x => x.Id.Equals(mediaId));
                if (tweetMedia != null)
                {
                    deletingTweetMedias.Add(tweetMedia);
                    await _fileUploadService.Delete(tweetMedia.path);
                }
            }

            manager.TweetMedias.DeleteRangeTweetMedias(deletingTweetMedias);
            await manager.SaveAsync();
        }


        public async Task SaveMediasByTweet(string tweetId, List<IFormFile> medias)
        {
            var tweetMedias = new List<TweetMedias>();
            foreach (var media in medias)
            {
                var tweetMedia = new TweetMedias { TweetId = tweetId };

                tweetMedia.path = await _fileUploadService.Upload(
                    new FileUpload(
                        file: media,
                        filePath: FolderPaths.Tweets,
                        filename: tweetMedia.Id));

                tweetMedias.Add(tweetMedia);
            }

            manager.TweetMedias.CreateRangeTweetMedias(tweetMedias);
            await manager.SaveAsync();
        }
    }
}
