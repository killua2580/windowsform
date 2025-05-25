using System;

namespace WinFormsApp.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? StudyLevel { get; set; }
        public string? StudyField { get; set; }
        public string? ProfilePicture { get; set; } // Changed from byte[]? to string? for URL
        public bool IsBlocked { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}