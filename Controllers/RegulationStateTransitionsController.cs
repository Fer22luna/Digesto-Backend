using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegulationStateTransitionsController : ControllerBase
{
    private readonly IRegulationStateTransitionService _stateTransitionService;
    private readonly ILogger<RegulationStateTransitionsController> _logger;

    public RegulationStateTransitionsController(
        IRegulationStateTransitionService stateTransitionService,
        ILogger<RegulationStateTransitionsController> logger)
    {
        _stateTransitionService = stateTransitionService;
        _logger = logger;
    }

    /// <summary>
    /// Get all state transitions for a specific regulation
    /// </summary>
    [HttpGet("regulation/{regulationId}")]
    public async Task<ActionResult<ApiResponse<List<RegulationStateTransitionDto>>>> GetTransitionsForRegulation(string regulationId)
    {
        var result = await _stateTransitionService.GetTransitionsForRegulationAsync(regulationId);
        return Ok(result);
    }

    /// <summary>
    /// Get all state transitions in the system
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<RegulationStateTransitionDto>>>> GetAllTransitions()
    {
        var result = await _stateTransitionService.GetAllTransitionsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Record a new state transition (typically called internally by RegulationsController)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<RegulationStateTransitionDto>>> RecordStateTransition([FromBody] CreateStateTransitionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrEmpty(dto.RegulationId) || string.IsNullOrEmpty(dto.ToState))
            return BadRequest(new ApiResponse<RegulationStateTransitionDto>
            {
                Success = false,
                Message = "RegulationId y ToState son requeridos"
            });

        var result = await _stateTransitionService.RecordTransitionAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetTransitionsForRegulation), 
            new { regulationId = dto.RegulationId }, result);
    }
}
