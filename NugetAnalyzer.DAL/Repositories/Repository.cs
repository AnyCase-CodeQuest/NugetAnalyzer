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
                this.context.Entry(null).State = EntityState.Detached;
            }

            return entity;
        }

        public async Task<T> GetSignleOrDefaultAsync(Expression<Func<T, bool>> predicates)
        {
            if (predicates == null)
            {
                throw new ArgumentNullException(nameof(predicates));
            }

            var entity = await dbSet.SingleOrDefaultAsync(predicates);

            if (entity != null)
            {
                this.context.Entry(null).State = EntityState.Detached;
            }

            return entity;
        }

        public async Task<IReadOnlyCollection<T>> GetAsync(Expression<Func<T, bool>> predicates)
        {
            if (predicates == null)
            {
                throw new ArgumentNullException(nameof(predicates));
            }

            var list = await dbSet.Where(predicates)
                .AsNoTracking()
                .ToListAsync();

            return list.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await dbSet.AsNoTracking()
                .ToListAsync();
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