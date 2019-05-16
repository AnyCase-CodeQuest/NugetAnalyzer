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
        private readonly UserConverter userConverter;
        private IRepository<User> usersRepository;

        public UserService(IUnitOfWork unitOfWork, UserConverter userConverter)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this.userConverter = userConverter ?? throw new ArgumentNullException(nameof(userConverter));
        }

        private IRepository<User> UserRepository
        {
            get
            {
                if (usersRepository == null)
                {
                    usersRepository = unitOfWork.GetRepository<User>();
                }

                return usersRepository;
            }
        }

        public async Task<UserViewModel> CreateUserAsync(UserRegisterModel profile)
        {
            var user = userConverter.ConvertRegisterModelToUser(profile);
            UserRepository.Add(user);
            await unitOfWork.SaveChangesAsync();
            return userConverter.ConvertUserToProfile(user);
        }

        public async Task<UserViewModel> GetUserByIdAsync(int userId)
        {
            var user = await UserRepository.GetByIdAsync(userId);
            return userConverter.ConvertUserToProfile(user);
        }
    }
}