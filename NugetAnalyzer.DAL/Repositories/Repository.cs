using System;
using System.Linq;
using System.Collections.Generic;
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
                throw new ArgumentNullException(nameof(item));

            dbSet.Add(item);
        }

        public IEnumerable<T> Find(Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return dbSet.AsNoTracking()
                .Where(predicate);
        }

        public IEnumerable<T> GetAll()
        {
            return dbSet.AsNoTracking()
                .ToList();
        }

        public void Delete(int id)
        {
            var item = dbSet.Find(id);

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            dbSet.Remove(item);
        }

        public void Update(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            dbSet.Attach(item);
            context.Entry(item).State = EntityState.Modified;
        }
    }
}