using Humanizer;
using Socialify.Application.DTOs.Post;
using Socialify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Automapper
{
    public static class PostMapper
    {
        internal static PostDto MapPost(Post post, string currentUserId) => new PostDto
        {
            Id = post.Id,
            CreatedAt = post.CreatedAt,
            Content = post.Content,
            MediaUrl = post.MediaUrl,
            MediaType = GetMediaType(post.MediaUrl),
            UserId = post.UserId,
            UserName = post.User.FullName,
            UserProfilePicUrl = post.User.ProfilePicUrl,
            LikesCount = post.LikesCount,
            CommentsCount = post.CommentsCount,
            TimeAgo = post.CreatedAt.Humanize(false),
            IsEdited = post.IsEdited,
            IsOwnedByCurrentUser = post.UserId == currentUserId,
            IsLikedByCurrentUser = post.Likes.Any(p => p.UserId == currentUserId),
            IsSavedByCurrentUser = post.SavedPosts.Any(sp => sp.UserId == currentUserId)
        };

        internal static string? GetMediaType(string? mediaUrl)
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
