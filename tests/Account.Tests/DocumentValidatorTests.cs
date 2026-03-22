using Account.Domain.Validations;

namespace Account.Tests
{
    public class DocumentValidatorTests
    {
        [Theory]
        [InlineData("12345678909")]
        [InlineData("75857750083")]
        public void IsValid_WithValidDocument_ShouldReturnTrue(string document)
        {
            // Act
            var result = DocumentValidator.IsValid(document);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("11111111111")]
        [InlineData("12345678900")] //Wrong check digit
        [InlineData("3321")]
        [InlineData("")]
        public void IsValid_WithInvalidDocument_ShouldReturnFalse(string document)
        {
            // Act
            var result = DocumentValidator.IsValid(document);

            // Assert
            Assert.False(result);
        }
    }
}
