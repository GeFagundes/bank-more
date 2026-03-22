using Account.Application.Services;
using Account.Infra.Context;
using Account.Infra.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Account.Tests
{
    public class AccountSearchTests : IDisposable
    {
        private readonly AccountDbContext _context;
        private readonly AccountRepository _repository;
        private readonly AccountService _service;

        public AccountSearchTests()
        {
            // In memory database.
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context= new AccountDbContext(options);
            _repository = new AccountRepository(_context);
            _service = new AccountService(_repository, _context);
        }

        [Fact]
        public async Task GetAccountByIdentifierAsync_WhenSearchingByAccount_ShouldReturnCorrectAccount()
        {
            // Arrange
            var document = "75857750083";
            var name = "Denzel";

            //Create account and ensures persistence
            var accountNumber = await _service.CreateAccountAsync(name, document, "passwordT332");

            // Act
            var result = await _service.GetAccountByIdentifierAsync(accountNumber);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(accountNumber, result.Number);
            Assert.Equal(document, result.Document);
        }

        [Fact]
        public async Task GetAccountByidentifierAsync_WhenSearchingByDocument_ShouldReturnCorretAccount()
        {
            // Arrange
            var document = "12345678901";
            var name = "Search Test";
            var accountNumber = await _service.CreateAccountAsync(name, document, "password32221");

            // Act
            var result = await _service.GetAccountByIdentifierAsync(document);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(document, result.Document);
            Assert.Equal(accountNumber, result.Number);
        }

        [Fact]
        public async Task GetAccountByIdentifierAsync_WithNonExistentIdentifier_ShouldReturnNull()
        {
            // Act
            var result = await _service.GetAccountByIdentifierAsync("non-existent-id");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAccountByIdentifierAsync_WithEmptyString_ShouldReturnNullImmediately()
        {
            // Act
            var result = await _service.GetAccountByIdentifierAsync("");

            // Assert
            // Validates the guard clause implemented in Service
            Assert.Null(result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
