
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
            dto.IsOwnedByCurrentUser = post.UserId == currentUserId;
            dto.MediaType = GetMediaType(post.MediaUrl);
            dto.MediaUrl = post.MediaUrl;
            dto.Content = post.Content;
            return dto;
        }

        public static partial UpdatePostDto ToUpdatePostDtoCore(this Post post);

        public static UpdatePostDto ToUpdatePostDto(this Post post)
        {
            var dto = post.ToUpdatePostDtoCore();
            dto.MediaType = GetMediaType(post.MediaUrl);
            dto.MediaUrl = post.MediaUrl;
            return dto;
        }

        private static string? GetMediaType(string? mediaUrl)
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
