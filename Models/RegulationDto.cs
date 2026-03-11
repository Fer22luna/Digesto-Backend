namespace Backend.Models;

using System.Text.Json.Serialization;

public class StateTransitionDto
{
    public string? FromState { get; set; }
    public string ToState { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string UserId { get; set; } = "system";
    public string UserRole { get; set; } = "ADMIN";
    public string? Notes { get; set; }
}

public class RegulationDto
{
    public string Id { get; set; } = string.Empty;
    public string SpecialNumber { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string? LegalStatus { get; set; }
    public string? Content { get; set; }
    public List<string>? Keywords { get; set; }
    public DateTime PublicationDate { get; set; }
    public string? FileUrl { get; set; }
    public string? PdfUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateRegulationDto
{
    [JsonPropertyName("specialNumber")]
    public string SpecialNumber { get; set; } = string.Empty;
    
    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = "DECREE";
    
    [JsonPropertyName("legalStatus")]
    public string? LegalStatus { get; set; }
    
    [JsonPropertyName("content")]
    public string? Content { get; set; }
    
    [JsonPropertyName("keywords")]
    public List<string>? Keywords { get; set; }
    
    [JsonPropertyName("publicationDate")]
    public DateTime? PublicationDate { get; set; }
    
    [JsonPropertyName("fileUrl")]
    public string? FileUrl { get; set; }
    
    [JsonPropertyName("pdfUrl")]
    public string? PdfUrl { get; set; }
}

public class UpdateRegulationDto
{
    [JsonPropertyName("specialNumber")]
    public string? SpecialNumber { get; set; }
    
    [JsonPropertyName("reference")]
    public string? Reference { get; set; }
    
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("state")]
    public string? State { get; set; }
    
    [JsonPropertyName("legalStatus")]
    public string? LegalStatus { get; set; }
    
    [JsonPropertyName("content")]
    public string? Content { get; set; }
    
    [JsonPropertyName("keywords")]
    public List<string>? Keywords { get; set; }
    
    [JsonPropertyName("publicationDate")]
    public DateTime? PublicationDate { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; } = true;
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
}
