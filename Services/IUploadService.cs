namespace Backend.Services;

public interface IUploadService
{
    /// <summary>
    /// Upload a PDF file to Supabase Storage
    /// </summary>
    /// <param name="fileStream">The file stream to upload</param>
    /// <param name="fileName">The file name to save as</param>
    /// <returns>The public URL of the uploaded file</returns>
    Task<string> UploadPdfAsync(Stream fileStream, string fileName);
}
