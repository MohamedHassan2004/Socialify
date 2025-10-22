using Socialify.Application.DTOs.Common;
using Socialify.Application.ReposInterfaces;
using Socialify.Domain.Common;
using Socialify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Repos_Interfaces
{
    public interface ILikeRepository : IRepository<Like>
    {
        Task<PagedResult<Like>> GetLikesOnPostAsync(int postId, string currentUserId, int pageNumber, int pageSize);
    }
}
