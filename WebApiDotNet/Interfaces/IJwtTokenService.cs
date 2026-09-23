using WebApiDotNet.Data.Entities;

namespace WebApiDotNet.Interfaces
{
    public interface IJwtTokenService
    {
        Task<string> CreateTokenAsync(UserEntity user);
    }
}
