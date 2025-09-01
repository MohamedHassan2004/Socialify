using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;
using Socialify.Infrastructure.Data.Context;
using Socialify.Application.Interfaces;
using Socialify.Application.RepoInterfaces;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly SocialifyDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(SocialifyDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
