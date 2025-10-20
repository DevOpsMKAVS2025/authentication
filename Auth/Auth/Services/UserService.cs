using Auth.Dto;
using Auth.Exceptions;
using Auth.Repository;
using Auth.Model;
using Microsoft.AspNetCore.Identity;

namespace Auth.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<string> _passwordHasher;
        public UserService(IUserRepository userRepository, IPasswordHasher<string> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<AccountResponse> createAccount(CreateAccountDto accountDto)
        {
            User? user = await _userRepository.GetUserByEmail(accountDto.Email);
            if (user != null)
            {
                throw new BadRequestException("Email already exists");
            }

            user = await _userRepository.GetUserByUsername(accountDto.Username);
            if (user != null)
            {
                throw new BadRequestException("Username already exists");
            }

            var hashedPassword = _passwordHasher.HashPassword(string.Empty, accountDto.Password);

            User userData = new User {
                Email = accountDto.Email,
                FirstName = accountDto.FirstName,
                LastName = accountDto.LastName,
                Username = accountDto.Username,
                Address = accountDto.Address,
                UserType = (UserType)Enum.Parse(typeof(UserType), accountDto.Type),
                Password = hashedPassword
            };

            User newUser = await _userRepository.Create(userData);
            return new AccountResponse
            {
                Id = newUser.Id.ToString(),
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Address = newUser.Address,
                Email = newUser.Email,
                Username = newUser.Username,
            };
        }
    }
}
