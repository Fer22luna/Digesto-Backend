using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadsController : ControllerBase
{
    private readonly IUploadService _uploadService;
    private readonly ILogger<UploadsController> _logger;

    public UploadsController(IUploadService uploadService, ILogger<UploadsController> logger)
    {
        _uploadService = uploadService;
        _logger = logger;
    }

    /// <summary>
    /// Upload a regulation PDF file to Supabase Storage
    /// </summary>
    [HttpPost("regulation-file")]
    public async Task<IActionResult> UploadRegulationFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "No file provided" });
        }

        if (file.ContentType != "application/pdf")
        {
            return BadRequest(new { error = "Only PDF files are allowed" });
        }

        if (file.Length > 50 * 1024 * 1024) // 50MB limit
        {
            return BadRequest(new { error = "File size exceeds 50MB limit" });
        }

        try
        {
            using (var stream = file.OpenReadStream())
            {
                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var url = await _uploadService.UploadPdfAsync(stream, fileName);

                return Ok(new
                {
                    url = url,
                    fileName = file.FileName,
                    size = file.Length
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error uploading file: {ex.Message}");
            return StatusCode(500, new { error = "Error uploading file", message = ex.Message });
        }
    }
}
