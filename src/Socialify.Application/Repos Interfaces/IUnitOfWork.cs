using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Application.Repos_Interfaces
{
    public interface IUnitOfWork
    {
        public IPostRepository Posts { get; }
        public ILikeRepository Likes { get; }
        public ICommentRepository Comments { get; }
        public ISavedPostRepository SavedPosts { get; }
        public Task<int> SaveAsync();
        public Task BeginTransactionAsync();
        public Task CommitTransactionAsync();
        public Task RollbackTransactionAsync();
    }
}
