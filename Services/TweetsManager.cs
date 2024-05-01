using AutoMapper;
using Entities.DataTransferObjects.Tweets;
using Entities.DataTransferObjects.User;
using Entities.Enums;
using Entities.Exceptions.TweetMedias;
using Entities.Exceptions.Tweets;
using Entities.Exceptions.User;
using Entities.Models;
using Entities.RequestFeatures;
using Repositories.Contracts;
using Services.Contracts;



namespace Services
{
    public class TweetsManager(
        IRepositoryManager manager,
        IMapper mapper,
        IUserService userService,
        ITweetMediasService tweetMediasService,
        ITweetLikesService tweetLikesService,
        IFollowsService followsService) : ITweetsService
    {
        private readonly IRepositoryManager _manager = manager;
        private readonly IMapper _mapper = mapper;
        private readonly IUserService _userService = userService;
        private readonly ITweetMediasService _tweetMediasService = tweetMediasService;
        private readonly ITweetLikesService _tweetLikesService = tweetLikesService;
        private readonly IFollowsService _followsService = followsService;


        protected Users? _createrUser;
        protected Users? _loggedInUser;


        public async Task<PagedResponse<TweetDtoForDetail>> GetAllFollowingTweetsByUserAsync(string loggedInUsername, TweetParameters parameters, bool trackChanges)
        {
            _loggedInUser = await _userService.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername);

            var followingIds = await _manager.Follows.GetAllFollowingIdsAsync(_loggedInUser.Id, trackChanges);
            var tweetsWithMetaData = await _manager.Tweets.GetAllFollowingTweets(followingIds, parameters, trackChanges);

            return new PagedResponse<TweetDtoForDetail>(
                 items: _mapper.Map<IEnumerable<TweetDtoForDetail>>(tweetsWithMetaData),
                 metaData: tweetsWithMetaData.MetaData);
        }


        public async Task<PagedResponse<TweetDtoForDetail>> GetAllTweetsByUserAsync(string username, string loggedInUsername, UserFeedType userFeedType, TweetParameters parameters, bool trackChanges)
        {
            if (await IsFeedsPermissionDenied(loggedInUsername, username))
                throw new UserAccountIsPrivateBadRequestException();


            var tweetsWithMetaData = userFeedType switch
            {
                UserFeedType.Tweets => await _manager.Tweets.GetAllTweetsByUserAsync(_createrUser.Id, parameters, trackChanges),
                UserFeedType.RetweetsWithReplies => await _manager.Tweets.GetAllRetweetsWithRepliesByUserAsync(_createrUser.Id, parameters, trackChanges),
                UserFeedType.Media => null,
                UserFeedType.Likes => null,
                _ => null
            };


            return new PagedResponse<TweetDtoForDetail>(
               items: _mapper.Map<IEnumerable<TweetDtoForDetail>>(tweetsWithMetaData),
               metaData: tweetsWithMetaData.MetaData);
        }


        public async Task<PagedResponse<TweetDtoForDetail>> GetAllTweetsByTweetAsync(string tweetId, string loggedInUsername, TweetType tweetType, TweetParameters parameters, bool trackChanges)
        {
            var tweet = await _manager.Tweets.GetTweetDetailbyId(tweetId, trackChanges) ?? throw new TweetsNotFoundException();

            if (await IsPermissionDenied(loggedInUsername, tweet.CreaterUserId))
                throw new TweetIsPrivateBadRequestException();

            var tweetsWithMetaData = tweetType switch
            {
                TweetType.Replies => await _manager.Tweets.GetAllRepliesByTweetAsync(tweet.Id, parameters, trackChanges),
                TweetType.Quotes => await _manager.Tweets.GetAllQuotesByTweetAsync(tweet.Id, parameters, trackChanges),
            };


            return new PagedResponse<TweetDtoForDetail>(
              items: _mapper.Map<IEnumerable<TweetDtoForDetail>>(tweetsWithMetaData),
              metaData: tweetsWithMetaData.MetaData);
        }



        public async Task<PagedResponse<UserDtoForTweets>> GetAllRetweetersByTweetAsync(string tweetId, string loggedInUsername, TweetParameters parameters, bool trackChanges)
        {
            var tweet = await _manager.Tweets.GetTweetDetailbyId(tweetId, trackChanges) ?? throw new TweetsNotFoundException();

            if (await IsPermissionDenied(loggedInUsername, tweet.CreaterUserId))
                throw new TweetIsPrivateBadRequestException();

            var retweetersWithMetaData = await _manager.Tweets.GetAllRetweetersByTweetAsync(tweetId, parameters, trackChanges);
            return new PagedResponse<UserDtoForTweets>(
                items: _mapper.Map<IEnumerable<UserDtoForTweets>>(retweetersWithMetaData),
                metaData: retweetersWithMetaData.MetaData);

        }


