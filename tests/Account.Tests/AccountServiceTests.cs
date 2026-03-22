using Account.Application.Services;
using Account.Domain.Interfaces;
using Account.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Account.Tests
{
    public class AccountServiceTests
    {
        private readonly Mock<IAccountRepository> _repositoryMock;
        private readonly AccountService _service;
        private readonly AccountDbContext _context;
        private readonly IConfiguration _configuration;
        
        public AccountServiceTests()
        {
            _repositoryMock = new Mock<IAccountRepository>();

            // In memory database.
            var options = new DbContextOptionsBuilder<AccountDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AccountDbContext(options);

            var myConfiguration = new Dictionary<string, string>
            {
                {"Jwt:Secret", "b2a14bbe-62f7-4f89-9630-88de0ffb0212" },
                {"Jwt:Issuer", "AccountService" },
                {"Jwt:Audience", "AccountServiceExtract" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration!)
                .Build();

            _service = new AccountService(_repositoryMock.Object, _context, _configuration);
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
