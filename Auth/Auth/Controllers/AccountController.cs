using Auth.Dto;
using Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        public AccountController(IUserService userService) {
            _userService = userService;
        }

        [HttpPost("")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto accountDto)
        {
            var response = await _userService.createAccount(accountDto);
            return Ok(response);
        }
    }
}
