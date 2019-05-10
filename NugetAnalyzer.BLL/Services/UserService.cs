using System;
using System.Threading.Tasks;
using NugetAnalyzer.BLL.Converters;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Models;
using NugetAnalyzer.DAL.Interfaces;

namespace NugetAnalyzer.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUsersRepository usersRepository;

        public UserService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            usersRepository = unitOfWork.UsersRepository;
        }

        public async Task CreateUserAsync(Profile profile, string gitHubToken)
        {
            var user = UserConverter.ConvertProfileToUser(profile);
            user.GitHubToken = gitHubToken;
            usersRepository.Add(user);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateGitHubTokenAsync(int gitHubId, string gitHubToken)
        {
            var user = await usersRepository.GetSingleOrDefaultAsync(p => p.GitHubId == gitHubId);
            user.GitHubToken = gitHubToken;
            usersRepository.Update(user);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<Profile> GetProfileByGitHubIdAsync(int gitHubId)
        {
            var user = await usersRepository.GetSingleOrDefaultAsync(p => p.GitHubId == gitHubId);
            return UserConverter.ConvertUserToProfile(user);
        }

        public async Task<Profile> GetProfileByUserNameAsync(string userName)
        {
            var user = await usersRepository.GetSingleOrDefaultAsync(p => p.UserName == userName);
            return UserConverter.ConvertUserToProfile(user);
        }

        public Task<string> GetGitHubTokenByGitHubIdAsync(int gitHubId)
        {
            return usersRepository.GetGitHubTokenByGitHubId(gitHubId);
        }
    }
}