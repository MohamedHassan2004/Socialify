using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.DTOs.Comment
{
    public class CommentBaseDto
    {
        [Required]
        [StringLength(200)]
        public string Content { get; set; } = string.Empty;
    }

    public class AddCommentDto : CommentBaseDto
    {
        public int PostId { get; set; }
    }

    public class EditCommentDto : CommentBaseDto
    {
        public int Id { get; set; }
    }

}
