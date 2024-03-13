
namespace Entities.DataTransferObjects.User
{
    public record UserSelfDto
    {
        public string? Id { get; set; }
        public string? FullName { get; init; }
        public string? UserName { get; init; }
        public string? ProfileImageUrl { get; init; }
        public string? BackgroundImageUrl { get; init; }
        public string? About { get; init; }
        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Gender { get; init; }
        public bool PhoneNumberConfirmed { get; init; }
    }

}
