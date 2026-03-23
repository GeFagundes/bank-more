using Account.Application.Services;
using Account.Domain.Interfaces;
using Account.Infra.Context;
using Account.Infra.Repositories;
using Account.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AccountEntity = Account.Domain.Entities.Account;

namespace Account.Tests
{
    public class LoginTests : IDisposable
    {
        private readonly AccountDbContext _context;
        private readonly IAccountRepository _repository;
        private readonly AccountService _service;
        private readonly IConfiguration _configuration;
        private readonly IIdempotencyRepository _idempotencyRepository;

        public LoginTests()
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
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var password = "testPassword";
            var salt = Guid.NewGuid().ToString();
            var hash = _service.HashPassword(password, salt);

            var account = new AccountEntity("906916", "Denzel Test", "75857750083", hash, salt);

            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();

            // Act
            var token =  await _service.LoginAsync("906916", password);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsIncorrect()
        {
            // Arrange
            var salt = Guid.NewGuid().ToString();
            var hash = _service.HashPassword("correct_password", salt);
            var account = new AccountEntity("906916", "Denzel Test", "75857750083", hash, salt);

            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();

            // Act
            var token = await _service.LoginAsync("906916", "wrong_password");

            // Assert
            Assert.Null(token);
        }

        public async Task LoginAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Act
            var token = await _service.LoginAsync("9999", "any_password");

            // Assert
            Assert.Null(token);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
