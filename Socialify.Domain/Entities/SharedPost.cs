using System;

namespace Socialify.Domain.Entities
{
    public class SharedPost
    {
        public int Id { get; set; }
        public int OriginalPostId { get; set; }
        public int SharedPostId { get; set; }
        public string SharedByUserId { get; set; } = string.Empty;
        public DateTime SharedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public Post OriginalPost { get; set; } = null!;
        public Post Post { get; set; } = null!;
        public ApplicationUser SharedByUser { get; set; } = null!;
    }
}
