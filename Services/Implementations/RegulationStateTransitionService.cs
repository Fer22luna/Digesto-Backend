using Backend.Data;
using Backend.Entities;
using Backend.Models;

namespace Backend.Services.Implementations;

public class RegulationStateTransitionService : IRegulationStateTransitionService
{
    private readonly SupabaseConnection _supabase;
    private readonly ILogger<RegulationStateTransitionService> _logger;
    private const string TABLE_NAME = "regulation_state_transitions";

    public RegulationStateTransitionService(SupabaseConnection supabase, ILogger<RegulationStateTransitionService> logger)
    {
        _supabase = supabase;
        _logger = logger;
    }

    public async Task<ApiResponse<RegulationStateTransitionDto>> RecordTransitionAsync(CreateStateTransitionDto dto)
    {
        try
        {
            var transition = new RegulationStateTransition
            {
                RegulationId = dto.RegulationId,
                FromState = dto.FromState,
                ToState = dto.ToState,
                Timestamp = DateTime.UtcNow,
                UserId = dto.UserId,
                UserRole = dto.UserRole,
                Notes = dto.Notes
            };

            _logger.LogInformation($"[STATE-TRANSITION] Recording transition for regulation {dto.RegulationId}: {dto.FromState} -> {dto.ToState}");

            var created = await _supabase.InsertAsync(TABLE_NAME, transition);

            if (created == null)
            {
                _logger.LogError($"[STATE-TRANSITION] Error recording transition for regulation {dto.RegulationId}");
                return new ApiResponse<RegulationStateTransitionDto>
                {
                    Success = false,
                    Message = "Error al registrar el cambio de estado"
                };
            }

            _logger.LogInformation($"[STATE-TRANSITION] Transition recorded successfully: {created.Id}");

            return new ApiResponse<RegulationStateTransitionDto>
            {
                Success = true,
                Data = MapToDto(created),
                Message = "Cambio de estado registrado exitosamente"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error recording state transition: {ex.Message}");
            return new ApiResponse<RegulationStateTransitionDto>
            {
                Success = false,
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<List<RegulationStateTransitionDto>>> GetTransitionsForRegulationAsync(string regulationId)
    {
        try
        {
            var transitions = await _supabase.QueryListAsync<RegulationStateTransition>(
                TABLE_NAME, 
                $"regulation_id=eq.{regulationId}"
            );

            // Sort by timestamp descending
            var sortedTransitions = transitions.OrderByDescending(t => t.Timestamp).ToList();
            var dtos = sortedTransitions.Select(MapToDto).ToList();

            return new ApiResponse<List<RegulationStateTransitionDto>>
            {
                Success = true,
                Data = dtos,
                Message = $"Se encontraron {dtos.Count} cambios de estado"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting transitions for regulation {regulationId}: {ex.Message}");
            return new ApiResponse<List<RegulationStateTransitionDto>>
            {
                Success = false,
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<List<RegulationStateTransitionDto>>> GetAllTransitionsAsync()
    {
        try
        {
            var transitions = await _supabase.QueryListAsync<RegulationStateTransition>(
                TABLE_NAME
            );

            // Sort by timestamp descending
            var sortedTransitions = transitions.OrderByDescending(t => t.Timestamp).ToList();
            var dtos = sortedTransitions.Select(MapToDto).ToList();

            return new ApiResponse<List<RegulationStateTransitionDto>>
            {
                Success = true,
                Data = dtos,
                Message = $"Se encontraron {dtos.Count} cambios de estado"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting all transitions: {ex.Message}");
            return new ApiResponse<List<RegulationStateTransitionDto>>
            {
                Success = false,
                Errors = new List<string> { ex.Message }
            };
        }
    }

    private RegulationStateTransitionDto MapToDto(RegulationStateTransition transition)
    {
        return new RegulationStateTransitionDto
        {
            Id = transition.Id,
            RegulationId = transition.RegulationId,
            FromState = transition.FromState,
            ToState = transition.ToState,
            Timestamp = transition.Timestamp,
            UserId = transition.UserId,
            UserRole = transition.UserRole,
            Notes = transition.Notes
        };
    }
}
