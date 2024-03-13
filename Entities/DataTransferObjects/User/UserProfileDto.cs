
namespace Entities.DataTransferObjects.User
{
    public record UserProfileDto
    {

        public string? Id { get; init; }
        public string? FullName { get; init; }
        public string? UserName { get; init; }
        public string? ProfileImageUrl { get; init; }
        public string? BackgroundImageUrl { get; init; }
        public string? About { get; init; }
        public string? Location { get; init; }
    }

}
