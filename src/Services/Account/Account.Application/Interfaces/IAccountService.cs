using Account.Application.DTOs;
using AccountEntity = Account.Domain.Entities.Account;

namespace Account.Application.Interfaces
{
   public interface IAccountService
    {
        Task<AccountEntity?> GetAccountByIdentifierAsync(string identifier);
        Task<string> CreateAccountAsync(string name, string document, string password);
        Task<string?> LoginAsync(string identifier, string password);
        string GenerateJwtToken(AccountEntity account);
        Task ProcessTransactionAsync(TransactionRequest request, string? loggedUserAccount);
        Task<BalanceResponse> GetBalanceAsync(string accountNumber);
        string HashPassword(string password, string salt);
    }
   
}
