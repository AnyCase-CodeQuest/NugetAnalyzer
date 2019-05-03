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
        private readonly IRepository<User> users;

        public UserService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            users = unitOfWork.GetRepository<User>();
        }

        public async Task CreateUserAsync(Profile profile)
        {
            User user = UserConverter.ConvertProfileToUser(profile);
            users.Add(user);
            await unitOfWork.SaveChangesAsync();
        }

        public Profile GetProfileByGitHubId(int gitHubId)
        {
            var user = users.GetSingleOrDefaultAsync(p => p.GitHubId == gitHubId).Result;
            return UserConverter.ConvertUserToProfile(user);
        }

        public Profile GetProfileByUserName(string userName)
        {
            var user = users.GetSingleOrDefaultAsync(p => p.UserName == userName).Result;
            return UserConverter.ConvertUserToProfile(user);
        }
    }
}