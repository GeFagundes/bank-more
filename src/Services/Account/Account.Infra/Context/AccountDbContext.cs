using Account.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Account.Infra.Context
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options) { }

        public DbSet<Account.Domain.Entities.Account> Accounts => Set<Account.Domain.Entities.Account>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<IdempotencyAccount> IdempotencyAccounts => Set<IdempotencyAccount>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account.Domain.Entities.Account>(builder =>
            {
                builder.ToTable("checking_account");
                builder.HasKey(a => a.AccountId);
                builder.Property(a => a.AccountId).HasColumnName("id_checking_account");

                builder.Property(a => a.Number).HasColumnName("account_number").IsRequired();
                builder.Property(a => a.Name).HasColumnName("name").IsRequired();
                builder.Property(a => a.Document).HasColumnName("document").IsRequired();
                builder.Property(a => a.IsActive).HasColumnName("is_active");
                builder.Property(a => a.PasswordHash).HasColumnName("password").IsRequired();
                builder.Property(a => a.Salt).HasColumnName("salt").IsRequired();
            });

            modelBuilder.Entity<Transaction>(builder =>
            {
                builder.ToTable("transactions");
                builder.HasKey(t => t.TransactionId);
                builder.Property(t => t.TransactionId).HasColumnName("id_transaction");

                builder.Property(t => t.TransactionId).HasColumnName("id_transaction");
                builder.Property(t => t.AccountId).HasColumnName("id_checking_account").IsRequired();
                builder.Property(t => t.TransactionType)
                       .HasColumnName("transaction_type")
                       .HasConversion<string>()
                       .IsRequired();
                builder.Property(t => t.Amount)
                       .HasColumnName("amount")
                       .HasColumnType("DECIMAL(18,2)")
                       .IsRequired();
            });

            modelBuilder.Entity<IdempotencyAccount>(builder =>
            {
                builder.ToTable("idempotency_checking_account");
                builder.HasKey(i => i.IdempotencyKey);

                builder.Property(i => i.IdempotencyKey).HasColumnName("idempotency_key").IsRequired();
                builder.Property(i => i.RequestBody).HasColumnName("request_body");
                builder.Property(i => i.ResponseBody).HasColumnName("response_body");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
