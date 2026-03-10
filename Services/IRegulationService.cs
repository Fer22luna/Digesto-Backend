using Backend.Entities;
using Backend.Models;

namespace Backend.Services;

public interface IRegulationService
{
    Task<ApiResponse<List<RegulationDto>>> GetAllRegulationsAsync();
    Task<ApiResponse<List<RegulationDto>>> GetPublishedRegulationsAsync();
    Task<ApiResponse<RegulationDto>> GetRegulationByIdAsync(string id);
    Task<ApiResponse<List<RegulationDto>>> SearchRegulationsAsync(string searchText, string? type = null);
    Task<ApiResponse<RegulationDto>> CreateRegulationAsync(CreateRegulationDto dto);
    Task<ApiResponse<RegulationDto>> UpdateRegulationAsync(string id, UpdateRegulationDto dto);
    Task<ApiResponse<bool>> DeleteRegulationAsync(string id);
}
