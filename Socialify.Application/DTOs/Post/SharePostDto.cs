using System.ComponentModel.DataAnnotations;

namespace Socialify.Application.DTOs.Post
{
    public class SharePostDto
    {
        [Required(ErrorMessage = "Original post ID is required")]
        public int OriginalPostId { get; set; }
        
        [StringLength(500, ErrorMessage = "Additional comment cannot exceed 500 characters")]
        public string? AdditionalComment { get; set; }
    }
}
