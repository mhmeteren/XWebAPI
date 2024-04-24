﻿
namespace Entities.DataTransferObjects.User
{
    public record UserDtoForTweets
    {
        public string? Id { get; init; }
        public string? FullName { get; init; }
        public string? UserName { get; init; }
        public string? ProfileImageUrl { get; init; }
    }
}
