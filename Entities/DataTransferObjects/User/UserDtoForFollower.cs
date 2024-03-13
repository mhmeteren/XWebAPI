
namespace Entities.DataTransferObjects.User
{
    public record UserDtoForFollow
    {
        public string? Id { get; init; }
        public string? FullName { get; init; }
        public string? UserName { get; init; }
        public string? ProfileImageUrl { get; init; }
        public string? About { get; init; }
    }
}
