using Unilevel.Models;

namespace Unilevel.Services
{
    public interface IUserRepository
    {
        public Task CreateUserAsync(AddUserDTO user);
        public Task<byte[]> LoginAsync(UserLoginDTO user);
    }
}
