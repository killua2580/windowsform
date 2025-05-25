using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        string targetHash = "973007ca58d3c1c6c9dea25bb13c5618b8c3b0fef26581e223c6618eca618e09";
        
        string[] commonPasswords = {
            "admin",
            "password",
            "123456",
            "admin123",
            "password123",
            "defaultPassword123",
            "test",
            "user",
            "library",
            "librarian",
            "student",
            "Admin",
            "Password",
            "admin@123",
            "Admin123",
            "Password123"
        };
        
        Console.WriteLine($"Looking for password that produces hash: {targetHash}");
        Console.WriteLine();
        
        foreach (string password in commonPasswords)
        {
            string hash = HashPassword(password);
            Console.WriteLine($"Password: '{password}' -> Hash: {hash}");
            
            if (hash == targetHash)
            {
                Console.WriteLine($"*** MATCH FOUND! Password is: '{password}' ***");
                return;
            }
        }
        
        Console.WriteLine("\nNo match found among common passwords.");
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
