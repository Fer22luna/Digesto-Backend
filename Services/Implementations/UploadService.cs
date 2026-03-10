namespace Backend.Services.Implementations;

public class UploadService : IUploadService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<UploadService> _logger;
    private readonly string _supabaseUrl;
    private readonly string _serviceRoleKey;
    private const string BUCKET_NAME = "regulations";

    public UploadService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<UploadService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _supabaseUrl = configuration["Supabase:Url"] ?? "https://your-supabase-url.supabase.co";
        // Use ServiceRoleKey for backend uploads (bypasses RLS)
        _serviceRoleKey = configuration["Supabase:ServiceRoleKey"] ?? "your-service-role-key";
    }

    public async Task<string> UploadPdfAsync(Stream fileStream, string fileName)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            
            // Create the upload URL with path inside the bucket
            // sanitize/encode the file name because Supabase storage keys cannot contain spaces or other invalid characters
            var safeName = Uri.EscapeDataString(fileName);
            var uploadUrl = $"{_supabaseUrl}/storage/v1/object/{BUCKET_NAME}/regulations/{safeName}";
            
            _logger.LogInformation($"Uploading to: {uploadUrl}");
            
            // Read the file stream into a ByteArrayContent
            var content = new StreamContent(fileStream);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");

            // Create the request
            var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl)
            {
                Content = content
            };

            // Use Authorization header with service role key for backend uploads
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _serviceRoleKey);

            // Upload the file
            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Supabase upload error: {response.StatusCode} - {errorContent}");
                throw new Exception($"Failed to upload file to Supabase: {response.StatusCode}");
            }

            // Return the public URL with correct path (use encoded name)
            var publicUrl = $"{_supabaseUrl}/storage/v1/object/public/{BUCKET_NAME}/regulations/{safeName}";
            
            _logger.LogInformation($"File uploaded successfully: {publicUrl}");
            return publicUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error uploading PDF to Supabase: {ex.Message}");
            throw;
        }
    }
}
