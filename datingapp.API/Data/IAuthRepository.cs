using System.Threading.Tasks;
using datingapp.API.Models;

namespace datingapp.API.Data
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password); 
        Task<User> Login(string  UserName, string password); 
        Task<bool> IsUserExists(string  UserName); 



    }
}