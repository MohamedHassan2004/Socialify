
using Humanizer;
using Riok.Mapperly.Abstractions;
using Socialify.Application.DTOs.Comment;
using Socialify.Domain.Entities;

namespace Socialify.Application.Mappers
{
    [Mapper]
    public static partial class CommentMapper
    {
        [MapProperty(nameof(Comment.User.FullName), nameof(CommentDto.UserName))]
        [MapProperty(nameof(Comment.User.ProfilePicUrl), nameof(CommentDto.UserProfilePictureUrl))]
        public static partial CommentDto ToCommentDto(this Comment comment);

        public static CommentDto ToCommentDtoWithCurrentUser(this Comment comment, string currentUserId)
        {
            var dto = comment.ToCommentDto();
            dto.CanEditOrDelete = comment.UserId == currentUserId;
            dto.TimeAgo = comment.CreatedAt.Humanize(false);
            return dto;
        }

        //public static partial Comment ToComment(this CommentDto dto);
    }
}
