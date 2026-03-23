namespace Account.Domain.Entities
{
    public class Transaction
    {
        public int TransactionId { get; private set; }
        public string RequestId { get; private set; }
        public string AccountNumber { get; private set; }
        public decimal Amount { get; private set; }
        public string TransactionType { get; private set; } 
        public DateTimeOffset CreatedAt { get; private set; }

        //EF Core
        private Transaction()
        {
            RequestId = null!;
            AccountNumber = null!;
            TransactionType = null!;
        }

        public Transaction(string requestId, string accountNumber, decimal amount, string type)
        {
            RequestId = requestId;
            AccountNumber = accountNumber;
            Amount = amount;
            TransactionType = type;
            CreatedAt = DateTimeOffset.UtcNow;
        }
    }
}
