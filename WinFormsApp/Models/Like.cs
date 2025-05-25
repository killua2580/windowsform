using System;

namespace WinFormsApp.Models
{
    public class Like
    {
        public int LikeID { get; set; }
        public int UserID { get; set; }
        public int BookID { get; set; }
        public DateTime LikeDate { get; set; }
        
        // Navigation properties (optional, for easier data access)
        public User? User { get; set; }
        public Book? Book { get; set; }
    }
}