using Backend.Data;
using Backend.Entities;
using Backend.Models;
using System.Text.Json;

namespace Backend.Services.Implementations;

public class RegulationService : IRegulationService
{
    private readonly SupabaseConnection _supabase;
    private readonly ILogger<RegulationService> _logger;
    private const string TABLE_NAME = "regulations";

    public RegulationService(SupabaseConnection supabase, ILogger<RegulationService> logger)
    {
        _supabase = supabase;
        _logger = logger;
    }

    public async Task<ApiResponse<List<RegulationDto>>> GetAllRegulationsAsync()
    {
        try
        {
            var regulations = await _supabase.QueryListAsync<Regulation>(TABLE_NAME);
            var dtos = regulations.Select(MapToDto).ToList();
            
            return new ApiResponse<List<RegulationDto>>
            {
                Success = true,
                Data = dtos,
                Message = $"Se encontraron {dtos.Count} normativas"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting all regulations: {ex.Message}");
            return new ApiResponse<List<RegulationDto>>
            {
                Success = false,
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<List<RegulationDto>>> GetPublishedRegulationsAsync()
    {
        try
        {
            // Filter for published regulations
            var regulations = await _supabase.QueryListAsync<Regulation>(TABLE_NAME, "state=eq.PUBLISHED");
            var dtos = regulations.Select(MapToDto).ToList();
            
            return new ApiResponse<List<RegulationDto>>
            {
                Success = true,
                Data = dtos,
                Message = $"Se encontraron {dtos.Count} normativas publicadas"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting published regulations: {ex.Message}");
            return new ApiResponse<List<RegulationDto>>
            {
                Success = false,
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<RegulationDto>> GetRegulationByIdAsync(string id)
    {
        try
        {
            var regulation = await _supabase.QueryAsync<Regulation>(TABLE_NAME, $"id=eq.{id}");
            
            if (regulation == null)
            {
                return new ApiResponse<RegulationDto>
                {
                    Success = false,
                    Message = "Normativa no encontrada"
                };
            }

            return new ApiResponse<RegulationDto>
            {
                Success = true,
                Data = MapToDto(regulation)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting regulation by id: {ex.Message}");
            return new ApiResponse<RegulationDto>
            {
                Success = false,
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<List<RegulationDto>>> SearchRegulationsAsync(string searchText, string? type = null)
    {
        try
        {
            // Build filter query
            var filter = $"or=(reference.ilike.%{searchText}%,content.ilike.%{searchText}%)";
            if (!string.IsNullOrEmpty(type))
            {
                filter += $",type=eq.{type}";
            }

            var regulations = await _supabase.QueryListAsync<Regulation>(TABLE_NAME, filter);
            var dtos = regulations.Select(MapToDto).ToList();
            
            return new ApiResponse<List<RegulationDto>>
            {
                Success = true,
                Data = dtos,
                Message = $"Se encontraron {dtos.Count} normativas"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error searching regulations: {ex.Message}");
            return new ApiResponse<List<RegulationDto>>
            {
                Success = false,
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<RegulationDto>> CreateRegulationAsync(CreateRegulationDto dto)
    {
        try
        {
            // Log incoming DTO for debugging
            _logger.LogInformation("CreateRegulationDto received: {dto}", JsonSerializer.Serialize(dto));

            var regulation = new Regulation
            {
                SpecialNumber = dto.SpecialNumber,
                Reference = dto.Reference,
                Type = dto.Type,
                LegalStatus = dto.LegalStatus,
                Content = dto.Content,
                Keywords = dto.Keywords,
                PublicationDate = dto.PublicationDate ?? DateTime.UtcNow,
                FileUrl = dto.FileUrl,
                PdfUrl = dto.PdfUrl
            };

            // Log regulation object before insert (will use JsonPropertyName attributes)
            _logger.LogInformation("Regulation to insert: {regulation}", JsonSerializer.Serialize(regulation, new JsonSerializerOptions { WriteIndented = false }));

            var created = await _supabase.InsertAsync(TABLE_NAME, regulation);
            
            if (created == null)
            {
                return new ApiResponse<RegulationDto>
                {
                    Success = false,
                    Message = "Error al crear la normativa"
                };
            }

            return new ApiResponse<RegulationDto>
            {
                Success = true,
                Data = MapToDto(created),
                Message = "Normativa creada exitosamente"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating regulation: {ex.Message}");
            return new ApiResponse<RegulationDto>
            {
                Success = false,
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<RegulationDto>> UpdateRegulationAsync(string id, UpdateRegulationDto dto)
    {
        try
        {
            _logger.LogInformation($"[UPDATE] ID recibido: '{id}' (length: {id?.Length})");
            
            var existing = await _supabase.QueryAsync<Regulation>(TABLE_NAME, $"id=eq.{id}");
            
            _logger.LogInformation($"[UPDATE] Regulation encontrada: {(existing != null ? "SÍ" : "NO")}");
            
            if (existing == null)
            {
                _logger.LogWarning($"[UPDATE] Normativa no encontrada para ID: {id}");
                return new ApiResponse<RegulationDto>
                {
                    Success = false,
                    Message = "Normativa no encontrada"
                };
            }

            // Crear el objeto actualizado con todos los campos
            var updated = new Regulation
            {
                Id = existing.Id,
                SpecialNumber = dto.SpecialNumber ?? existing.SpecialNumber,
                Reference = dto.Reference ?? existing.Reference,
                Type = dto.Type ?? existing.Type,
                State = dto.State ?? existing.State,
                LegalStatus = dto.LegalStatus ?? existing.LegalStatus,
                Content = dto.Content ?? existing.Content,
                Keywords = dto.Keywords ?? existing.Keywords,
                PublicationDate = dto.PublicationDate ?? existing.PublicationDate,
                FileUrl = existing.FileUrl,
                PdfUrl = existing.PdfUrl,
                CreatedAt = existing.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("[UPDATE] Regulation a actualizar: {regulation}", JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = false }));

            var success = await _supabase.UpdateAsync(TABLE_NAME, id, updated);
            
            _logger.LogInformation($"[UPDATE] Resultado Supabase: {success}");
            
            if (!success)
            {
                _logger.LogError($"[UPDATE] Error al actualizar en Supabase para ID: {id}");
                return new ApiResponse<RegulationDto>
                {
                    Success = false,
                    Message = "Error al actualizar la normativa"
                };
            }

            return new ApiResponse<RegulationDto>
            {
                Success = true,
                Data = MapToDto(updated),
                Message = "Normativa actualizada exitosamente"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating regulation with ID '{id}': {ex.Message}\n{ex.StackTrace}");
            return new ApiResponse<RegulationDto>
            {
                Success = false,
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> DeleteRegulationAsync(string id)
    {
        try
        {
            var success = await _supabase.DeleteAsync(TABLE_NAME, id);
            
            return new ApiResponse<bool>
            {
                Success = success,
                Data = success,
                Message = success ? "Normativa eliminada exitosamente" : "Error al eliminar la normativa"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting regulation: {ex.Message}");
            return new ApiResponse<bool>
            {
                Success = false,
                Errors = new List<string> { ex.Message }
            };
        }
    }

    private RegulationDto MapToDto(Regulation regulation)
    {
        return new RegulationDto
        {
            Id = regulation.Id,
            SpecialNumber = regulation.SpecialNumber,
            Reference = regulation.Reference,
            Type = regulation.Type,
            State = regulation.State,
            LegalStatus = regulation.LegalStatus,
            Content = regulation.Content,
            Keywords = regulation.Keywords,
            PublicationDate = regulation.PublicationDate,
            FileUrl = regulation.FileUrl,
            PdfUrl = regulation.PdfUrl,

            CreatedAt = regulation.CreatedAt,
            UpdatedAt = regulation.UpdatedAt
        };
    }
}
