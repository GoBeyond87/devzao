using Core.Entities;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUserService
    {
        Task<User?> Authenticate(string email, string password);
        Task<User?> GetUserByEmail(string email);
    }
}
