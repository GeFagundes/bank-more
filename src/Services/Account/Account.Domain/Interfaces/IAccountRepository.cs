using Account.Domain.Entities;

namespace Account.Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<Entities.Account?> GetByDocumentOrAccountAsync(string number);
        Task CreateAsync(Account.Domain.Entities.Account account);
        Task SaveTransactionAsync(Transaction transaction);

        Task<List<Transaction>> GetTransactionsByAccountAsync(string number);
        //Task UpdateAsync(Account.Domain.Entities.Account account);
        //Task<IdempotencyAccount?> GetIdempotencyAsync(string key);
        //Task SaveIdempotencyAsync(IdempotencyAccount idempotency);
    }
}