        public async Task<PagedResponse<UserDtoForTweets>> GetAllLikeUsersByTweetAsync(string tweetId, string loggedInUsername, TweetLikesParameters parameters, bool trackChanges)
        {
            var tweet = await _manager.Tweets.GetTweetDetailbyId(tweetId, trackChanges) ?? throw new TweetsNotFoundException();

            if (await IsPermissionDenied(loggedInUsername, tweet.CreaterUserId))
                throw new TweetIsPrivateBadRequestException();

            return await _tweetLikesService.GetAllLikesByTweet(tweetId, parameters, trackChanges);
        }





        public async Task CreateTweet(string loggedInUsername, TweetDtoForCreate tweetDto)
        {
            _loggedInUser = await _userService.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername);


            var tweet = _mapper.Map<Tweets>(tweetDto);
            if (tweetDto.MainTweetID != null)
            {
                var mainTweet = await GetTweetByIdCheckAndExits(tweetDto.MainTweetID, false);
                _createrUser = _loggedInUser.Id.Equals(mainTweet.CreaterUserId) ?
                _loggedInUser : await _userService.GetUserByIdCheckAndExistsAsync(mainTweet.CreaterUserId);

                if (await IsTweetViewPermissionDenied())
                    throw new TweetIsPrivateBadRequestException();

                tweet.MainTweetID = mainTweet.Id;

                if (tweet.IsRetweet)
                {
                    if (await IsAlreadyRetweeted(mainTweet.Id))
                        throw new TweetIsAlreadyRetweetedBadRequestException();

                    mainTweet.ReTweetCount++;
                }
                else
                {
                    if (!await CheckCommentWritePermissions(mainTweet))
                        throw new CommentWritePermissionBadRequestException();

                    mainTweet.CommentCount++;
                }

                _manager.Tweets.Update(mainTweet);

                //[NOTE] Send Notification to Main Tweet Creater User
            }

            tweet.CreaterUserId = _loggedInUser.Id;
            tweet.AllowedRepliers = (int)Enum.Parse<ReplierType>(tweetDto.Repliers);
            _manager.Tweets.CreateTweet(tweet);
            await _manager.SaveAsync();

