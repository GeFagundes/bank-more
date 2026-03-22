using Account.Application.Services;
using Account.Domain.Interfaces;
using Account.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Account.Tests
{
    public class AccountServiceTests
    {
        private readonly Mock<IAccountRepository> _repositoryMock;
        private readonly AccountService _service;
        private readonly AccountDbContext _context;

        public AccountServiceTests()
        {
            _repositoryMock = new Mock<IAccountRepository>();

            // In memory database.
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AccountDbContext(options);
            _service = new AccountService(_repositoryMock.Object, _context);
        }

        [Fact]
        public async Task CreateAccountAsync_WithValidData_ShouldReturnAccountNumber()
        {
            // Arrange
            var document = "12345678901";
            var password = "PasswordSafe332";
            var name = "Denzel Test";

            // Act
            var result = await _service.CreateAccountAsync(name, document, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Length);

            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Domain.Entities.Account>()), Times.Once);
        }

        [Fact]
        public async Task GetAccountByIdentifierAsync_WithEmptyIdentifier_ShouldReturnNull()
        {
            // Act
            var result = await _service.GetAccountByIdentifierAsync("");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAccountByIdentifierAsync_ShouldSearchByDocumentOrAccount()
        {
            // Arrage
            var identifier = "75857750083";

            _repositoryMock.Setup(r => r.GetByDocumentOrAccountAsync(identifier))
                .ReturnsAsync(new Domain.Entities.Account());

            // Act
            await _service.GetAccountByIdentifierAsync(identifier);

            // Assert
            _repositoryMock.Verify(r => r.GetByDocumentOrAccountAsync(identifier), Times.Once);
        }
    }
}
