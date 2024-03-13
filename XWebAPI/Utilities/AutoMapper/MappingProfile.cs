using AutoMapper;
using Entities.DataTransferObjects.BlockedUsers;
using Entities.DataTransferObjects.Follower;
using Entities.DataTransferObjects.User;
using Entities.Models;

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
            CreateMap<UserDtoForRegister, Users>();
            CreateMap<UserDtoForAccountUpdate, Users>();



            //Follow
            CreateMap<Follows, FollowerDto>();
            CreateMap<Follows, FollowingDto>();


            //BlockedUsers
            CreateMap<BlockedUsers, BlockedUserDto>();

        }

    }
}
