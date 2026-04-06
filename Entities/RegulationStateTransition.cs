using System.Text.Json.Serialization;

namespace Backend.Entities;

public class RegulationStateTransition
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [JsonPropertyName("regulation_id")]
    public string RegulationId { get; set; } = string.Empty;
    
    [JsonPropertyName("from_state")]
    public string? FromState { get; set; }
    
    [JsonPropertyName("to_state")]
    public string ToState { get; set; } = string.Empty;
    
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = "system";
    
    [JsonPropertyName("user_role")]
    public string UserRole { get; set; } = "ADMIN";
    
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}
