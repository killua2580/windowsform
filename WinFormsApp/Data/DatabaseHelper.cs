using System;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace WinFormsApp.Data
{
    public class DatabaseHelper
    {
        private static string connectionString = "Server=DESKTOP-HS2GE4G;Database=lib;Trusted_Connection=true;TrustServerCertificate=true;";

        public static void InitializeDatabase()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Create database tables if they don't exist
                CreateTablesIfNotExists(connection);
            }
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        private static void CreateTablesIfNotExists(SqlConnection connection)
        {
            string createBooksTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Books' AND xtype='U')
                CREATE TABLE Books (
                    BookID int IDENTITY(1,1) PRIMARY KEY,
                    Title nvarchar(255) NOT NULL,
                    Author nvarchar(255) NOT NULL,
                    ISBN nvarchar(50),
                    Category nvarchar(100),
                    StudyField nvarchar(100),
                    AvailableCount int NOT NULL DEFAULT 0,
                    TotalCount int NOT NULL DEFAULT 0,
                    Description ntext,
                    CoverImage nvarchar(500)
                )";

            string createUsersTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
                CREATE TABLE Users (
                    UserID int IDENTITY(1,1) PRIMARY KEY,
                    Email nvarchar(255) UNIQUE NOT NULL,
                    FirstName nvarchar(100) NOT NULL,
                    LastName nvarchar(100) NOT NULL,
                    StudyLevel nvarchar(50),
                    StudyField nvarchar(100),
                    ProfilePicture nvarchar(500),
                    IsBlocked bit NOT NULL DEFAULT 0,
                    IsAdmin bit NOT NULL DEFAULT 0,
                    CreatedDate datetime NOT NULL DEFAULT GETDATE(),
                    Password nvarchar(255) NOT NULL DEFAULT 'defaultPassword123'
                )";

            // Update existing columns to nvarchar if they are varbinary
            string updateProfilePictureColumn = @"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                          WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'ProfilePicture' AND DATA_TYPE = 'varbinary')
                BEGIN
                    ALTER TABLE Users DROP COLUMN ProfilePicture
                    ALTER TABLE Users ADD ProfilePicture nvarchar(500)
                END";

            string updateCoverImageColumn = @"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                          WHERE TABLE_NAME = 'Books' AND COLUMN_NAME = 'CoverImage' AND DATA_TYPE = 'varbinary')
                BEGIN
                    ALTER TABLE Books DROP COLUMN CoverImage
                    ALTER TABLE Books ADD CoverImage nvarchar(500)
                END";

            // Add Password column to existing Users table if it doesn't exist
            string addPasswordColumn = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                              WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'Password')
                BEGIN
                    ALTER TABLE Users ADD Password nvarchar(255) NOT NULL DEFAULT 'defaultPassword123'
                END";            // Update existing users without passwords or with old hash format
            string updateExistingPasswords = @"
                UPDATE Users 
                SET Password = @defaultHashedPassword
                WHERE Password = 'defaultPassword123' OR Password IS NULL OR LEN(Password) >= 64";

            string createReservationsTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Reservations' AND xtype='U')
                CREATE TABLE Reservations (
                    ReservationID int IDENTITY(1,1) PRIMARY KEY,
                    UserID int NOT NULL,
                    BookID int NOT NULL,
                    ReservationDate datetime NOT NULL DEFAULT GETDATE(),
                    Status nvarchar(50) NOT NULL DEFAULT 'Active',
                    FOREIGN KEY (UserID) REFERENCES Users(UserID),
                    FOREIGN KEY (BookID) REFERENCES Books(BookID)
                )";

            string createLikesTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Likes' AND xtype='U')
                CREATE TABLE Likes (
                    LikeID int IDENTITY(1,1) PRIMARY KEY,
                    UserID int NOT NULL,
                    BookID int NOT NULL,
                    LikeDate datetime NOT NULL DEFAULT GETDATE(),
                    FOREIGN KEY (UserID) REFERENCES Users(UserID),
                    FOREIGN KEY (BookID) REFERENCES Books(BookID),
                    UNIQUE(UserID, BookID)
                )";

            try
            {
                using (var command = new SqlCommand(createBooksTable, connection))
                    command.ExecuteNonQuery();
                
                using (var command = new SqlCommand(createUsersTable, connection))
                    command.ExecuteNonQuery();

                // Update existing columns if needed
                using (var command = new SqlCommand(updateProfilePictureColumn, connection))
                    command.ExecuteNonQuery();

                using (var command = new SqlCommand(updateCoverImageColumn, connection))
                    command.ExecuteNonQuery();
                
                // Add Password column to existing table if needed
                using (var command = new SqlCommand(addPasswordColumn, connection))
                    command.ExecuteNonQuery();
                  // Update existing users with hashed default password
                using (var command = new SqlCommand(updateExistingPasswords, connection))
                {
                    command.Parameters.AddWithValue("@defaultHashedPassword", HashPassword("defaultPassword123"));
                    command.ExecuteNonQuery();
                }
                
                using (var command = new SqlCommand(createReservationsTable, connection))
                    command.ExecuteNonQuery();
                
                using (var command = new SqlCommand(createLikesTable, connection))
                    command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating database tables: {ex.Message}", ex);
            }
        }

        public static string HashPassword(string inputPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputPassword));
                string hashedInput = BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
                return hashedInput;
            }
        }
    }
}