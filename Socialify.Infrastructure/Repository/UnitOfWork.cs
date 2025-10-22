using Microsoft.EntityFrameworkCore.Storage;
using Socialify.Application.Repos_Interfaces;
using Socialify.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialify.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SocialifyDbContext _context;
        private IDbContextTransaction _transaction;

        public IPostRepository Posts { get; }
        public ILikeRepository Likes { get; }
        public ICommentRepository Comments { get; }
        public ISavedPostRepository SavedPosts { get; }


        public UnitOfWork(
            SocialifyDbContext context,
            IPostRepository postRepository,
            ILikeRepository likeRepository,
            ICommentRepository commentRepository,
            ISavedPostRepository savedPosts)
        {
            _context = context;
            Posts = postRepository;
            Likes = likeRepository;
            Comments = commentRepository;
            SavedPosts = savedPosts;
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
