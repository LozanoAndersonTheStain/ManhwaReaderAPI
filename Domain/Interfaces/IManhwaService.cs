using ManhwaReaderAPI.Application.DTOs.Requests;
using ManhwaReaderAPI.Application.DTOs.Response;
using ManhwaReaderAPI.Domain.Enums;

namespace ManhwaReaderAPI.Domain.Interfaces
{
    public interface IManhwaService
    {
        Task<ManhwaResponse> CreateManhwaAsync(CreateManhwaRequest request);
        Task<ManhwaResponse?> GetManhwaByIdAsync(Guid id);
        Task<ManhwaListResponse> GetManhwasAsync(int page = 1, int pageSize = 10, string? searchTerm = null, Genre? genre = null, Status? status = null);
        Task<ManhwaResponse> UpdateManhwaAsync(Guid id, UpdateManhwaRequest request);
        Task<bool> DeleteManhwaAsync(Guid id);
        Task<ManhwaListResponse> GetTopRatedManhwasAsync(int limit = 10);
        Task<ManhwaListResponse> GetRecentlyUpdatedManhwasAsync(int limit = 10);
    }
}
