namespace Account.Api.DTOs
{
    public record CreateAccountRequest(
        string Name,
        string Document,
        string Password
    );
}
