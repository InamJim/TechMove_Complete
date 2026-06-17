using Microsoft.AspNetCore.Http;
using Moq;
using TechMoveServices.Validators;
using Xunit;

namespace TechMoveTests.Files
{
    public class FileValidationTests
    {
        [Fact]
        public void ValidatePdf_ShouldAcceptPdf()
        {
            // Arrange
            var validator = new FileValidator();

            var fileMock = new Mock<IFormFile>();

            fileMock.Setup(f => f.FileName)
                    .Returns("agreement.pdf");

            // Act
            var exception = Record.Exception(() =>
                validator.ValidatePdf(fileMock.Object));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void ValidatePdf_ShouldRejectExe()
        {
            // Arrange
            var validator = new FileValidator();

            var fileMock = new Mock<IFormFile>();

            fileMock.Setup(f => f.FileName)
                    .Returns("virus.exe");

            // Act + Assert
            Assert.Throws<InvalidOperationException>(() =>
                validator.ValidatePdf(fileMock.Object));
        }

        [Fact]
        public void ValidatePdf_ShouldRejectNull()
        {
            var validator = new FileValidator();

            Assert.Throws<ArgumentNullException>(() =>
                validator.ValidatePdf(null!));
        }
    }
}