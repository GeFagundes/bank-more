namespace Account.Application.DTOs
{
    public class TransactionItem
    {
        public string RequestId { get; set; } = null!;
        public string Type { get; set; } = null!;
        public decimal Value { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
