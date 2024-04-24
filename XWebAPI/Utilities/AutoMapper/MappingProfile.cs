using AutoMapper;
using Entities.DataTransferObjects.BlockedUsers;
using Entities.DataTransferObjects.Follower;
using Entities.DataTransferObjects.Tweets;
using Entities.DataTransferObjects.User;
using Entities.Models;
using static System.Net.Mime.MediaTypeNames;

namespace XWebAPI.Utilities.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User
            CreateMap<Users, UserProfileDto>();
            CreateMap<Users, UserSelfDto>();
            CreateMap<Users, UserDtoForFollow>();
            CreateMap<Users, UserDtoForBlocked>();
            CreateMap<Users, UserDtoForTweets>();
            CreateMap<UserDtoForRegister, Users>();
            CreateMap<UserDtoForAccountUpdate, Users>();



            //Follow
            CreateMap<Follows, FollowerDto>();
            CreateMap<Follows, FollowingDto>();


            //BlockedUsers
            CreateMap<BlockedUsers, BlockedUserDto>();


            //Tweets
            CreateMap<TweetDtoForCreate, Tweets>();
            CreateMap<TweetDtoForEdit, Tweets>();
            CreateMap<Tweets, TweetDtoForDetail>()
                .ForMember(dest => dest.MediaPaths, opt => opt.MapFrom(src => src.TweetMedias.Select(tm => tm.Id).ToList()));





            //TweetMedias
  
        }

    }
}
