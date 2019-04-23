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

        public Repository(NugetAnalyzerDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            context.Set<T>().Add(item);
        }

        public IEnumerable<T> Get(Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return context.Set<T>()
                .AsNoTracking()
                .AsEnumerable()
                .Where(predicate);
        }

        public IEnumerable<T> GetAll()
        {
            return context.Set<T>()
                .AsNoTracking()
                .ToList();
        }

        public void Delete(int id)
        {
            var item = context.Set<T>().Find(id);

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            context.Set<T>().Remove(item);
        }

        public void Update(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            context.Set<T>().Attach(item);
            context.Entry(item).State = EntityState.Modified;
        }
    }
}