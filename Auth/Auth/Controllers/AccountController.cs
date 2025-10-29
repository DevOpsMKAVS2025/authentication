using Auth.Dto;
using Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpPut("")]
        [Authorize]
        public async Task<IActionResult> UpdateProperty([FromBody] UpdateValueDto valueDto)
        {
            string principalId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _userService.UpdateProperty(Guid.Parse(principalId), valueDto.Property, valueDto.Value);

            return NoContent();
        }

        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> GetAccountInformation()
        {
            string principalId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var response = await _userService.GetAccountInformation(Guid.Parse(principalId));
            return Ok(response);
        }

        [HttpDelete("")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            string principalId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _userService.DeleteAccount(Guid.Parse(principalId));
            return NoContent();
        }
    }
}
