using System;

namespace Socialify.Application.DTOs.Post
{
    public class OriginalPostDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
        public bool IsEdited { get; set; }
        public string? Content { get; set; }
        public string? MediaUrl { get; set; }
        public string? MediaType { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserProfilePicUrl { get; set; } = string.Empty;
    }
}
