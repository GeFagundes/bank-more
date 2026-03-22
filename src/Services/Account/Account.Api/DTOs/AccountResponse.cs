namespace Account.Api.DTOs
{
    public record AccountResponse(
        string Number,
        string Name,
        string Document,
        bool IsActive
    );
    
}