            if (tweetDto.Medias?.Count > 0)
                await _tweetMediasService.SaveMediasByTweet(tweet.Id, tweetDto.Medias);
        }

        private async Task<bool> IsAlreadyRetweeted(string tweetId)
        {
            return await _manager.Tweets.GetRetweetbyMainTweetAndUser(tweetId, _loggedInUser.Id, false) != null;
        }
            


        private async Task<bool> CheckCommentWritePermissions(Tweets mainTweet)
        {
            if (_createrUser.Id.Equals(_loggedInUser.Id))
                return true;

            return mainTweet.AllowedRepliers switch
            {
                (int)ReplierType.Everyone => true,
                (int)ReplierType.FollowedAccounts => IsMentionedAccount(mainTweet.Content) || await _followsService.IsFollower(_createrUser.Id, _loggedInUser.Id),
                (int)ReplierType.VerifiedAccounts => IsMentionedAccount(mainTweet.Content) || _loggedInUser.IsVerifiedAccount,
                (int)ReplierType.MentionedAccountsOnly => IsMentionedAccount(mainTweet.Content),
                _ => false,
            };
        }

        private bool IsMentionedAccount(string tweetContent)
        {
            if (tweetContent == null)
                return false;

            var words = tweetContent.Split(' ');

            foreach (string word in words)
            {
                if (word.StartsWith("@"))
                {
                    string mentionedUsername = new(word.Skip(1).ToArray());
                    if (mentionedUsername.Equals(_loggedInUser.UserName))
                        return true;
                }
            }

            return false;
        }






        public async Task<TweetDtoForDetail> GetTweetById(string tweetId, string loggedInUsername, bool trackChanges)
        {

            var tweet = await _manager.Tweets.GetTweetDetailbyId(tweetId, trackChanges) ?? throw new TweetsNotFoundException();

            if (await IsPermissionDenied(loggedInUsername, tweet.CreaterUserId))
                throw new TweetIsPrivateBadRequestException();

            return _mapper.Map<TweetDtoForDetail>(tweet);
        }


        public async Task<string> CheckTweetMediaViewPermission(string mediaId, string loggedInUsername)
        {
            var tweetMedia = await _manager.TweetMedias.GetTweetMediaById(mediaId, false) ?? throw new TweetMediaNotFoundException();
            var tweet = await GetTweetByIdCheckAndExits(tweetMedia.TweetId, false);

            if (await IsPermissionDenied(loggedInUsername, tweet.CreaterUserId))
                throw new TweetIsPrivateBadRequestException();


            return tweetMedia.path;
        }






        public async Task EditTweet(string tweetId, string loggedInUsername, TweetDtoForEdit tweetDto)
        {
            _loggedInUser = await _userService.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername);
            var tweet = await GetTweetByIdAndUserCheckAndExits(tweetId, _loggedInUser.Id, false);

            _mapper.Map(tweetDto, tweet);
            tweet.Id = tweetId;
            tweet.IsEditing = true;

            _manager.Tweets.Update(tweet);
            await _manager.SaveAsync();


            if (tweetDto.DeletedMediaIds?.Count > 0)
                await _tweetMediasService.DeleteMediasByIds(tweet.Id, tweetDto.DeletedMediaIds);

            if (tweetDto.Medias?.Count > 0)
                await _tweetMediasService.SaveMediasByTweet(tweet.Id, tweetDto.Medias);
        }


        public async Task DeleteTweet(string tweetId, string loggedInUsername)
        {
            _loggedInUser = await _userService.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername);
            var tweet = await GetTweetByIdAndUserCheckAndExits(tweetId, _loggedInUser.Id, false);

            if (tweet.MainTweetID != null)
            {
                var mainTweet = await _manager.Tweets.GetTweetbyId(tweet.MainTweetID, false);

                if (mainTweet != null)
                {
                    if (tweet.IsRetweet)
                    {
                        mainTweet.ReTweetCount--;
                    }
                    else
                    {
                        mainTweet.CommentCount--;
                    }

                    _manager.Tweets.UpdateTweet(mainTweet);
                }
            }

            tweet.IsDeleting = true;
            _manager.Tweets.Update(tweet);
            await _manager.SaveAsync();
        }

        public async Task<Tweets> GetTweetByIdAndUserCheckAndExits(string tweetId, string createrUserId, bool trackChanges) =>
         await _manager.Tweets.GetTweetbyIdAndUser(tweetId, createrUserId, trackChanges) ?? throw new TweetsNotFoundException();





        public async Task CreateTweetLike(string tweetId, string loggedInUsername)
        {
            var tweet = await GetTweetByIdCheckAndExits(tweetId, false);
            if (await IsPermissionDenied(loggedInUsername, tweet.CreaterUserId))
                throw new TweetIsPrivateBadRequestException();
            
            await _tweetLikesService.CreateLike(tweet.Id, _loggedInUser.Id);

            tweet.LikeCount++;
            _manager.Tweets.Update(tweet);
            await _manager.SaveAsync();
            //[NOTE] Send Notification to Tweet Creater User
        }

        public async Task DeleteTweetLike(string tweetId, string loggedInUsername)
        {
            var tweet = await GetTweetByIdCheckAndExits(tweetId, false);
            _loggedInUser = await _userService.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername);

            await _tweetLikesService.DeleteLike(tweet.Id, _loggedInUser.Id);

            tweet.LikeCount--;
            _manager.Tweets.Update(tweet);
            await _manager.SaveAsync();

        }





        public async Task<Tweets> GetTweetByIdCheckAndExits(string tweetId, bool trackChanges) =>
             await _manager.Tweets.GetTweetbyId(tweetId, trackChanges) ?? throw new TweetsNotFoundException();



        private async Task<bool> IsFeedsPermissionDenied(string loggedInUsername, string ownerUsername)
        {
            _loggedInUser = await _userService.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername);
            _createrUser = _loggedInUser.UserName.Equals(ownerUsername) ?
                _loggedInUser : await _userService.GetUserByIdentityNameCheckAndExistsAsync(ownerUsername);

            return await IsTweetViewPermissionDenied();
        }


        private async Task<bool> IsPermissionDenied(string loggedInUsername, string createrUserId)
        {
            _loggedInUser = await _userService.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername);
            _createrUser = _loggedInUser.Id.Equals(createrUserId) ?
                _loggedInUser : await _userService.GetUserByIdCheckAndExistsAsync(createrUserId);

            return await IsTweetViewPermissionDenied();
        }

        private async Task<bool> IsTweetViewPermissionDenied()
        {
            return (!_createrUser.Id.Equals(_loggedInUser.Id)
                && _createrUser.IsPrivateAccount
                && !await _followsService.IsFollower(_loggedInUser.Id, _createrUser.Id));
        }


    }
}
