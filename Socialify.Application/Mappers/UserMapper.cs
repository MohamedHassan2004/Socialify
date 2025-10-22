
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
            dto.Status = GetRelationshipStatus(user.Id, currentUserId);
            return dto;
        }
        private static bool MapIsCurrentUser(ApplicationUser user, string currentUserId)
            => user.Id == currentUserId;

        private static RelationshipStatus GetRelationshipStatus(string userId, string currentUserId)
        {
            if (currentUserId == userId)
            {
                return RelationshipStatus.Self;
            }
            //else if()
            //{ 
            //}
            return RelationshipStatus.None;
        }

        public static partial ProfileDto ToProfileDto(this ApplicationUser user);
        public static partial void ToApplicationUser(this UpdateProfileInfoDto dto, ApplicationUser user);
        public static partial ProfileBasicInfoDto ToProfileBasicInfoDto(this ApplicationUser user);
        public static ProfileBasicInfoDto ToProfileBasicInfoDto(this ApplicationUser user, string currentUserId)
        {
            var dto = user.ToProfileBasicInfoDto();
            dto.RelationshipStatus = GetRelationshipStatus(user.Id, currentUserId);
            return dto;
        }

        public static partial UpdateProfileInfoDto ToUpdateProfileInfoDto(this ApplicationUser user);
    }
}
