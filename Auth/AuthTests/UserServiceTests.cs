using System;
using System.Threading.Tasks;
using Auth.Dto;
using Auth.Exceptions;
using Auth.Model;
using Auth.Repository;
using Auth.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace AuthTests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly Mock<IPasswordHasher<string>> _mockHasher;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _mockHasher = new Mock<IPasswordHasher<string>>();
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.Setup(c => c["BookingServiceUrl"]).Returns("http://localhost:5112");

            _service = new UserService(
                _mockRepo.Object,
                _mockHasher.Object,
                _mockConfiguration.Object
            );
        }

        [Fact]
        public async Task CreateAccount_ShouldThrow_WhenEmailExists()
        {
            var dto = new CreateAccountDto
            {
                Email = "test@test.com",
                Username = "user1",
                FirstName = "John",
                LastName = "Doe",
                Password = "password",
                Address = "123 Street",
                Type = "Guest"
            };

            _mockRepo.Setup(r => r.GetUserByEmail(dto.Email))
                     .ReturnsAsync(new User
                     {
                         Id = Guid.NewGuid(),
                         Email = "existing@test.com",
                         Username = "existingUser",
                         FirstName = "Existing",
                         LastName = "User",
                         Password = "hashed",
                         Address = "Some address",
                         UserType = UserType.Guest
                     });

            await Assert.ThrowsAsync<BadRequestException>(() => _service.createAccount(dto));
        }

        [Fact]
        public async Task CreateAccount_ShouldThrow_WhenUsernameExists()
        {
            var dto = new CreateAccountDto
            {
                Email = "new@test.com",
                Username = "user1",
                FirstName = "John",
                LastName = "Doe",
                Password = "password",
                Address = "123 Street",
                Type = "Guest"
            };

            _mockRepo.Setup(r => r.GetUserByEmail(dto.Email)).ReturnsAsync((User?)null);
            _mockRepo.Setup(r => r.GetUserByUsername(dto.Username))
                     .ReturnsAsync(new User
                     {
                         Id = Guid.NewGuid(),
                         Email = "other@test.com",
                         Username = "user1",
                         FirstName = "Existing",
                         LastName = "User",
                         Password = "hashed",
                         Address = "Some address",
                         UserType = UserType.Guest
                     });

            await Assert.ThrowsAsync<BadRequestException>(() => _service.createAccount(dto));
        }

        [Fact]
        public async Task CreateAccount_ShouldReturnAccountResponse_WhenSuccess()
        {
            var dto = new CreateAccountDto
            {
                Email = "new@test.com",
                Username = "user1",
                FirstName = "John",
                LastName = "Doe",
                Password = "password",
                Address = "123 St",
                Type = "Guest"
            };

            _mockRepo.Setup(r => r.GetUserByEmail(dto.Email)).ReturnsAsync((User?)null);
            _mockRepo.Setup(r => r.GetUserByUsername(dto.Username)).ReturnsAsync((User?)null);
            _mockHasher.Setup(h => h.HashPassword(It.IsAny<string>(), It.IsAny<string>()))
                       .Returns("hashedPassword");

            var createdUser = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Username = dto.Username,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Address = dto.Address,
                Password = "hashedPassword",
                UserType = UserType.Guest
            };

            _mockRepo.Setup(r => r.Create(It.IsAny<User>())).ReturnsAsync(createdUser);

            var result = await _service.createAccount(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.Email, result.Email);
            Assert.Equal(dto.Username, result.Username);
            Assert.Equal(dto.FirstName, result.FirstName);
            Assert.Equal(dto.LastName, result.LastName);
            Assert.Equal(dto.Address, result.Address);
            Assert.Equal(createdUser.Id.ToString(), result.Id);
        }
    }
}
