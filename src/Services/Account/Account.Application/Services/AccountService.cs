using Account.Application.DTOs;
using Account.Application.Interfaces;
using Account.Domain.Entities;
using Account.Domain.Exceptions;
using Account.Domain.Interfaces;
using Account.Infra.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AccountEntity = Account.Domain.Entities.Account;

namespace Account.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;
        private readonly AccountDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountService(IAccountRepository repository, AccountDbContext context, IConfiguration configuration)
        {
            _repository = repository;
            _context = context;
            _configuration = configuration;
        }

        public async Task<AccountEntity?> GetAccountByIdentifierAsync(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                return null;
            }

            return await _repository.GetByDocumentOrAccountAsync(identifier);
        }

        public async Task<string> CreateAccountAsync(string name, string document, string password)
        {
            var salt = Guid.NewGuid().ToString();
            var hashedPassword = HashPassword(password, salt);

            // Single Account Number Generation. Example 6 digits
            var accountNumber = new Random().Next(100000, 999999).ToString();

            // Creation of the entity mapped to the bank.
            var newAccount = new AccountEntity(accountNumber, name, document, hashedPassword, salt);
            
            await _repository.CreateAsync(newAccount);
            await _context.SaveChangesAsync();

            return newAccount.Number;
        }

        public async Task<string?> LoginAsync(string identifier, string password)
        {
            var account = await GetAccountByIdentifierAsync(identifier);

            // Validate that the account exists and the password matches
            if (account == null || !VerifyPassword(password, account.PasswordHash, account.Salt)) 
            {
                return null;
            }

            // If valid, return JWT
            return GenerateJwtToken(account);

        }

        public string GenerateJwtToken(AccountEntity account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // The key
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    // GUID
                    // (PII - Personally Identifiable Information)
                    new Claim(JwtRegisteredClaimNames.Sub, account.AccountId.ToString()),

                    // JTI is a unique ID for the token (avoids Réplay Attack)
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                    new Claim("AccountNumber", account.Number)
                }),

                Expires = DateTime.UtcNow.AddMinutes(50),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task ProcessTransactionAsync(TransactionRequest request, string? loggedUserAccount)
        {
            string targetAccount = !string.IsNullOrEmpty(request.AccountNumber) ? request.AccountNumber : loggedUserAccount;

            if (string.IsNullOrEmpty(targetAccount))
            {
                throw new BusinessException("Account number not provided.", "INVALID_ACCOUNT");
            }
            
            if (request.Value <= 0)
            {
                throw new BusinessException("Only positive values.", "INVALID_VALUE");
            }

            if (request.Type != "C" && request.Type != "D")
            {
                throw new BusinessException("Type should be C or D.", "INVALID_TYPE");
            }

            var account = await _repository.GetByDocumentOrAccountAsync(targetAccount);

            if (account == null)
            {
                throw new BusinessException("Unregistered account.", "INVALID_ACCOUNT");
            }

            if (!account.IsActive)
            {
                throw new BusinessException("Inactive account.", "INACTIVE_ACCOUNT");
            }

            if(targetAccount != loggedUserAccount && request.Type == "D")
            {
                throw new BusinessException("Only credits is accepted for third party accounts.", "INVALID_TYPE");
            }

            var transaction = new Transaction(request.RequestId, targetAccount, request.Value, request.Type);
            
            await _repository.SaveTransactionAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<BalanceResponse> GetBalanceAsync(string accountNumber)
        {
            var account = await _repository.GetByDocumentOrAccountAsync(accountNumber) 
                ?? throw new BusinessException("Unregistered account.", "INVALID_ACCOUNT");

            if (!account.IsActive)
            {
                throw new BusinessException("Inactive account", "INACTIVE_ACCOUNT");
            }

            var transactions = await _repository.GetTransactionsByAccountAsync(accountNumber);
            var totalCredits = transactions.Where(t => t.TransactionType == "C").Sum(t => t.Amount);
            var totalDebits = transactions.Where(t => t.TransactionType == "D").Sum(t => t.Amount);
            var currentBalance = totalCredits - totalDebits;

            return new BalanceResponse
            {
                AccountNumber = account.Number,
                AccountHolderName = account.Name,
                RequestDate = DateTime.Now,
                CurrentBalance = currentBalance
            };
        }

        public async Task ProcessTransactionAsync(TransactionRequest request)
        {
            // Check if the request has already been processed (Idempotence)
            //var alreadyProcessed = await 
        }

        public string HashPassword(string password, string salt)
        {
            using var sha256 = SHA256.Create();
            var saltedPassword = password + salt;
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));

            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string enteredPassword, string storedHash, string salt)
        {
            // Generates the hash of the entered password using the same salt as the bank
            using var sha256 = SHA256.Create();
            var saltedPassword = enteredPassword + salt;
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            var enteredHash = Convert.ToBase64String(bytes);

            return storedHash == enteredHash;
        }
    }
}
