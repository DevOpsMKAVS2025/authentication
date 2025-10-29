using Auth.Dto;

namespace Auth.Services
{
    public interface IUserService
    {
        Task<AccountResponse> createAccount(CreateAccountDto accountDto);
        Task UpdateProperty(Guid accountId, string property, string value);
        Task<AccountResponse> GetAccountInformation(Guid principaId);
        Task DeleteAccount(Guid principalId);
        Task<IEnumerable<AccountResponse>> GetAllUsers();
        Task<AccountResponse> GetUserById(Guid id);
    }
}
