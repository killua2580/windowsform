using System;
using System.Security.Cryptography;
using System.Text;

// Test the specific hash from database
string targetHash1 = "ef92b778bafe771e89245b89ecbc08a44a4e166c0665991188181f383d4473e94f";
string targetHash2 = "36e47dd992c4e177ba3e3c0b493f1a05036a05e62b17e2560aafab6d8a6695817";

Console.WriteLine("Testing password hashes:");
Console.WriteLine($"Hash 1 (john.doe): {targetHash1}");
Console.WriteLine($"Hash 2 (slim): {targetHash2}");
Console.WriteLine();

// Common passwords to test
string[] passwords = {
    "password", "Password", "123456", "admin", "Admin", "test", "Test",
    "user", "User", "1234", "12345", "password123", "Password123",
    "admin123", "Admin123", "defaultPassword123", "lib", "library",
    "Library", "student", "Student", "john", "John", "doe", "Doe",
    "slim", "Slim", "johndoe", "JohnDoe", "john.doe", "John.Doe"
};

foreach (string password in passwords)
{
    string hash = HashPassword(password);
    Console.WriteLine($"'{password}' -> {hash}");

    if (hash.Equals(targetHash1, StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine($"*** MATCH FOUND for john.doe! Password is: '{password}' ***");
    }
    if (hash.Equals(targetHash2, StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine($"*** MATCH FOUND for slim! Password is: '{password}' ***");
    }
}

Console.WriteLine("\nDone testing.");

static string HashPassword(string inputPassword)
{
    using (var sha256 = SHA256.Create())
    {
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputPassword));
        string hashedInput = BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        return hashedInput;
    }
}
