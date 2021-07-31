using CursoNetCore.Domain.Entities;
using CursoNetCore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CursoNetCore.Data.Repository
{
    public class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly Context _context;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(Context context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;

            await _dbSet.AddAsync(entity);
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            var queryable = _dbSet.AsQueryable();

            if (predicate != null)
            {
                queryable = queryable.Where(predicate);
            }

            return await queryable.CountAsync();
        }
        
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate = null)
        {
            var queryable = _dbSet.AsQueryable();

            if (predicate != null)
            {
                queryable = queryable.Where(predicate);
            }

            return await queryable.AnyAsync();
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null)
        {
            var queryable = _dbSet.AsQueryable();

            if (predicate != null)
            {
                queryable = queryable.Where(predicate);
            }

            return await queryable.ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate = null)
        {
            var queryable = _dbSet.AsQueryable();

            if (predicate != null)
            {
                queryable = queryable.Where(predicate);
            }

            return await queryable.FirstOrDefaultAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(entity => entity.Id.Equals(id));
        }

        public void Update(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }
    }
}
