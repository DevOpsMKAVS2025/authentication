using Auth.Dto;
using Auth.Model;

namespace Auth.Services
{
    public interface IAuthService
    {
        Task<Tokens> Login(LoginCredentialsDto credentials);
    }
}
