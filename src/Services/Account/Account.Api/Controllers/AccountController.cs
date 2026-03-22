using Account.Api.DTOs;
using Account.Application.Services;
using Account.Domain.Validations;
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

        [HttpGet("{identifier}")]
        public async Task<IActionResult> GetByIdentifier(string identifier)
        {
            var account = await _accountService.GetAccountByIdentifierAsync(identifier);

            if (account == null){
                return NotFound(new { message = "Checking account not found."});
            }

            return Ok(account);
        }

        [HttpPost]
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

            return Ok(new { accountNumber });
        }
    }
}
