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
        private readonly IRepository<User> usersRepository;

        public UserService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            usersRepository = unitOfWork.GetRepository<User>();
        }

        public async Task CreateUserAsync(ProfileViewModel profile)
        {
            var user = UserConverter.ConvertProfileToUser(profile);
            usersRepository.Add(user);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(ProfileViewModel profile)
        {
            var user = await usersRepository.GetSingleOrDefaultAsync(p => p.GitHubId == profile.GitHubId);
            user.GitHubToken = profile.AccessToken;
            usersRepository.Update(user);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<ProfileViewModel> GetProfileByGitHubIdAsync(int gitHubId)
        {
            var user = await usersRepository.GetSingleOrDefaultAsync(p => p.GitHubId == gitHubId);
            return UserConverter.ConvertUserToProfile(user);
        }

        public async Task<ProfileViewModel> GetProfileByUserNameAsync(string userName)
        {
            var user = await usersRepository.GetSingleOrDefaultAsync(p => p.UserName == userName);
            return UserConverter.ConvertUserToProfile(user);
        }
    }
}