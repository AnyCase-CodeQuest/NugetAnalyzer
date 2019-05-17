using System.Threading.Tasks;
using NugetAnalyzer.Dtos.Models;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> CreateUserAsync(UserRegisterModel profile);

        Task<UserDTO> GetUserByIdAsync(int userId);
    }
}