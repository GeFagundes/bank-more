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
    public class AccountService
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
                }),

                Expires = DateTime.UtcNow.AddMinutes(5),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
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
