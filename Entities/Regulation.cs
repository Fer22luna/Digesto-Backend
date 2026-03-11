using System.Text.Json.Serialization;

namespace Backend.Entities;

public class Regulation
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [JsonPropertyName("special_number")]
    public string SpecialNumber { get; set; } = string.Empty;
    
    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = "DECREE"; // DECREE, RESOLUTION, ORDINANCE, TRIBUNAL_RESOLUTION, BID
    
    [JsonPropertyName("state")]
    public string State { get; set; } = "DRAFT"; // DRAFT, REVIEW, PUBLISHED, ARCHIVED
    
    [JsonPropertyName("legal_status")]
    public string? LegalStatus { get; set; } = "SIN_ESTADO"; // VIGENTE, PARCIAL, SIN_ESTADO
    
    [JsonPropertyName("content")]
    public string? Content { get; set; }
    
    [JsonPropertyName("keywords")]
    public List<string>? Keywords { get; set; } = new();
    
    [JsonPropertyName("publication_date")]
    public DateTime PublicationDate { get; set; } = DateTime.UtcNow;
    
    [JsonPropertyName("file_url")]
    public string? FileUrl { get; set; }
    
    [JsonPropertyName("pdf_url")]
    public string? PdfUrl { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
