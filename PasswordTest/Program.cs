using System;
using System.Security.Cryptography;
using System.Text;

namespace PasswordTest
{
    class Program
    {        static void Main()
        {
            Console.WriteLine("Testing password hashes:");
            
            // Show what our default password hash looks like
            string defaultHash = HashPassword("defaultPassword123");
            Console.WriteLine($"Hash of 'defaultPassword123': {defaultHash}");
            Console.WriteLine($"Length: {defaultHash.Length}");
            Console.WriteLine();
            
            // Test both hashes from the database
            string hash1 = "ef92b778bafe771e89245b89ecbc08a44a4e166c0665991188181f383d4473e94f"; // john.doe
            string hash2 = "36e47dd992c4e177ba3e3c0b493f1a05036a05e62b17e2560aafab6d8a6695817"; // slim
            
            Console.WriteLine($"Hash 1 (john.doe): {hash1}");
            Console.WriteLine($"Hash 2 (slim): {hash2}");
            Console.WriteLine();
            
            // Check if these are 64 characters (they should be caught by LEN(Password) = 64)
            Console.WriteLine($"Hash 1 length: {hash1.Length}");
            Console.WriteLine($"Hash 2 length: {hash2.Length}");
            Console.WriteLine();
            
            Console.WriteLine("Press any key to exit...");
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
