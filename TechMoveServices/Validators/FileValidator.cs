using Microsoft.AspNetCore.Http;

namespace TechMoveServices.Validators
{
    public class FileValidator
    {
        private readonly string[] _allowedExtensions =
        {
            ".pdf"
        };

        public void ValidatePdf(IFormFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            var extension =
                Path.GetExtension(file.FileName).ToLower();

            if (!_allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException(
                    "Only PDF files are allowed.");
            }
        }
    }
}