using Socialify.Application.DTOs.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.DTOs.Post
{
    public class PostWithDetailsDto
    {
        public PostDto Post { get; set; }
        public List<CommentDto> Comments { get; set; } = new();
    }
}
