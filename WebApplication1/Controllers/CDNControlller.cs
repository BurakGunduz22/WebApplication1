using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using WebApplication1.Data;
using WebApplication1.Models;
namespace WebApplication1.Controllers
{
    [Route("cdn")]
    [ApiController]
    // CDN controller
    public class CDNController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public CDNController(IMemoryCache cache, ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _cache = cache;
            _context = context;
            _environment = environment;

        }

        [HttpPost("upload/{id}")]
        public async Task<IActionResult> Upload(List<IFormFile> files, string id)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files uploaded.");

            if (files.Count > 6)
                return BadRequest("You can upload up to 6 files.");

            var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", id);

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var fileMetadataList = new List<FileMetadata>();

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file.Length == 0)
                    continue;

                var uniqueFileName = $"{i}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadPath, uniqueFileName);

                // Dizinin var olup olmadığını kontrol et ve yoksa oluştur
                var directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileMetadata = new FileMetadata
                {
                    FileName = uniqueFileName,
                    FilePath = filePath,
                    UploadedAt = DateTime.UtcNow
                };

                fileMetadataList.Add(fileMetadata);
            }

            _context.FileMetadatas.AddRange(fileMetadataList);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"{fileMetadataList.Count} files uploaded successfully." });
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            // Base directory where files are stored
            string baseDirectory = Path.Combine(_environment.WebRootPath, "uploads", id);

            // Check if the directory exists
            if (!Directory.Exists(baseDirectory))
            {
                return NotFound(); // Directory not found
            }

            // Get the list of files in the directory
            string[] files = Directory.GetFiles(baseDirectory, "*", SearchOption.AllDirectories);

            // Ensure there's at least one file in the directory
            if (files.Length == 0)
            {
                return Ok(files); // No files found in the directory
            }

            var fileUrls = new List<string>();

            foreach (var filePath in files)
            {
                // Get the relative path to use as the response's filename
                string relativePath = Path.GetRelativePath(_environment.WebRootPath, filePath);

                // Check if the file is already in cache
                if (!_cache.TryGetValue(filePath, out byte[]? content))
                {
                    // Load content from disk and cache it
                    content = LoadContentFromDisk(filePath);
                    if (content != null)
                    {
                        _cache.Set(filePath, content, TimeSpan.FromMinutes(30));
                    }
                }

                var fileUrl = Url.Content($"~/{relativePath.Replace("\\", "/")}");
                fileUrls.Add(fileUrl);
            }

            return Ok(new { Urls = fileUrls });
        
    }



        private byte[] LoadContentFromDisk(string filePath)
        {
            try
            {
                return System.IO.File.ReadAllBytes(filePath);
            }
            catch (Exception)
            {   
                return null; // Handle exceptions as necessary
            }
        }
    }
}
