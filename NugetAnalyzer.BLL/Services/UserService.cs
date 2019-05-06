using System;
using System.Threading.Tasks;
using NugetAnalyzer.BLL.Converters;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Models;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserRepository userRepository;

        public UserService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            userRepository = unitOfWork.GetRepository<IUserRepository>();
        }

        public async Task CreateUserAsync(Profile profile, string gitHubToken)
        {
            User user = UserConverter.ConvertProfileToUser(profile);
            user.GitHubToken = gitHubToken;
            userRepository.Add(user);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateGitHubToken(int gitHubId, string gitHubToken)
        {
            var user = await userRepository.GetSingleOrDefaultAsync(p => p.GitHubId == gitHubId);
            user.GitHubToken = gitHubToken;
            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync();
        }

        public Profile GetProfileByGitHubId(int gitHubId)
        {
            var user = userRepository.GetSingleOrDefaultAsync(p => p.GitHubId == gitHubId).Result;
            return UserConverter.ConvertUserToProfile(user);
        }

        public Profile GetProfileByUserName(string userName)
        {
            var user = userRepository.GetSingleOrDefaultAsync(p => p.UserName == userName).Result;
            return UserConverter.ConvertUserToProfile(user);
        }

        public string GetGitHubTokenByGitHubId(int gitHubId)
        {
            return userRepository.GetGitHubTokenByGitHubId(gitHubId);
        }
    }
}