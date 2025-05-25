using System;
using System.Security.Cryptography;
using System.Text;

namespace PasswordTest
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Testing password hashes from database:");
            
            // Test both hashes from the database
            string hash1 = "ef92b778bafe771e89245b89ecbc08a44a4e166c0665991188181f383d4473e94f"; // john.doe
            string hash2 = "36e47dd992c4e177ba3e3c0b493f1a05036a05e62b17e2560aafab6d8a6695817"; // slim
            
            Console.WriteLine($"Hash 1 (john.doe): {hash1}");
            Console.WriteLine($"Hash 2 (slim): {hash2}");
            Console.WriteLine();
            
            // Common passwords to test
            string[] passwords = {
                "password", "Password", "123456", "admin", "Admin", "test", "Test",
                "user", "User", "1234", "12345", "password123", "Password123",
                "admin123", "Admin123", "defaultPassword123", "lib", "library",
                "Library", "student", "Student", "john", "John", "doe", "Doe",
                "johndoe", "JohnDoe", "john.doe", "John.Doe", "slim", "Slim",
                "SLIM", "secret", "Secret", "123", "qwerty", "abc123", "welcome",
                "Welcome", "login", "Login"
            };
            
            Console.WriteLine("Testing hash 1 (john.doe@ihec.ucar.tn):");
            bool found1 = false;
            foreach (string password in passwords)
            {
                string hash = HashPassword(password);
                if (hash.Equals(hash1, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"*** MATCH FOUND for john.doe! Password is: '{password}' ***");
                    found1 = true;
                    break;
                }
            }
            if (!found1) Console.WriteLine("Password not found for john.doe");
            
            Console.WriteLine("\nTesting hash 2 (slim@ihec.ucar.tn):");
            bool found2 = false;
            foreach (string password in passwords)
            {
                string hash = HashPassword(password);
                if (hash.Equals(hash2, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"*** MATCH FOUND for slim! Password is: '{password}' ***");
                    found2 = true;
                    break;
                }
            }
            if (!found2) Console.WriteLine("Password not found for slim");
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
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
