using Socialify.Domain.Enums;

namespace Socialify.Application.DTOs.Profile
{
    public class FriendWithRelationshipDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string ProfilePicUrl { get; set; }
        public string? Bio { get; set; }
        public RelationshipStatus RelationshipStatus { get; set; }
    }
}
