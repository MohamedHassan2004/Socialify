using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.DTOs.Post
{
    public class PostDto
    {
        public int Id { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsEdited { get; set; }
        public string? Content { get; set; }
        public string? MediaUrl { get; set; }
        public string? MediaType { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserProfilePicUrl { get; set; } = string.Empty;
        public int LikesCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public int CommentsCount { get; set; }
        public bool IsSavedByCurrentUser { get; set; }
        public bool IsOwnedByCurrentUser { get; set; }
        public int SharesCount { get; set; }
        public bool IsShared { get; set; }
        public bool IsSharedByCurrentUser { get; set; }
        public OriginalPostDto? OriginalPost { get; set; }
    }
}
