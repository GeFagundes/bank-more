namespace Account.Application.DTOs
{
    public class StatementResponse
    {
        public string AccountNumber { get; set; } = null!;
        public decimal CurrentBalance { get; set; }
        public List<TransactionItem> History { get; set; } = new();
    }
}
