using Microsoft.AspNetCore.Http;

namespace TechMoveServices.Services
{
    public class FileService
    {
        private readonly string _uploadPath;

        public FileService()
        {
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/contracts");

            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<string> SavePdfAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded.");

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (extension != ".pdf")
                throw new Exception("Only PDF files are allowed.");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/contracts/{fileName}";
        }
    }
}