using Account.Application.Services;
using Account.Domain.Interfaces;
using Account.Infra.Context;
using Account.Infra.Repositories;
using Account.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Account.Tests
{
    public class AccountIntegrationTests
    {
        private readonly AccountDbContext _context;
        private readonly IAccountRepository _repository;
        private readonly AccountService _service;
        private readonly IConfiguration _configuration;
        private readonly IIdempotencyRepository _idempotencyRepository;

        public AccountIntegrationTests()
        {
            // In memory database.
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(databaseName: "AccountIntegrationDb")
                .Options;

            _context = new AccountDbContext(options);
            _repository = new AccountRepository(_context);

            var myConfiguration = new Dictionary<string, string>
            {
                {"Jwt:Secret", "b2a14bbe-62f7-4f89-9630-88de0ffb0212" },
                {"Jwt:Issuer", "AccountService" },
                {"Jwt:Audience", "AccountServiceExtract" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration!)
                .Build();

            _idempotencyRepository = new IdempotencyRepository(_context);

            _service = new AccountService(_repository, _context, _configuration, _idempotencyRepository);
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
