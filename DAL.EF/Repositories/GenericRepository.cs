using DAL.EF.DbCreating;
using DAL.EF.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAL.EF.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly SummerDbContext Context;
        protected readonly DbSet<T> DbSet;

        public GenericRepository(SummerDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return DbSet.AsQueryable();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            Expression<Func<T, bool>>? filter = null,
            CancellationToken cancellationToken = default)
        {
            var query = DbSet.AsQueryable();

            if (include != null)
            {
                query = include(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.AsNoTracking().ToListAsync(cancellationToken);
        }

        public virtual async Task<T?> GetByIdAsync(
            Guid id,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            CancellationToken cancellationToken = default)
        {
            var query = DbSet.AsQueryable();

            if (include != null)
            {
                query = include(query);
            }

            // Оскільки ми не знаємо ім'я властивості ID, використовуємо EF Core Find
            var entity = await DbSet.FindAsync(new object[] { id }, cancellationToken);

            if (entity == null)
                return null;

            // Якщо є include, довантажуємо пов'язані дані
            if (include != null)
            {
                await Context.Entry(entity)
                    .ReloadAsync(cancellationToken);
                
                var queryWithIncludes = include(DbSet.AsQueryable());
                entity = await queryWithIncludes
                    .FirstOrDefaultAsync(e => Microsoft.EntityFrameworkCore.EF.Property<Guid>(e, "Id") == id, cancellationToken);
            }

            return entity;
        }

        public async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            CancellationToken cancellationToken = default)
        {
            var query = DbSet.AsQueryable();

            if (include != null)
            {
                query = include(query);
            }

            return await query
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(T entity)
        {
            DbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }
    }
}
