using WebApplication1.Services;

namespace ServiceTests
{
    public class TokenServiceTests
    {
        [Fact]
        public void GenerateToken_ValidUsername_ReturnsValidToken()
        {
            // Arrange
            var username = "testuser";

            // Act
            var token = TokenService.GenerateToken(username);

            // Assert
            Assert.False(string.IsNullOrEmpty(token));
            Assert.True(token.Split(':').Length == 3);
        }

        [Fact]
        public void IsTokenValid_ValidToken_ReturnsTrue()
        {
            // Arrange
            var username = "validuser";
            var token = TokenService.GenerateToken(username);

            // Act
            var isValid = TokenService.IsTokenValid(token);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void IsTokenValid_InvalidSignature_ReturnsFalse()
        {
            // Arrange
            var username = "validuser";
            var token = TokenService.GenerateToken(username);

            var tokenParts = token.Split(':');
            var tamperedToken = $"{tokenParts[0]}:{tokenParts[1]}:invalidsignature";

            // Act
            var isValid = TokenService.IsTokenValid(tamperedToken);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void IsTokenValid_MalformedToken_ReturnsFalse()
        {
            // Arrange
            var malformedToken = "this:is:malformed";

            // Act
            var isValid = TokenService.IsTokenValid(malformedToken);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void IsTokenValid_TokenWithIncorrectPartsCount_ReturnFalse()
        {
            // Arrange
            var incorrectToken = "onlytwo:parts";

            // Act
            var isValid = TokenService.IsTokenValid(incorrectToken);

            // Assert
            Assert.False(isValid);
        }
    }
}