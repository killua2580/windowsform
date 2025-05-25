using System;
using System.Security.Cryptography;
using System.Text;

namespace WinFormsApp
{    public class PasswordTester
    {
        public static string? FindPassword(string targetHash)
        {            // Common passwords to test
            string[] commonPasswords = {
                "password",
                "Password",
                "123456",
                "admin",
                "Admin",
                "test",
                "Test",
                "user",
                "User",
                "1234",
                "12345",
                "password123",
                "Password123",
                "admin123",
                "Admin123",
                "defaultPassword123",
                "lib",
                "library",
                "Library",
                "student",
                "Student",
                "john",
                "John",
                "doe",
                "Doe",
                "johndoe",
                "JohnDoe",
                "john.doe",
                "John.Doe",
                "slim",
                "Slim",
                "SLIM",
                "secret",
                "Secret",
                "123",
                "qwerty",
                "abc123",
                "welcome",
                "Welcome",
                "login",
                "Login"
            };

            Console.WriteLine($"Target hash: {targetHash}");
            Console.WriteLine("Testing common passwords...\n");

            foreach (string password in commonPasswords)
            {
                string hash = HashPassword(password);
                Console.WriteLine($"Password: '{password}' -> Hash: {hash}");
                
                if (hash.Equals(targetHash, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"*** MATCH FOUND! Password is: '{password}' ***");
                    return password;
                }
            }

            Console.WriteLine("\nNo match found in common passwords.");
            return null;
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
