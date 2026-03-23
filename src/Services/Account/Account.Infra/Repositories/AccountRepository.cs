
using Account.Domain.Entities;
using Account.Domain.Interfaces;
using Account.Infra.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;
using AccountEntity = Account.Domain.Entities.Account;



namespace Account.Infra.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountDbContext _context;

        public AccountRepository(AccountDbContext context)
        {
            _context = context;
        }

        public async Task<AccountEntity?> GetByDocumentOrAccountAsync(string number)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Number == number || a.Document == number);
        }

        public async Task CreateAsync(AccountEntity account)
        {
            await _context.Accounts.AddAsync(account);
        }

        public async Task SaveTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
        }

        public async Task<List<Transaction>> GetTransactionsByAccountAsync(string number)
        {
            return await _context.Transactions.Where(t => t.AccountNumber == number).ToListAsync();
        }

        //public async Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(int id)
        //{
        //    return await _context.Transactions.Where(t => t.AccountId == id).ToListAsync();
        //}

        // Changes automatically detected by EF
        //public async Task UpdateAsync(AccountEntity account)
        //{
        //    _context.Accounts.Update(account);
        //    await Task.CompletedTask;
        //}

        //#region Idempotency Methods
        //public async Task<IdempotencyAccount?> GetIdempotencyAsync(string key)
        //{
        //    return await _context.IdempotencyAccounts.FirstOrDefaultAsync(i => i.IdempotencyKey == key);  
        //}

        //public async Task SaveIdempotencyAsync(IdempotencyAccount idempotency)
        //{
        //    await _context.IdempotencyAccounts.AddAsync(idempotency);
        //}
        //#endregion
    }

}
