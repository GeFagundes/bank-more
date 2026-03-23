using Microsoft.EntityFrameworkCore;
using Account.Domain.Entities;
using Account.Domain.Interfaces;
using Account.Infra.Context;

namespace Account.Infrastructure.Repositories
{
    public class IdempotencyRepository : IIdempotencyRepository
    {
        private readonly AccountDbContext _context;
        public IdempotencyRepository(AccountDbContext context)
        {
            _context = context;
        }
        public async Task<Idempotency?> GetByIdAsync(string idempotencyKey)
        {
            return await _context.Idempotencies
                .FirstOrDefaultAsync(i => i.IdempotencyKey == idempotencyKey);
        }

        public async Task SaveAsync(Idempotency idempotency)
        {
            await _context.Idempotencies.AddAsync(idempotency);
        }
    }
}
