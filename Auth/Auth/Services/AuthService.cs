
using Auth.Dto;
using Auth.Exceptions;
using Auth.Repository;
using Auth.Security;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using System.Text.Json;

namespace Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;        
        private readonly IPasswordHasher<string> _passwordHasher;
        private readonly IJwtHelper _jwtHelper;
        private readonly ILogger<AuthService> _logger;
        private readonly IConnectionMultiplexer _redis;

        public AuthService(IUserRepository userRepository, IJwtHelper jwtHelper,
            IPasswordHasher<string> passwordHasher, ILogger<AuthService> logger,
            IConnectionMultiplexer redis)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _redis = redis;
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
                RefreshToken = token,
            };
            return response;
        }

        private async Task<Object> GenerateRefreshToken()
        {
            string token = AuthService.GenerateRandomString(128);
            string hashedToken = _passwordHasher.HashPassword(String.Empty, token);
            var expiration = DateTime.Now.AddDays(7);
            string tokenId = Guid.NewGuid().ToString();

            var db = _redis.GetDatabase();
            string key = $"refresh_token:{tokenId}";

            await db.StringSetAsync(key, token, TimeSpan.FromDays(7));

            return new
            {
                Id = tokenId,
                Token = token,
                ExpiresAt = expiration
            };
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
