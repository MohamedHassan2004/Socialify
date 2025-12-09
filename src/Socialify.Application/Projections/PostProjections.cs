using Socialify.Application.DTOs.Comment;
using Socialify.Application.DTOs.Post;
using Socialify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Projections
{
    public static class PostProjections
    {
        public static Expression<Func<Post, PostWithDetailsDto>> ToPostWithDetailsDto(string currentUserId)
        {
            return p => new PostWithDetailsDto
            {
                Post = new PostDto
                {
                    Id = p.Id,
                    CreatedAt = p.CreatedAt,
                    TimeAgo = "",
                    IsEdited = p.IsEdited,
                    Content = p.Content,
                    MediaUrl = p.MediaUrl,
                    MediaType = "",

                    // User Info
                    UserId = p.UserId,
                    UserName = p.User.FullName,
                    UserProfilePicUrl = p.User.ProfilePicUrl,

                    LikesCount = p.LikesCount,
                    CommentsCount = p.CommentsCount,
                    SharesCount = p.SharesCount,

                    IsLikedByCurrentUser = p.Likes.Any(l => l.UserId == currentUserId),
                    IsSavedByCurrentUser = p.SavedPosts.Any(sp => sp.UserId == currentUserId),
                    IsSharedByCurrentUser = p.SharedPosts.Any(sp => sp.SharedByUserId == currentUserId),
                    IsOwnedByCurrentUser = p.UserId == currentUserId,

                    // Share Info
                    IsShared = p.IsShared,
                    OriginalPost = p.IsShared && p.OriginalPost != null
                        ? new OriginalPostDto
                        {
                            Id = p.OriginalPost.Id,
                            CreatedAt = p.OriginalPost.CreatedAt,
                            TimeAgo = "",
                            IsEdited = p.OriginalPost.IsEdited,
                            Content = p.OriginalPost.Content,
                            MediaUrl = p.OriginalPost.MediaUrl,
                            MediaType = "",
                            UserId = p.OriginalPost.UserId,
                            UserName = p.OriginalPost.User != null
                                ? p.OriginalPost.User.FullName
                                : "Unknown User",
                            UserProfilePicUrl = p.OriginalPost.User != null
                                ? p.OriginalPost.User.ProfilePicUrl
                                : ""
                        }
                        : null
                },
                Comments = p.Comments
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        TimeAgo = "",
                        UserId = c.UserId,
                        UserName = c.User.FullName,
                        UserProfilePictureUrl = c.User.ProfilePicUrl,
                        IsEdited = c.IsEdited,
                        CanEditOrDelete = c.UserId == currentUserId
                    }).ToList()
            };
        }

        public static Expression<Func<Post, PostDto>> ToFeedDto(string currentUserId)
        {
            return p => new PostDto
            {
                Id = p.Id,
                CreatedAt = p.CreatedAt,
                TimeAgo = "",
                IsEdited = p.IsEdited,
                Content = p.Content,
                MediaUrl = p.MediaUrl,
                MediaType = "",

                // User Info
                UserId = p.UserId,
                UserName = p.User.FullName,
                UserProfilePicUrl = p.User.ProfilePicUrl,

                LikesCount = p.LikesCount,
                CommentsCount = p.CommentsCount,
                SharesCount = p.SharesCount,

                IsLikedByCurrentUser = p.Likes.Any(l => l.UserId == currentUserId),
                IsSavedByCurrentUser = p.SavedPosts.Any(sp => sp.UserId == currentUserId),
                IsSharedByCurrentUser = p.SharedPosts.Any(sp => sp.SharedByUserId == currentUserId),
                IsOwnedByCurrentUser = p.UserId == currentUserId,

                // Share Info
                IsShared = p.IsShared,
                OriginalPost = p.IsShared && p.OriginalPost != null
                    ? new OriginalPostDto
                    {
                        Id = p.OriginalPost.Id,
                        CreatedAt = p.OriginalPost.CreatedAt,
                        TimeAgo = "",
                        IsEdited = p.OriginalPost.IsEdited,
                        Content = p.OriginalPost.Content,
                        MediaUrl = p.OriginalPost.MediaUrl,
                        MediaType = "",
                        UserId = p.OriginalPost.UserId,
                        UserName = p.OriginalPost.User != null
                            ? p.OriginalPost.User.FullName
                            : "Unknown User",
                        UserProfilePicUrl = p.OriginalPost.User != null
                            ? p.OriginalPost.User.ProfilePicUrl
                            : ""
                    }
                    : null
            };
        }
    }
}
