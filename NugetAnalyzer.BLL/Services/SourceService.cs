using System;
using System.Linq;
using System.Threading.Tasks;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NugetAnalyzer.Dtos.Converters;

namespace NugetAnalyzer.BLL.Services
{
    public class SourceService : ISourceService
    {
        private readonly IUnitOfWork unitOfWork;
        private IRepository<Source> sourcesRepository;

        public SourceService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        private IRepository<Source> SourcesRepository
        {
            get
            {
                if (sourcesRepository == null)
                {
                    sourcesRepository = unitOfWork.GetRepository<Source>();
                }

                return sourcesRepository;
            }
        }

        public async Task<int> GetSourceIdByName(string sourceName)
        {
            var sources = await SourcesRepository.GetAllAsync();
            return sources.First(source => source.Name == sourceName).Id;
        }
    }
}
