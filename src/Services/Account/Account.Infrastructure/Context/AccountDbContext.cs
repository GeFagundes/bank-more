using Account.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AccountEntity = Account.Domain.Entities.Account;
namespace Account.Infra.Context
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options) { }

        public DbSet<Account.Domain.Entities.Account> Accounts => Set<Account.Domain.Entities.Account>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<Idempotency> IdempotencyAccounts => Set<Idempotency>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountEntity>(builder =>
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

                builder.HasIndex(a => a.Number).IsUnique();
            });

            modelBuilder.Entity<Transaction>(builder =>
            {
                builder.ToTable("transactions");
                builder.HasKey(t => t.TransactionId);
                builder.Property(t => t.TransactionId).HasColumnName("id_transaction");

                builder.HasOne<AccountEntity>()
                       .WithMany()
                       .HasPrincipalKey(a => a.Number)
                       .HasForeignKey(t => t.AccountNumber);

                builder.Property(t => t.AccountNumber)
                      .HasColumnName("account_number")
                      .IsRequired();

                builder.Property(t => t.RequestId)
                       .HasColumnName("request_id")
                       .IsRequired();

                builder.Property(t => t.TransactionType)
                       .HasColumnName("transaction_type")
                       .HasMaxLength(1)
                       .IsFixedLength() // CHAR(1)
                       .IsUnicode(false) // Performance gain by not being Unicode.
                       .IsRequired();

                builder.Property(t => t.Amount)
                       .HasColumnName("amount")
                       .HasColumnType("DECIMAL(18,2)")
                       .IsRequired();

                builder.Property(t => t.CreatedAt)
                       .HasColumnName("transaction_date")
                       .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Idempotency>(builder =>
            {
                builder.ToTable("idempotency_checking_account");
                builder.HasKey(i => i.IdempotencyKey);

                builder.Property(i => i.IdempotencyKey)
                       .HasColumnName("idempotency_key")
                       .IsRequired();

                builder.Property(i => i.RequestBody)
                       .HasColumnName("request_body")
                       .IsRequired();
                builder.Property(i => i.ResponseBody)
                       .HasColumnName("response_body")
                       .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
