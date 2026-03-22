using Account.Api.DTOs;
using Account.Application.Services;
using Account.Domain.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Account.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AccountCreatedResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] CreateAccountRequest request)
        {
            // Document validation
            if (!DocumentValidator.IsValid(request.Document))
            {
                return BadRequest(new
                {
                    message = "The document is invalid.",
                    type = "INVALID_DOCUMENT"
                });
            }

            var accountNumber = await _accountService.CreateAccountAsync(request.Name, request.Document, request.Password);

            return Ok(new AccountCreatedResponse(accountNumber));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _accountService.LoginAsync(request.Identifier, request.Password);
            
            if (token == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid user or password.",
                    tpy = "USER_UNAUTHORIZED"
                });
            }

            return Ok(new LoginResponse(token));
        }

        [HttpGet("{identifier}")]
        [Authorize]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByIdentifier(string identifier)
        {
            var account = await _accountService.GetAccountByIdentifierAsync(identifier);

            if (account == null)
            {
                return NotFound(new { message = "Checking account not found." });
            }

            var response = new AccountResponse(
                account.Number,
                account.Name,
                account.Document,
                account.IsActive
            );
            return Ok(response);
        }
    }
}
