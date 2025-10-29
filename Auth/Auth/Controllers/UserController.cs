using Auth.Dto;
using Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Auth.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService) {
            _userService = userService;
        }

        [HttpGet("/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var response = await _userService.GetUserById(id);
            return Ok(response);
        }

        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }
    }
}
