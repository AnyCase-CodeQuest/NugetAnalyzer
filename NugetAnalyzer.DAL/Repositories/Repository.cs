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
        protected readonly DbSet<T> DbSet;

        public Repository(NugetAnalyzerDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            DbSet = context.Set<T>();
        }

        public void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            DbSet.Add(item);
        }

        public Task<T> GetByIdAsync(int id)
        {
            return DbSet.FindAsync(id);
        }

        public Task<T> GetSingleOrDefaultAsync(Expression<Func<T, bool>> predicates)
        {
            if (predicates == null)
            {
                throw new ArgumentNullException(nameof(predicates));
            }

            return DbSet.SingleOrDefaultAsync(predicates);
        }

        public async Task<IReadOnlyCollection<T>> GetAsync(Expression<Func<T, bool>> predicates)
        {
            if (predicates == null)
            {
                throw new ArgumentNullException(nameof(predicates));
            }

            return await DbSet
                .Where(predicates)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await DbSet
                .AsNoTracking()
                .ToListAsync();
        }

        public void Delete(int id)
        {
            T item = DbSet.Find(id);

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            DbSet.Remove(item);
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