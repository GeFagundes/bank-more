using Account.Domain.Interfaces;
using Account.Infra.Context;
using System.Security.Cryptography;
using System.Text;
using AccountEntity = Account.Domain.Entities.Account;

namespace Account.Application.Services
{
    public class AccountService
    {
        private readonly IAccountRepository _repository;
        private readonly AccountDbContext _context;

        public AccountService(IAccountRepository repository, AccountDbContext context)
        {
            _repository = repository;
            _context = context;
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

        private string HashPassword(string password, string salt)
        {
            using var sha256 = SHA256.Create();
            var saltedPassword = password + salt;
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));

            return Convert.ToBase64String(bytes);
        }
    }
}
