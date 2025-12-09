
using Riok.Mapperly.Abstractions;
using Socialify.Application.DTOs.Profile;
using Socialify.Domain.Entities;
using Socialify.Domain.Enums;

namespace Socialify.Application.Mappers
{
    [Mapper]
    public static partial class UserMapper
    {
        public static ProfileDto ToProfileDtoWithCurrentUser(this ApplicationUser user, string currentUserId)
        {
            var dto = user.ToProfileDto();
            dto.JoinedOn = user.CreatedAt;
            dto.IsCurrentUser = MapIsCurrentUser(user, currentUserId);
            dto.RelationshipStatus = GetRelationshipStatus(user, currentUserId);
            return dto;
        }
        private static bool MapIsCurrentUser(ApplicationUser user, string currentUserId)
            => user.Id == currentUserId;

        private static RelationshipStatus GetRelationshipStatus(ApplicationUser user, string currentUserId)
        {
            if (currentUserId == user.Id)
            {
                return RelationshipStatus.Self;
            }
            else if (user.Friendships.Any(f => f.UserId == currentUserId || f.FriendId == currentUserId))
            {
                return RelationshipStatus.Friend;
            }
            else if (user.SentFriendRequests.Any(fr => fr.ReceiverId == currentUserId))
            {
                return RelationshipStatus.ReceivedRequest;
            }
            else if (user.ReceivedFriendRequests.Any(fr => fr.SenderId == currentUserId))
            {
                return RelationshipStatus.SentRequest;
            }
            return RelationshipStatus.None;
        }

        public static partial ProfileDto ToProfileDto(this ApplicationUser user);
        public static partial void ToApplicationUser(this UpdateProfileInfoDto dto, ApplicationUser user);
        public static partial ProfileBasicInfoDto ToProfileBasicInfoDto(this ApplicationUser user);
        public static ProfileBasicInfoDto ToProfileBasicInfoDto(this ApplicationUser user, string currentUserId, RelationshipStatus? relationshipStatus = null)
        {
            var dto = user.ToProfileBasicInfoDto();
            if (relationshipStatus != null)
            {
                dto.RelationshipStatus = relationshipStatus.Value;
            }
            else
            {
                dto.RelationshipStatus = GetRelationshipStatus(user, currentUserId);
            }
            return dto;
        }

        public static partial UpdateProfileInfoDto ToUpdateProfileInfoDto(this ApplicationUser user);
    }
}
