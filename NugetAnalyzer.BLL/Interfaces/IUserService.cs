using System.Threading.Tasks;
using NugetAnalyzer.Dtos.Models;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserViewModel> CreateUserAsync(UserRegisterModel profile);

        Task<UserViewModel> GetUserByIdAsync(int userId);
    }
}