namespace Account.Application.DTOs
{
    public class BalanceResponse
    {
        public string AccountNumber { get; set; } = null!;
        public string AccountHolderName { get; set; } = null!;
        public DateTime RequestDate { get; set; }
        public decimal CurrentBalance { get; set; }
    }
}
