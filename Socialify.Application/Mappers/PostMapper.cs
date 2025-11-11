using Humanizer;
using Riok.Mapperly.Abstractions;
using Socialify.Application.DTOs.Post;
using Socialify.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Socialify.Application.Mappers
{
    [Mapper]
    public static partial class PostMapper
    {
        public static partial PostDto ToPostDto(this Post post);
        public static PostDto ToPostDto(this Post post, string currentUserId)
        {
            var dto = post.ToPostDto();
            dto.UserName = post.User.FullName;
            dto.UserProfilePicUrl = post.User.ProfilePicUrl;
            dto.TimeAgo = post.CreatedAt.Humanize(false);
            dto.IsLikedByCurrentUser = post.Likes.Any(l => l.UserId == currentUserId);
            dto.IsSavedByCurrentUser = post.SavedPosts.Any(sp => sp.UserId == currentUserId);
            dto.IsSharedByCurrentUser = post.SharedPosts.Any(sp => sp.SharedByUserId == currentUserId);
            dto.IsOwnedByCurrentUser = post.UserId == currentUserId;
            dto.MediaType = MediaTypeHelper.GetMediaType(post.MediaUrl);

            // Map original post with only essential information
            if (post.IsShared && post.OriginalPost != null)
            {
                var originalPost = post.OriginalPost;
               
                dto.OriginalPost = new OriginalPostDto
                {
                    Id = originalPost.Id,
                    CreatedAt = originalPost.CreatedAt,
                    TimeAgo = originalPost.CreatedAt.Humanize(false),
                    IsEdited = originalPost.IsEdited,
                    Content = originalPost.Content,
                    MediaUrl = originalPost.MediaUrl,
                    MediaType = MediaTypeHelper.GetMediaType(originalPost.MediaUrl),
                    UserId = originalPost.UserId,
                    UserName = originalPost.User?.FullName ?? "Unknown User",
                    UserProfilePicUrl = originalPost.User?.ProfilePicUrl ?? ""
                };
            }
            return dto;
        }

        public static partial UpdatePostDto ToUpdatePostDtoCore(this Post post);

        [MapperIgnore]
        public static UpdatePostDto ToUpdatePostDto(this Post post)
        {
            var dto = post.ToUpdatePostDtoCore();
            dto.MediaType = MediaTypeHelper.GetMediaType(post.MediaUrl);
            return dto;
        }
    }

    internal static class MediaTypeHelper
    {
        public static string? GetMediaType(string? mediaUrl)
        {
            if (string.IsNullOrEmpty(mediaUrl)) return null;

            string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            string[] VideoExtensions = { ".mp4", ".avi", ".mov", ".wmv", ".mkv", ".webm", ".mpeg", ".3gp" };
            string[] AudioExtensions = { ".mp3", ".wav", ".aac", ".ogg", ".wma", ".m4a" };

            var extension = Path.GetExtension(mediaUrl)?.ToLowerInvariant();

            if (ImageExtensions.Contains(extension)) return "image";
            if (VideoExtensions.Contains(extension)) return "video";
            if (AudioExtensions.Contains(extension)) return "audio";

            return null;
        }
    }
}
