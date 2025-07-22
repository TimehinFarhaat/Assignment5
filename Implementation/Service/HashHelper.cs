using System.Security.Cryptography;

public static class HashHelper
{
    public static string ComputeSHA256Hash(Stream fileStream)
    {
        using var sha256 = SHA256.Create();
        fileStream.Position = 0; // Reset stream position
        var hashBytes = sha256.ComputeHash(fileStream);
        fileStream.Position = 0; // Reset again after hashing
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}
