using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegulationsController : ControllerBase
{
    private readonly IRegulationService _regulationService;
    private readonly ILogger<RegulationsController> _logger;

    public RegulationsController(IRegulationService regulationService, ILogger<RegulationsController> logger)
    {
        _regulationService = regulationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all regulations (admin only)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<RegulationDto>>>> GetAllRegulations()
    {
        var result = await _regulationService.GetAllRegulationsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Search regulations by text or type
    /// </summary>
    [HttpGet("search/{searchText}")]
    public async Task<ActionResult<ApiResponse<List<RegulationDto>>>> SearchRegulations(string searchText, [FromQuery] string? type = null)
    {
        var result = await _regulationService.SearchRegulationsAsync(searchText, type);
        return Ok(result);
    }

    /// <summary>
    /// Get published regulations only (public)
    /// </summary>
    [HttpGet("published")]
    public async Task<ActionResult<ApiResponse<List<RegulationDto>>>> GetPublishedRegulations()
    {
        var result = await _regulationService.GetPublishedRegulationsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Get a specific regulation by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<RegulationDto>>> GetRegulationById(string id)
    {
        var result = await _regulationService.GetRegulationByIdAsync(id);
        
        if (!result.Success)
            return NotFound(result);
            
        return Ok(result);
    }

    /// <summary>
    /// Create a new regulation
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<RegulationDto>>> CreateRegulation([FromBody] CreateRegulationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _regulationService.CreateRegulationAsync(dto);
        
        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetRegulationById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Update an existing regulation
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<RegulationDto>>> UpdateRegulation(string id, [FromBody] UpdateRegulationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _regulationService.UpdateRegulationAsync(id, dto);
        
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Partially update a regulation (PATCH)
    /// </summary>
    [HttpPatch("{id}")]
    public async Task<ActionResult<ApiResponse<RegulationDto>>> PatchRegulation(string id, [FromBody] UpdateRegulationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _regulationService.UpdateRegulationAsync(id, dto);
        
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Delete a regulation
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteRegulation(string id)
    {
        var result = await _regulationService.DeleteRegulationAsync(id);
        
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}
