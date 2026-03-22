using Account.Domain.Enums;
using Account.Domain.Exceptions;

namespace Account.Domain.Entities
{
    public class Transaction
    {
        public int TransactionId { get; private set; }
        public int AccountId { get; private set; }
        public DateTimeOffset TransactionDate { get; private set; }
        public TransactionType TransactionType { get; private set; }
        public decimal Amount {  get; private set; }

        //EF Core
        private Transaction() { }

        public Transaction(int accountId, TransactionType transactionType, decimal amount)
        {
            if (amount <= 0)
            {
                throw new DomainException("INVALID_VALUE: Amount must be greater than zero.");
            }

            if (!Enum.IsDefined(typeof(TransactionType), transactionType))
            {
                throw new DomainException("INVALID_TRANSACTION_TYPE");
            }

            AccountId = accountId;
            TransactionDate = DateTimeOffset.UtcNow;
            Amount = amount;
        }
    }
}
