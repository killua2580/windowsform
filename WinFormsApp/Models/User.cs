using System;

namespace WinFormsApp.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudyLevel { get; set; }
        public string StudyField { get; set; }
        public byte[] ProfilePicture { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreatedDate { get; set; }
    }
} 