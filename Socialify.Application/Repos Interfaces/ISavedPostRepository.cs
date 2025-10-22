using Socialify.Application.DTOs.Common;
using Socialify.Application.ReposInterfaces;
using Socialify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Repos_Interfaces
{
    public interface ISavedPostRepository : IRepository<SavedPost>
    {
        public Task<PagedResult<SavedPost>> GetSavedPostsAsync(string userId, int pageNumber, int pageSize);
    }
}
