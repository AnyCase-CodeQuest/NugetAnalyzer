using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NugetAnalyzer.DAL.Context;
using NugetAnalyzer.DAL.Interfaces;

namespace NugetAnalyzer.DAL.Repositories
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        private readonly NugetAnalyzerDbContext context;
        private readonly DbSet<T> dbSet;

        public Repository(NugetAnalyzerDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.dbSet = context.Set<T>();
        }

        public void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            dbSet.Add(item);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await dbSet.FindAsync(id);

            if (entity != null)
            {
                this.context.Entry(entity).State = EntityState.Detached;
            }

            return entity;
        }

        public Task<T> GetSignleOrDefaultAsync(Expression<Func<T, bool>> predicates)
        {
            if (predicates == null)
            {
                throw new ArgumentNullException(nameof(predicates));
            }

            return dbSet
                .AsNoTracking()
                .SingleOrDefaultAsync(predicates);
        }

        public Task<IReadOnlyCollection<T>> GetAsync(Expression<Func<T, bool>> predicates)
        {
            if (predicates == null)
            {
                throw new ArgumentNullException(nameof(predicates));
            }

            return dbSet
                .Where(predicates)
                .AsNoTracking()
                .ToListAsync()
                as Task<IReadOnlyCollection<T>>;
        }

        public Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return dbSet
                .AsNoTracking()
                .ToListAsync()
                as Task<IReadOnlyCollection<T>>;
        }

        public void Delete(int id)
        {
            var item = dbSet.Find(id);

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            dbSet.Remove(item);
        }

        public void Update(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            context.Entry(item).State = EntityState.Modified;
        }
    }
}