using ManhwaReaderAPI.Application.DTOs.Requests;
using ManhwaReaderAPI.Application.DTOs.Response;
using ManhwaReaderAPI.Domain.Entities;
using ManhwaReaderAPI.Domain.Enums;
using ManhwaReaderAPI.Domain.Interfaces;
using Npgsql;

namespace ManhwaReaderAPI.Application.Services
{
    public class ManhwaService : IManhwaService
    {
        private readonly string _connectionString;

        public ManhwaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                throw new ArgumentNullException("DefaultConnection string is not configured");
        }

        public async Task<ManhwaResponse> CreateManhwaAsync(CreateManhwaRequest request)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var manhwa = new Manhwa
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Author = request.Author,
                CoverImage = request.CoverImage,
                Genres = request.Genres,
                Status = request.Status,
                ChapterCount = request.ChapterCount,
                ReleaseDate = request.ReleaseDate,
                CreatedAt = DateTime.UtcNow,
                LastUpdate = DateTime.UtcNow
            };

            const string sql = @"
                INSERT INTO Manhwas (Id, Title, Description, Author, CoverImage, Genres, Status, 
                    ChapterCount, ReleaseDate, LastUpdate, Rating, ViewCount, IsActive, CreatedAt)
                VALUES (@Id, @Title, @Description, @Author, @CoverImage, @Genres, @Status,
                    @ChapterCount, @ReleaseDate, @LastUpdate, 0, 0, true, @CreatedAt)
                RETURNING *";

            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Id", manhwa.Id);
            cmd.Parameters.AddWithValue("@Title", manhwa.Title);
            cmd.Parameters.AddWithValue("@Description", manhwa.Description);
            cmd.Parameters.AddWithValue("@Author", manhwa.Author);
            cmd.Parameters.AddWithValue("@CoverImage", manhwa.CoverImage);
            cmd.Parameters.AddWithValue("@Genres", manhwa.Genres);
            cmd.Parameters.AddWithValue("@Status", manhwa.Status);
            cmd.Parameters.AddWithValue("@ChapterCount", manhwa.ChapterCount);
            cmd.Parameters.AddWithValue("@ReleaseDate", manhwa.ReleaseDate);
            cmd.Parameters.AddWithValue("@LastUpdate", manhwa.LastUpdate);
            cmd.Parameters.AddWithValue("@CreatedAt", manhwa.CreatedAt);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ManhwaResponse
                {
                    Id = manhwa.Id,
                    Title = manhwa.Title,
                    Description = manhwa.Description,
                    Author = manhwa.Author,
                    CoverImage = manhwa.CoverImage,
                    Genres = manhwa.Genres,
                    Status = manhwa.Status,
                    ChapterCount = manhwa.ChapterCount,
                    ReleaseDate = manhwa.ReleaseDate,
                    LastUpdate = manhwa.LastUpdate,
                    Rating = 0,
                    ViewCount = 0
                };
            }

            throw new Exception("Failed to create manhwa");
        }

        public async Task<bool> DeleteManhwaAsync(Guid id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = "UPDATE Manhwas SET IsActive = false WHERE Id = @Id";
            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<ManhwaResponse?> GetManhwaByIdAsync(Guid id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = "SELECT * FROM Manhwas WHERE Id = @Id AND IsActive = true";
            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ManhwaResponse
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    Author = reader.GetString(reader.GetOrdinal("Author")),
                    CoverImage = reader.GetString(reader.GetOrdinal("CoverImage")),
                    Genres = reader.GetFieldValue<List<Genre>>(reader.GetOrdinal("Genres")),
                    Status = reader.GetFieldValue<Status>(reader.GetOrdinal("Status")),
                    ChapterCount = reader.GetInt32(reader.GetOrdinal("ChapterCount")),
                    ReleaseDate = reader.GetDateTime(reader.GetOrdinal("ReleaseDate")),
                    LastUpdate = reader.GetDateTime(reader.GetOrdinal("LastUpdate")),
                    Rating = reader.GetDouble(reader.GetOrdinal("Rating")),
                    ViewCount = reader.GetInt32(reader.GetOrdinal("ViewCount"))
                };
            }

            return null;
        }

        public async Task<ManhwaListResponse> GetManhwasAsync(int page = 1, int pageSize = 10, string? searchTerm = null, Genre? genre = null, Status? status = null)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var whereClause = "WHERE IsActive = true";
            if (!string.IsNullOrEmpty(searchTerm))
            {
                whereClause += " AND (Title ILIKE @SearchTerm OR Description ILIKE @SearchTerm OR Author ILIKE @SearchTerm)";
            }
            if (genre.HasValue)
            {
                whereClause += " AND @Genre = ANY(Genres)";
            }
            if (status.HasValue)
            {
                whereClause += " AND Status = @Status";
            }

            var sql = $@"
                SELECT COUNT(*) FROM Manhwas {whereClause};
                SELECT * FROM Manhwas {whereClause}
                ORDER BY LastUpdate DESC
                OFFSET @Offset LIMIT @Limit";

            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
            cmd.Parameters.AddWithValue("@Limit", pageSize);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
            }
            if (genre.HasValue)
            {
                cmd.Parameters.AddWithValue("@Genre", genre.Value);
            }
            if (status.HasValue)
            {
                cmd.Parameters.AddWithValue("@Status", status.Value);
            }

            using var reader = await cmd.ExecuteReaderAsync();

            await reader.ReadAsync();
            var totalCount = reader.GetInt32(0);

            await reader.NextResultAsync();
            var manhwas = new List<ManhwaResponse>();

            while (await reader.ReadAsync())
            {
                manhwas.Add(new ManhwaResponse
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    Author = reader.GetString(reader.GetOrdinal("Author")),
                    CoverImage = reader.GetString(reader.GetOrdinal("CoverImage")),
                    Genres = reader.GetFieldValue<List<Genre>>(reader.GetOrdinal("Genres")),
                    Status = reader.GetFieldValue<Status>(reader.GetOrdinal("Status")),
                    ChapterCount = reader.GetInt32(reader.GetOrdinal("ChapterCount")),
                    ReleaseDate = reader.GetDateTime(reader.GetOrdinal("ReleaseDate")),
                    LastUpdate = reader.GetDateTime(reader.GetOrdinal("LastUpdate")),
                    Rating = reader.GetDouble(reader.GetOrdinal("Rating")),
                    ViewCount = reader.GetInt32(reader.GetOrdinal("ViewCount"))
                });
            }

            return new ManhwaListResponse
            {
                Manhwas = manhwas,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<ManhwaListResponse> GetRecentlyUpdatedManhwasAsync(int limit = 10)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT COUNT(*) FROM Manhwas WHERE IsActive = true;
                SELECT * FROM Manhwas 
                WHERE IsActive = true
                ORDER BY LastUpdate DESC 
                LIMIT @Limit";

            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Limit", limit);

            using var reader = await cmd.ExecuteReaderAsync();

            await reader.ReadAsync();
            var totalCount = reader.GetInt32(0);

            await reader.NextResultAsync();
            var manhwas = new List<ManhwaResponse>();

            while (await reader.ReadAsync())
            {
                manhwas.Add(new ManhwaResponse
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    Author = reader.GetString(reader.GetOrdinal("Author")),
                    CoverImage = reader.GetString(reader.GetOrdinal("CoverImage")),
                    Genres = reader.GetFieldValue<List<Genre>>(reader.GetOrdinal("Genres")),
                    Status = reader.GetFieldValue<Status>(reader.GetOrdinal("Status")),
                    ChapterCount = reader.GetInt32(reader.GetOrdinal("ChapterCount")),
                    ReleaseDate = reader.GetDateTime(reader.GetOrdinal("ReleaseDate")),
                    LastUpdate = reader.GetDateTime(reader.GetOrdinal("LastUpdate")),
                    Rating = reader.GetDouble(reader.GetOrdinal("Rating")),
                    ViewCount = reader.GetInt32(reader.GetOrdinal("ViewCount"))
                });
            }

            return new ManhwaListResponse
            {
                Manhwas = manhwas,
                TotalCount = totalCount,
                PageNumber = 1,
                PageSize = limit
            };
        }

        public async Task<ManhwaListResponse> GetTopRatedManhwasAsync(int limit = 10)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT COUNT(*) FROM Manhwas WHERE IsActive = true;
                SELECT * FROM Manhwas 
                WHERE IsActive = true
                ORDER BY Rating DESC, ViewCount DESC 
                LIMIT @Limit";

            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Limit", limit);

            using var reader = await cmd.ExecuteReaderAsync();

            await reader.ReadAsync();
            var totalCount = reader.GetInt32(0);

            await reader.NextResultAsync();
            var manhwas = new List<ManhwaResponse>();

            while (await reader.ReadAsync())
            {
                manhwas.Add(new ManhwaResponse
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    Author = reader.GetString(reader.GetOrdinal("Author")),
                    CoverImage = reader.GetString(reader.GetOrdinal("CoverImage")),
                    Genres = reader.GetFieldValue<List<Genre>>(reader.GetOrdinal("Genres")),
                    Status = reader.GetFieldValue<Status>(reader.GetOrdinal("Status")),
                    ChapterCount = reader.GetInt32(reader.GetOrdinal("ChapterCount")),
                    ReleaseDate = reader.GetDateTime(reader.GetOrdinal("ReleaseDate")),
                    LastUpdate = reader.GetDateTime(reader.GetOrdinal("LastUpdate")),
                    Rating = reader.GetDouble(reader.GetOrdinal("Rating")),
                    ViewCount = reader.GetInt32(reader.GetOrdinal("ViewCount"))
                });
            }

            return new ManhwaListResponse
            {
                Manhwas = manhwas,
                TotalCount = totalCount,
                PageNumber = 1,
                PageSize = limit
            };
        }

        public async Task<ManhwaResponse> UpdateManhwaAsync(Guid id, UpdateManhwaRequest request)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var updates = new List<string>();
            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(request.Title))
            {
                updates.Add("Title = @Title");
                parameters["@Title"] = request.Title;
            }
            if (!string.IsNullOrEmpty(request.Description))
            {
                updates.Add("Description = @Description");
                parameters["@Description"] = request.Description;
            }
            if (!string.IsNullOrEmpty(request.Author))
            {
                updates.Add("Author = @Author");
                parameters["@Author"] = request.Author;
            }
            if (!string.IsNullOrEmpty(request.CoverImage))
            {
                updates.Add("CoverImage = @CoverImage");
                parameters["@CoverImage"] = request.CoverImage;
            }
            if (request.Genres != null && request.Genres.Any())
            {
                updates.Add("Genres = @Genres");
                parameters["@Genres"] = request.Genres;
            }
            if (request.Status.HasValue)
            {
                updates.Add("Status = @Status");
                parameters["@Status"] = request.Status.Value;
            }
            if (request.ChapterCount.HasValue)
            {
                updates.Add("ChapterCount = @ChapterCount");
                parameters["@ChapterCount"] = request.ChapterCount.Value;
            }

            updates.Add("LastUpdate = @LastUpdate");
            updates.Add("UpdatedAt = @UpdatedAt");
            parameters["@LastUpdate"] = DateTime.UtcNow;
            parameters["@UpdatedAt"] = DateTime.UtcNow;
            parameters["@Id"] = id;

            var sql = $@"
                UPDATE Manhwas 
                SET {string.Join(", ", updates)}
                WHERE Id = @Id AND IsActive = true
                RETURNING *";

            using var cmd = new NpgsqlCommand(sql, connection);
            foreach (var param in parameters)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ManhwaResponse
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    Author = reader.GetString(reader.GetOrdinal("Author")),
                    CoverImage = reader.GetString(reader.GetOrdinal("CoverImage")),
                    Genres = reader.GetFieldValue<List<Genre>>(reader.GetOrdinal("Genres")),
                    Status = reader.GetFieldValue<Status>(reader.GetOrdinal("Status")),
                    ChapterCount = reader.GetInt32(reader.GetOrdinal("ChapterCount")),
                    ReleaseDate = reader.GetDateTime(reader.GetOrdinal("ReleaseDate")),
                    LastUpdate = reader.GetDateTime(reader.GetOrdinal("LastUpdate")),
                    Rating = reader.GetDouble(reader.GetOrdinal("Rating")),
                    ViewCount = reader.GetInt32(reader.GetOrdinal("ViewCount"))
                };
            }

            throw new Exception("Manhwa not found or update failed");
        }
    }
}
