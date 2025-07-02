using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ManhwaReaderAPI.Application.DTOs.Requests;
using ManhwaReaderAPI.Application.DTOs.Response;
using ManhwaReaderAPI.Domain.Interfaces;
using ManhwaReaderAPI.Domain.Enums;

namespace ManhwaReaderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ManhwasController : ControllerBase
    {
        private readonly IManhwaService _manhwaService;

        public ManhwasController(IManhwaService manhwaService)
        {
            _manhwaService = manhwaService;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new manhwa",
            Description = "Creates a new manhwa with the specified details",
            OperationId = "CreateManhwa",
            Tags = new[] { "Manhwas" }
        )]
        [SwaggerResponse(201, "The manhwa was created successfully", typeof(ManhwaResponse))]
        [SwaggerResponse(400, "The request data is invalid")]
        public async Task<IActionResult> CreateManhwa([FromBody] CreateManhwaRequest request)
        {
            var result = await _manhwaService.CreateManhwaAsync(request);
            return CreatedAtAction(nameof(GetManhwaById), new { id = result.Id }, result);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Get a manhwa by ID",
            Description = "Retrieves a specific manhwa by its ID",
            OperationId = "GetManhwaById",
            Tags = new[] { "Manhwas" }
        )]
        [SwaggerResponse(200, "The manhwa was found", typeof(ManhwaResponse))]
        [SwaggerResponse(404, "The manhwa was not found")]
        public async Task<IActionResult> GetManhwaById(Guid id)
        {
            var result = await _manhwaService.GetManhwaByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all manhwas",
            Description = "Retrieves all manhwas with optional filtering and pagination",
            OperationId = "GetManhwas",
            Tags = new[] { "Manhwas" }
        )]
        [SwaggerResponse(200, "The manhwas were retrieved successfully", typeof(ManhwaListResponse))]
        public async Task<IActionResult> GetManhwas(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] Genre? genre = null,
            [FromQuery] Status? status = null)
        {
            var result = await _manhwaService.GetManhwasAsync(page, pageSize, searchTerm, genre, status);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        [SwaggerOperation(
            Summary = "Update a manhwa",
            Description = "Updates an existing manhwa with new information",
            OperationId = "UpdateManhwa",
            Tags = new[] { "Manhwas" }
        )]
        [SwaggerResponse(200, "The manhwa was updated successfully", typeof(ManhwaResponse))]
        [SwaggerResponse(400, "The request data is invalid")]
        [SwaggerResponse(404, "The manhwa was not found")]
        public async Task<IActionResult> UpdateManhwa(Guid id, [FromBody] UpdateManhwaRequest request)
        {
            try
            {
                var result = await _manhwaService.UpdateManhwaAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex) when (ex.Message == "Manhwa not found")
            {
                return NotFound();
            }
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation(
            Summary = "Delete a manhwa",
            Description = "Deletes an existing manhwa",
            OperationId = "DeleteManhwa",
            Tags = new[] { "Manhwas" }
        )]
        [SwaggerResponse(204, "The manhwa was deleted successfully")]
        [SwaggerResponse(404, "The manhwa was not found")]
        public async Task<IActionResult> DeleteManhwa(Guid id)
        {
            var result = await _manhwaService.DeleteManhwaAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("top-rated")]
        [SwaggerOperation(
            Summary = "Get top rated manhwas",
            Description = "Retrieves the top rated manhwas",
            OperationId = "GetTopRated",
            Tags = new[] { "Manhwas" }
        )]
        [SwaggerResponse(200, "The top rated manhwas were retrieved successfully", typeof(List<ManhwaResponse>))]
        public async Task<IActionResult> GetTopRated([FromQuery] int limit = 10)
        {
            var result = await _manhwaService.GetTopRatedManhwasAsync(limit);
            return Ok(result);
        }

        [HttpGet("recently-updated")]
        [SwaggerOperation(
            Summary = "Get recently updated manhwas",
            Description = "Retrieves the most recently updated manhwas",
            OperationId = "GetRecentlyUpdated",
            Tags = new[] { "Manhwas" }
        )]
        [SwaggerResponse(200, "The recently updated manhwas were retrieved successfully", typeof(List<ManhwaResponse>))]
        public async Task<IActionResult> GetRecentlyUpdated([FromQuery] int limit = 10)
        {
            var result = await _manhwaService.GetRecentlyUpdatedManhwasAsync(limit);
            return Ok(result);
        }
    }
}