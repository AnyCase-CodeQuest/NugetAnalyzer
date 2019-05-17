using System.Threading.Tasks;
using NugetAnalyzer.DTOs.Models;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> CreateUserAsync(UserRegisterModel profile);

        Task<UserDTO> GetUserByIdAsync(int userId);
    }
}