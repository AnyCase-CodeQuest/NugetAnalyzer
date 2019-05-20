using System;
using System.Xml;
using System.Threading.Tasks;
using System.Collections.Generic;
using NugetAnalyzer.Common.Interfaces;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DTOs.Models;

namespace NugetAnalyzer.BLL.Services
{
	public class RepositoryAnalyzerService : IRepositoryAnalyzerService
	{
		private const string SolutionSearchPattern = "*.sln";
		private const string CsProjSearchPattern = "*.csproj";
		private const string PackagesConfigSearchPattern = "packages.config";

		private const string CoreAppPackagesXPath = "//Project/ItemGroup/PackageReference";
		private const string FrameworkAppPackagesXPath = "//packages/package";

		private const string PackageVersionAttributeSearchPatternOfCoreApp = "Version";
		private const string PackageNameAttributeSearchPatternOfCoreApp = "Include";
		private const string PackageVersionAttributeSearchPatternOfFrameworkApp = "version";
		private const string PackageNameAttributeSearchPatternOfFrameworkApp = "id";

		private readonly IDirectoryService directoryService;
		private readonly IFileService fileService;

		public RepositoryAnalyzerService(IDirectoryService directoryService, IFileService fileService)
		{
			this.directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
			this.fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
		}

		private enum FrameworkType
		{
			Core = 0,
			Framework = 1
		}

		public async Task<RepositoryDTO> GetParsedRepositoryAsync(string repositoryPath)
		{
			if (!directoryService.Exists(repositoryPath))
			{
				return null;
			}

			RepositoryDTO repositoryDTO = new RepositoryDTO
			{
				Name = directoryService.GetName(repositoryPath),
				Path = repositoryPath
			};

			await AddSolutionsToRepositoryAsync(repositoryDTO);

			return repositoryDTO;
		}

		#region Private Methods

		private async Task AddSolutionsToRepositoryAsync(RepositoryDTO repositoryDTO)
		{
			string[] solutionsFilesPaths = fileService.GetFilesPaths(repositoryDTO.Path, SolutionSearchPattern);

			foreach (string solutionDirectoryPath in fileService.GetFilesDirectoriesPaths(solutionsFilesPaths))
			{
				SolutionDTO solutionDTO = new SolutionDTO
				{
					Name = directoryService.GetName(solutionDirectoryPath),
					Path = solutionDirectoryPath
				};

				await AddProjectsToSolutionAsync(solutionDTO);

				repositoryDTO.Solutions.Add(solutionDTO);
			}
		}

		private async Task AddProjectsToSolutionAsync(SolutionDTO solutionDTO)
		{
			string[] projectsFilesPaths = fileService.GetFilesPaths(solutionDTO.Path, CsProjSearchPattern);

			foreach (string projectDirectoryPath in fileService.GetFilesDirectoriesPaths(projectsFilesPaths))
			{
				ProjectDTO projectDTO = new ProjectDTO
				{
					Name = directoryService.GetName(projectDirectoryPath),
					Path = projectDirectoryPath
				};

				await AddPackagesToProjectAsync(projectDTO);

				solutionDTO.Projects.Add(projectDTO);
			}
		}

		private async Task AddPackagesToProjectAsync(ProjectDTO projectDTO)
		{
			if (fileService.GetFilePath(projectDTO.Path, PackagesConfigSearchPattern) != null)
			{
				string filePath = fileService.GetFilePath(projectDTO.Path, PackagesConfigSearchPattern);

				projectDTO.Packages = await GetPackagesAsync(FrameworkType.Framework, filePath);
			}
			else
			{
				string filePath = fileService.GetFilePath(projectDTO.Path, CsProjSearchPattern);

				projectDTO.Packages = await GetPackagesAsync(FrameworkType.Core, filePath);
			}
		}

		private async Task<ICollection<PackageDTO>> GetPackagesAsync(FrameworkType frameworkType, string filePath)
		{
			List<PackageDTO> packages = new List<PackageDTO>();
			XmlDocument document = new XmlDocument();

			string fileContent = await fileService.GetContentAsync(filePath);
			document.LoadXml(fileContent);

			XmlNodeList nodesList = GetXmlNodeListWithPackages(document, frameworkType);

			AddPackagesToPackagesList(frameworkType, packages, nodesList);

			return packages;
		}

		private XmlNodeList GetXmlNodeListWithPackages(XmlDocument document, FrameworkType frameworkType)
		{
			switch (frameworkType)
			{
				case FrameworkType.Core:
					{
						return document.SelectNodes(CoreAppPackagesXPath);
					}
				case FrameworkType.Framework:
					{
						return document.SelectNodes(FrameworkAppPackagesXPath);
					}
				default:
					{
						return null;
					}
			}
		}

		private void AddPackagesToPackagesList(FrameworkType frameworkType, ICollection<PackageDTO> packages, XmlNodeList nodesList)
		{
			switch (frameworkType)
			{
				case FrameworkType.Core:
					{
						AddPackagesToPackagesListForCoreApp(packages, nodesList);
						break;
					}
				case FrameworkType.Framework:
					{
						AddPackagesToPackagesListForFrameworkApp(packages, nodesList);
						break;
					}
			}
		}

		private void AddPackagesToPackagesListForCoreApp(ICollection<PackageDTO> packages, XmlNodeList nodesList)
		{
			foreach (XmlNode node in nodesList)
			{
				if (node.Attributes[PackageVersionAttributeSearchPatternOfCoreApp] != null)
				{
					packages.Add(
						new PackageDTO
						{
							Name = node.Attributes[PackageNameAttributeSearchPatternOfCoreApp].Value,
							Version = node.Attributes[PackageVersionAttributeSearchPatternOfCoreApp].Value
						});
				}
			}
		}

		private void AddPackagesToPackagesListForFrameworkApp(ICollection<PackageDTO> packages, XmlNodeList nodesList)
		{
			foreach (XmlNode node in nodesList)
			{
				if (node.Attributes[PackageVersionAttributeSearchPatternOfFrameworkApp] != null)
				{
					packages.Add(
						new PackageDTO
						{
							Name = node.Attributes[PackageNameAttributeSearchPatternOfFrameworkApp].Value,
							Version = node.Attributes[PackageVersionAttributeSearchPatternOfFrameworkApp].Value
						});
				}
			}
		}

		#endregion Private Methods
	}
}