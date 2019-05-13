using System;
using System.Threading.Tasks;
using NugetAnalyzer.Dtos.Converters;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Dtos.Models;
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

        public async Task<UserViewModel> CreateUserAsync(UserRegisterModel profile)
        {
            var user = UserConverter.ConvertRegisterModelToUser(profile);
            usersRepository.Add(user);
            await unitOfWork.SaveChangesAsync();
            return UserConverter.ConvertUserToProfile(user);
        }

        public async Task<UserViewModel> GetUserByIdAsync(int userId)
        {
            var user = await usersRepository.GetByIdAsync(userId);
            return UserConverter.ConvertUserToProfile(user);
        }
    }
}