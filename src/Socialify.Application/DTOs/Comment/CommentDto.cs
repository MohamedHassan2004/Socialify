using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.DTOs.Comment
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserProfilePictureUrl { get; set; }
        public bool IsEdited { get; set; }
        public bool CanEditOrDelete { get; set; } = false;
    }
}
