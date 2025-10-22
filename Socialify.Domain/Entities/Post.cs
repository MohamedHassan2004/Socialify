using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Domain.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsEdited { get; set; } = false;
        public DateTime? EditedAt { get; set; }
        public string? Content { get; set; }
        public string? MediaUrl { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int LikesCount { get; set; } = 0;
        public int CommentsCount { get; set; } = 0;
        public ApplicationUser User { get; set; } = null!;
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<SavedPost> SavedPosts { get; set; } = new List<SavedPost>();

        public void IncrementLikes() => LikesCount++;
        public void DecrementLikesCount() { if (LikesCount > 0) LikesCount--; }
        public void IncrementCommentsCount() => CommentsCount++;
        public void DecrementCommentsCount() { if (CommentsCount > 0) CommentsCount--; }

    }
}
