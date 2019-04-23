using System;
using NugetAnalyzer.DAL.Context;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.DAL.Repositories;

namespace NugetAnalyzer.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NugetAnalyzerDbContext context;

        public UnitOfWork(NugetAnalyzerDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IRepository<T> GetRepository<T>()
            where T : class
        {
            return new Repository<T>(context);
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
