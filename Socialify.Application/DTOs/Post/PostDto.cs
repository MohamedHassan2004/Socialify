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
        public DateTime CreatedAt { get; set; }
        public string? Content { get; set; }
        public string? mediaUrl { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserProfilePictureUrl { get; set; }
        public int LikesCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public int CommentsCount { get; set; }
        public bool IsSavedByCurrentUser { get; set; }
    }
}
