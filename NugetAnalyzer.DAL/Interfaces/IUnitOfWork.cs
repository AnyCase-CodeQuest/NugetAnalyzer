﻿using System.Threading.Tasks;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<T> GetRepository<T>()
            where T : class;

        IRepositoriesRepository RepositoriesRepository { get; }

        IPackageVersionsRepository PackageVersionsRepository { get; }

        Task<int> SaveChangesAsync();
    }
}