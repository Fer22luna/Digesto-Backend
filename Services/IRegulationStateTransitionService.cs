using Backend.Models;

namespace Backend.Services;

public interface IRegulationStateTransitionService
{
    Task<ApiResponse<RegulationStateTransitionDto>> RecordTransitionAsync(CreateStateTransitionDto dto);
    Task<ApiResponse<List<RegulationStateTransitionDto>>> GetTransitionsForRegulationAsync(string regulationId);
    Task<ApiResponse<List<RegulationStateTransitionDto>>> GetAllTransitionsAsync();
}
