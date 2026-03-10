using System.Text.Json;
using Backend.Entities;

namespace Backend.Data;

public class SupabaseConnection
{
    private readonly string _supabaseUrl;
    private readonly string _supabaseKey;
    private readonly HttpClient _httpClient;
    private readonly ILogger<SupabaseConnection> _logger;

    // JSON serialization options - uses [JsonPropertyName] attributes from entity classes
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = false,
        PropertyNameCaseInsensitive = true
    };

    public SupabaseConnection(string supabaseUrl, string supabaseKey, HttpClient httpClient, ILogger<SupabaseConnection> logger)
    {
        _supabaseUrl = supabaseUrl;
        _supabaseKey = supabaseKey;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<T?> QueryAsync<T>(string table, string? filter = null) where T : class
    {
        try
        {
            var url = $"{_supabaseUrl}/rest/v1/{table}";
            if (!string.IsNullOrEmpty(filter))
                url += $"?{filter}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("apikey", _supabaseKey);
            request.Headers.Add("Authorization", $"Bearer {_supabaseKey}");

            var response = await _httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Supabase error: {response.StatusCode}");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"QueryAsync response (length: {content.Length}): {content}");
            
            // Handle empty response
            if (string.IsNullOrWhiteSpace(content) || content == "[]")
            {
                return null;
            }
            
            // Supabase returns an array, even for single queries
            // Try to deserialize as a list and return the first element
            try
            {
                var result = JsonSerializer.Deserialize<List<T>>(content, _jsonOptions);
                return result?.FirstOrDefault();
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError($"JSON deserialization failed for List<{typeof(T).Name}>: {jsonEx.Message}. Content: {content}");
                // If that fails, try as a single object
                try
                {
                    return JsonSerializer.Deserialize<T>(content, _jsonOptions);
                }
                catch (JsonException singleEx)
                {
                    _logger.LogError($"JSON deserialization also failed for single {typeof(T).Name}: {singleEx.Message}");
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Supabase query error: {ex.Message}");
            return null;
        }
    }

    public async Task<List<T>> QueryListAsync<T>(string table, string? filter = null) where T : class
    {
        try
        {
            var url = $"{_supabaseUrl}/rest/v1/{table}";
            if (!string.IsNullOrEmpty(filter))
                url += $"?{filter}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("apikey", _supabaseKey);
            request.Headers.Add("Authorization", $"Bearer {_supabaseKey}");

            var response = await _httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Supabase error: {response.StatusCode}");
                return new List<T>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<T>>(content);
            return result ?? new List<T>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Supabase query error: {ex.Message}");
            return new List<T>();
        }
    }

    public async Task<T?> InsertAsync<T>(string table, T entity) where T : class
    {
        try
        {
            var url = $"{_supabaseUrl}/rest/v1/{table}";
            var json = JsonSerializer.Serialize(entity, _jsonOptions);
            
            // Log the serialized JSON for debugging
            _logger.LogInformation($"Inserting into {table}: {json}");
            
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            request.Headers.Add("apikey", _supabaseKey);
            request.Headers.Add("Authorization", $"Bearer {_supabaseKey}");
            // Tell Supabase to return the inserted record
            request.Headers.Add("Prefer", "return=representation");

            var response = await _httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Supabase insert error: {response.StatusCode} - {errorContent}");
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Insert response: {responseContent}");
            
            // Handle empty response (should not occur with Prefer: return=representation)
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                _logger.LogWarning("Empty response from Supabase insert, returning original entity");
                return entity;
            }
            
            // Supabase returns an array, so deserialize as List<T> and get first element
            var result = JsonSerializer.Deserialize<List<T>>(responseContent, _jsonOptions);
            return result?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Supabase insert error: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateAsync<T>(string table, string id, T entity) where T : class
    {
        try
        {
            var url = $"{_supabaseUrl}/rest/v1/{table}?id=eq.{id}";
            var json = JsonSerializer.Serialize(entity, _jsonOptions);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = content
            };
            request.Headers.Add("apikey", _supabaseKey);
            request.Headers.Add("Authorization", $"Bearer {_supabaseKey}");
            request.Headers.Add("Prefer", "return=representation");

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Supabase update error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteAsync(string table, string id)
    {
        try
        {
            var url = $"{_supabaseUrl}/rest/v1/{table}?id=eq.{id}";
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.Headers.Add("apikey", _supabaseKey);
            request.Headers.Add("Authorization", $"Bearer {_supabaseKey}");

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Supabase delete error: {ex.Message}");
            return false;
        }
    }
}
