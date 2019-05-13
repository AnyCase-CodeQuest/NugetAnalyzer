using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NugetAnalyzer.Dtos.Converters;
using NugetAnalyzer.Dtos.Models;

namespace NugetAnalyzer.BLL.Services
{
    public class SourceService : ISourceService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IRepository<Source> sourcesRepository;

        public SourceService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            sourcesRepository = unitOfWork.GetRepository<Source>();
        }

        public async Task<ICollection<SourceViewModel>> GetSourceList()
        {
            var sources = await sourcesRepository.GetAllAsync();
            return SourceConverter.ConvertSourceListToViewModelList(sources);
        }
    }
}
