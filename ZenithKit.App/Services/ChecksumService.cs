using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ZenithKit.App.Services;

public interface IChecksumService
{
    Task<string> ComputeAsync(string filePath, string algorithm, CancellationToken cancellationToken = default);
}

public sealed class ChecksumService : IChecksumService
{
    public Task<string> ComputeAsync(string filePath, string algorithm, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("filePath required", nameof(filePath));
            if (!File.Exists(filePath)) throw new FileNotFoundException("File not found", filePath);

            using var stream = File.OpenRead(filePath);
            HashAlgorithm hash = algorithm.ToUpperInvariant() switch
            {
                "MD5" => MD5.Create(),
                "SHA1" => SHA1.Create(),
                "SHA256" => SHA256.Create(),
                _ => SHA256.Create()
            };
            using (hash)
            {
                var bytes = hash.ComputeHash(stream);
                return BytesToHex(bytes);
            }
        }, cancellationToken);
    }

    private static string BytesToHex(byte[] data)
    {
        var sb = new StringBuilder(data.Length * 2);
        foreach (var b in data)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }
}
