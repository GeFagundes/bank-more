using Account.Api.DTOs;
using Account.Application.DTOs;
using Account.Application.Interfaces;
using Account.Application.Services;
using Account.Domain.Exceptions;
using Account.Domain.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Account.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
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

        [HttpPost("transaction")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ProcessTransaction([FromBody] TransactionRequest request)
        {
            var loggedUserAccount = User.FindFirst("AccountNumber")?.Value;

            try
            {
                await _accountService.ProcessTransactionAsync(request, loggedUserAccount);

                return NoContent();
            }
            catch (BusinessException ex)
            {

                return BadRequest(new { message = ex.Message, type = ex.ErrorCode }); 
            }
        }
    }
}
