using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProxyController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private const string RemoteHeaderUrl = "http://10.0.0.24/header/";

    public ProxyController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet("header/css")]
    public async Task<IActionResult> GetHeaderCss()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{RemoteHeaderUrl}header.css");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "text/css");
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpGet("header/html")]
    public async Task<IActionResult> GetHeaderHtml()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{RemoteHeaderUrl}index.html");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "text/html");
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpGet("header/js")]
    public async Task<IActionResult> GetHeaderJs()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{RemoteHeaderUrl}header.js");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "text/javascript");
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpGet("header/image")]
    public async Task<IActionResult> GetHeaderImage(string path)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{RemoteHeaderUrl}{path}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsByteArrayAsync();
                var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
                return File(content, contentType);
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }
}
