using Bytehive.Data.Models;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts.Services
{
    public interface IUsersService
    {
        Task<User> GetUser(string email);

        Task<bool> Create(User user);

    }
}