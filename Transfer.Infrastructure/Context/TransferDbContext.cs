using Microsoft.EntityFrameworkCore;

namespace Transfer.Infrastructure.Context
{
    public class TransferDbContext : DbContext
    {
        public TransferDbContext(DbContextOptions<TransferDbContext> options) : base(options) { }
    }
}
