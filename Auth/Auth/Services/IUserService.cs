using Auth.Dto;

namespace Auth.Services
{
    public interface IUserService
    {
        Task<AccountResponse> createAccount(CreateAccountDto accountDto);
        Task updateProperty(Guid accountId, string property, string value);
        Task<AccountResponse> GetAccountInformation(Guid principaId);
        Task DeleteAccount(Guid principalId);
    }
}
