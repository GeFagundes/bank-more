using Account.Application.Services;
using Account.Domain.Interfaces;
using Account.Infra.Context;
using Account.Infra.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Account.Tests
{
    public class AccountIntegrationTests
    {
        private readonly AccountDbContext _context;
        private readonly IAccountRepository _repository;
        private readonly AccountService _service;

        public AccountIntegrationTests()
        {
            // In memory database.
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(databaseName: "AccountIntegrationDb")
                .Options;

            _context = new AccountDbContext(options);
            _repository = new AccountRepository(_context);
            _service = new AccountService(_repository, _context);
        }

        [Fact]
        public async Task CreateAccountAsync_ShouldPersistInDatabase()
        {
            // Arrange
            var name = "Denzel Test";
            var document = "75857750083";
            var password = "testPassword";

            // Act
            var accountNumber = await _service.CreateAccountAsync(name, document, password);

            // Assert
            // Checks if the service return is as expected
            Assert.False(string.IsNullOrEmpty(accountNumber));

            //Search directly in the database to confirm persistence
            var accountInDb = await _context.Accounts.FirstOrDefaultAsync(a => a.Document == document);

            Assert.NotNull(accountInDb);
            Assert.Equal(name, accountInDb.Name);
            Assert.Equal(accountNumber, accountInDb.Number);
            Assert.True(accountInDb.IsActive);
        }
    }
}
