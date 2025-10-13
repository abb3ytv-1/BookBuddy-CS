using System.Security.Cryptography;
using System.Text;
public static class EncryptionHelper
{
    private static readonly string Key = Environment.GetEnvironmentVariable("ENCRYPTION_KEY")!;

    public static string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(Key);
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var result = new byte[aes.IV.Length + cipherBytes.Length];
        aes.IV.CopyTo(result, 0);
        cipherBytes.CopyTo(result, aes.IV.Length);

        return Convert.ToBase64String(result);
    }

    public static string Decrypt(string cipherText)
    {
        var fullCipher = Convert.FromBase64String(cipherText);
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(Key);
        var iv = fullCipher.Take(16).ToArray();
        var cipher = fullCipher.Skip(16).ToArray();
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipher, 0, cipherText.Length);
        return Encoding.UTF8.GetString(plainBytes);
    }
}