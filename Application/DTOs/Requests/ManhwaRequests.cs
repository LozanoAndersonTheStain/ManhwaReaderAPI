using ManhwaReaderAPI.Domain.Enums;

namespace ManhwaReaderAPI.Application.DTOs.Requests
{
    public class CreateManhwaRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string CoverImage { get; set; } = string.Empty;
        public List<Genre> Genres { get; set; } = new();
        public Status Status { get; set; }
        public int ChapterCount { get; set; }
        public DateTime ReleaseDate { get; set; }
    }

    public class UpdateManhwaRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public string? CoverImage { get; set; }
        public List<Genre>? Genres { get; set; }
        public Status? Status { get; set; }
        public int? ChapterCount { get; set; }
    }
}
