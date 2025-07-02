// using Microsoft.AspNetCore.Mvc;
// using ManhwaReaderAPI.Application.DTOs.Requests;
// using ManhwaReaderAPI.Domain.Interfaces;
// using ManhwaReaderAPI.Domain.Enums;

// namespace ManhwaReaderAPI.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class ManhwasController : ControllerBase
//     {
//         private readonly IManhwaService _manhwaService;

//         public ManhwasController(IManhwaService manhwaService)
//         {
//             _manhwaService = manhwaService;
//         }

//         [HttpPost]
//         public async Task<IActionResult> CreateManhwa([FromBody] CreateManhwaRequest request)
//         {
//             try
//             {
//                 var result = await _manhwaService.CreateManhwaAsync(request);
//                 return CreatedAtAction(nameof(GetManhwaById), new { id = result.Id }, result);
//             }
//             catch (Exception ex)
//             {
//                 return BadRequest(new { message = ex.Message });
//             }
//         }

//         [HttpGet("{id}")]
//         public async Task<IActionResult> GetManhwaById(Guid id)
//         {
//             var manhwa = await _manhwaService.GetManhwaByIdAsync(id);
//             if (manhwa == null)
//             {
//                 return NotFound(new { message = "Manhwa not found" });
//             }
//             return Ok(manhwa);
//         }

//         [HttpGet]
//         public async Task<IActionResult> GetManhwas(
//             [FromQuery] int page = 1,
//             [FromQuery] int pageSize = 10,
//             [FromQuery] string? searchTerm = null,
//             [FromQuery] Genre? genre = null,
//             [FromQuery] Status? status = null)
//         {
//             var result = await _manhwaService.GetManhwasAsync(page, pageSize, searchTerm, genre, status);
//             return Ok(result);
//         }

//         [HttpPut("{id}")]
//         public async Task<IActionResult> UpdateManhwa(Guid id, [FromBody] UpdateManhwaRequest request)
//         {
//             try
//             {
//                 var result = await _manhwaService.UpdateManhwaAsync(id, request);
//                 return Ok(result);
//             }
//             catch (Exception ex)
//             {
//                 return BadRequest(new { message = ex.Message });
//             }
//         }

//         [HttpDelete("{id}")]
//         public async Task<IActionResult> DeleteManhwa(Guid id)
//         {
//             var result = await _manhwaService.DeleteManhwaAsync(id);
//             if (!result)
//             {
//                 return NotFound(new { message = "Manhwa not found" });
//             }
//             return NoContent();
//         }

//         [HttpGet("top-rated")]
//         public async Task<IActionResult> GetTopRatedManhwas([FromQuery] int limit = 10)
//         {
//             var result = await _manhwaService.GetTopRatedManhwasAsync(limit);
//             return Ok(result);
//         }

//         [HttpGet("recently-updated")]
//         public async Task<IActionResult> GetRecentlyUpdatedManhwas([FromQuery] int limit = 10)
//         {
//             var result = await _manhwaService.GetRecentlyUpdatedManhwasAsync(limit);
//             return Ok(result);
//         }
//     }
// }
