using System;

namespace WinFormsApp.Models
{
    public class Book
    {
        public int BookID { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public string? ISBN { get; set; }
        public string? Category { get; set; }
        public string? StudyField { get; set; }
        public int AvailableCount { get; set; }
        public int TotalCount { get; set; }
        public string? Description { get; set; }
        public string? CoverImage { get; set; } // Changed from byte[]? to string? for URL
    }
}