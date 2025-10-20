using Auth.Dto;

namespace Auth.Services
{
    public interface IUserService
    {
        Task<AccountResponse> createAccount(CreateAccountDto accountDto);
    }
}
