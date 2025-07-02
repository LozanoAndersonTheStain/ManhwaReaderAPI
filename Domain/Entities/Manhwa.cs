using ManhwaReaderAPI.Domain.Enums;

namespace ManhwaReaderAPI.Domain.Entities
{
    public class Manhwa
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string CoverImage { get; set; } = string.Empty;
        public List<Genre> Genres { get; set; } = new();
        public Status Status { get; set; }
        public int ChapterCount { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public double Rating { get; set; }
        public int ViewCount { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
