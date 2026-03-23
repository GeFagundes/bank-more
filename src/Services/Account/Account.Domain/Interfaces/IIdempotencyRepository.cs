
using Account.Domain.Entities;

namespace Account.Domain.Interfaces
{
    public interface IIdempotencyRepository
    {
        Task<Idempotency?> GetByIdAsync(string idempotencyKey);
        Task SaveAsync(Idempotency idempotency);
    }
}
