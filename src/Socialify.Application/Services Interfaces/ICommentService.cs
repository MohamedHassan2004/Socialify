using Socialify.Application.DTOs.Comment;
using Socialify.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Services_Interfaces
{
    public interface ICommentService
    {
        Task<Result<CommentDto>> AddCommentAsync(AddCommentDto addCommentDto, string currentUserId);
        Task<Result> EditCommentAsync(EditCommentDto editCommentDto, string currentUserId);
        Task<Result> DeleteCommentAsync(int commentId, string currentUserId);
    }
}
