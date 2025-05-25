using System;
using System.Windows.Forms;
using WinFormsApp.Data;
using WinFormsApp.Forms;
using Microsoft.Data.SqlClient;

namespace WinFormsApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Initialize database
            DatabaseHelper.InitializeDatabase();
            // Add some sample books
            AddSampleBooks();
            Application.Run(new LoginForm());
        }

        static void AddSampleBooks()
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                // Check if books already exist
                string checkQuery = "SELECT COUNT(*) FROM Books";
                using (var command = new SqlCommand(checkQuery, connection))
                {
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    if (count > 0) return; // Books already added
                }
                // Sample books data
                var sampleBooks = new[]
                {
                    new { Title = "Financial Management Principles", Author = "Robert Smith", Category = "Textbook", Field = "Finance", Count = 5 },
                    new { Title = "Marketing Strategy and Planning", Author = "Jane Doe", Category = "Textbook", Field = "Marketing", Count = 3 },
                    new { Title = "Business Intelligence Analytics", Author = "John Wilson", Category = "Technical", Field = "BI", Count = 4 },
                    new { Title = "Advanced Accounting", Author = "Mary Johnson", Category = "Textbook", Field = "Accounting", Count = 6 },
                    new { Title = "Operations Management", Author = "David Brown", Category = "Textbook", Field = "Management", Count = 4 },
                    new { Title = "Big Data Processing", Author = "Alice Cooper", Category = "Technical", Field = "Big Data", Count = 2 },
                    new { Title = "Introduction to Economics", Author = "Peter Clark", Category = "General", Field = "General", Count = 8 },
                    new { Title = "Statistics for Business", Author = "Sarah Davis", Category = "Reference", Field = "General", Count = 5 }
                };
                string insertQuery = @"INSERT INTO Books (Title, Author, Category, StudyField, AvailableCount, TotalCount)
                                     VALUES (@title, @author, @category, @field, @count, @count)";
                foreach (var book in sampleBooks)
                {
                    using (var command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@title", book.Title);
                        command.Parameters.AddWithValue("@author", book.Author);
                        command.Parameters.AddWithValue("@category", book.Category);
                        command.Parameters.AddWithValue("@field", book.Field);
                        command.Parameters.AddWithValue("@count", book.Count);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}