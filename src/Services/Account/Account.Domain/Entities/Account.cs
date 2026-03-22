namespace Account.Domain.Entities
{
    public class Account
    {
        public int AccountId { get; private set; }
        public string Number { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public string Document { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty ;
        public string Salt { get; private set; } = string .Empty ;
        public bool IsActive { get; private set; }

        public Account() { }
        public Account(string number, string name, string document, string hashPassword, string salt)
        {
            Number = number;
            Name = name;
            Document = document;
            PasswordHash = hashPassword;
            Salt = salt;
            IsActive = true;
        }

        public void Deactivate() => IsActive = false;
    }
}
