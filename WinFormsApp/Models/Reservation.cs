using System;

namespace WinFormsApp.Models
{
    public class Reservation
    {
        public int ReservationID { get; set; }
        public int UserID { get; set; }
        public int BookID { get; set; }
        public DateTime ReservationDate { get; set; }
        public string Status { get; set; } = "Active";
        
        // Navigation properties (optional, for easier data access)
        public User? User { get; set; }
        public Book? Book { get; set; }
    }
}