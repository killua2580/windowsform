using System;

namespace WinFormsApp.Models
{
    public class Book
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Category { get; set; }
        public string StudyField { get; set; }
        public int AvailableCount { get; set; }
        public int TotalCount { get; set; }
        public string Description { get; set; }
        public byte[] CoverImage { get; set; }
    }
} 