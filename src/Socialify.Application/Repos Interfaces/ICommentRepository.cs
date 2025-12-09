using Socialify.Application.DTOs.Comment;
using Socialify.Application.ReposInterfaces;
using Socialify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Repos_Interfaces
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<Comment?> GetCommentWithUserAsync(int commentId);
    }
}
