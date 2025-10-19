
using Auth.Dto;
using Auth.Exceptions;
using Auth.Repository;
using Auth.Security;
using Microsoft.AspNetCore.Identity;

namespace Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;        
        private readonly IPasswordHasher<string> _passwordHasher;
        private readonly IJwtHelper _jwtHelper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IJwtHelper jwtHelper,
            IPasswordHasher<string> passwordHasher, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<Tokens> Login(LoginCredentialsDto credentials)
        {
            _logger.LogInformation("User login attempt with email address: {Email}", credentials.Email);

            var user = await _userRepository.GetUserByEmail(credentials.Email);
            if (user == null)
            {
                _logger.LogError("Login failed: User with email address {Email} not found.", credentials.Email);
                throw new UnauthorizedException("Wrong email or password");
            }
            if (_passwordHasher.VerifyHashedPassword(String.Empty, user.Password, credentials.Password) != PasswordVerificationResult.Success)
            {
                _logger.LogError("Login failed: User with email address {Email} not found.", credentials.Email);
                throw new UnauthorizedException("Wrong email or password");
            }

            var token = _jwtHelper.GenerateToken(user.GetType().Name, user.Id.ToString(), user.Email, user.Username);

            _logger.LogInformation("User {Email} has successfully logged in.", credentials.Email);

            Tokens response = new Tokens{
                AccessToken = token,
            };
            return response;
        }
    }
}
