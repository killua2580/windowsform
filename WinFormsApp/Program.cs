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
            DatabaseHelper.InitializeDatabase();            // Add some sample books
            AddSampleBooks();
            
            // Uncomment the line below to test password finding
            // TestPasswordFinder();
            
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
        }        static void TestPasswordFinder()
        {
            Console.WriteLine("Testing password hashes from database:");
            
            // Test both hashes from the database
            string hash1 = "ef92b778bafe771e89245b89ecbc08a44a4e166c0665991188181f383d4473e94f"; // john.doe
            string hash2 = "36e47dd992c4e177ba3e3c0b493f1a05036a05e62b17e2560aafab6d8a6695817"; // slim
            
            Console.WriteLine($"Hash 1 (john.doe): {hash1}");
            Console.WriteLine($"Hash 2 (slim): {hash2}");
            Console.WriteLine();
            
            Console.WriteLine("Testing hash 1 (john.doe@ihec.ucar.tn):");
            string? foundPassword1 = PasswordTester.FindPassword(hash1);
            if (foundPassword1 != null)
            {
                Console.WriteLine($"Found password for john.doe: {foundPassword1}");
            }
            else
            {
                Console.WriteLine("Password not found for john.doe");
            }
            
            Console.WriteLine("\nTesting hash 2 (slim@ihec.ucar.tn):");
            string? foundPassword2 = PasswordTester.FindPassword(hash2);
            if (foundPassword2 != null)
            {
                Console.WriteLine($"Found password for slim: {foundPassword2}");
            }
            else
            {
                Console.WriteLine("Password not found for slim");
            }
            
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}