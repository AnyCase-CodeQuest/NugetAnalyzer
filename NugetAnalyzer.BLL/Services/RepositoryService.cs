using System;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Models.Repositories;
using NugetAnalyzer.DAL.Interfaces;

namespace NugetAnalyzer.BLL.Services
{
    public class RepositoryService : IRepository
    {
        private IUnitOfWork UnitOfWork { get; }

        public RepositoryService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException("Unit of work not initialized.");
        }

        public void Save(Repository repository, int userId)
        {
            RepositoryMapper mapper = new RepositoryMapper(UnitOfWork);
            UnitOfWork.GetRepository<Domain.Repository>().Add(mapper.ToDomain(repository, userId));
            UnitOfWork.SaveChangesAsync();
        }
    }
}
