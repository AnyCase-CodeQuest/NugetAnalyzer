using System;
using System.Threading.Tasks;
using NugetAnalyzer.DTOs.Converters;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DTOs.Models;
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

        public async Task<UserDTO> CreateUserAsync(UserRegisterModel profile)
        {
            var user = userConverter.ConvertRegisterModelToUser(profile);
            UserRepository.Add(user);
            await unitOfWork.SaveChangesAsync();
            return userConverter.ConvertUserToDTO(user);
        }

        public async Task<UserDTO> GetUserByIdAsync(int userId)
        {
            var user = await UserRepository.GetByIdAsync(userId);
            return userConverter.ConvertUserToDTO(user);
        }
    }
}